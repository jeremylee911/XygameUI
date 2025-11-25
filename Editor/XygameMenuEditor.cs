using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace XygameUI
{
    public class XygameMenuEditor : MonoBehaviour
    {
        // 预制体路径常量
        private const string PrefabPath = "Packages/com.xygame.ui/Runtime/Prefabs/";
        #region 组件创建菜单项
        [MenuItem("GameObject/UI/XygameUI/Component/Switch", false, 10)]
        private static void CreateSwitchComponent() => CreateFromPrefab("Switch");

        [MenuItem("GameObject/UI/XygameUI/Component/Button", false, 11)]
        private static void CreateButtonComponent() => CreateFromPrefab("Button");

        [MenuItem("GameObject/UI/XygameUI/Component/Slider", false, 12)]
        private static void CreateSliderComponent() => CreateFromPrefab("Slider");
        [MenuItem("GameObject/UI/XygameUI/Component/ToggleImg", false, 13)]
        private static void CreateToggleComponent1() => CreateFromPrefab("ToggleImg");
        [MenuItem("GameObject/UI/XygameUI/Component/ToggleText", false, 14)]
        private static void CreateToggleComponent2() => CreateFromPrefab("ToggleText");
        [MenuItem("GameObject/UI/XygameUI/Component/InfiniteScrollHorizontal", false, 15)]
        private static void CreateInfiniteScroll1() => CreateFromPrefab("InfiniteScrollHonizontal");
        [MenuItem("GameObject/UI/XygameUI/Component/InfiniteScrollVertical", false, 16)]
        private static void CreateInfiniteScroll2() => CreateFromPrefab("InfiniteScrollVertical");
        [MenuItem("GameObject/UI/XygameUI/Component/InfiniteScrollGrid", false, 17)]
        private static void CreateInfiniteScroll3() => CreateFromPrefab("InfiniteScrollGrid");
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/Carousel", false, 18)]
        private static void CreateCarousel() => CreateFromPrefab("Carousel");
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/CountingBox", false, 19)]
        private static void CreateCountingBox() => CreateFromPrefab("CountingBox");
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/DialogBox", false, 20)]
        private static void CreateDialogBox() => CreateFromPrefab("DialogBoxPanel");
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/PopMsgBox", false, 21)]
        private static void CreatePopMsgBox() => CreateFromPrefab("PopMsgPanel");
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/ArrowLineUI", false, 22)]
        private static void CreateArrowLineUI() => CreateFromPrefab("ArrowLineUI");
        [MenuItem("GameObject/UI/XygameUI/Component/FoldGroupHonizontal", false, 23)]
        private static void FoldGroup1() => CreateFromPrefab("FoldGroupHonizontal");
        [MenuItem("GameObject/UI/XygameUI/Component/FoldGroupVertical", false, 24)]
        private static void FoldGroup2() => CreateFromPrefab("FoldGroupVertical");
        #endregion
        #region 通用组件创建方法
        // 验证方法（确保菜单项在合适时可用）
        [MenuItem("GameObject/UI/XygameUI/CreateBasePanel", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/Switch", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/Button", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/Slider", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/ToggleImg", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/ToggleText", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/InfiniteScrollHorizontal", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/InfiniteScrollVertical", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/InfiniteScrollGrid", true)]
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/Carousel", true)]
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/CountingBox", true)]
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/DialogBox", true)]
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/PopMsgBox", true)]
        [MenuItem("GameObject/UI/XygameUI/ExtraComponent/ArrowLineUI", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/FoldGroupHonizontal", true)]
        [MenuItem("GameObject/UI/XygameUI/Component/FoldGroupVertical", true)]
        private static bool ValidateCreateUIComponent()
        {
            return Selection.activeGameObject == null ||
                   Selection.activeGameObject.GetComponentInParent<Canvas>() != null;
        }

        // 创建通用组件
        private static void CreateFromPrefab(string prefabName)
        {
            // 加载预制体
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabPath}{prefabName}.prefab");

            if (prefab == null)
            {
                Debug.LogError($"找不到预制体: {PrefabPath}{prefabName}.prefab");
                return;
            }

            GameObject selectedObject = Selection.activeGameObject;
            Canvas canvas = EnsureCanvas(selectedObject);

            // 实例化预制体
            GameObject instance = Instantiate(prefab);
            Undo.RegisterCreatedObjectUndo(instance, $"Create {prefabName}");

            // 设置父对象
            instance.transform.SetParent(canvas != null ? canvas.transform : selectedObject?.transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.name = prefabName;

            // 选中新对象
            Selection.activeGameObject = instance;
        }
        // 确保有可用的Canvas
        private static Canvas EnsureCanvas(GameObject selectedObject)
        {
            // 如果选中的对象已经有Canvas，返回null（表示使用选中对象作为父级）
            if (selectedObject != null && selectedObject.GetComponentInParent<Canvas>() != null)
            {
                return null;
            }

            // 查找场景中的Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                // 创建新Canvas
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();

                Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
            }
            return canvas;
        }
        #endregion
    }
}
