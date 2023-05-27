namespace Savor22b.Tests;

using System;
using System.IO;

public static class Paths
{
    public static string GetProjectPath()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        return Path.GetFullPath(projectDirectory);
    }

    public static string GetTestDataPath()
    {
        return Path.Combine(GetProjectPath(), "TestData");
    }
}
