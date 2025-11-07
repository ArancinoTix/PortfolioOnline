using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    /// <summary>
    /// Base UI Motion editor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class TypedMotionEditor<T, T2> : Editor where T : Component
    {
        // Visual brightness of catergory boxes
        protected const float boxBrightness = 0.2f;

        protected SerializedProperty identifierProperty;

        protected SerializedProperty targetProperty;
        protected SerializedProperty predifinedCurveProperty;
        protected SerializedProperty curveProperty;

        protected SerializedProperty startAtCurrentProperty;
        protected SerializedProperty startAtProperty;
        protected SerializedProperty endAtProperty;
        protected SerializedProperty scaleProperty;
        protected SerializedProperty durationProperty;
        protected SerializedProperty startDelayProperty;
        protected SerializedProperty endDelayProperty;
        protected SerializedProperty unscaledTimeProperty;

        protected SerializedProperty allowFinalizeProperty;
        protected SerializedProperty lockableProperty;

        protected SerializedProperty onPlayProperty;
        protected SerializedProperty onStartedProperty;
        protected SerializedProperty onFinishedProperty;

        protected SerializedProperty foldoutCurveDefProperty;
        protected SerializedProperty foldoutMotionSettingsProperty;
        protected SerializedProperty foldoutCallbackProperty;

        bool m_PreviewWasEnabled = false;
        bool m_PreviewEnabled = false;
        float m_PreviewValue = 0.0f;
        bool m_ResetPreview = false;
        bool m_Playback = false;
        float m_PreviewStartTime = 0.0f;
        float m_PreviewPauseTime = 0.0f;

        public void OnEnable()
        {
            identifierProperty = serializedObject.FindProperty("Identifier");

            targetProperty = serializedObject.FindProperty("Target");
            predifinedCurveProperty = serializedObject.FindProperty("PreDefinedCurve");
            curveProperty = serializedObject.FindProperty("Curve");

            startAtCurrentProperty = serializedObject.FindProperty("StartAtCurrent");
            startAtProperty = serializedObject.FindProperty("StartAt");
            endAtProperty = serializedObject.FindProperty("EndAt");
            scaleProperty = serializedObject.FindProperty("Scale");
            durationProperty = serializedObject.FindProperty("Duration");
            startDelayProperty = serializedObject.FindProperty("StartDelay");
            endDelayProperty = serializedObject.FindProperty("EndDelay"); 
            unscaledTimeProperty = serializedObject.FindProperty("UseUnscaledTime");

            allowFinalizeProperty = serializedObject.FindProperty("FinalizeOnInactivePlay");
            lockableProperty = serializedObject.FindProperty("Lockable");

            onPlayProperty = serializedObject.FindProperty("OnPlayMotion");
            onStartedProperty = serializedObject.FindProperty("OnStartedMotion");
            onFinishedProperty = serializedObject.FindProperty("OnFinishedMotion");

            foldoutCurveDefProperty = serializedObject.FindProperty("foldoutCurveDef");
            foldoutMotionSettingsProperty = serializedObject.FindProperty("foldoutMotionSettings");
            foldoutCallbackProperty = serializedObject.FindProperty("foldoutCallbacks");

            // When selected return the editor preivew value back to zero
            m_ResetPreview = true;   
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TypedMotion<T, T2> targetMotion = target as TypedMotion<T, T2>;

            // Reset the preview (this will happen if you select another object then this one again in the editor)
            if (m_ResetPreview)
            {
                m_PreviewEnabled = false;
                m_PreviewValue = 0.0f;

                if (targetMotion)
                    targetMotion.SetTime(m_PreviewValue);

                m_ResetPreview = false;
            }

            // Handy for selecting script
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            EditorGUILayout.Space();

            // Editor only
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.magenta * boxBrightness);
            {
                EditorGUILayout.Space();

                // Name the motion to make it easier to understand what will happen when played
                EditorGUILayout.PropertyField(identifierProperty);

                EditorGUILayout.BeginHorizontal();

                m_PreviewEnabled = EditorGUILayout.Toggle("Preview", m_PreviewEnabled);

                if (m_PreviewEnabled)
                {
                    m_PreviewWasEnabled = true;

                    if (!m_Playback)
                    {
                        if (GUILayout.Button("Play"))
                        {
                            m_Playback = true;
                            m_PreviewStartTime = Time.realtimeSinceStartup - m_PreviewPauseTime;
                        }
                    }
                    else if (GUILayout.Button("Pause"))
                    {
                        m_Playback = false;
                        m_PreviewPauseTime = Time.realtimeSinceStartup - m_PreviewStartTime;
                    }

                    if (GUILayout.Button("Stop"))
                    {
                        m_Playback = false;
                        m_PreviewPauseTime = 0.0f;
                        m_PreviewValue = 0.0f;
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }

                    if (targetMotion && m_Playback)
                    {
                        float elapsedTime = (Time.realtimeSinceStartup - m_PreviewStartTime);
                        m_PreviewValue = Mathf.Repeat(elapsedTime, targetMotion.GetMotionDuration());
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }

                    EditorGUILayout.EndHorizontal();

                    if (m_PreviewValue != (m_PreviewValue = EditorGUILayout.Slider(m_PreviewValue, 0.0f, targetMotion.GetMotionDuration())))
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

                    if (targetMotion)
                        targetMotion.SetTime(m_PreviewValue);
                }
                else
                {
                    if(m_PreviewWasEnabled)
                    {
                        m_ResetPreview = true;
                        m_Playback = false;
                        m_PreviewPauseTime = 0.0f;
                        m_PreviewValue = 0.0f;
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }

                    m_PreviewWasEnabled = false;

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.yellow * boxBrightness);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(targetProperty);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.red * boxBrightness);
            EditorGUILayout.Space();

            foldoutMotionSettingsProperty.isExpanded = EditorGUILayout.Foldout(foldoutMotionSettingsProperty.isExpanded, "Motion Settings", EditorStyles.foldoutHeader);
            if (foldoutMotionSettingsProperty.isExpanded)
            {
                EditorGUILayout.Space();
                switch (targetMotion.CurveUsage)
                {
                    case CurveUsage.NON_DIRECT:
                        EditorGUILayout.PropertyField(startAtCurrentProperty);
                        if(!targetMotion.StartAtCurrent)
                            EditorGUILayout.PropertyField(startAtProperty);
                        EditorGUILayout.PropertyField(endAtProperty);
                        break;
                    case CurveUsage.DIRECT_WITH_SCALE:
                        EditorGUILayout.PropertyField(scaleProperty);
                        break;
                }
                EditorGUILayout.PropertyField(durationProperty);
                EditorGUILayout.PropertyField(startDelayProperty);
                EditorGUILayout.PropertyField(endDelayProperty);
                EditorGUILayout.PropertyField(unscaledTimeProperty);
                EditorGUILayout.PropertyField(allowFinalizeProperty);
                EditorGUILayout.PropertyField(lockableProperty);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (predifinedCurveProperty != null)
            {
                EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
                EditorGUILayout.Space();

                foldoutCurveDefProperty.isExpanded = EditorGUILayout.Foldout(foldoutCurveDefProperty.isExpanded, "Curve Definition", EditorStyles.foldoutHeader);

                if (foldoutCurveDefProperty.isExpanded)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.ObjectField(predifinedCurveProperty);
                    EditorGUILayout.Space();

                    if (targetMotion.PreDefinedCurve != null)
                    {
                        // Copy keys from predefined
                        targetMotion.Curve.keys = targetMotion.PreDefinedCurve.Curve.keys;

                        // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                        GUI.enabled = false;
                    }
                    EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
                    EditorGUILayout.LabelField("Preview");
                    EditorGUILayout.CurveField("", targetMotion.Curve, GUILayout.Height(200));
                    EditorGUILayout.EndVertical();
                    GUI.enabled = true;
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // Callbacks
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.green * boxBrightness);
            {
                EditorGUILayout.Space();

                // When at certain stages Invoke these Unity Events so other things can be triggered
                foldoutCallbackProperty.isExpanded = EditorGUILayout.Foldout(foldoutCallbackProperty.isExpanded, new GUIContent("Callbacks", "OnPlayMotion: Event that will be played immediately when the Player is told to Play\n\n" +
                    "OnStartedMotion: Event that will be played just as the Motion starts (Can be delayed using Start Delay\n\n" +
                    "OnFinishedMotion: Event that will be played once all Motions have been finished (Can be delayed using End Delay)"), EditorStyles.foldoutHeader);
                if (foldoutCallbackProperty.isExpanded)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(onPlayProperty);
                    EditorGUILayout.PropertyField(onStartedProperty);
                    EditorGUILayout.PropertyField(onFinishedProperty);
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    public class ColourTypedMotionEditor<T, T2> : Editor where T : Component
    {
        // Visual brightness of catergory boxes
        protected const float boxBrightness = 0.2f;

        protected SerializedProperty identifierProperty;

        protected SerializedProperty targetProperty;
        protected SerializedProperty predifinedCurveProperty;
        protected SerializedProperty curveProperty;

        protected SerializedProperty startAtCurrentProperty;
        protected SerializedProperty startAtProperty;
        protected SerializedProperty endAtProperty;
        protected SerializedProperty scaleProperty;
        protected SerializedProperty durationProperty;
        protected SerializedProperty startDelayProperty;
        protected SerializedProperty endDelayProperty;
        protected SerializedProperty unscaledTimeProperty;

        protected SerializedProperty allowFinalizeProperty;
        protected SerializedProperty lockableProperty;

        protected SerializedProperty onPlayProperty;
        protected SerializedProperty onStartedProperty;
        protected SerializedProperty onFinishedProperty;

        protected SerializedProperty foldoutCurveDefProperty;
        protected SerializedProperty foldoutMotionSettingsProperty;
        protected SerializedProperty foldoutCallbackProperty;

        bool m_PreviewWasEnabled = false;
        bool m_PreviewEnabled = false;
        float m_PreviewValue = 0.0f;
        bool m_ResetPreview = false;
        bool m_Playback = false;
        float m_PreviewStartTime = 0.0f;
        float m_PreviewPauseTime = 0.0f;

        public void OnEnable()
        {
            identifierProperty = serializedObject.FindProperty("Identifier");

            targetProperty = serializedObject.FindProperty("Target");
            predifinedCurveProperty = serializedObject.FindProperty("PreDefinedCurve");
            curveProperty = serializedObject.FindProperty("Curve");

            startAtCurrentProperty = serializedObject.FindProperty("StartAtCurrent");
            startAtProperty = serializedObject.FindProperty("StartAt");
            endAtProperty = serializedObject.FindProperty("EndAt");
            scaleProperty = serializedObject.FindProperty("Scale");
            durationProperty = serializedObject.FindProperty("Duration");
            startDelayProperty = serializedObject.FindProperty("StartDelay");
            endDelayProperty = serializedObject.FindProperty("EndDelay");
            unscaledTimeProperty = serializedObject.FindProperty("UseUnscaledTime");

            allowFinalizeProperty = serializedObject.FindProperty("FinalizeOnInactivePlay");
            lockableProperty = serializedObject.FindProperty("Lockable");

            onPlayProperty = serializedObject.FindProperty("OnPlayMotion");
            onStartedProperty = serializedObject.FindProperty("OnStartedMotion");
            onFinishedProperty = serializedObject.FindProperty("OnFinishedMotion");

            foldoutCurveDefProperty = serializedObject.FindProperty("foldoutCurveDef");
            foldoutMotionSettingsProperty = serializedObject.FindProperty("foldoutMotionSettings");
            foldoutCallbackProperty = serializedObject.FindProperty("foldoutCallbacks");

            // When selected return the editor preivew value back to zero
            m_ResetPreview = true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ColourTypedMotion<T, T2> targetMotion = target as ColourTypedMotion<T, T2>;

            // Reset the preview (this will happen if you select another object then this one again in the editor)
            if (m_ResetPreview)
            {
                m_PreviewEnabled = false;
                m_PreviewValue = 0.0f;

                if (targetMotion)
                    targetMotion.SetTime(m_PreviewValue);

                m_ResetPreview = false;
            }

            // Handy for selecting script
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            EditorGUILayout.Space();

            // Editor only
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.magenta * boxBrightness);
            {
                EditorGUILayout.Space();

                // Name the motion to make it easier to understand what will happen when played
                EditorGUILayout.PropertyField(identifierProperty);

                EditorGUILayout.BeginHorizontal();

                m_PreviewEnabled = EditorGUILayout.Toggle("Preview", m_PreviewEnabled);

                if (m_PreviewEnabled)
                {
                    m_PreviewWasEnabled = true;

                    if (!m_Playback)
                    {
                        if (GUILayout.Button("Play"))
                        {
                            m_Playback = true;
                            m_PreviewStartTime = Time.realtimeSinceStartup - m_PreviewPauseTime;
                        }
                    }
                    else if (GUILayout.Button("Pause"))
                    {
                        m_Playback = false;
                        m_PreviewPauseTime = Time.realtimeSinceStartup - m_PreviewStartTime;
                    }

                    if (GUILayout.Button("Stop"))
                    {
                        m_Playback = false;
                        m_PreviewPauseTime = 0.0f;
                        m_PreviewValue = 0.0f;
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }

                    if (targetMotion && m_Playback)
                    {
                        float elapsedTime = (Time.realtimeSinceStartup - m_PreviewStartTime);
                        m_PreviewValue = Mathf.Repeat(elapsedTime, targetMotion.GetMotionDuration());
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }

                    EditorGUILayout.EndHorizontal();

                    if (m_PreviewValue != (m_PreviewValue = EditorGUILayout.Slider(m_PreviewValue, 0.0f, targetMotion.GetMotionDuration())))
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

                    if (targetMotion)
                        targetMotion.SetTime(m_PreviewValue);
                }
                else
                {
                    if (m_PreviewWasEnabled)
                    {
                        m_ResetPreview = true;
                        m_Playback = false;
                        m_PreviewPauseTime = 0.0f;
                        m_PreviewValue = 0.0f;
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }

                    m_PreviewWasEnabled = false;

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.yellow * boxBrightness);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(targetProperty);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.red * boxBrightness);
            EditorGUILayout.Space();

            foldoutMotionSettingsProperty.isExpanded = EditorGUILayout.Foldout(foldoutMotionSettingsProperty.isExpanded, "Motion Settings", EditorStyles.foldoutHeader);
            if (foldoutMotionSettingsProperty.isExpanded)
            {
                EditorGUILayout.Space();
                switch (targetMotion.CurveUsage)
                {
                    case CurveUsage.NON_DIRECT:
                        EditorGUILayout.PropertyField(startAtCurrentProperty);
                        if (!targetMotion.StartAtCurrent)
                            EditorGUILayout.PropertyField(startAtProperty);
                        EditorGUILayout.PropertyField(endAtProperty);
                        break;
                    case CurveUsage.DIRECT_WITH_SCALE:
                        EditorGUILayout.PropertyField(scaleProperty);
                        break;
                }
                EditorGUILayout.PropertyField(durationProperty);
                EditorGUILayout.PropertyField(startDelayProperty);
                EditorGUILayout.PropertyField(endDelayProperty);
                EditorGUILayout.PropertyField(unscaledTimeProperty);
                EditorGUILayout.PropertyField(allowFinalizeProperty);
                EditorGUILayout.PropertyField(lockableProperty);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (predifinedCurveProperty != null)
            {
                EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
                EditorGUILayout.Space();

                foldoutCurveDefProperty.isExpanded = EditorGUILayout.Foldout(foldoutCurveDefProperty.isExpanded, "Curve Definition", EditorStyles.foldoutHeader);

                if (foldoutCurveDefProperty.isExpanded)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.ObjectField(predifinedCurveProperty);
                    EditorGUILayout.Space();

                    if (targetMotion.PreDefinedCurve != null)
                    {
                        // Copy keys from predefined
                        targetMotion.Curve.keys = targetMotion.PreDefinedCurve.Curve.keys;

                        // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                        GUI.enabled = false;
                    }
                    EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
                    EditorGUILayout.LabelField("Preview");
                    EditorGUILayout.CurveField("", targetMotion.Curve, GUILayout.Height(200));
                    EditorGUILayout.EndVertical();
                    GUI.enabled = true;
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // Callbacks
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.green * boxBrightness);
            {
                EditorGUILayout.Space();

                // When at certain stages Invoke these Unity Events so other things can be triggered
                foldoutCallbackProperty.isExpanded = EditorGUILayout.Foldout(foldoutCallbackProperty.isExpanded, new GUIContent("Callbacks", "OnPlayMotion: Event that will be played immediately when the Player is told to Play\n\n" +
                    "OnStartedMotion: Event that will be played just as the Motion starts (Can be delayed using Start Delay\n\n" +
                    "OnFinishedMotion: Event that will be played once all Motions have been finished (Can be delayed using End Delay)"), EditorStyles.foldoutHeader);
                if (foldoutCallbackProperty.isExpanded)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(onPlayProperty);
                    EditorGUILayout.PropertyField(onStartedProperty);
                    EditorGUILayout.PropertyField(onFinishedProperty);
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// Additive Editor for UI Motions with Curve Usage enabled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class CurveUsageEnabledEditor<T, T2> : TypedMotionEditor<T, T2> where T : Component
    {
        SerializedProperty curveUsageProperty;

        public new void OnEnable()
        {
            base.OnEnable();

            curveUsageProperty = serializedObject.FindProperty("CurveUsage");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(curveUsageProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// Additive Editor for UI Motions with 3 optional axis
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class ThreeAxisEditor<T, T2> : CurveUsageEnabledEditor<T, T2> where T : Component
    {
        SerializedProperty localSpaceProperty;

        SerializedProperty definitionXProperty;
        SerializedProperty definitionYProperty;
        SerializedProperty definitionZProperty;

        SerializedProperty foldoutCurveDefsProperty;

        public new void OnEnable()
        {
            base.OnEnable();

            // Standard definition property disabled
            predifinedCurveProperty = null;

            localSpaceProperty = serializedObject.FindProperty("LocalSpace");

            definitionXProperty = serializedObject.FindProperty("PreDefinedCurveX");
            definitionYProperty = serializedObject.FindProperty("PreDefinedCurveY");
            definitionZProperty = serializedObject.FindProperty("PreDefinedCurveZ");

            foldoutCurveDefsProperty = serializedObject.FindProperty("foldoutCurveDefs");

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (localSpaceProperty != null)
                EditorGUILayout.PropertyField(localSpaceProperty);
            EditorGUILayout.Space();

            ThreeAxisMotion<T, T2> typedMotionTarget = target as ThreeAxisMotion<T, T2>;

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
            EditorGUILayout.Space();

            foldoutCurveDefsProperty.isExpanded = EditorGUILayout.Foldout(foldoutCurveDefsProperty.isExpanded, "Curve Definition(s)", EditorStyles.foldoutHeader);

            if (foldoutCurveDefsProperty.isExpanded)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Enabled Axis");
                typedMotionTarget.XEnabled = EditorGUILayout.ToggleLeft("X", typedMotionTarget.XEnabled, GUILayout.MinWidth(40));
                typedMotionTarget.YEnabled = EditorGUILayout.ToggleLeft("Y", typedMotionTarget.YEnabled, GUILayout.MinWidth(40));
                typedMotionTarget.ZEnabled = EditorGUILayout.ToggleLeft("Z", typedMotionTarget.ZEnabled, GUILayout.MinWidth(40));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                int numberOfAxisEnabled = 0;

                if (typedMotionTarget.XEnabled)
                {
                    EditorGUILayout.PropertyField(definitionXProperty);
                    numberOfAxisEnabled++;
                }
                if (typedMotionTarget.YEnabled)
                {
                    EditorGUILayout.PropertyField(definitionYProperty);
                    numberOfAxisEnabled++;
                }
                if (typedMotionTarget.ZEnabled)
                {
                    EditorGUILayout.PropertyField(definitionZProperty);
                    numberOfAxisEnabled++;
                }

                EditorGUILayout.Space();

                if (numberOfAxisEnabled > 0)
                {
                    EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
                    EditorGUILayout.LabelField(numberOfAxisEnabled > 1 ? "Curves" : "Curve");

                    EditorGUILayout.BeginHorizontal();

                    if (typedMotionTarget.XEnabled)
                    {
                        EditorGUILayout.BeginVertical();

                        if (typedMotionTarget.PreDefinedCurveX != null)
                        {
                            // Copy keys from predefined
                            typedMotionTarget.CurveX.keys = typedMotionTarget.PreDefinedCurveX.Curve.keys;
                            // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                            GUI.enabled = false;
                        }
                        
                        EditorGUILayout.CurveField("", typedMotionTarget.CurveX, Color.red, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                        GUI.enabled = true;

                        EditorGUILayout.EndVertical();
                    }
                    if (typedMotionTarget.YEnabled)
                    {
                        EditorGUILayout.BeginVertical();

                        if (typedMotionTarget.PreDefinedCurveY != null)
                        {
                            // Copy keys from predefined
                            typedMotionTarget.CurveY.keys = typedMotionTarget.PreDefinedCurveY.Curve.keys;
                            // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                            GUI.enabled = false;
                        }

                        EditorGUILayout.CurveField("", typedMotionTarget.CurveY, Color.green, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                        GUI.enabled = true;

                        EditorGUILayout.EndVertical();
                    }
                    if (typedMotionTarget.ZEnabled)
                    {
                        EditorGUILayout.BeginVertical();

                        if (typedMotionTarget.PreDefinedCurveZ != null)
                        {
                            // Copy keys from predefined
                            typedMotionTarget.CurveZ.keys = typedMotionTarget.PreDefinedCurveZ.Curve.keys;
                            // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                            GUI.enabled = false;
                        }

                        EditorGUILayout.CurveField("", typedMotionTarget.CurveZ, Color.blue, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                        GUI.enabled = true;

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// Additive Editor for UI Motions with 2 optional axis
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class TwoAxisEditor<T, T2> : CurveUsageEnabledEditor<T, T2> where T : Component
    {
        SerializedProperty localSpaceProperty;

        SerializedProperty definitionXProperty;
        SerializedProperty definitionYProperty;

        SerializedProperty foldoutCurveDefsProperty;

        public new void OnEnable()
        {
            base.OnEnable();

            // Standard definition property disabled
            predifinedCurveProperty = null;

            localSpaceProperty = serializedObject.FindProperty("LocalSpace");

            definitionXProperty = serializedObject.FindProperty("PreDefinedCurveX");
            definitionYProperty = serializedObject.FindProperty("PreDefinedCurveY");

            foldoutCurveDefsProperty = serializedObject.FindProperty("foldoutCurveDefs");

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (localSpaceProperty != null)
                EditorGUILayout.PropertyField(localSpaceProperty);
            EditorGUILayout.Space();

            TwoAxisMotion<T, T2> typedMotionTarget = target as TwoAxisMotion<T, T2>;

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
            EditorGUILayout.Space();

            foldoutCurveDefsProperty.isExpanded = EditorGUILayout.Foldout(foldoutCurveDefsProperty.isExpanded, "Curve Definition(s)", EditorStyles.foldoutHeader);

            if (foldoutCurveDefsProperty.isExpanded)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Enabled Axis");
                typedMotionTarget.XEnabled = EditorGUILayout.ToggleLeft("X", typedMotionTarget.XEnabled, GUILayout.MinWidth(40));
                typedMotionTarget.YEnabled = EditorGUILayout.ToggleLeft("Y", typedMotionTarget.YEnabled, GUILayout.MinWidth(40));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                int numberOfAxisEnabled = 0;

                if (typedMotionTarget.XEnabled)
                {
                    EditorGUILayout.PropertyField(definitionXProperty);
                    numberOfAxisEnabled++;
                }
                if (typedMotionTarget.YEnabled)
                {
                    EditorGUILayout.PropertyField(definitionYProperty);
                    numberOfAxisEnabled++;
                }

                EditorGUILayout.Space();

                if (numberOfAxisEnabled > 0)
                {
                    EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
                    EditorGUILayout.LabelField(numberOfAxisEnabled > 1 ? "Curves" : "Curve");

                    EditorGUILayout.BeginHorizontal();

                    if (typedMotionTarget.XEnabled)
                    {
                        EditorGUILayout.BeginVertical();

                        if (typedMotionTarget.PreDefinedCurveX != null)
                        {
                            // Copy keys from predefined
                            typedMotionTarget.CurveX.keys = typedMotionTarget.PreDefinedCurveX.Curve.keys;
                            // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                            GUI.enabled = false;
                        }

                        EditorGUILayout.CurveField("", typedMotionTarget.CurveX, Color.red, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                        GUI.enabled = true;

                        EditorGUILayout.EndVertical();
                    }
                    if (typedMotionTarget.YEnabled)
                    {
                        EditorGUILayout.BeginVertical();

                        if (typedMotionTarget.PreDefinedCurveY != null)
                        {
                            // Copy keys from predefined
                            typedMotionTarget.CurveY.keys = typedMotionTarget.PreDefinedCurveY.Curve.keys;
                            // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                            GUI.enabled = false;
                        }

                        EditorGUILayout.CurveField("", typedMotionTarget.CurveY, Color.green, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                        GUI.enabled = true;

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(FadeCanvasGroup))]
    [CanEditMultipleObjects]
    public class FadeCanvasGroupEditor : TypedMotionEditor<CanvasGroup, float>
    {
        SerializedProperty interactabilityProperty;
        SerializedProperty raycastBlockingProperty;
        SerializedProperty alphaProperty;

        public new void OnEnable()
        {
            base.OnEnable();

            interactabilityProperty = serializedObject.FindProperty("AdjustInteractability");
            raycastBlockingProperty = serializedObject.FindProperty("AdjustRaycastBlocking");
            alphaProperty = serializedObject.FindProperty("AlphaBeforeInteractable");
        }
        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(interactabilityProperty);

            FadeCanvasGroup typedMotionTarget = target as FadeCanvasGroup;
            if (typedMotionTarget.AdjustInteractability)
            {
                EditorGUILayout.PropertyField(raycastBlockingProperty);
                EditorGUILayout.PropertyField(alphaProperty);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(AudioVolume))]
    [CanEditMultipleObjects]
    public class AudioVolumeEditor : TypedMotionEditor<AudioSource, float>
    {
    }

    [CustomEditor(typeof(FontColour))]
    [CanEditMultipleObjects]
    public class FontColourEditor : TypedMotionEditor<TMP_Text, Color>
    {
    }

    [CustomEditor(typeof(FontSize))]
    [CanEditMultipleObjects]
    public class FontSizeEditor : CurveUsageEnabledEditor<TMP_Text, float>
    {
    }

    [CustomEditor(typeof(RawImageColour))]
    [CanEditMultipleObjects]
    public class RawImageColourEditor : TypedMotionEditor<RawImage, Color>
    {
    }

    [CustomEditor(typeof(ImageColour))]
    [CanEditMultipleObjects]
    public class ImageColourEditor : TypedMotionEditor<Image, Color>
    {
    }

    [CustomEditor(typeof(ImageFill))]
    [CanEditMultipleObjects]
    public class ImageFillEditor : TypedMotionEditor<Image, float>
    {
    }

    [CustomEditor(typeof(ScrollbarValue))]
    [CanEditMultipleObjects]
    public class ScrollbarValueEditor : TypedMotionEditor<Scrollbar, float>
    {
    }

    [CustomEditor(typeof(TextBody))]
    [CanEditMultipleObjects]
    public class TextBodyEditor : Editor
    {
        // Visual brightness of catergory boxes
        protected const float boxBrightness = 0.2f;

        protected SerializedProperty identifierProperty;

        protected SerializedProperty targetProperty;

        protected SerializedProperty definition1Property;
        protected SerializedProperty definition2Property;
        protected SerializedProperty curve1Property;
        protected SerializedProperty curve2Property;

        protected SerializedProperty startAtProperty;
        protected SerializedProperty endAtProperty;
        protected SerializedProperty scaleProperty;
        protected SerializedProperty durationProperty;
        protected SerializedProperty startDelayProperty;
        protected SerializedProperty endDelayProperty;

        protected SerializedProperty allowFinalizeProperty;

        protected SerializedProperty startingDistanceProperty;

        protected SerializedProperty foldoutCurveDefProperty;
        protected SerializedProperty foldoutMotionSettingsProperty;

        public void OnEnable()
        {
            identifierProperty = serializedObject.FindProperty("Identifier");

            targetProperty = serializedObject.FindProperty("Target");

            definition1Property = serializedObject.FindProperty("PreDefinedCurve");
            definition2Property = serializedObject.FindProperty("PreDefinedCurve2");
            curve1Property = serializedObject.FindProperty("Curve");
            curve2Property = serializedObject.FindProperty("Curve2");

            startAtProperty = serializedObject.FindProperty("StartAt");
            endAtProperty = serializedObject.FindProperty("EndAt");
            scaleProperty = serializedObject.FindProperty("Scale");
            durationProperty = serializedObject.FindProperty("Duration");
            startDelayProperty = serializedObject.FindProperty("StartDelay");
            endDelayProperty = serializedObject.FindProperty("EndDelay");

            allowFinalizeProperty = serializedObject.FindProperty("FinalizeOnInactivePlay");

            startingDistanceProperty = serializedObject.FindProperty("StartingDistance");

            foldoutCurveDefProperty = serializedObject.FindProperty("foldoutCurveDef");
            foldoutMotionSettingsProperty = serializedObject.FindProperty("foldoutMotionSettings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Handy for selecting script
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            EditorGUILayout.Space();

            // Name the motion to make it easier to understand what will happen when played
            EditorGUILayout.PropertyField(identifierProperty);
            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.yellow * boxBrightness);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(targetProperty);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.red * boxBrightness);
            EditorGUILayout.Space();

            TextBody motionTarget = target as TextBody;

            foldoutMotionSettingsProperty.isExpanded = EditorGUILayout.Foldout(foldoutMotionSettingsProperty.isExpanded, "Motion Settings", EditorStyles.foldoutHeader);
            if (foldoutMotionSettingsProperty.isExpanded)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(durationProperty);
                EditorGUILayout.PropertyField(startDelayProperty);
                EditorGUILayout.PropertyField(endDelayProperty);
                EditorGUILayout.PropertyField(allowFinalizeProperty);
                EditorGUILayout.PropertyField(startingDistanceProperty);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
            EditorGUILayout.Space();

            foldoutCurveDefProperty.isExpanded = EditorGUILayout.Foldout(foldoutCurveDefProperty.isExpanded, "Curve Definitions", EditorStyles.foldoutHeader);

            if (foldoutCurveDefProperty.isExpanded)
            {
                EditorGUILayout.Space();
                EditorGUILayout.ObjectField(definition1Property);
                EditorGUILayout.Space();

                if (motionTarget.PreDefinedCurve != null)
                {
                    // Copy keys from predefined
                    motionTarget.Curve.keys = motionTarget.PreDefinedCurve.Curve.keys;

                    // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                    GUI.enabled = false;
                }
                EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
                EditorGUILayout.LabelField("Preview");
                EditorGUILayout.CurveField("", motionTarget.Curve, GUILayout.Height(200));
                EditorGUILayout.EndVertical();
                GUI.enabled = true;

                EditorGUILayout.Space();
                EditorGUILayout.ObjectField(definition2Property);
                EditorGUILayout.Space();

                if (motionTarget.PreDefinedCurve2 != null)
                {
                    // Copy keys from predefined
                    motionTarget.Curve2.keys = motionTarget.PreDefinedCurve2.Curve.keys;

                    // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                    GUI.enabled = false;
                }
                EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
                EditorGUILayout.LabelField("Preview");
                EditorGUILayout.CurveField("", motionTarget.Curve2, GUILayout.Height(200));
                EditorGUILayout.EndVertical();
                GUI.enabled = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(ImageSequence))]
    [CanEditMultipleObjects]
    public class ImageSequenceEditor : TypedMotionEditor<Image, int>
    {
        SerializedProperty spritesProperty;

        public new void OnEnable()
        {
            base.OnEnable();

            // Definition property disabled as no Curve is used for Image Sequence
            predifinedCurveProperty = null;

            spritesProperty = serializedObject.FindProperty("Sprites");

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spritesProperty);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(MoveIn))]
    [CanEditMultipleObjects]
    public class MoveInEditor : ThreeAxisEditor<Transform, Vector3>
    {
    }

    [CustomEditor(typeof(MoveInRect))]
    [CanEditMultipleObjects]
    public class MoveInRectEditor : ThreeAxisEditor<RectTransform, Vector3>
    {
    }

    [CustomEditor(typeof(RotateIn))]
    [CanEditMultipleObjects]
    public class RotateInEditor : ThreeAxisEditor<Transform, Vector3>
    {
    }

    [CustomEditor(typeof(ScaleIn))]
    [CanEditMultipleObjects]
    public class ScaleInEditor : ThreeAxisEditor<Transform, Vector3>
    {
    }

    [CustomEditor(typeof(ScaleInRect))]
    [CanEditMultipleObjects]
    public class ScaleInRectEditor : TwoAxisEditor<RectTransform, Vector2>
    {
    }

    

    [CustomEditor(typeof(MotionShaderVector))]
    [CanEditMultipleObjects]
    public class MotionShaderVectorEditor : ThreeAxisEditor<MaterialComponent, Vector3>
    {
        SerializedProperty parameterNameProperty;
        SerializedProperty globalProperty;

        public new void OnEnable()
        {
            base.OnEnable();
            parameterNameProperty = serializedObject.FindProperty("ParameterName");
            globalProperty = serializedObject.FindProperty("GlobalParameter");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(parameterNameProperty);
            EditorGUILayout.PropertyField(globalProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(MotionShaderColour))]
    [CanEditMultipleObjects]
    public class MotionShaderColourEditor : ColourTypedMotionEditor<MaterialComponent, Color>
    {
        SerializedProperty parameterNameProperty;
        SerializedProperty globalProperty;

        public new void OnEnable()
        {
            base.OnEnable();
            parameterNameProperty = serializedObject.FindProperty("ParameterName");
            globalProperty = serializedObject.FindProperty("GlobalParameter");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(parameterNameProperty);
            EditorGUILayout.PropertyField(globalProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(MotionShaderFloat))]
    [CanEditMultipleObjects]
    public class MotionShaderFloatEditor : TypedMotionEditor<MaterialComponent, float>
    {
        SerializedProperty parameterNameProperty;
        SerializedProperty globalProperty;

        public new void OnEnable()
        {
            base.OnEnable();
            parameterNameProperty = serializedObject.FindProperty("ParameterName");
            globalProperty = serializedObject.FindProperty("GlobalParameter");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(parameterNameProperty);
            EditorGUILayout.PropertyField(globalProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(ImageSequenceMaterial))]
    [CanEditMultipleObjects]
    public class ImageSequenceMaterialEditor : TypedMotionEditor<MaterialComponent, int>
    {
        SerializedProperty spritesProperty;

        SerializedProperty parameterNameProperty;
        SerializedProperty globalProperty;

        public new void OnEnable()
        {
            base.OnEnable();

            // Definition property disabled as no Curve is used for Image Sequence
            predifinedCurveProperty = null;

            spritesProperty = serializedObject.FindProperty("Sprites");

            parameterNameProperty = serializedObject.FindProperty("ParameterName");
            globalProperty = serializedObject.FindProperty("GlobalParameter");

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spritesProperty);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(parameterNameProperty);
            EditorGUILayout.PropertyField(globalProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}