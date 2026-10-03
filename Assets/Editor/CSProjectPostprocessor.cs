using UnityEditor;

public class CSProjectPostprocessor : AssetPostprocessor
{
    private static string OnGeneratedCSProject(string path, string content)
    {
        return content.Replace("<NoWarn>0169;USG0001</NoWarn>", "<NoWarn>0162;0168;0169;0219;0414;0649;0618;0675;USG0001</NoWarn>");
    }
}
