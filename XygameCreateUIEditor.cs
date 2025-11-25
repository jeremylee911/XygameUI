using System.IO;
using UnityEditor;
using UnityEngine;
namespace XygameUI
{
    public class XygameCreateUIEditor : Editor
    {
        // 添加菜单项到Hierarchy右键菜单
        [MenuItem("GameObject/UI/XygameUI/CreateBasePanel", false, 9)]
        static void CreatePanel(MenuCommand menuCommand)
        {
            // 获取当前选中的GameObject
            GameObject selectedGameObject = menuCommand.context as GameObject;
            if (selectedGameObject == null)
            {
                Debug.LogError("未选择对象!");
                return;
            }

            // 检查类名是否存在
            string gameObjectName = selectedGameObject.name;
            if (ScriptClassExists(gameObjectName))
            {
                Debug.LogError($"类名已存在：{gameObjectName}");
                return;
            }

            string root = "Assets/ArtRes/UI";
            string path = $"{root}/{gameObjectName}";
            string scriptsPath = $"{path}/Scripts";
            string scriptPath = $"{scriptsPath}/{gameObjectName}.cs";

            try
            {
                // 确保根目录存在
                if (!AssetDatabase.IsValidFolder(root))
                {
                    AssetDatabase.CreateFolder("Assets/ArtRes", "UI");
                }
                // 创建主目录（如果不存在）
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder(root, gameObjectName);
                }
                // 创建Scripts子目录（如果不存在）
                if (!AssetDatabase.IsValidFolder(scriptsPath))
                {
                    AssetDatabase.CreateFolder(path, "Scripts");
                }
                // 确保所有目录都已创建完成
                AssetDatabase.Refresh();
                // 检查脚本文件是否存在
                if (!File.Exists(scriptPath))
                {
                    // 创建一个简单的C#脚本
                    string scriptContent = $"using XygameUI;\nusing UnityEngine;\n\npublic class {gameObjectName} : BasePanel \n{{\n\t\t\n}}";
                    // 确保目录确实存在
                    if (!Directory.Exists(scriptsPath))
                    {
                        Directory.CreateDirectory(scriptsPath);
                    }
                    File.WriteAllText(scriptPath, scriptContent);
                    AssetDatabase.ImportAsset(scriptPath);
                    Debug.Log($"脚本创建成功: {scriptPath}");
                }
                else
                {
                    Debug.Log($"脚本文件已存在: {scriptPath}");
                }

                // 创建其他必要目录
                string[] subFolders = { "Res", "Prefabs" };
                foreach (var folder in subFolders)
                {
                    string folderPath = $"{path}/{folder}";
                    if (!AssetDatabase.IsValidFolder(folderPath))
                    {
                        AssetDatabase.CreateFolder(path, folder);
                    }
                }

                // 保存更改+刷新
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建过程中发生错误: {e.Message}\n{e.StackTrace}");
            }
        }
        // 检查C#脚本类是否已存在
        private static bool ScriptClassExists(string className)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetType(className) != null)
                    return true;
            }
            return false;
        }
    }
}
