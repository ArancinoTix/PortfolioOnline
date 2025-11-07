using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace U9.Motion
{
    [CustomEditor(typeof(CustomMotion))]
    [CanEditMultipleObjects]
    public class CustomMotionEditor : Editor
    {
        // Visual brightness of catergory boxes
        private const float boxBrightness = 0.2f;

        SerializedProperty identifierProperty;

        SerializedProperty targetProperty;
        SerializedProperty findProperty;

        SerializedProperty modifierProperty;
        SerializedProperty selectedProperty;

        SerializedProperty startAtCurrentProperty;

        private readonly GUIContent startAt = new GUIContent("Start At");
        private readonly GUIContent endAt = new GUIContent("End At");
        private readonly GUIContent scale = new GUIContent("Scale");

        SerializedProperty floatStartAtValueProperty;
        SerializedProperty floatEndAtValueProperty;
        SerializedProperty floatScaleValueProperty;

        SerializedProperty intStartAtValueProperty;
        SerializedProperty intEndAtValueProperty;
        SerializedProperty intScaleValueProperty;

        SerializedProperty vector2StartAtValueProperty;
        SerializedProperty vector2EndAtValueProperty;
        SerializedProperty vector2ScaleValueProperty;

        SerializedProperty vector3StartAtValueProperty;
        SerializedProperty vector3EndAtValueProperty;
        SerializedProperty vector3ScaleValueProperty;

        SerializedProperty vector4StartAtValueProperty;
        SerializedProperty vector4EndAtValueProperty;
        SerializedProperty vector4ScaleValueProperty;

        SerializedProperty colorStartAtValueProperty;
        SerializedProperty colorEndAtValueProperty;
        SerializedProperty colorScaleValueProperty;

        SerializedProperty spritesProperty;

        SerializedProperty durationProperty;
        SerializedProperty startDelayProperty;
        SerializedProperty endDelayProperty;
        SerializedProperty unscaledTimeProperty;

        SerializedProperty allowFinalizeProperty;
        SerializedProperty lockableProperty;

        SerializedProperty onPlayProperty;
        SerializedProperty onStartedProperty;
        SerializedProperty onFinishedProperty;

        SerializedProperty curveUsageProperty;

        SerializedProperty foldoutCurveDefProperty;
        SerializedProperty foldoutMotionSettingsProperty;
        SerializedProperty foldoutCallbackProperty;

        SerializedProperty globalShaderProperty;

        SerializedProperty definitionXProperty;
        SerializedProperty definitionYProperty;
        SerializedProperty definitionZProperty;
        SerializedProperty definitionWProperty;

        SerializedProperty foldoutCurveDefsProperty;

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
            modifierProperty = serializedObject.FindProperty("ValueType");

            findProperty = serializedObject.FindProperty("FindExposedProperties");
            selectedProperty = serializedObject.FindProperty("SelectableProperties");

            startAtCurrentProperty = serializedObject.FindProperty("StartAtCurrent");

            floatStartAtValueProperty = serializedObject.FindProperty("FStartAt");
            floatEndAtValueProperty = serializedObject.FindProperty("FEndAt");
            floatScaleValueProperty = serializedObject.FindProperty("FScale");

            intStartAtValueProperty = serializedObject.FindProperty("IStartAt");
            intEndAtValueProperty = serializedObject.FindProperty("IEndAt");
            intScaleValueProperty = serializedObject.FindProperty("IScale");

            vector2StartAtValueProperty = serializedObject.FindProperty("V2StartAt");
            vector2EndAtValueProperty = serializedObject.FindProperty("V2EndAt");
            vector2ScaleValueProperty = serializedObject.FindProperty("V2Scale");

            vector3StartAtValueProperty = serializedObject.FindProperty("V3StartAt");
            vector3EndAtValueProperty = serializedObject.FindProperty("V3EndAt");
            vector3ScaleValueProperty = serializedObject.FindProperty("V3Scale");

            vector4StartAtValueProperty = serializedObject.FindProperty("V4StartAt");
            vector4EndAtValueProperty = serializedObject.FindProperty("V4EndAt");
            vector4ScaleValueProperty = serializedObject.FindProperty("V4Scale");

            colorStartAtValueProperty = serializedObject.FindProperty("CStartAt");
            colorEndAtValueProperty = serializedObject.FindProperty("CEndAt");
            colorScaleValueProperty = serializedObject.FindProperty("CScale");

            spritesProperty = serializedObject.FindProperty("Sprites");

            durationProperty = serializedObject.FindProperty("Duration");
            startDelayProperty = serializedObject.FindProperty("StartDelay");
            endDelayProperty = serializedObject.FindProperty("EndDelay");
            unscaledTimeProperty = serializedObject.FindProperty("UseUnscaledTime");
            
            allowFinalizeProperty = serializedObject.FindProperty("FinalizeOnInactivePlay");
            lockableProperty = serializedObject.FindProperty("Lockable");

            onPlayProperty = serializedObject.FindProperty("OnPlayMotion");
            onStartedProperty = serializedObject.FindProperty("OnStartedMotion");
            onFinishedProperty = serializedObject.FindProperty("OnFinishedMotion");

            curveUsageProperty = serializedObject.FindProperty("CurveUsage");

            foldoutCurveDefProperty = serializedObject.FindProperty("foldoutCurveDef");
            foldoutMotionSettingsProperty = serializedObject.FindProperty("foldoutMotionSettings");

            globalShaderProperty = serializedObject.FindProperty("GlobalShaderProperty");

            definitionXProperty = serializedObject.FindProperty("PreDefinedCurveX");
            definitionYProperty = serializedObject.FindProperty("PreDefinedCurveY");
            definitionZProperty = serializedObject.FindProperty("PreDefinedCurveZ");
            definitionWProperty = serializedObject.FindProperty("PreDefinedCurveW");

            foldoutCurveDefsProperty = serializedObject.FindProperty("foldoutCurveDefs");
            foldoutCallbackProperty = serializedObject.FindProperty("foldoutCallbacks");

            // When selected return the editor preivew value back to zero
            m_ResetPreview = true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomMotion targetMotion = target as CustomMotion;

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
            EditorGUILayout.PropertyField(modifierProperty);
            EditorGUILayout.Space();

            

            Type type = null;

            if (targetMotion.Target)
                type = targetMotion.Target.GetType();

            if (type == typeof(MaterialComponent))
                EditorGUILayout.PropertyField(globalShaderProperty);
            else
                targetMotion.GlobalShaderProperty = false;

            if (targetMotion.GlobalShaderProperty)
            {
                targetMotion.GlobalShaderPropertyName = EditorGUILayout.TextArea(targetMotion.GlobalShaderPropertyName);
                targetMotion.SelectableProperties = new List<CustomMotion.ExposedProperty>(1)
                {
                    new CustomMotion.ExposedProperty { Modify = true, Name = targetMotion.GlobalShaderPropertyName }
                };
                EditorGUILayout.Space();
            }
            else
            {
                if (targetMotion != null)
                {
                    string valueTypeToModify = "Single";
                    switch (targetMotion.ValueType)
                    {
                        case ModifyValueType.SINGLE:
                            valueTypeToModify = "Single";
                            break;
                        case ModifyValueType.INT:
                            valueTypeToModify = "Int32";
                            break;
                        case ModifyValueType.VECTOR2:
                            valueTypeToModify = "Vector2";
                            break;
                        case ModifyValueType.VECTOR3:
                            valueTypeToModify = "Vector3";
                            break;
                        case ModifyValueType.VECTOR4:
                            valueTypeToModify = "Vector4";
                            break;
                        case ModifyValueType.COLOR:
                            valueTypeToModify = "Color";
                            break;
                        case ModifyValueType.SPRITE:
                            valueTypeToModify = "Sprite";
                            break;
                    }

                    EditorGUILayout.Space();


                    if (GUILayout.Button("Update Exposed Properties", GUILayout.Height(60)))
                    {
                        if (targetMotion.Target)
                        {
                            targetMotion.SelectableProperties.Clear();

                            // If the type is material, then we are altering shader property values instead, so expose those
                            if (type == typeof(MaterialComponent))
                            {
                                MaterialComponent component = (MaterialComponent)targetMotion.Target;
                                string[] AvailablePropertyNames = component.material.shaderKeywords;

                                ShaderUtil.ShaderPropertyType shaderPropertyType = ShaderUtil.ShaderPropertyType.Vector;
                                ShaderUtil.ShaderPropertyType shaderPropertyType2 = ShaderUtil.ShaderPropertyType.Float;
                                switch (targetMotion.ValueType)
                                {
                                    case ModifyValueType.SINGLE:
                                    case ModifyValueType.INT:
                                        shaderPropertyType = ShaderUtil.ShaderPropertyType.Range;
                                        break;
                                    case ModifyValueType.COLOR:
                                        shaderPropertyType = ShaderUtil.ShaderPropertyType.Color;
                                        break;
                                    case ModifyValueType.SPRITE:
                                        shaderPropertyType = ShaderUtil.ShaderPropertyType.TexEnv;
                                        break;
                                }

                                if (component.material)
                                {
                                    for (int i = 0; i < ShaderUtil.GetPropertyCount(component.material.shader); i++)
                                    {
                                        if (ShaderUtil.GetPropertyType(component.material.shader, i) == shaderPropertyType || (targetMotion.ValueType == ModifyValueType.SINGLE && ShaderUtil.GetPropertyType(component.material.shader, i) == shaderPropertyType2))
                                        {
                                            CustomMotion.ExposedProperty exposedProperty = new CustomMotion.ExposedProperty
                                            {
                                                Name = ShaderUtil.GetPropertyName(component.material.shader, i),
                                                Modify = false
                                            };
                                            targetMotion.SelectableProperties.Add(exposedProperty);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int I = 0; I < type.GetProperties().Length; I++)
                                {
                                    PropertyInfo p = type.GetProperties()[I];

                                    if (p.CanWrite && p.PropertyType.Name.Equals(valueTypeToModify) && !p.IsDefined(typeof(ObsoleteAttribute), true))
                                    {
                                        CustomMotion.ExposedProperty exposedProperty = new CustomMotion.ExposedProperty
                                        {
                                            Name = p.Name,
                                            Modify = false
                                        };
                                        targetMotion.SelectableProperties.Add(exposedProperty);
                                    }
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(selectedProperty);
                EditorGUILayout.Space();

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.red * boxBrightness);
            EditorGUILayout.Space();

            foldoutMotionSettingsProperty.isExpanded = EditorGUILayout.Foldout(foldoutMotionSettingsProperty.isExpanded, "Motion Settings", EditorStyles.foldoutHeader);

            if (targetMotion != null && foldoutMotionSettingsProperty.isExpanded)
            {
                EditorGUILayout.Space();

                switch (targetMotion.CurveUsage)
                {
                    case CurveUsage.NON_DIRECT:
                        EditorGUILayout.PropertyField(startAtCurrentProperty);
                        switch (targetMotion.ValueType)
                        {
                            case ModifyValueType.SINGLE:
                                if (!targetMotion.StartAtCurrent)
                                    EditorGUILayout.PropertyField(floatStartAtValueProperty, startAt);
                                EditorGUILayout.PropertyField(floatEndAtValueProperty, endAt);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.INT:
                                if (!targetMotion.StartAtCurrent)
                                    EditorGUILayout.PropertyField(intStartAtValueProperty, startAt);
                                EditorGUILayout.PropertyField(intEndAtValueProperty, endAt);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.VECTOR2:
                                if (!targetMotion.StartAtCurrent)
                                    EditorGUILayout.PropertyField(vector2StartAtValueProperty, startAt);
                                EditorGUILayout.PropertyField(vector2EndAtValueProperty, endAt);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.VECTOR3:
                                if (!targetMotion.StartAtCurrent)
                                    EditorGUILayout.PropertyField(vector3StartAtValueProperty, startAt);
                                EditorGUILayout.PropertyField(vector3EndAtValueProperty, endAt);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.VECTOR4:
                                if (!targetMotion.StartAtCurrent)
                                    EditorGUILayout.PropertyField(vector4StartAtValueProperty, startAt);
                                EditorGUILayout.PropertyField(vector4EndAtValueProperty, endAt);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.COLOR:
                                if (!targetMotion.StartAtCurrent)
                                    EditorGUILayout.PropertyField(colorStartAtValueProperty, startAt);
                                EditorGUILayout.PropertyField(colorEndAtValueProperty, endAt);
                                EditorGUILayout.Space();
                                break;
                        }
                        break;
                    case CurveUsage.DIRECT_WITH_SCALE:
                        
                        switch (targetMotion.ValueType)
                        {
                            case ModifyValueType.SINGLE:
                                EditorGUILayout.PropertyField(floatScaleValueProperty, scale);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.INT:
                                EditorGUILayout.PropertyField(intScaleValueProperty, scale);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.VECTOR2:
                                EditorGUILayout.PropertyField(vector2ScaleValueProperty, scale);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.VECTOR3:
                                EditorGUILayout.PropertyField(vector3ScaleValueProperty, scale);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.VECTOR4:
                                EditorGUILayout.PropertyField(vector4ScaleValueProperty, scale);
                                EditorGUILayout.Space();
                                break;
                            case ModifyValueType.COLOR:
                                EditorGUILayout.PropertyField(colorScaleValueProperty, scale);
                                EditorGUILayout.Space();
                                break;
                        }
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

            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.blue * boxBrightness);
            EditorGUILayout.Space();

            if (targetMotion.ValueType == ModifyValueType.SPRITE)
                EditorGUILayout.PropertyField(spritesProperty);
            else
            {

                foldoutCurveDefsProperty.isExpanded = EditorGUILayout.Foldout(foldoutCurveDefsProperty.isExpanded, "Curve Settings", EditorStyles.foldoutHeader);

                if (foldoutCurveDefsProperty.isExpanded)
                {
                    EditorGUILayout.Space();

                    if (targetMotion.ValueType != ModifyValueType.COLOR)
                    {
                        EditorGUILayout.PropertyField(curveUsageProperty);
                        EditorGUILayout.Space();
                    }

                    if (targetMotion.ValueType != ModifyValueType.COLOR)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Enabled Axis");
                        targetMotion.XEnabled = EditorGUILayout.ToggleLeft("X", targetMotion.XEnabled, GUILayout.MinWidth(40));
                        if (targetMotion.ValueType != ModifyValueType.SINGLE && targetMotion.ValueType != ModifyValueType.INT)
                        {
                            targetMotion.YEnabled = EditorGUILayout.ToggleLeft("Y", targetMotion.YEnabled, GUILayout.MinWidth(40));

                            if (targetMotion.ValueType != ModifyValueType.VECTOR2)
                            {
                                targetMotion.ZEnabled = EditorGUILayout.ToggleLeft("Z", targetMotion.ZEnabled, GUILayout.MinWidth(40));

                                if (targetMotion.ValueType != ModifyValueType.VECTOR3)
                                    targetMotion.WEnabled = EditorGUILayout.ToggleLeft("W", targetMotion.WEnabled, GUILayout.MinWidth(40));
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                    else
                    {
                        targetMotion.XEnabled = true;

                        targetMotion.YEnabled = targetMotion.ZEnabled = targetMotion.WEnabled = false;
                    }


                    int numberOfAxisEnabled = 0;

                    if (targetMotion.XEnabled)
                    {
                        if (targetMotion.ValueType != ModifyValueType.COLOR)
                            EditorGUILayout.PropertyField(definitionXProperty);
                        else
                            EditorGUILayout.PropertyField(definitionXProperty, new GUIContent("Pre Defined Curve"));
                        numberOfAxisEnabled++;
                    }
                    if (targetMotion.YEnabled)
                    {
                        EditorGUILayout.PropertyField(definitionYProperty);
                        numberOfAxisEnabled++;
                    }
                    if (targetMotion.ZEnabled)
                    {
                        EditorGUILayout.PropertyField(definitionZProperty);
                        numberOfAxisEnabled++;
                    }
                    if (targetMotion.WEnabled)
                    {
                        EditorGUILayout.PropertyField(definitionWProperty);
                        numberOfAxisEnabled++;
                    }

                    EditorGUILayout.Space();

                    if (numberOfAxisEnabled > 0)
                    {
                        EditorGUI.DrawRect(EditorGUILayout.BeginVertical(), Color.grey * boxBrightness);
                        EditorGUILayout.LabelField(numberOfAxisEnabled > 1 ? "Curves" : "Curve");

                        EditorGUILayout.BeginHorizontal();

                        if (targetMotion.XEnabled)
                        {
                            EditorGUILayout.BeginVertical();

                            if (targetMotion.PreDefinedCurveX != null)
                            {
                                // Copy keys from predefined
                                targetMotion.CurveX.keys = targetMotion.PreDefinedCurveX.Curve.keys;
                                // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                                GUI.enabled = false;
                            }

                            EditorGUILayout.CurveField("", targetMotion.CurveX, Color.red, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                            GUI.enabled = true;

                            EditorGUILayout.EndVertical();
                        }
                        if (targetMotion.YEnabled)
                        {
                            EditorGUILayout.BeginVertical();

                            if (targetMotion.PreDefinedCurveY != null)
                            {
                                // Copy keys from predefined
                                targetMotion.CurveY.keys = targetMotion.PreDefinedCurveY.Curve.keys;
                                // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                                GUI.enabled = false;
                            }

                            EditorGUILayout.CurveField("", targetMotion.CurveY, Color.green, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                            GUI.enabled = true;

                            EditorGUILayout.EndVertical();
                        }
                        if (targetMotion.ZEnabled)
                        {
                            EditorGUILayout.BeginVertical();

                            if (targetMotion.PreDefinedCurveZ != null)
                            {
                                // Copy keys from predefined
                                targetMotion.CurveZ.keys = targetMotion.PreDefinedCurveZ.Curve.keys;
                                // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                                GUI.enabled = false;
                            }

                            EditorGUILayout.CurveField("", targetMotion.CurveZ, Color.blue, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                            GUI.enabled = true;

                            EditorGUILayout.EndVertical();
                        }
                        if (targetMotion.WEnabled)
                        {
                            EditorGUILayout.BeginVertical();

                            if (targetMotion.PreDefinedCurveW != null)
                            {
                                // Copy keys from predefined
                                targetMotion.CurveW.keys = targetMotion.PreDefinedCurveW.Curve.keys;
                                // Disable editing (So we dont accidentally edit a predefined curve unknowingly)
                                GUI.enabled = false;
                            }

                            EditorGUILayout.CurveField("", targetMotion.CurveW, Color.yellow, Rect.zero, GUILayout.Height(100), GUILayout.MaxWidth(20), GUILayout.ExpandWidth(true));
                            GUI.enabled = true;

                            EditorGUILayout.EndVertical();
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            EditorGUILayout.Space();
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

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }
}