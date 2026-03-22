using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

public class BuildScript
{
    public static void PerformBuild()
    {
        // 1. Extract arguments from the command line
        string[] args = System.Environment.GetCommandLineArgs();
        string buildNumber = GetArg("-buildNumber") ?? "0";
        string commitSha = GetArg("-commitSha") ?? "unknown";
        string buildDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // 2. Update the ScriptableObject with data (must be in Resources so the game can see it at runtime)
        BuildData buildData = Resources.Load<BuildData>("BuildData");
        if (buildData != null)
        {
            buildData.buildNumber = buildNumber;
            buildData.commitSha = commitSha;
            buildData.buildDate = buildDate;
            EditorUtility.SetDirty(buildData);
            AssetDatabase.SaveAssets();
        }

        // 3. Configure the Android Build
        BuildPlayerOptions bpo = new BuildPlayerOptions();
        bpo.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        bpo.locationPathName = "build/Android/PlanetAttack.apk";
        bpo.target = BuildTarget.Android;
        bpo.options = BuildOptions.None;

        // 4. Error checking logic
        BuildReport report = BuildPipeline.BuildPlayer(bpo);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build Succeeded! Size: {summary.totalSize} bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build Failed!");
            System.Environment.Exit(1); // Closes the Runner with an error so you see a red color in Forgejo
        }
    }

    private static string GetArg(string name)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1) return args[i + 1];
        }
        return null;
    }
}