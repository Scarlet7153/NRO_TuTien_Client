using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class AutoDeleteBackup 
{
    [PostProcessBuild(999)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string buildDirectory = Path.GetDirectoryName(pathToBuiltProject);
        string projectName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        string backupFolderPath = Path.Combine(buildDirectory, projectName + "_BackUpThisFolder_ButDontShipItWithYourGame");

        if (Directory.Exists(backupFolderPath))
        {
            Directory.Delete(backupFolderPath, true);
        }
    }
}
