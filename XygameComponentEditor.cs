using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
namespace XygameUI
{
    #region 组件相关
    /// <summary>
    /// Toggle组件
    /// </summary>
    [CustomEditor(typeof(XygameToggleGroup))]
    public class XygameToggleGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // 添加一些空间
            EditorGUILayout.Space();
            // 添加按钮
            if (GUILayout.Button("自动获得子组件", GUILayout.Height(30)))
            {
                // 获取目标组件并调用方法
                XygameToggleGroup myComponent = (XygameToggleGroup)target;
                myComponent.AutoFindChilds();
            }
        }
    }
    /// <summary>
    /// Toggle子组件
    /// </summary>
    [CustomEditor(typeof(XygameToggleText))]
    [CanEditMultipleObjects] // 添加这行以支持多选编辑
    public class XygameToggleTextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // 添加一些空间
            EditorGUILayout.Space();
            // 添加按钮
            if (GUILayout.Button("自动绑定对象", GUILayout.Height(30)))
            {
                // 获取所有选中的组件
                foreach (var t in targets)
                {
                    XygameToggleText myComponent = (XygameToggleText)t;
                    myComponent.AutoFindObject();
                }
            }
        }
    }
    /// <summary>
    /// Switch组件
    /// </summary>
    [CustomEditor(typeof(XygameSwitch))]
    public class XygameSwitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // 添加一些空间
            EditorGUILayout.Space();
            // 添加按钮
            if (GUILayout.Button("切换状态", GUILayout.Height(30)))
            {
                XygameSwitch myComponent = (XygameSwitch)target;
                myComponent.Switch();
            }
            // 添加按钮
            if (GUILayout.Button("重新计算", GUILayout.Height(24)))
            {
                XygameSwitch myComponent = (XygameSwitch)target;
                myComponent.CalcPosition(true);
            }
        }
    }
    /// <summary>
    /// Slider组件
    /// </summary>
    [CustomEditor(typeof(XygameSlider))]
    public class XygameSliderEditor : Editor
    {
        private float value = 0.5f;
        private XygameSlider component;
        private void OnEnable()
        {
            component = (XygameSlider)target;
            component.Init();
            value = component.value;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            value = EditorGUILayout.Slider("Value", value, 0f, 1f);
            if (value != component.value)
            {
                component.SetValue(value);
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(component);
            }
        }
    }
    /// <summary>
    /// 无限列表组件
    /// </summary>
    [CustomEditor(typeof(XygameInfiniteGridScroll))]
    public class XygameInfiniteGridScrollEditor : Editor
    {
        private SerializedProperty prefabProp;
        private SerializedProperty spacingProp;
        private SerializedProperty axisProp;
        private SerializedProperty modeProp;
        private SerializedProperty offsetLeftProp;
        private SerializedProperty offsetTopProp;
        private SerializedProperty rowCountProp;
        private SerializedProperty columnCountProp;
        private void OnEnable()
        {
            prefabProp = serializedObject.FindProperty("prefab");
            spacingProp = serializedObject.FindProperty("Spacing");
            axisProp = serializedObject.FindProperty("axis");
            modeProp = serializedObject.FindProperty("mode");
            offsetLeftProp = serializedObject.FindProperty("offsetLeft");
            offsetTopProp = serializedObject.FindProperty("offsetTop");
            rowCountProp = serializedObject.FindProperty("rowCount");
            columnCountProp = serializedObject.FindProperty("columnCount");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(prefabProp);
            EditorGUILayout.PropertyField(spacingProp);
            EditorGUILayout.PropertyField(axisProp);
            EditorGUILayout.PropertyField(modeProp);
            EditorGUILayout.PropertyField(offsetLeftProp);
            EditorGUILayout.PropertyField(offsetTopProp);

            bool isHorizontal = axisProp.enumValueIndex == (int)RectTransform.Axis.Horizontal;
            bool isSingle = modeProp.enumValueIndex == (int)ScrollMode.Single;

            if (!isSingle)
            {
                if (!isHorizontal)
                    EditorGUILayout.PropertyField(rowCountProp);
                else
                    EditorGUILayout.PropertyField(columnCountProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion
    #region 扩展组件
    [CustomEditor(typeof(XygameCarousel))]
    public class XygameCarouselEditor : Editor
    {
        private XygameCarousel component;
        private void OnEnable()
        {
            component = (XygameCarousel)target;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("自动绑定对象", GUILayout.Height(30)))
            {
                component.BindData();
            }
        }
    }
    #endregion
}
