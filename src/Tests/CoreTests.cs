[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CoreTests
{
    [Test]
    public async Task MissingOrderBy()
    {
        await using var database = await DbContextBuilder.GetOrderRequiredDatabase();
        var data = database.Context;
        await ThrowsTask(
                () => data.Companies
                    .ToListAsync())
            .IgnoreStackTrace();
    }

    [Test]
    public async Task NestedMissingOrderBy()
    {
        await using var database = await DbContextBuilder.GetOrderRequiredDatabase();
        var data = database.Context;
        await ThrowsTask(
                () => data.Companies
                    .Include(_ => _.Employees)
                    .OrderBy(_ => _.Name)
                    .ToListAsync())
            .IgnoreStackTrace();
    }

    [Test]
    public async Task WithOrderBy()
    {
        await using var database = await DbContextBuilder.GetOrderRequiredDatabase();
        var data = database.Context;
        await Verify(
            data.Companies
                .OrderBy(_ => _.Name)
                .ToListAsync());
    }

    [Test]
    public async Task SingleMissingOrder()
    {
        await using var database = await DbContextBuilder.GetOrderRequiredDatabase();
        var data = database.Context;
        await Verify(data.Companies.Where(_ => _.Name == "Company1").SingleAsync());
    }

    [Test]
    public async Task WithNestedOrderBy()
    {
        await using var database = await DbContextBuilder.GetOrderRequiredDatabase();
        var data = database.Context;
        await Verify(
            data.Companies
                .Include(_ => _.Employees.OrderBy(_ => _.Age))
                .OrderBy(_ => _.Name)
                .ToListAsync());
    }



    [Test]
    public async Task SomePropsModified()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        var employee = new Employee
        {
            Name = "before",
            Age = 10
        };
        data.Add(employee);
        await data.SaveChangesAsync();

        data.Employees.Single()
            .Name = "after";
        await Verify(data.ChangeTracker);
    }

    [Test]
    public Task ShouldIgnoreDbFactory() =>
        Verify(new MyDbContextFactory());

    [Test]
    public Task ShouldIgnoreDbFactoryInterface()
    {
        var target = new TargetWithFactoryInterface
        {
            Factory = new MyDbContextFactory()
        };
        return Verify(target);
    }

    class TargetWithFactoryInterface
    {
        public IDbContextFactory<SampleDbContext> Factory { get; set; } = null!;
    }

    [Test]
    public Task ShouldIgnoreDbContext() =>
        Verify(
            new
            {
                Factory = new SampleDbContext(new DbContextOptions<SampleDbContext>())
            });

    class MyDbContextFactory : IDbContextFactory<SampleDbContext>
    {
        public SampleDbContext CreateDbContext() =>
            throw new NotImplementedException();
    }

    [Test]
    public async Task UpdateEntity()
    {
        var options = DbContextOptions();

        await using var data = new SampleDbContext(options);
        data.Add(new Employee
        {
            Name = "before"
        });
        await data.SaveChangesAsync();

        var employee = data.Employees.Single();
        data.Update(employee)
            .Entity.Name = "after";
        await Verify(data.ChangeTracker);
    }

    [Test]
    public async Task AllData()
    {
        var database = await DbContextBuilder.GetDatabase();
        var data = database.Context;

        #region AllData

        await Verify(data.AllData())
            .AddExtraSettings(
                serializer =>
                    serializer.TypeNameHandling = TypeNameHandling.Objects);

        #endregion
    }

    [Test]
    public async Task Queryable()
    {
        var database = await DbContextBuilder.GetDatabase();
        await database.AddData(
            new Company
            {
                Name = "company name"
            });
        var data = database.Context;

        #region Queryable

        var queryable = data.Companies
            .Where(_ => _.Name == "company name");
        await Verify(queryable);

        #endregion
    }

    [Test]
    public async Task SetSelect()
    {
        var database = await DbContextBuilder.GetDatabase();
        var data = database.Context;

        var query = data
            .Set<Company>()
            .Select(_ => _.Id);
        await Verify(query);
    }

    [Test]
    public async Task NestedQueryable()
    {
        var database = await DbContextBuilder.GetDatabase();
        await database.AddData(
            new Company
            {
                Name = "value"
            });
        var data = database.Context;
        var queryable = data.Companies
            .Where(_ => _.Name == "value");
        await Verify(
            new
            {
                queryable
            });
    }

    // ReSharper disable once UnusedVariable
    static void Build(string connection)
    {
        #region EnableRecording

        var builder = new DbContextOptionsBuilder<SampleDbContext>();
        builder.UseSqlServer(connection);
        builder.EnableRecording();
        var data = new SampleDbContext(builder.Options);

        #endregion
    }

    [Test]
    public async Task Parameters()
    {
        var database = await DbContextBuilder.GetDatabase();
        var data = database.Context;
        data.Add(
            new Company
            {
                Name = Guid
                    .NewGuid()
                    .ToString()
            }
        );
        Recording.Start();
        await data.SaveChangesAsync();
        await Verify();
    }

    [Test]
    public async Task RecordingTest()
    {
        var database = await DbContextBuilder.GetDatabase();
        var data = database.Context;

        #region Recording

        var company = new Company
        {
            Name = "Title"
        };
        data.Add(company);
        await data.SaveChangesAsync();

        Recording.Start();

        await data
            .Companies
            .Where(_ => _.Name == "Title")
            .ToListAsync();

        await Verify();

        #endregion
    }

    [Test]
    public async Task RecordingDisabledTest()
    {
        var database = await DbContextBuilder.GetDatabase();
        var data = database.Context;

        #region RecordingDisableForInstance

        var company = new Company
        {
            Name = "Title"
        };
        data.Add(company);
        await data.SaveChangesAsync();

        Recording.Start();

        await data
            .Companies
            .Where(_ => _.Name == "Title")
            .ToListAsync();
        data.DisableRecording();
        await data
            .Companies
            .Where(_ => _.Name == "Disabled")
            .ToListAsync();

        await Verify();

        #endregion
    }

    static DbContextOptions<SampleDbContext> DbContextOptions(
        [CallerMemberName] string databaseName = "") =>
        new DbContextOptionsBuilder<SampleDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
}