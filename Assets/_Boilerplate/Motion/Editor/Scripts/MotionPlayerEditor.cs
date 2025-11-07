using UnityEngine;
using UnityEditor;

namespace U9.Motion
{
    [CustomEditor(typeof(MotionPlayer))]
    [CanEditMultipleObjects]
    public class MotionPlayerEditor : Editor
    {
        // Visual brightness of catergory boxes
        protected const float boxBrightness = 0.2f;

        SerializedProperty identifierProperty;

        SerializedProperty motionsProperty;

        SerializedProperty onPlayProperty;
        SerializedProperty onStartedProperty;
        SerializedProperty onFinishedProperty;

        SerializedProperty playOnEnableProperty;
        SerializedProperty playMethodProperty;
        SerializedProperty unscaledTimeProperty;
        SerializedProperty replayMethodProperty;
        SerializedProperty staggerDelayProperty;

        SerializedProperty overrideDurationProperty;
        SerializedProperty durationProperty;
        SerializedProperty startDelayProperty;
        SerializedProperty endDelayProperty;
        SerializedProperty allowFinalizeProperty;
        protected SerializedProperty lockableProperty;

        SerializedProperty foldoutPlaybackSettingsProperty;
        SerializedProperty foldoutCallbackProperty;

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

            motionsProperty = serializedObject.FindProperty("Motions");

            onPlayProperty = serializedObject.FindProperty("OnPlayMotion"); 
            onStartedProperty = serializedObject.FindProperty("OnStartedMotion");
            onFinishedProperty = serializedObject.FindProperty("OnFinishedMotion");

            playOnEnableProperty = serializedObject.FindProperty("PlayOnEnable");
            playMethodProperty = serializedObject.FindProperty("PlayMethod");
            unscaledTimeProperty = serializedObject.FindProperty("UseUnscaledTime");
            replayMethodProperty = serializedObject.FindProperty("ReplayMethod");
            staggerDelayProperty = serializedObject.FindProperty("StaggerDelay");

            overrideDurationProperty = serializedObject.FindProperty("OverrideDuration");
            durationProperty = serializedObject.FindProperty("DurationOverride");
            startDelayProperty = serializedObject.FindProperty("StartDelay");
            endDelayProperty = serializedObject.FindProperty("EndDelay");
            allowFinalizeProperty = serializedObject.FindProperty("FinalizeOnInactivePlay");
            lockableProperty = serializedObject.FindProperty("Lockable");

            foldoutPlaybackSettingsProperty = serializedObject.FindProperty("foldoutPlaybackSettings");
            foldoutCallbackProperty = serializedObject.FindProperty("foldoutCallbacks");

            // When selected return the editor preivew value back to zero
            m_ResetPreview = true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MotionPlayer targetMotion = target as MotionPlayer;

            // Reset the preview (this will happen if you select another object then this one again in the editor)
            if (m_ResetPreview)
            {
                m_PreviewEnabled = false;
                m_PreviewValue = 0.0f;

                if (targetMotion)
                    targetMotion.SetTime(m_PreviewValue);

                m_ResetPreview = false;
            }

            // Handy for seeing the name of the script and select/link to it
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

                    targetMotion.CalculateDurationMultiplier();

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

            // Motions
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.yellow * boxBrightness);
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(motionsProperty);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Play Settings
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.red * boxBrightness);
            {
                EditorGUILayout.Space();
                foldoutPlaybackSettingsProperty.isExpanded = EditorGUILayout.Foldout(foldoutPlaybackSettingsProperty.isExpanded, "Play Settings", EditorStyles.foldoutHeader);

                if (foldoutPlaybackSettingsProperty.isExpanded)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(playOnEnableProperty);
                    EditorGUILayout.PropertyField(playMethodProperty);
                    EditorGUILayout.PropertyField(replayMethodProperty);
                    EditorGUILayout.PropertyField(unscaledTimeProperty);

                    if (targetMotion.PlayMethod == PlayType.STAGGERED)
                        EditorGUILayout.PropertyField(staggerDelayProperty);
                    else
                    {
                        EditorGUILayout.PropertyField(overrideDurationProperty);
                        if (targetMotion.OverrideDuration)
                            EditorGUILayout.PropertyField(durationProperty);
                    }

                    EditorGUILayout.PropertyField(startDelayProperty);
                    EditorGUILayout.PropertyField(endDelayProperty);
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

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

            EditorGUILayout.Space();

            // Additional settings, like should this player play OnFinalize on motions that are disabled
            // Each Motion also has this property as well, to ignore this if not wanted
            EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(allowFinalizeProperty);
            EditorGUILayout.PropertyField(lockableProperty);

            EditorGUILayout.Space();
            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Currently Playing Motion(s)");
            foreach (BaseMotion motion in targetMotion.CurrentlyPlayingMotions)
            {
                EditorGUILayout.LabelField(" - " + motion.name + " (" + motion.Identifier + ")");
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();               
        }
    }
}