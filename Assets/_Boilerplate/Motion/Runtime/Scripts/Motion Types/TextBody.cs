using System.Collections;
using TMPro;
using UnityEngine;

namespace U9.Motion
{
    public class TextBody : BaseMotion
    {
        public TMP_Text Target;

        [Tooltip("A Pre Defined Curve to use (This makes it easier to reuse/edit curves for many parts at once)")]
        public MotionDefinition PreDefinedCurve2 = null;

        [Tooltip("If a pre defined curve is not set then this is the curve to use")]
        public AnimationCurve Curve2 = AnimationCurve.Linear(0, 0, 1, 1);

        [Tooltip("How far should each line move from ")]
        public float StartingDistance = 20.0f;

        private TMP_MeshInfo[] m_CachedMeshInfo;
        private TMP_LineInfo[] m_LineInfoArray;
        private int m_LineCount = 1;

        private Color32[] m_NewVertexColors;
        private Vector3[] m_CurrentVertices;
        private Vector3[] m_CachedVertices;
        private Matrix4x4 m_Matrix;

        private void Awake()
        {
            // Try to find matching component on this gameobject
            if (Target == null)
                Target = GetComponent<TMP_Text>();
        }

        public override void Finalize(bool fromInactivePlay = false)
        {
            if (Target)
            {
                SetTarget(1);
            }
        }

        public override void Initialize()
        {
            if (Target)
            {
                SetTarget(0);
                Target.alpha = 0;
            }
        }

        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            StopAllCoroutines();

            if (Target)
            {
                StartCoroutine(FadeText(backwards));
            }
            else
                Complete = true;
        }

        public override void SetTarget(float time)
        {
            byte alpha;

            for (int l = 0; l < m_LineCount; l++)
            {
                float alphaFloat = Mathf.Lerp(0, 1, time * (m_LineCount - l));
                alphaFloat = Curve.Evaluate(alphaFloat) * 255;
                alpha = (byte)Mathf.Clamp(alphaFloat, 0, 255);

                if (m_LineInfoArray != null && m_LineInfoArray[l].characterCount != 0)
                {
                    for (int c = m_LineInfoArray[l].firstCharacterIndex; c < m_LineInfoArray[l].lastCharacterIndex + 1; c++)
                    {
                        if (!Target.textInfo.characterInfo[c].isVisible) continue;

                        int materialIndex = Target.textInfo.characterInfo[c].materialReferenceIndex;
                        m_NewVertexColors = Target.textInfo.meshInfo[materialIndex].colors32;

                        int vertexIndex = Target.textInfo.characterInfo[c].vertexIndex;

                        // Set new alpha values.
                        m_NewVertexColors[vertexIndex + 0].a = alpha;
                        m_NewVertexColors[vertexIndex + 1].a = alpha;
                        m_NewVertexColors[vertexIndex + 2].a = alpha;
                        m_NewVertexColors[vertexIndex + 3].a = alpha;

                        // Move vertices up from slightly moved down position, to desired position
                        m_CurrentVertices = Target.textInfo.meshInfo[materialIndex].vertices;
                        m_CachedVertices = m_CachedMeshInfo[materialIndex].vertices;

                        // Adjust current vertices based on max offset (m_StartingDistance)
                        m_Matrix = Matrix4x4.TRS(Vector3.down * StartingDistance, Quaternion.identity, Vector3.one);
                        m_CurrentVertices[vertexIndex + 0] = m_Matrix.MultiplyPoint3x4(m_CachedVertices[vertexIndex + 0]);
                        m_CurrentVertices[vertexIndex + 1] = m_Matrix.MultiplyPoint3x4(m_CachedVertices[vertexIndex + 1]);
                        m_CurrentVertices[vertexIndex + 2] = m_Matrix.MultiplyPoint3x4(m_CachedVertices[vertexIndex + 2]);
                        m_CurrentVertices[vertexIndex + 3] = m_Matrix.MultiplyPoint3x4(m_CachedVertices[vertexIndex + 3]);

                        // Lerp between max offset and where the text should be based on progress
                        m_CurrentVertices[vertexIndex + 0] = Vector3.Lerp(m_CurrentVertices[vertexIndex + 0], m_CachedVertices[vertexIndex + 0], Curve2.Evaluate(time * (m_LineCount - l)));
                        m_CurrentVertices[vertexIndex + 1] = Vector3.Lerp(m_CurrentVertices[vertexIndex + 1], m_CachedVertices[vertexIndex + 1], Curve2.Evaluate(time * (m_LineCount - l)));
                        m_CurrentVertices[vertexIndex + 2] = Vector3.Lerp(m_CurrentVertices[vertexIndex + 2], m_CachedVertices[vertexIndex + 2], Curve2.Evaluate(time * (m_LineCount - l)));
                        m_CurrentVertices[vertexIndex + 3] = Vector3.Lerp(m_CurrentVertices[vertexIndex + 3], m_CachedVertices[vertexIndex + 3], Curve2.Evaluate(time * (m_LineCount - l)));
                    }
                }

                Target.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);
            }
        }

        private void StartFadeInWhenReady()
        {
            StartCoroutine(FadeText());
        }

        private IEnumerator FadeText(bool backwards = false)
        {
            OnPlayMotion.Invoke();

            Complete = false;

            float elapsedTime = 0.0f;

            if (StartDelay > 0.0f)
            {
                while (elapsedTime < StartDelay)
                {
                    if(m_IsPlaying)
                        elapsedTime += Time.deltaTime;
                    yield return null;
                }

                elapsedTime = 0.0f;
            }

            OnStartedMotion.Invoke();

            Target.ForceMeshUpdate();
            m_CachedMeshInfo = Target.textInfo.CopyMeshInfoVertexData();
            m_LineInfoArray = Target.textInfo.lineInfo;
            Target.alpha = 0.0f;
            m_LineCount = Target.textInfo.lineCount;

            while (elapsedTime < Duration)
            {
                if (m_IsPlaying)
                    elapsedTime += Time.deltaTime;

                if(backwards)
                    SetTarget(1.0f - elapsedTime / Duration);
                else
                    SetTarget(elapsedTime / Duration);

                yield return null;
            }

            elapsedTime = 0.0f;

            if (EndDelay > 0.0f)
            {
                while (elapsedTime < EndDelay)
                {
                    if (m_IsPlaying)
                        elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            OnFinishedMotion.Invoke();

            Complete = true;
        }
    }
}