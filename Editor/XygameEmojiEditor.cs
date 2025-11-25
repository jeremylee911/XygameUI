using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
namespace XygameUI
{
    public class XygameEmojiEditor : EditorWindow
    {
        private string directoryPath = "";
        private string directoryPath2 = "";
        private int[] sizeOptions = new int[] { 128, 256, 512, 1024, 2048 };
        private int selectedSizeIndex = 1;
        private int maxWidth;
        // 新增：TMP 精灵图集资源字段
        private TMP_SpriteAsset tmpSpriteAsset;
        [MenuItem("Tools/Xygame/EmojiWindow")]
        public static void ShowWindow()
        {
            GetWindow<XygameEmojiEditor>("XygameEmoji").Show();
        }
        private void OnGUI()
        {
            GUILayout.Label("Emoji 资源导入工具", EditorStyles.boldLabel);

            // 目录拖放区域
            EditorGUILayout.Space();
            GUILayout.Label("拖放目录到这里:");

            Rect dropArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, string.IsNullOrEmpty(directoryPath) ? "拖放文件夹到这里" : directoryPath, EditorStyles.helpBox);

            // 处理拖放事件
            HandleDragAndDrop(dropArea);

            EditorGUILayout.Space();
            // 最大宽度设置
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("雪碧图尺寸:", GUILayout.Width(70));

                // 创建下拉选择框
                selectedSizeIndex = EditorGUILayout.Popup(
                    selectedSizeIndex,
                    sizeOptions.Select(x => $"{x} 像素").ToArray());

                // 显示当前选中值
                maxWidth = sizeOptions[selectedSizeIndex];
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // 其他功能按钮
            if (GUILayout.Button("处理选中目录", GUILayout.Height(30)))
            {
                if (!string.IsNullOrEmpty(directoryPath)
                    && Directory.Exists(directoryPath))
                {
                    ProcessDirectory(directoryPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "请选择有效的目录", "确定");
                }
            }

