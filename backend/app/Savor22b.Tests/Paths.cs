namespace Savor22b.Tests;


public static class Paths
{
    private static readonly string baseDirectory = AppContext.BaseDirectory;

    public static string ProjectDirectory => Path.GetFullPath(Path.Combine(baseDirectory, "../../../"));

    public static string RootDirectory => Path.GetFullPath(Path.Combine(ProjectDirectory, "../../../"));

    // CSV 파일들이 저장된 디렉토리 경로
    public static string CsvDirectory => Path.Combine(RootDirectory, "resources/savor22b/tabledata/");

    public static string GetCSVDataPath(string csvFilePath)
    {
        string csvPath = Path.Combine(CsvDirectory, csvFilePath);
        return csvPath;
    }

    public static string GetTestDataPath()
    {
        return Path.Combine(ProjectDirectory, "TestData");
    }
}
