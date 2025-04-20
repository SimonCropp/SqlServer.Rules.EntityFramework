[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CoreTests
{
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
}