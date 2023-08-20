namespace Savor22b.Tests.Helpers;

using System.Collections.Immutable;
using Savor22b.Helpers;

public class CsvParserTests
{
    public static readonly string testCavDataPath = Path.Combine(Paths.GetTestDataPath(), "sample_csv_data.csv");
    public static readonly string testCavDataContainListPath = Path.Combine(Paths.GetTestDataPath(), "sample_csv_data_contain_list.csv");

    class SampleCsv {
        public int Test1 {get; set;}
        public string Test2 {get; set;}
    }

    class SampleCsvContainNullable {
        public int? Test1 {get; set;}
        public string? Test2 {get; set;}
    }

    class SampleCsvContainList {
        public int Test1 {get; set;}
        public ImmutableList<int> Test2List {get; set;}
        public ImmutableList<string> Test3List {get; set;}
    }

    [Fact]
    public void ParseCsv_Success_NormalClass()
    {
        string filePath = testCavDataPath;
        var csvParser = new CsvParser<SampleCsv>();

        var result = csvParser.ParseCsv(filePath);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void ParseCsv_Success_ContainListClass()
    {
        string filePath = testCavDataContainListPath;
        var csvParser = new CsvParser<SampleCsvContainList>();

        var result = csvParser.ParseCsv(filePath);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void ParseCsv_SuccessContainNullableValueClass()
    {
        string filePath = testCavDataPath;
        var csvParser = new CsvParser<SampleCsvContainNullable>();

        var result = csvParser.ParseCsv(filePath);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
