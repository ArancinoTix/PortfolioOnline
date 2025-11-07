using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using U9.Audio.Data;
using System;
using UnityEngine.UIElements;

namespace U9.Audio.Editor
{
    public class AudioIdEditorHelper
    {
        public static string[] GetAllAudioIdGUIDs()
        {
            return AssetDatabase.FindAssets("t:AudioId", null);
        }

        public static AudioId GetAudioIdFromGUID(string guid)
        {
            return AssetDatabase.LoadAssetAtPath<AudioId>(AssetDatabase.GUIDToAssetPath(guid));
        }

        /// <summary>
        /// Does an ID with this name exist?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Exists(string name)
        {
            string[] guids = GetAllAudioIdGUIDs();

            foreach(string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (System.IO.Path.GetFileNameWithoutExtension(assetPath) == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates the specified audio ID
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static AudioId Create(string name, AudioCategory category)
        {
            //try
            //{
                AudioId audioId = ScriptableObject.CreateInstance<AudioId>();
                audioId.SetCategory(category);

            AudioController controller = GameObject.FindAnyObjectByType<AudioController>();
                string rootFolder = controller == null? "": controller.ConfigSavePath;
                if (string.IsNullOrEmpty(rootFolder))
                    rootFolder = "Assets/_Project/Audio/Configurations/Audio Ids";

                string subFolder = System.IO.Path.Combine(rootFolder, category.name);
                string filePath = System.IO.Path.Combine(subFolder, name + ".asset");

                if (!AssetDatabase.IsValidFolder(subFolder))
                    AssetDatabase.CreateFolder(rootFolder, category.name);

                AssetDatabase.CreateAsset(audioId, filePath);
                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(audioId);

                return audioId;
            /*
            }
            catch(Exception e)
            {
                Debug.LogError("Failed to create ID: " + e.Message);
                return null;
            }*/
        }

        public static VisualElement DrawCreateID(VisualElement root,  Action<string> attemptCreate)
        {
            var createIdRoot = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                }
            };

            var inputField = new TextField()
            {
                style =
                {
                    flexGrow = new StyleFloat(1),
                    marginRight = 5
                }
            };
            createIdRoot.Add(inputField);

            var createButton = new Button(() =>
            {
                if (string.IsNullOrEmpty(inputField.text))
                {
                    //Cannot create null
                    Debug.LogError("Failed to make new ID, id is empty");
                }
                else if (Exists(inputField.text))
                {
                    //Cannot create duplicate
                    Debug.LogError("Failed to make new ID, already exists");
                }
                else
                {
                    attemptCreate(inputField.text);
                    inputField.value = string.Empty;
                }
            }
            )
            {
                text = "Create ID"
            };
            createIdRoot.Add(createButton);

            root.Add(createIdRoot);

            return createIdRoot;
        }
    }
}
