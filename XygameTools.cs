using UnityEngine;
using UnityEditor;
using System.IO;

public class XygameTools : Editor
{
    [MenuItem("Tools/Xygame/CreateDirectory")]
    public static void CreateDefaultDirectory()
    {
        //创建StreamingAssets目录
        Directory.CreateDirectory(Application.streamingAssetsPath);
        //创建Resources目录
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
        //创建ArtRes目录
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "ArtRes"));
        //创建Plugins目录
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Plugins"));
        //创建Editor目录
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Editor"));
        //创建Scripts目录
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Scripts"));
        AssetDatabase.Refresh();
    }
}
