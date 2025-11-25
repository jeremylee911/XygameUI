using System.Linq;
using UnityEditor;
using UnityEngine;
namespace XygameUI
{
    [CustomEditor(typeof(PanelManager))]
    public class PanelManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // 显示默认Inspector

            PanelManager manager = (PanelManager)target;

            // 显示Stack信息
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Panel Stack", EditorStyles.boldLabel);

            if (manager.stacks.Count == 0)
            {
                EditorGUILayout.HelpBox("Stack is Empty", MessageType.Info);
            }
            else
            {
                // 按从栈顶到栈底的顺序显示
                int index = 0;
                foreach (var panel in manager.stacks.Reverse())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"[{index}]", GUILayout.Width(30));
                    EditorGUILayout.ObjectField(panel.GetType().Name, panel, typeof(BaseMain), false);
                    EditorGUILayout.EndHorizontal();
                    index++;
                }
            }
        }
    }
}