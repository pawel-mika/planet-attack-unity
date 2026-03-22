using UnityEngine;

[CreateAssetMenu(fileName = "BuildData", menuName = "Build/Create BuildData")]
public class BuildData : ScriptableObject
{
    public string buildNumber = "LOCAL";
    public string commitSha = "DEV-BRANCH";
    public string buildDate = "DEBUG-BUILD";
    public string buildEnvironment = "Local Development";

    // Right-click on the BuildData asset in the Inspector to see this option
    [ContextMenu("Fetch Local Git Info")]
    public void FetchGitInfo()
    {
        #if UNITY_EDITOR
        // Execute git command to get short SHA
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "git",
            Arguments = "rev-parse --short HEAD",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
        {
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    commitSha = output.Trim();
                    buildDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    buildNumber = "LOCAL-DEV";

                    // Mark the asset as modified so Unity saves the changes
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                    Debug.Log($"[BuildData] Updated local Git SHA: {commitSha}");
                }
            }
        }
        #endif
    }
}