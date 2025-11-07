using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{  
    class TypedMotionClipTimelineEditor<T> : ClipEditor
    {
        readonly string k_NoCurveAssignedError = L10n.Tr("No curve assigned");
        readonly Dictionary<TimelineClip, AnimationCurve> m_PersistentCurves = new Dictionary<TimelineClip, AnimationCurve>();
        ColorSpace m_ColorSpace = ColorSpace.Uninitialized;

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            var controlAsset = clip.asset as TypedMotionClip<T>;
            if (controlAsset != null && controlAsset.values.curve == null)
                clipOptions.errorText = k_NoCurveAssignedError;
            return clipOptions;
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            var rect = region.position;
            if (rect.width <= 0)
                return;

            var curve = GetOrCreateCurve(clip);
            if (curve == null)
                return;

            var quantizedRect = new Rect(Mathf.Ceil(rect.x), Mathf.Ceil(rect.y), Mathf.Ceil(rect.width), Mathf.Ceil(rect.height));

            DrawCurvePreview(curve, quantizedRect);
        }

        public AnimationCurve GetOrCreateCurve(TimelineClip clip)
        {
            if (QualitySettings.activeColorSpace != m_ColorSpace)
            {
                m_ColorSpace = QualitySettings.activeColorSpace;
                m_PersistentCurves.Clear();
            }

            bool curveExists = m_PersistentCurves.TryGetValue(clip, out AnimationCurve curve);
            bool curveHasChanged = curve != null && curve != ((TypedMotionClip<T>)clip.asset).values.curve;

            if (!curveExists || curveHasChanged)
            {
                curve = CreateCurve(clip);
                m_PersistentCurves[clip] = curve;
            }

            return curve;
        }

        public static void DrawCurvePreview(AnimationCurve curve, Rect rect)
        {
            if (curve != null)
            {
                float startTime = curve.keys[0].time;
                float endTime = curve.keys[curve.keys.Length - 1].time;
                int sampleCount = 30;

                Vector3[] curvePoints = new Vector3[sampleCount + 1];

                for (int i = 0; i <= sampleCount; i++)
                {
                    float normalizedTime = i / (float)sampleCount;
                    float time = Mathf.Lerp(startTime, endTime, normalizedTime);
                    float value = curve.Evaluate(time);

                    float x = rect.x + rect.width * normalizedTime;
                    float y = rect.y + rect.height * (1 - value);

                    curvePoints[i] = new Vector3(x, y, 0);
                }

                Handles.DrawAAPolyLine(2, curvePoints);
            }
        }

        static AnimationCurve CreateCurve(TimelineClip clip)
        {
            return ((TypedMotionClip<T>)clip.asset).values.curve;
        }
    }

    [CustomTimelineEditor(typeof(FloatClip))]
    class FloatClipTimelineEditor : TypedMotionClipTimelineEditor<float> 
    { 
    }

    [CustomTimelineEditor(typeof(IntClip))]
    class IntClipTimelineEditor : TypedMotionClipTimelineEditor<int>
    {
    }

    [CustomTimelineEditor(typeof(DoubleClip))]
    class DoubleClipTimelineEditor : TypedMotionClipTimelineEditor<double>
    {
    }

    [CustomTimelineEditor(typeof(QuaternionClip))]
    class QuaternionClipTimelineEditor : TypedMotionClipTimelineEditor<Quaternion>
    {
    }

    class TypedMotionClipEditor : Editor
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

            EditorGUILayout.PropertyField(startAtProperty);
            EditorGUILayout.PropertyField(endAtProperty);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(curveProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(FloatClip))]
    class FloatClipEditor : TypedMotionClipEditor
    {
    }

    [CustomEditor(typeof(IntClip))]
    class IntClipEditor : TypedMotionClipEditor
    {
    }

    [CustomEditor(typeof(DoubleClip))]
    class DoubleClipEditor : TypedMotionClipEditor
    {
    }

    [CustomEditor(typeof(QuaternionClip))]
    class QuaternionClipEditor : TypedMotionClipEditor
    {
    }
}