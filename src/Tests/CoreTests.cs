[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CoreTests
{
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
}