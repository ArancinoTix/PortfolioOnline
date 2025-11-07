using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U9.Utils;
using UnityEditor;
using UnityEditor.UI;

namespace U9.Utils.Editor
{
    [CustomEditor(typeof(ButtonWithLabel), true)]
    public class ButtonWithLabelEditor : SelectableEditor
    {
        SerializedProperty _labelProperty;
        SerializedProperty m_OnClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _labelProperty = serializedObject.FindProperty("_label");
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(_labelProperty);
            EditorGUILayout.PropertyField(m_OnClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
