using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    [CustomTimelineEditor(typeof(SpriteClip))]
    class SpriteClipTimelineEditor : ClipEditor
    {
        readonly string k_AssignedError = L10n.Tr("No Sprites Assigned");
        const int MaxSampledSprites = 5;
        readonly Color dividerColor = Color.white;

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            var controlAsset = clip.asset as SpriteClip;
            if (controlAsset != null && controlAsset.values.sprites == null)
                clipOptions.errorText = k_AssignedError;
            return clipOptions;
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            // Draw sprites in the background
            var rect = region.position;
            var controlAsset = clip.asset as SpriteClip;
            var sprites = controlAsset?.values.sprites;

            if (sprites != null && sprites.Length > 0)
            {
                List<int> sampledIndices = new List<int>();

                // Always sample the first sprite
                sampledIndices.Add(0); 

                int sampledSpriteCount = Mathf.Min(MaxSampledSprites, sprites.Length);

                // If more than one sprite, sample evenly
                if (sampledSpriteCount > 1) 
                {
                    float interval = (sprites.Length - 1) / (float)(sampledSpriteCount - 1);
                    for (int i = 1; i < sampledSpriteCount - 1; i++)
                    {
                        int index = Mathf.RoundToInt(i * interval);
                        sampledIndices.Add(index);
                    }
                }

                // Always sample the last sprite
                sampledIndices.Add(sprites.Length - 1);

                int sampleIndex = 0;
                foreach (int index in sampledIndices)
                {
                    if (sprites[index] != null)
                    {
                        Rect spriteRect = GetSpriteRect(rect, sampleIndex++, sampledSpriteCount);
                        GUI.DrawTexture(spriteRect, sprites[index].texture, ScaleMode.ScaleToFit, true);

                        if (sampleIndex < sampledSpriteCount)
                        {
                            DrawDividerLine(spriteRect);
                        }
                    }
                }
            }
        }

        private Rect GetSpriteRect(Rect clipRect, int spriteIndex, int totalSprites)
        {
            float spriteWidth;
            if (totalSprites > 1 && totalSprites <= MaxSampledSprites)
                spriteWidth = clipRect.width / totalSprites;
            else
                spriteWidth = clipRect.width / (MaxSampledSprites - 1);

            float xPos = clipRect.x + Mathf.Round(spriteWidth * spriteIndex);
            return new Rect(xPos, clipRect.y, spriteWidth, clipRect.height);
        }

        private void DrawDividerLine(Rect spriteRect)
        {
            Handles.color = dividerColor;
            Handles.DrawLine(new Vector3(spriteRect.xMax, spriteRect.y), new Vector3(spriteRect.xMax, spriteRect.yMax));
        }
    }

    [CustomEditor(typeof(SpriteClip))]
    class SpriteClipEditor : Editor
    {
        SerializedProperty spritesProperty;
        SerializedProperty extrapolationProperty;

        public void OnEnable()
        {
            extrapolationProperty = serializedObject.FindProperty("values.clipExtrapolation");
            spritesProperty = serializedObject.FindProperty("values.sprites");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(extrapolationProperty);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spritesProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
