using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    [CustomTimelineEditor(typeof(Vector3Clip))]
    class Vector3ClipTimelineEditor : ClipEditor
    {
        readonly string k_NoCurveAssignedError = L10n.Tr("No curve assigned");
        readonly Dictionary<TimelineClip, AnimationCurve> m_PersistentCurves = new Dictionary<TimelineClip, AnimationCurve>();
        ColorSpace m_ColorSpace = ColorSpace.Uninitialized;

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            var controlAsset = clip.asset as Vector3Clip;
            if (controlAsset != null &&
                (controlAsset.values.curveX == null ||
                controlAsset.values.curveY == null))
                clipOptions.errorText = k_NoCurveAssignedError;
            return clipOptions;
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            var rect = region.position;
            if (rect.width <= 0)
                return;

            var curveX = GetOrCreateCurve(clip, ((Vector3Clip)clip.asset).values.curveX);
            if (curveX == null)
                return;

            var curveY = GetOrCreateCurve(clip, ((Vector3Clip)clip.asset).values.curveY);
            if (curveY == null)
                return;

            var curveZ = GetOrCreateCurve(clip, ((Vector3Clip)clip.asset).values.curveZ);
            if (curveZ == null)
                return;

            var quantizedRect = new Rect(Mathf.Ceil(rect.x), Mathf.Ceil(rect.y), Mathf.Ceil(rect.width), Mathf.Ceil(rect.height));

            if (((Vector3Clip)clip.asset).values.xEnabled)
                DrawCurvePreview(curveX, quantizedRect, Color.red, 30, 5);
            if (((Vector3Clip)clip.asset).values.yEnabled)
                DrawCurvePreview(curveY, quantizedRect, Color.green, 30, 4);
            if (((Vector3Clip)clip.asset).values.zEnabled)
                DrawCurvePreview(curveZ, quantizedRect, Color.blue, 30, 3);
        }

        public AnimationCurve GetOrCreateCurve(TimelineClip clip, AnimationCurve currentCurve)
        {
            if (QualitySettings.activeColorSpace != m_ColorSpace)
            {
                m_ColorSpace = QualitySettings.activeColorSpace;
                m_PersistentCurves.Clear();
            }

            bool curveExists = m_PersistentCurves.TryGetValue(clip, out AnimationCurve curve);
            bool curveHasChanged = curve != null && curve != currentCurve;

            if (!curveExists || curveHasChanged)
            {
                curve = currentCurve;
                m_PersistentCurves[clip] = curve;
            }

            return curve;
        }

        public static void DrawCurvePreview(AnimationCurve curve, Rect rect, Color color, int samples = 25, float thickness = 2)
        {
            if (curve != null)
            {
                float startTime = curve.keys[0].time;
                float endTime = curve.keys[curve.keys.Length - 1].time;

                Vector3[] curvePoints = new Vector3[samples + 1];
                Color[] curveColours = new Color[samples + 1];

                for (int i = 0; i <= samples; i++)
                {
                    float normalizedTime = i / (float)samples;
                    float time = Mathf.Lerp(startTime, endTime, normalizedTime);
                    float value = curve.Evaluate(time);

                    float x = rect.x + rect.width * normalizedTime;
                    float y = rect.y + rect.height * (1 - value);

                    curvePoints[i] = new Vector3(x, y, 0);
                    curveColours[i] = color;
                }

                Handles.DrawAAPolyLine(thickness, curveColours, curvePoints);
            }
        }
    }

    [CustomEditor(typeof(Vector3Clip))]
    class Vector3ClipEditor : Editor
    {
        SerializedProperty worldProperty;
        SerializedProperty relativeProperty;

        SerializedProperty startAtProperty;
        SerializedProperty endAtProperty;
        SerializedProperty extrapolationProperty;

        SerializedProperty xEnabledProperty;
        SerializedProperty yEnabledProperty;
        SerializedProperty zEnabledProperty;
        SerializedProperty curveXProperty;
        SerializedProperty curveYProperty;
        SerializedProperty curveZProperty;

        public void OnEnable()
        {
            worldProperty = serializedObject.FindProperty("values.useWorldSpace");
            relativeProperty = serializedObject.FindProperty("values.useRelative");

            startAtProperty = serializedObject.FindProperty("values.startAt");
            endAtProperty = serializedObject.FindProperty("values.endAt");

            extrapolationProperty = serializedObject.FindProperty("values.clipExtrapolation");

            xEnabledProperty = serializedObject.FindProperty("values.xEnabled");
            yEnabledProperty = serializedObject.FindProperty("values.yEnabled");
            zEnabledProperty = serializedObject.FindProperty("values.zEnabled");
            curveXProperty = serializedObject.FindProperty("values.curveX");
            curveYProperty = serializedObject.FindProperty("values.curveY");
            curveZProperty = serializedObject.FindProperty("values.curveZ");
        }

        public override void OnInspectorGUI()
        {
            Vector3Clip clipTarget = target as Vector3Clip;

            EditorGUILayout.PropertyField(extrapolationProperty);
            EditorGUILayout.Space();

            if (clipTarget.values.spaceUnlocked)
            {
                EditorGUILayout.PropertyField(worldProperty);
                EditorGUILayout.Space();
            }

            if (clipTarget.values.relativeUnlocked)
            {
                EditorGUILayout.PropertyField(relativeProperty);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(startAtProperty);
            EditorGUILayout.PropertyField(endAtProperty);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Enabled Axis");
            xEnabledProperty.boolValue = EditorGUILayout.ToggleLeft("X", xEnabledProperty.boolValue, GUILayout.MinWidth(40));
            yEnabledProperty.boolValue = EditorGUILayout.ToggleLeft("Y", yEnabledProperty.boolValue, GUILayout.MinWidth(40));
            zEnabledProperty.boolValue = EditorGUILayout.ToggleLeft("Z", zEnabledProperty.boolValue, GUILayout.MinWidth(40));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (xEnabledProperty.boolValue)
                EditorGUILayout.PropertyField(curveXProperty);
            if (yEnabledProperty.boolValue)
                EditorGUILayout.PropertyField(curveYProperty);
            if (zEnabledProperty.boolValue)
                EditorGUILayout.PropertyField(curveZProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
