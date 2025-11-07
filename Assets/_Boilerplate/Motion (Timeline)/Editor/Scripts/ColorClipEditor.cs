using UnityEditor.Timeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace U9.Motion.Timeline
{
    [CustomTimelineEditor(typeof(ColorClip))]
    class ColorClipTimelineEditor : ClipEditor
    {
        readonly string k_AssignedError = L10n.Tr("No Curve Assigned");
        const int SampleCount = 30;
        const float SampleInterval = 1f / SampleCount;

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            var controlAsset = clip.asset as ColorClip;
            if (controlAsset != null && controlAsset.values.curve == null)
                clipOptions.errorText = k_AssignedError;
            return clipOptions;
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            var rect = region.position;
            var controlAsset = clip.asset as ColorClip;
            var curve = controlAsset?.values.curve;

            if (curve != null)
            {
                for (int i = 0; i < SampleCount; i++)
                {
                    float normalizedTime = i * SampleInterval;
                    Color color = Color.Lerp(controlAsset.values.startAt, controlAsset.values.endAt, curve.Evaluate(normalizedTime));

                    // Calculate the rect for the color sample
                    Rect sampleRect = GetSampleRect(rect, i);

                    // Draw the color sample
                    EditorGUI.DrawRect(sampleRect, color);
                }
            }
        }

        private Rect GetSampleRect(Rect clipRect, int sampleIndex)
        {
            float sampleWidth = clipRect.width / SampleCount;
            float xPos = clipRect.x + sampleIndex * sampleWidth;
            return new Rect(xPos, clipRect.y, sampleWidth + 1, clipRect.height);
        }
    }

    [CustomEditor(typeof(ColorClip))]
    class ColorClipEditor : Editor
    {
        SerializedProperty startAtProperty;
        SerializedProperty endAtProperty;
        SerializedProperty extrapolationProperty;

        SerializedProperty curveProperty;

        public void OnEnable()
        {
            startAtProperty = serializedObject.FindProperty("values.startAt");
            endAtProperty = serializedObject.FindProperty("values.endAt");

            extrapolationProperty = serializedObject.FindProperty("values.clipExtrapolation");

            curveProperty = serializedObject.FindProperty("values.curve");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(extrapolationProperty);
            EditorGUILayout.Space();

            startAtProperty.colorValue = EditorGUILayout.ColorField(new GUIContent("Start At"), startAtProperty.colorValue, true, true, true);
            endAtProperty.colorValue = EditorGUILayout.ColorField(new GUIContent("End At"), endAtProperty.colorValue, true, true, true);
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(curveProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}