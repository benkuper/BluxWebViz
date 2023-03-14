#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Linq.Expressions;

namespace VLB
{
    public class EditorCommon : Editor
    {
        protected virtual void OnEnable() {}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        }

        protected static void Header(string label)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        protected SerializedProperty FindProperty<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            Debug.Assert(serializedObject != null);
            return serializedObject.FindProperty(ReflectionUtils.GetFieldPath(expr));
        }

        protected void ButtonOpenConfig()
        {
            if (GUILayout.Button(EditorStrings.ButtonOpenGlobalConfig, EditorStyles.miniButton))
                Config.EditorSelectInstance();
        }
    }
}
#endif
