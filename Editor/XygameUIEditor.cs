using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UIElements;
namespace XygameUI
{
    public class XygameUIEditor : EditorWindow
    {
        //[MenuItem("Tools/XygameUI")]
        public static void ShowWindow()
        {
            // 创建窗口实例
            var window = GetWindow<XygameUIEditor>();
            window.titleContent = new GUIContent("XygameUI");
            window.Show();
        }
        // 存储选择的文件夹路径
        private string selectedFolderPath = "Assets/";
        // 图集名称
        private string atlasName = "NewAtlas";
        // 打包设置
        private bool includeSubfolders = true;
        private int padding = 2;
        private Texture2D[] selectedTextures;
        private void OnGUI()
        {
            GUILayout.Label("Xygame UI Atlas Packer", EditorStyles.boldLabel);

            // 选择文件夹
            EditorGUILayout.BeginHorizontal();
            selectedFolderPath = EditorGUILayout.TextField("Folder Path", selectedFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Folder", selectedFolderPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    selectedFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();

            // 图集名称
            atlasName = EditorGUILayout.TextField("Atlas Name", atlasName);

            // 其他设置
            includeSubfolders = EditorGUILayout.Toggle("Include Subfolders", includeSubfolders);
            padding = EditorGUILayout.IntField("Padding", padding);

            // 显示选择的图片数量
            if (selectedTextures != null)
            {
                EditorGUILayout.HelpBox($"{selectedTextures.Length} textures selected", MessageType.Info);
            }

            // 查找图片按钮
            if (GUILayout.Button("Find Textures"))
            {
                FindTexturesInFolder();
            }

            // 创建图集按钮
            if (GUILayout.Button("Create Atlas"))
            {
                CreateAtlas();
            }
        }
        // 在选定文件夹中查找图片
        private void FindTexturesInFolder()
        {
            string searchPattern = includeSubfolders ? "t:Texture2D" : "t:Texture2D";
            string[] guids = AssetDatabase.FindAssets(searchPattern, new[] { selectedFolderPath });

            List<Texture2D> textures = new List<Texture2D>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null)
                {
                    textures.Add(texture);
                }
            }

            selectedTextures = textures.ToArray();
        } // 创建图集
        private void CreateAtlas()
        {
            if (selectedTextures == null || selectedTextures.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No textures selected!", "OK");
                return;
            }

            // 创建SpriteAtlas
            SpriteAtlas atlas = new SpriteAtlas();

            // 设置打包参数
            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings()
            {
                padding = padding,
                enableRotation = false
            };
            atlas.SetPackingSettings(packingSettings);

            // 添加所有选中的纹理
            List<Object> objectsToPack = new List<Object>();
            foreach (Texture2D texture in selectedTextures)
            {
                objectsToPack.Add(texture);
            }
            atlas.Add(objectsToPack.ToArray());

            // 保存图集
            string atlasPath = Path.Combine(selectedFolderPath, $"{atlasName}.spriteatlas");
            AssetDatabase.CreateAsset(atlas, atlasPath);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", $"Atlas created at {atlasPath}", "OK");
        }
    }
}