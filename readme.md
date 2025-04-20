# <img src="/src/icon.png" height="30px"> SqlServer.Rules.EntityFramework

[![Build status](https://ci.appveyor.com/api/projects/status/p3tyb89wgpt3v876?svg=true)](https://ci.appveyor.com/project/SimonCropp/sqlserver-rules-entityframework)
[![NuGet Status](https://img.shields.io/nuget/v/SqlServer.Rules.EntityFramework.svg?label=SqlServer.Rules.EntityFramework)](https://www.nuget.org/packages/SqlServer.Rules.EntityFramework/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow snapshot testing with EntityFramework.

**See [Milestones](../../milestones?state=closed) for release notes.**


## NuGet package

 * https://nuget.org/packages/SqlServer.Rules.EntityFramework/


## Enable

Call `EfRecording.EnableRecording()` on `DbContextOptionsBuilder`.

<!-- snippet: EnableRecording -->
<a id='snippet-EnableRecording'></a>
```cs
var builder = new DbContextOptionsBuilder<SampleDbContext>();
builder.UseSqlServer(connection);
builder.EnableRecording();
var data = new SampleDbContext(builder.Options);
```
<sup><a href='/src/Tests/CoreTests.cs#L202-L209' title='Snippet source file'>snippet source</a> | <a href='#snippet-EnableRecording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

`EnableRecording` should only be called in the test context.


### Usage

To start recording call `EfRecording.StartRecording()`. The results will be automatically included in verified file.

<!-- snippet: Recording -->
<a id='snippet-Recording'></a>
```cs
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
```
<sup><a href='/src/Tests/CoreTests.cs#L269-L287' title='Snippet source file'>snippet source</a> | <a href='#snippet-Recording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

<!-- snippet: CoreTests.RecordingTest.verified.txt -->
<a id='snippet-CoreTests.RecordingTest.verified.txt'></a>
```txt
{
  ef: {
    Type: ReaderExecutedAsync,
    HasTransaction: false,
    Text:
select c.Id,
       c.Name
from   Companies as c
where  c.Name = N'Title'
  }
}
```
<sup><a href='/src/Tests/CoreTests.RecordingTest.verified.txt#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-CoreTests.RecordingTest.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Queryable

This test:

<!-- snippet: Queryable -->
<a id='snippet-Queryable'></a>
```cs
var queryable = data.Companies
    .Where(_ => _.Name == "company name");
await Verify(queryable);
```
<sup><a href='/src/Tests/CoreTests.cs#L159-L165' title='Snippet source file'>snippet source</a> | <a href='#snippet-Queryable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified files:


### EF Core


#### CoreTests.Queryable.verified.txt

<!-- snippet: CoreTests.Queryable.verified.txt -->
<a id='snippet-CoreTests.Queryable.verified.txt'></a>
```txt
[
  {
    Name: company name
  }
]
```
<sup><a href='/src/Tests/CoreTests.Queryable.verified.txt#L1-L5' title='Snippet source file'>snippet source</a> | <a href='#snippet-CoreTests.Queryable.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### CoreTests.Queryable.verified.sql

<!-- snippet: CoreTests.Queryable.verified.sql -->
<a id='snippet-CoreTests.Queryable.verified.sql'></a>
```sql
SELECT [c].[Id], [c].[Name]
FROM [Companies] AS [c]
WHERE [c].[Name] = N'company name'
```
<sup><a href='/src/Tests/CoreTests.Queryable.verified.sql#L1-L3' title='Snippet source file'>snippet source</a> | <a href='#snippet-CoreTests.Queryable.verified.sql' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## AllData

This test:

<!-- snippet: AllData -->
<a id='snippet-AllData'></a>
```cs
await Verify(data.AllData())
    .AddExtraSettings(
        serializer =>
            serializer.TypeNameHandling = TypeNameHandling.Objects);
```
<sup><a href='/src/Tests/CoreTests.cs#L138-L145' title='Snippet source file'>snippet source</a> | <a href='#snippet-AllData' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file with all data in the database:

<!-- snippet: CoreTests.AllData.verified.txt -->
<a id='snippet-CoreTests.AllData.verified.txt'></a>
```txt
[
  {
    $type: Company,
    Id: 1,
    Name: Company1
  },
  {
    $type: Company,
    Id: 4,
    Name: Company2
  },
  {
    $type: Company,
    Id: 6,
    Name: Company3
  },
  {
    $type: Company,
    Id: 7,
    Name: Company4
  },
  {
    $type: Employee,
    Id: 2,
    CompanyId: 1,
    Name: Employee1,
    Age: 25
  },
  {
    $type: Employee,
    Id: 3,
    CompanyId: 1,
    Name: Employee2,
    Age: 31
  },
  {
    $type: Employee,
    Id: 5,
    CompanyId: 4,
    Name: Employee4,
    Age: 34
  }
]
```
<sup><a href='/src/Tests/CoreTests.AllData.verified.txt#L1-L43' title='Snippet source file'>snippet source</a> | <a href='#snippet-CoreTests.AllData.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Database](https://thenounproject.com/term/database/310841/) designed by [Creative Stall](https://thenounproject.com/creativestall/) from [The Noun Project](https://thenounproject.com).
