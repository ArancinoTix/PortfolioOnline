using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace U9.Motion
{
    [Serializable]
    public enum ModifyValueType
    {
        SINGLE,
        VECTOR2,
        VECTOR3,
        VECTOR4,
        COLOR,
        SPRITE,
        INT
    }

    public class CustomMotion : BaseMotion
    {
        [Tooltip("Drop Component directly")]
        public Component Target;

        [Tooltip("The type of value you want to control")]
        public ModifyValueType ValueType = ModifyValueType.SINGLE;

        [Tooltip("If enabled then this motion will try to use the objects current status as its starting state")]
        public bool StartAtCurrent = false;

        [Serializable]
        public class ExposedProperty
        {
            public string Name;
            public bool Modify;

            // This must be collected at runtime unfortunately
            public PropertyInfo PInfo;

            // Special shader property ID for setting and getting shader properies without searching by name
            [HideInInspector] public int ShaderPropertyID = -1;
        }
        [Tooltip("List of the available properties that have specified value type")]

        public List<ExposedProperty> SelectableProperties = new List<ExposedProperty>();

        [Tooltip("Designated value to represent what the Curves Y = 0 equates to")]
        public float FStartAt;
        [Tooltip("Designated value to represent what the Curves Y = 1 equates to")]
        public float FEndAt;
        [Tooltip("Multiplies Curve Y values (Only in use if Curve Usage is DIRECT_WITH_SCALE)")]
        public float FScale;

        [Tooltip("Designated value to represent what the Curves Y = 0 equates to")]
        public int IStartAt;
        [Tooltip("Designated value to represent what the Curves Y = 1 equates to")]
        public int IEndAt;
        [Tooltip("Multiplies Curve Y values (Only in use if Curve Usage is DIRECT_WITH_SCALE)")]
        public int IScale;

        [Tooltip("Designated value to represent what the Curves Y = 0 equates to")]
        public Vector2 V2StartAt;
        [Tooltip("Designated value to represent what the Curves Y = 1 equates to")]
        public Vector2 V2EndAt;
        [Tooltip("Multiplies Curve Y values (Only in use if Curve Usage is DIRECT_WITH_SCALE)")]
        public Vector2 V2Scale;

        [Tooltip("Designated value to represent what the Curves Y = 0 equates to")]
        public Vector3 V3StartAt;
        [Tooltip("Designated value to represent what the Curves Y = 1 equates to")]
        public Vector3 V3EndAt;
        [Tooltip("Multiplies Curve Y values (Only in use if Curve Usage is DIRECT_WITH_SCALE)")]
        public Vector3 V3Scale;

        [Tooltip("Designated value to represent what the Curves Y = 0 equates to")]
        public Vector4 V4StartAt;
        [Tooltip("Designated value to represent what the Curves Y = 1 equates to")]
        public Vector4 V4EndAt;
        [Tooltip("Multiplies Curve Y values (Only in use if Curve Usage is DIRECT_WITH_SCALE)")]
        public Vector4 V4Scale;

        [Tooltip("Designated value to represent what the Curves Y = 0 equates to")]
        [ColorUsage(true, true)] public Color CStartAt;
        [Tooltip("Designated value to represent what the Curves Y = 1 equates to")]
        [ColorUsage(true, true)] public Color CEndAt;
        [Tooltip("Multiplies Curve Y values (Only in use if Curve Usage is DIRECT_WITH_SCALE)")]
        public Color CScale;

        [Tooltip("The sprites to use over time")]
        public Sprite[] Sprites;

        [Tooltip("Should this affect a global shader property?")]
        public bool GlobalShaderProperty = false;
        [Tooltip("The global shader property name to change")]
        public string GlobalShaderPropertyName;

        public bool XEnabled, YEnabled, ZEnabled, WEnabled = false;

        public MotionDefinition PreDefinedCurveX = default;
        public MotionDefinition PreDefinedCurveY = default;
        public MotionDefinition PreDefinedCurveZ = default;
        public MotionDefinition PreDefinedCurveW = default;

        public AnimationCurve CurveX = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve CurveY = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve CurveZ = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve CurveW = AnimationCurve.Linear(0, 0, 1, 1);

        public override void Initialize()
        {
            StopAllCoroutines();

            if (Target)
                SetTarget(0);

            m_IsInFinalState = false;
        }

        public override void Finalize(bool fromInactivePlay = false)
        {
            // If playing finalize because Play was called on inactive object and this Motion disallows this, then don't carry on here
            if (fromInactivePlay && !FinalizeOnInactivePlay)
                return;

            if (Target)
                SetTarget(1);

            if (fromInactivePlay)
                OnFinishedMotion?.Invoke();

            m_IsInFinalState= true;
        }

        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
            {
                foreach (ExposedProperty target in SelectableProperties)
                {
                    if (target.Modify)
                    {
                        targetType = Target.GetType();

                        if (targetType == typeof(MaterialComponent))
                        {
                            materialComponent = Target as MaterialComponent;

                            if (target.ShaderPropertyID == -1)
                            {
                                target.ShaderPropertyID = Shader.PropertyToID(target.Name);
                            }
                        }
                        else if (target.PInfo == null)
                            target.PInfo = targetType.GetProperty(target.Name);
                        switch (ValueType)
                        {
                            case ModifyValueType.SINGLE:
                                FStartAt = materialComponent ? GlobalShaderProperty ? Shader.GetGlobalFloat(target.ShaderPropertyID) : materialComponent.material.GetFloat(target.ShaderPropertyID) : (float)target.PInfo.GetValue(Target);
                                break;
                            case ModifyValueType.INT:
                                IStartAt = materialComponent ? GlobalShaderProperty ? Shader.GetGlobalInt(target.ShaderPropertyID) : materialComponent.material.GetInt(target.ShaderPropertyID) : (int)target.PInfo.GetValue(Target);
                                break;
                            case ModifyValueType.VECTOR2:
                                if (materialComponent)
                                {
                                    Vector4 vec4Value = materialComponent.material.GetVector(target.ShaderPropertyID);
                                    V2StartAt = new Vector2(vec4Value.x, vec4Value.y);
                                }
                                else
                                    V2StartAt = (Vector2)target.PInfo.GetValue(Target);
                                break;
                            case ModifyValueType.VECTOR3:
                                if (materialComponent)
                                {
                                    Vector4 vec4Value = materialComponent.material.GetVector(target.ShaderPropertyID);
                                    V3StartAt = new Vector3(vec4Value.x, vec4Value.y, vec4Value.z);
                                }
                                else
                                    V3StartAt = (Vector3)target.PInfo.GetValue(Target);
                                break;
                            case ModifyValueType.VECTOR4:
                                V4StartAt = materialComponent ? materialComponent.material.GetVector(target.ShaderPropertyID) : (Vector4)target.PInfo.GetValue(Target);
                                break;
                            case ModifyValueType.COLOR:
                                CStartAt = materialComponent ? materialComponent.material.GetColor(target.ShaderPropertyID) : (Color)target.PInfo.GetValue(Target);
                                break;
                            case ModifyValueType.SPRITE:
                                // Not Supported
                                break;
                        }
                    }
                }
            }

            Complete = false;

            m_PlayBackwards = backwards;
            m_CurrentTime = startTime;

            m_HasSentStarted = false;
            m_HasSentPlaying = false;
            m_HasSentFinished = false;

            // If Inactive
            if (!isActiveAndEnabled)
            {
                // But can finalize on InActive Play
                if (FinalizeOnInactivePlay)
                {
                    if (backwards)
                        Initialize();
                    else
                        Finalize(true); // Set Final state

                    Complete = true;
                }
                else
                    return;
            }
            else if (Target)
                m_IsPlaying = true;
            else
                Complete = true;
        }

        private Type targetType;
        private MaterialComponent materialComponent;
        private float tempFloatValue = 0;
        private float tempGetFloatValue = 0;
        private int tempIntValue = 0;
        private int tempGetIntValue = 0;
        private Vector2 tempVector2Value = Vector2.zero;
        private Vector4 tempGetVector2Value = Vector2.zero;
        private Vector3 tempVector3Value = Vector3.zero;
        private Vector4 tempGetVector3Value = Vector3.zero;
        private Vector4 tempVector4Value = Vector4.zero;
        private Vector4 tempGetVector4Value = Vector4.zero;
        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="normalizedTime"></param>
        public override void SetTarget(float normalizedTime)
        {
            if (!Target)
                return;

            foreach (ExposedProperty target in SelectableProperties)
            {
                if (target.Modify)
                {
                    targetType = Target.GetType();

                    if (targetType == typeof(MaterialComponent))
                    {
                        materialComponent = Target as MaterialComponent;

                        if(target.ShaderPropertyID == -1)
                        {
                            target.ShaderPropertyID = Shader.PropertyToID(target.Name);
                        }
                    }
                    else if (target.PInfo == null)
                        target.PInfo = targetType.GetProperty(target.Name);

                    switch (ValueType)
                    {
                        case ModifyValueType.SINGLE:
                            // Get current float value either from material shader property of PropertyInfo Property
                            tempGetFloatValue = materialComponent ? GlobalShaderProperty ? Shader.GetGlobalFloat(target.ShaderPropertyID) : materialComponent.material.GetFloat(target.ShaderPropertyID) : (float)target.PInfo.GetValue(Target);

                            // Alter value using given method, if Enabled
                            if (CurveUsage == CurveUsage.NON_DIRECT)
                                tempFloatValue = XEnabled ? Mathf.Lerp(FStartAt, FEndAt, CurveX.Evaluate(normalizedTime)) : tempGetFloatValue;
                            else if (CurveUsage == CurveUsage.DIRECT)
                                tempFloatValue = XEnabled ? CurveX.Evaluate(normalizedTime) : tempGetFloatValue;
                            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                                tempFloatValue = XEnabled ? CurveX.Evaluate(normalizedTime) * FScale : tempGetFloatValue;

                            // Set material shader value or property value
                            if (materialComponent)
                            {
                                if(GlobalShaderProperty)
                                    Shader.SetGlobalFloat(target.ShaderPropertyID, tempFloatValue);
                                else
                                    materialComponent.material.SetFloat(target.ShaderPropertyID, tempFloatValue);
                            }
                            else
                                target.PInfo.SetValue(Target, tempFloatValue);
                            break;
                        case ModifyValueType.INT:
                            // Get current float value either from material shader property of PropertyInfo Property
                            tempGetIntValue = materialComponent ? GlobalShaderProperty ? Shader.GetGlobalInt(target.ShaderPropertyID) : materialComponent.material.GetInt(target.ShaderPropertyID) : (int)target.PInfo.GetValue(Target);

                            // Alter value using given method, if Enabled
                            if (CurveUsage == CurveUsage.NON_DIRECT)
                                tempIntValue = XEnabled ? (int)Mathf.Lerp(IStartAt, IEndAt, CurveX.Evaluate(normalizedTime)) : tempGetIntValue;
                            else if (CurveUsage == CurveUsage.DIRECT)
                                tempIntValue = XEnabled ? (int)CurveX.Evaluate(normalizedTime) : tempGetIntValue;
                            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                                tempIntValue = XEnabled ? (int)CurveX.Evaluate(normalizedTime) * IScale : tempGetIntValue;

                            // Set material shader value or property value
                            if (materialComponent)
                            {
                                if (GlobalShaderProperty)
                                    Shader.SetGlobalInt(target.ShaderPropertyID, tempIntValue);
                                else
                                    materialComponent.material.SetInt(target.ShaderPropertyID, tempIntValue);
                            }
                            else
                                target.PInfo.SetValue(Target, tempIntValue);
                            break;
                        case ModifyValueType.VECTOR2:
                            // Get current vector value either from material shader property of PropertyInfo Property
                            if (materialComponent)
                            {
                                Vector4 vec4Value = materialComponent.material.GetVector(target.ShaderPropertyID);
                                tempGetVector2Value = new Vector2(vec4Value.x, vec4Value.y);
                            }
                            else
                                tempGetVector2Value = (Vector2)target.PInfo.GetValue(Target);

                            // Alter value using given method, if Enabled
                            if (CurveUsage == CurveUsage.NON_DIRECT)
                                tempVector2Value = new Vector2( XEnabled ? Mathf.Lerp(V2StartAt.x, V2EndAt.x, CurveX.Evaluate(normalizedTime)) : tempGetVector2Value.x,
                                                                YEnabled ? Mathf.Lerp(V2StartAt.y, V2EndAt.y, CurveY.Evaluate(normalizedTime)) : tempGetVector2Value.y);
                            else if (CurveUsage == CurveUsage.DIRECT)
                                tempVector2Value = new Vector2( XEnabled ? CurveX.Evaluate(normalizedTime) : tempGetVector2Value.x,
                                                                YEnabled ? CurveY.Evaluate(normalizedTime) : tempGetVector2Value.y);
                            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                                tempVector2Value = new Vector2( XEnabled ? CurveX.Evaluate(normalizedTime) * V2Scale.x : tempGetVector2Value.x,
                                                                YEnabled ? CurveY.Evaluate(normalizedTime) * V2Scale.y : tempGetVector2Value.y);
                            // Set material shader value or property value
                            if (materialComponent)
                            {
                                if (GlobalShaderProperty)
                                    Shader.SetGlobalVector(target.ShaderPropertyID, tempVector4Value);
                                else
                                    materialComponent.material.SetVector(target.ShaderPropertyID, tempVector4Value);
                            }
                            else
                                target.PInfo.SetValue(Target, tempVector2Value);
                            break;
                        case ModifyValueType.VECTOR3:
                            // Get current vector value either from material shader property of PropertyInfo Property
                            if (materialComponent)
                            {
                                Vector4 vec4Value = materialComponent.material.GetVector(target.ShaderPropertyID);
                                tempGetVector3Value = new Vector3(vec4Value.x, vec4Value.y, vec4Value.z);
                            }
                            else
                                tempGetVector3Value = (Vector3)target.PInfo.GetValue(Target);

                            if (CurveUsage == CurveUsage.NON_DIRECT)
                                tempVector3Value = new Vector3( XEnabled ? Mathf.Lerp(V3StartAt.x, V3EndAt.x, CurveX.Evaluate(normalizedTime)) : tempGetVector3Value.x,
                                                                YEnabled ? Mathf.Lerp(V3StartAt.y, V3EndAt.y, CurveY.Evaluate(normalizedTime)) : tempGetVector3Value.y,
                                                                ZEnabled ? Mathf.Lerp(V3StartAt.z, V3EndAt.z, CurveZ.Evaluate(normalizedTime)) : tempGetVector3Value.z);
                            else if (CurveUsage == CurveUsage.DIRECT)
                                tempVector3Value = new Vector3( XEnabled ? CurveX.Evaluate(normalizedTime) : tempGetVector3Value.x,
                                                                YEnabled ? CurveY.Evaluate(normalizedTime) : tempGetVector3Value.y,
                                                                ZEnabled ? CurveZ.Evaluate(normalizedTime) : tempGetVector3Value.z);
                            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                                tempVector3Value = new Vector3( XEnabled ? CurveX.Evaluate(normalizedTime) * V3Scale.x : tempGetVector3Value.x,
                                                                YEnabled ? CurveY.Evaluate(normalizedTime) * V3Scale.y : tempGetVector3Value.y,
                                                                ZEnabled ? CurveZ.Evaluate(normalizedTime) * V3Scale.z : tempGetVector3Value.z);

                            // Set material shader value or property value
                            if (materialComponent)
                            {
                                if (GlobalShaderProperty)
                                    Shader.SetGlobalVector(target.ShaderPropertyID, tempVector4Value);
                                else
                                    materialComponent.material.SetVector(target.ShaderPropertyID, tempVector4Value);
                            }
                            else
                                target.PInfo.SetValue(Target, tempVector3Value);
                            break;
                        case ModifyValueType.VECTOR4:
                            // Get current vector value either from material shader property of PropertyInfo Property
                            tempGetVector4Value = materialComponent ? materialComponent.material.GetVector(target.ShaderPropertyID) : (Vector4)target.PInfo.GetValue(Target);

                            if (CurveUsage == CurveUsage.NON_DIRECT)
                                tempVector4Value = new Vector4( XEnabled ? Mathf.Lerp(V4StartAt.x, V4EndAt.x, CurveX.Evaluate(normalizedTime)) : tempGetVector4Value.x,
                                                                YEnabled ? Mathf.Lerp(V4StartAt.y, V4EndAt.y, CurveY.Evaluate(normalizedTime)) : tempGetVector4Value.y,
                                                                ZEnabled ? Mathf.Lerp(V4StartAt.z, V4EndAt.z, CurveZ.Evaluate(normalizedTime)) : tempGetVector4Value.z,
                                                                WEnabled ? Mathf.Lerp(V4StartAt.w, V4EndAt.w, CurveW.Evaluate(normalizedTime)) : tempGetVector4Value.w);
                            else if (CurveUsage == CurveUsage.DIRECT)
                                tempVector4Value = new Vector4( XEnabled ? CurveX.Evaluate(normalizedTime) : tempGetVector4Value.x,
                                                                YEnabled ? CurveY.Evaluate(normalizedTime) : tempGetVector4Value.y,
                                                                ZEnabled ? CurveZ.Evaluate(normalizedTime) : tempGetVector4Value.z,
                                                                WEnabled ? CurveW.Evaluate(normalizedTime) : tempGetVector4Value.w);
                            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                                tempVector4Value = new Vector4( XEnabled ? CurveX.Evaluate(normalizedTime) * V4Scale.x : tempGetVector4Value.x,
                                                                YEnabled ? CurveY.Evaluate(normalizedTime) * V4Scale.y : tempGetVector4Value.y,
                                                                ZEnabled ? CurveZ.Evaluate(normalizedTime) * V4Scale.z : tempGetVector4Value.z,
                                                                WEnabled ? CurveW.Evaluate(normalizedTime) * V4Scale.w : tempGetVector4Value.w);

                            // Set material shader value or property value
                            if (materialComponent)
                            {
                                if (GlobalShaderProperty)
                                    Shader.SetGlobalVector(target.ShaderPropertyID, tempVector4Value);
                                else
                                    materialComponent.material.SetVector(target.ShaderPropertyID, tempVector4Value);
                            }
                            else
                                target.PInfo.SetValue(Target, tempVector4Value);
                            break;
                        case ModifyValueType.COLOR:
                            // Set material shader value or property value
                            if (materialComponent)
                            {
                                if (GlobalShaderProperty)
                                    Shader.SetGlobalColor(target.ShaderPropertyID, Color.Lerp(CStartAt, CEndAt, CurveX.Evaluate(normalizedTime)));
                                else
                                    materialComponent.material.SetColor(target.ShaderPropertyID, Color.Lerp(CStartAt, CEndAt, CurveX.Evaluate(normalizedTime)));
                            }
                            else
                                target.PInfo.SetValue(Target, Color.Lerp(CStartAt, CEndAt, CurveX.Evaluate(normalizedTime)));
                            break;
                        case ModifyValueType.SPRITE:
                            if (Sprites != null && Sprites.Length > 0)
                            {
                                // Set material shader value or property value
                                if (materialComponent)
                                {
                                    if (GlobalShaderProperty)
                                        Shader.SetGlobalTexture(target.ShaderPropertyID, Sprites[(int)Mathf.Lerp(0, Sprites.Length - 1, normalizedTime)].texture);
                                    else
                                        materialComponent.material.SetTexture(target.ShaderPropertyID, Sprites[(int)Mathf.Lerp(0, Sprites.Length - 1, normalizedTime)].texture);
                                }
                                else
                                    target.PInfo.SetValue(Target, Sprites[(int)Mathf.Lerp(0, Sprites.Length - 1, normalizedTime)]);
                            }
                            break;

                    }
#if UNITY_EDITOR
                    // Make sure we don't save the editors shader ID value
                    if (targetType == typeof(MaterialComponent))
                    {
                        target.ShaderPropertyID = -1;
                    }
#endif
                }
            }
        }
    }
}