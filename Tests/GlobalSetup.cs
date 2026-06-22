using NUnit.Framework;
using System;
using System.IO;

namespace SauceDemo_Automation.Tests
{
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
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"[GlobalSetup] Created: {path}");
                return;
            }

            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }

            Console.WriteLine($"[GlobalSetup] Cleared: {path}");
        }
    }
}