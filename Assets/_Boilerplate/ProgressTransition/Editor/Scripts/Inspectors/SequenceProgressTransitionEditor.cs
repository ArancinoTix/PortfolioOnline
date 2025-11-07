using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace U9.ProgressTransition.Editor
{
    [CustomEditor(typeof(SequenceProgressTransition), true)]
    public class SequenceProgressTransitionEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new VisualElement();
            var classField = new ObjectField("Script")
            {
                value = MonoScript.FromMonoBehaviour((MonoBehaviour)target),
                objectType = GetType(),
                allowSceneObjects = false
            };
            classField.SetEnabled(false);
            myInspector.Add(classField);

            DrawHeader("State Settings", myInspector);
            DrawMessageBox("This transition combines multiple transitions into one." +
            "\nDuration will be overriden by the sum of the sequences transitions." +
            "\n\n- [Parallel] will cause the transitions to play side by side." +
            "\n- [Staggered] will cause the transitions to play at a defined stagger offset from one another." +
            "\n- [Sequential] will have the transitions play one after the other.", myInspector);
            
            DrawSpace(10,myInspector);

            myInspector.Add(new PropertyField(serializedObject.FindProperty("_sequenceType")));
            myInspector.Add(new PropertyField(serializedObject.FindProperty("_staggerAmount")));

            DrawSpace(10, myInspector);

            myInspector.Add(new PropertyField(serializedObject.FindProperty("_transitionsToSequence")));

            DrawSeperator(myInspector);

            DrawHeader("Editor", myInspector);
            myInspector.Add(new PropertyField(serializedObject.FindProperty("_description")));

            DrawSeperator(myInspector);

            DrawHeader("Progress", myInspector);
            myInspector.Add(new PropertyField(serializedObject.FindProperty("_progress")));

            DrawSeperator(myInspector);

            DrawHeader("Easing Settings", myInspector);
            var durationField = new PropertyField(serializedObject.FindProperty("_duration"));
            durationField.SetEnabled(false); //Duration is set by the children
            myInspector.Add(durationField);

            myInspector.Add(new PropertyField(serializedObject.FindProperty("_easeType")));
            myInspector.Add(new PropertyField(serializedObject.FindProperty("_useTimeScale")));

            DrawSeperator(myInspector);

            var editorButton = new Button(() => { ProgressTransitionNodeEditor.OpenWindow((SequenceProgressTransition)serializedObject.targetObject,false); })
            {
                text = "Open Editor with all nodes"
            };
            var editorConnectedOnlyButton = new Button(() => { ProgressTransitionNodeEditor.OpenWindow((SequenceProgressTransition)serializedObject.targetObject,true); })
            {
                text = "Open Editor with connected nodes only"
            };
            myInspector.Add(editorButton);
            myInspector.Add(editorConnectedOnlyButton);
            DrawSpace(10, myInspector);

            return myInspector;
        }

        private void DrawHeader(string text, VisualElement element)
        {
            var header = new Label
            {
                style =
                {
                    paddingTop = 2,
                    paddingLeft = 3.5f,
                    paddingBottom = 3,
                    fontSize = 18,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)
                },
                text = text
            };
            element.Add(header);
        }

        private void DrawMessageBox(string text, VisualElement element)
        {
            var messageBox = new HelpBox(text, HelpBoxMessageType.Info);
            messageBox.style.marginBottom = 8;

            element.Add(messageBox);
        }

        private void DrawSeperator(VisualElement element)
        {
            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            separator.style.marginTop = 18;
            separator.style.marginBottom = 18;

            element.Add(separator);
        }
        private void DrawSpace(float size, VisualElement element)
        {
            var separator = new VisualElement();
            separator.style.height = size;

            element.Add(separator);
        }
    }
}
