namespace Savor22b.Tests.Helpers;

using Savor22b.Helpers;

public class CsvParserTests
{
    public static readonly string testCavDataPath = Path.Combine(Paths.GetTestDataPath(), "sample_csv_data.csv");

    class SampleCsv {
        public int Test1 {get; set;}
        public string Test2 {get; set;}
    }

    [Fact]
    public void ParseCsv_ValidReturnsParsedItems()
    {
        string filePath = testCavDataPath;
        var csvParser = new CsvParser<SampleCsv>();

        var result = csvParser.ParseCsv(filePath);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
