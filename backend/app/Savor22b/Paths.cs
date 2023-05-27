namespace Savor22b;
public static class Paths
{
    public static string GetProjectPath()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectPath = currentDirectory;

        while (!string.IsNullOrEmpty(projectPath))
        {
            if (File.Exists(Path.Combine(projectPath, "appsettings.json")))
            {
                return projectPath;
            }

            projectPath = Directory.GetParent(projectPath)?.FullName;
        }

        return currentDirectory;
    }

    public static string GetCSVDataPath(string csvFileName)
    {
        string projectPath = GetProjectPath();
        string csvPath = Path.Combine(projectPath, "Data", csvFileName);
        return csvPath;
    }
}
