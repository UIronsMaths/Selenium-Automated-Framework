using NUnit.Framework;
using System;
using System.IO;

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Console.WriteLine("[GlobalSetup] Starting artifact cleanup...");

        ClearDirectory(ArtifactPaths.Screenshots);
        ClearDirectory(ArtifactPaths.ExtentReport);
        ClearDirectory(ArtifactPaths.Logs);
        ClearDirectory(ArtifactPaths.Allure);

        Console.WriteLine("[GlobalSetup] Artifact cleanup complete.");
    }

    private static void ClearDirectory(string path)
    {
        try
        {
            // Ensure directory exists and is clean. Delete and recreate to remove stale files and subdirs.
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }

            Directory.CreateDirectory(path);



            Console.WriteLine($"[GlobalSetup] Cleared/Created: {path}");
        }
        catch (Exception ex)
        {
            // Log and continue; tests should still run even if cleanup fails.
            Console.WriteLine($"[GlobalSetup] Failed to clear/create '{path}': {ex.Message}");
        }
    }
}