            // 新增：TMP 精灵图集拖放区域
            EditorGUILayout.Space();
            GUILayout.Label("拖放 TMP Sprite Asset 到这里:");
            Rect tmpDropArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
            GUI.Box(tmpDropArea, string.IsNullOrEmpty(directoryPath2) ? "拖放 TMP_SpriteAsset 到这里" : directoryPath2, EditorStyles.helpBox);
            HandleTmpSpriteAssetDragAndDrop(tmpDropArea);
            // 新增：更新 TMP Sprite Asset 按钮
            if (tmpSpriteAsset != null && GUILayout.Button("更新 TMP Sprite Asset", GUILayout.Height(30)))
            {
                UpdateTmpSpriteAsset();
            }
        }
        private void HandleDragAndDrop(Rect dropArea)
        {
            Event currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(currentEvent.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                        {
                            // 只取第一个拖放的目录
                            string path = DragAndDrop.paths[0];

                            // 检查是否是目录
                            if (Directory.Exists(path))
                            {
                                directoryPath = path;
                                Repaint();
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("提示", "请拖放文件夹而不是文件", "确定");
                            }
                        }
                    }
                    break;
            }
        }
        private void ProcessDirectory(string path)
        {
            // 这里添加你的目录处理逻辑
            //Debug.Log($"开始处理目录: {path}");
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                // 存储每行的信息（宽度和高度）
                maxWidth = sizeOptions[selectedSizeIndex];
                List<(int width, int height)> rows = new List<(int, int)>();
                List<int> rowHeight = new List<int>() { 0 };
                int currentRowWidth = 0;
                int currentRowHeight = 0;
                int spacing = 10;
                // 检查是否已存在该 asset
                string outputPath = Path.Combine(Path.GetDirectoryName(directoryPath), "emojiSO.asset");
                EmojiSO asset = AssetDatabase.LoadAssetAtPath<EmojiSO>(outputPath);
                if (asset == null)
                {
                    // 如果不存在，创建新的
                    asset = ScriptableObject.CreateInstance<EmojiSO>();
                    AssetDatabase.CreateAsset(asset, outputPath);
                }
                // 清空现有数据
                asset.group.Clear();
                List<Texture2D> allTextures = new List<Texture2D>();
                // 遍历每个子目录(emoji组)
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dirName = Path.GetFileName(dirs[i]);
                    EditorUtility.DisplayProgressBar("处理中", "正在处理表情资源...", (float)i / dirs.Length);
                    // 为每个子目录创建一个新的EmojiGroup
                    EmojiGroup newGroup = new EmojiGroup();
                    string[] pngFiles = Directory.GetFiles(dirs[i], "*.png");
                    for (int j = 0; j < pngFiles.Length; j++)
                    {
                        string filePath = pngFiles[j];
                        // 使用AssetDatabase加载确保导入设置正确
                        TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                        if (importer != null && !importer.isReadable)
                        {
                            importer.isReadable = true;
                            importer.SaveAndReimport();
                        }

                        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                        if (tex != null)
                        {
                            allTextures.Add(tex);
                            // 为每个表情帧创建EmojiSingle并添加到当前组
                            EmojiSingle single = new EmojiSingle();
                            single.Name = $"{dirName}_{j}";
                            // Size将在后面设置实际值
                            newGroup.AddData(single);
                            // 如果当前行放不下且不是空行，则换行
                            if (currentRowWidth + tex.width > maxWidth && currentRowWidth > 0)
                            {
                                rows.Add((currentRowWidth - spacing, currentRowHeight)); // 减去最后一个spacing
                                currentRowWidth = 0;
                                currentRowHeight = 0;
                            }
                            currentRowWidth += tex.width + spacing;
                            if (tex.height > currentRowHeight)
                                currentRowHeight = tex.height;
                        }
                    }
                    // 将当前组添加到asset
                    asset.group.Add(newGroup);
                }
                // 添加最后一行
                if (currentRowWidth > 0)
                    rows.Add((currentRowWidth - spacing, currentRowHeight)); // 减去最后一个spacing

                // 计算总高度（所有行高 + 行间间距）
                int totalHeight = rows.Sum(r => r.height) + (rows.Count - 1) * spacing;

                // 确保尺寸是4的倍数
                maxWidth = ((maxWidth + 3) / 4) * 4;
                totalHeight = ((totalHeight + 3) / 4) * 4;
                //Debug.Log($"雪碧图尺寸: {maxWidth}x{totalHeight}");

                // 3. 创建雪碧图
                Texture2D atlas = new Texture2D(maxWidth, totalHeight, TextureFormat.ARGB32, false);
                // 填充透明背景
                Color[] clearPixels = new Color[maxWidth * totalHeight];
                for (int j = 0; j < clearPixels.Length; j++)
                    clearPixels[j] = Color.clear;
                atlas.SetPixels(clearPixels);

                // 4. 排列图片（从左上角开始）
                int currentX = 0;
                int currentY = totalHeight; // 从顶部开始
                int currentRow = 0;
                int currentInRow = 0;
                int textureIndex = 0;

                foreach (var group in asset.group)
                {
                    foreach (var single in group.list)
                    {
                        if (textureIndex >= allTextures.Count) break;
                        Texture2D tex = allTextures[textureIndex];
                        // 检查是否需要换行
                        if (currentX + tex.width > maxWidth && currentX > 0)
                        {
                            currentY -= rows[currentRow].height + spacing;
                            currentX = 0;
                            currentRow++;
                            currentInRow = 0;
                        }

                        int x = currentX;// 计算位置（从左上角开始）
                        int y = currentY - rows[currentRow].height;// 从当前行顶部开始

                        // 设置实际位置信息
                        single.Size = new Rect(x, y, tex.width, tex.height);
                        group.Width = tex.width;
                        group.Height = tex.height;
                        group.BX = 0;
                        group.BY = tex.height - 4;
                        group.Scale = 1.2f;
                        group.AD = Mathf.CeilToInt(group.Scale * tex.width);
                        atlas.SetPixels(x, y, tex.width, tex.height, tex.GetPixels());

                        currentX += tex.width + spacing;
                        currentInRow++;
                        textureIndex++;
                    }
                    group.Frame = group.list.Count;
                }

                atlas.Apply();
                outputPath = Path.Combine(Path.GetDirectoryName(directoryPath), "emoji.png");
                asset.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath);
                File.WriteAllBytes(outputPath, atlas.EncodeToPNG());
                EditorUtility.ClearProgressBar();
                // 标记为需要保存
                EditorUtility.SetDirty(asset);
                // 保存所有更改
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"合成完毕，输出位置：{outputPath}");
                asset.SpiteTex();
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("错误", $"处理失败: {e.Message}", "确定");
                Debug.LogError(e);
            }
        }
        // 新增：处理 TMP Sprite Asset 拖放
        private void HandleTmpSpriteAssetDragAndDrop(Rect dropArea)
        {
            Event currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(currentEvent.mousePosition)) return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                        {
                            var draggedObj = DragAndDrop.objectReferences[0];
                            if (draggedObj is TMP_SpriteAsset)
                            {
                                directoryPath2 = DragAndDrop.paths[0];
                                tmpSpriteAsset = draggedObj as TMP_SpriteAsset;
                                Repaint();
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("提示", "请拖放 TMP_SpriteAsset 资源", "确定");
                            }
                        }
                    }
                    break;
            }
        }
        // 新增：更新 TMP Sprite Asset 的方法
        private void UpdateTmpSpriteAsset()
        {
            if (tmpSpriteAsset == null)
            {
                EditorUtility.DisplayDialog("错误", "未指定 TMP Sprite Asset", "确定");
                return;
            }

            string emojiSoPath = Path.Combine(Path.GetDirectoryName(directoryPath2), "emojiSO.asset");
            EmojiSO emojiSo = AssetDatabase.LoadAssetAtPath<EmojiSO>(emojiSoPath);
            if (emojiSo == null || emojiSo.texture == null || emojiSo.group.Count == 0)
            {
                EditorUtility.DisplayDialog("错误", "未找到有效的 EmojiSO 数据", "确定");
                return;
            }

            int index = 0;
            for (int i = 0; i < emojiSo.group.Count; i++)
            {
                var group = emojiSo.group[i];
                for (int j = 0; j < group.list.Count; j++)
                {
                    tmpSpriteAsset.spriteGlyphTable[index].metrics = new UnityEngine.TextCore.GlyphMetrics()
                    {
                        width = group.Width,
                        height = group.Height,
                        horizontalBearingX = group.BX,
                        horizontalBearingY = group.BY,
                        horizontalAdvance = group.AD,
                    };
                    tmpSpriteAsset.spriteGlyphTable[index].scale = group.Scale;
                    index++;
                }
            }
            EditorUtility.SetDirty(tmpSpriteAsset);
            AssetDatabase.SaveAssets();
            emojiSo.SaveJson();
            // 4. 刷新所有使用此Asset的TMP文本组件
            RefreshAllTMPTextComponents(tmpSpriteAsset);
            Debug.Log("TMP Sprite Asset 更新完成！");
            AssetDatabase.Refresh();
        }
        // 刷新场景中所有使用此SpriteAsset的TMP文本
        private void RefreshAllTMPTextComponents(TMP_SpriteAsset spriteAsset)
        {
#if UNITY_EDITOR
            var allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (var text in allTexts)
            {
                if (text.spriteAsset == spriteAsset)
                {
                    // 两种刷新方式
                    text.SetAllDirty(); // 方法1：标记脏数据
                    text.ForceMeshUpdate(); // 方法2：强制网格更新
                    EditorUtility.SetDirty(text); // 确保编辑器保存变更
                }
            }
#endif
        }
    }
}
