using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class TestReorder : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public GameObject[] test_Obj;

#if UNITY_EDITOR
    [CustomEditor(typeof(TestReorder))]
    public class ExampleInspector : Editor
    {
        ReorderableList reorderableList;

        void OnEnable()
        {
            SerializedProperty prop = serializedObject.FindProperty("test_Obj");

            reorderableList = new ReorderableList(serializedObject, prop);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = prop.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, element);
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}