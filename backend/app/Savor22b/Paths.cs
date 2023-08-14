namespace Savor22b;
public static class Paths
{
    private static readonly string baseDirectory = AppContext.BaseDirectory;

    public static string ProjectDirectory => Path.GetFullPath(Path.Combine(baseDirectory, "../../../"));

    public static string RootDirectory => Path.GetFullPath(Path.Combine(ProjectDirectory, "../../../"));
}
