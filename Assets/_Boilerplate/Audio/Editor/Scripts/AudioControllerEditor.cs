using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using U9.Audio;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using U9.Audio.Data;
using U9.Audio.Service;

namespace U9.Audio.Editor
{
    [CustomEditor(typeof(AudioController))]
    public class AudioControllerEditor : UnityEditor.Editor
    {
        private SerializedProperty _masterCategory;
        private SerializedProperty _bgmCategory;
        private SerializedProperty _otherCategories;
        private SerializedProperty _savePath;

        private List<IAudioObject> _audioObjects;

        private AudioController _controller;
        private AudioCategory[] _categories;
        private DropdownField _categoriesDropdown;
        private List<AudioId> _categoryAudioIds;
        private AudioCategory _selectedCategory;
        private VisualElement _audioIdVisualElementContainer;
        private static int _selectedCategoryIndex = 0;

        void OnEnable()
        {
            _masterCategory = serializedObject.FindProperty("_masterCategory");
            _bgmCategory = serializedObject.FindProperty("_bgmCategory");
            _otherCategories = serializedObject.FindProperty("_availableSubCategories");
            _savePath = serializedObject.FindProperty("_configSavePath");
            _audioObjects = new List<IAudioObject>();
        }

        private void OnDisable()
        {
            if(_audioObjects != null)
            {
                foreach(var obj in _audioObjects)
                {
                    if (obj != null)
                        obj.Stop();
                }

                _audioObjects.Clear();
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            _controller = (AudioController)serializedObject.targetObject;
            VisualElement myInspector = new VisualElement();

            var classField = new ObjectField("Script")
            {
                value = MonoScript.FromMonoBehaviour((MonoBehaviour)target),
                objectType = GetType(),
                allowSceneObjects = false
            };
            classField.SetEnabled(false);
            myInspector.Add(classField);

            myInspector.Add(GetHeader("Main Categories"));
            myInspector.Add(new PropertyField(_masterCategory));
            myInspector.Add(new PropertyField(_bgmCategory));

            myInspector.Add(GetHeader("Sub Categories"));
            myInspector.Add(new PropertyField(_otherCategories));
            DrawConfigPath(myInspector);

            myInspector.Add(GetHeader("Audio IDs"));

            _categories = _controller.GetAllCategories();
            List<string> categoryChoices = new List<string>();
            for (int i = 0; i < _categories.Length; i++)
                categoryChoices.Add(_categories[i].name);

            _categoriesDropdown = new DropdownField(categoryChoices, _selectedCategoryIndex);
            _categoriesDropdown.RegisterValueChangedCallback(CategoryDropdownSet);
            myInspector.Add(_categoriesDropdown);

            _audioIdVisualElementContainer = new VisualElement();
            myInspector.Add(_audioIdVisualElementContainer);

            UpdateAudioIdList(_selectedCategoryIndex);
            return myInspector;
        }

        private void DrawConfigPath(VisualElement myInspector)
        {
            myInspector.Add(GetHeader("Config Path"));
            var configElement = new VisualElement()
            {
                style =
                {
                    paddingTop = 5,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    alignItems = new StyleEnum<Align>(Align.Stretch)
                }
            };
            myInspector.Add(configElement);
            var selectSaveFolderButton = new Button(
               () =>
               {
                   var folderAsset = AssetDatabase.LoadAssetAtPath<Object>(_savePath.stringValue);
                   EditorGUIUtility.PingObject(folderAsset);
               })
            {
                style =
                {
                },
                text = "F"
            };
            configElement.Add(selectSaveFolderButton);

            var savePathInputField = new PropertyField(_savePath, "")
            {
                style =
                {
                    flexGrow = new StyleFloat(1)
                }
            };
            configElement.Add(savePathInputField);
        }

        private void CategoryDropdownSet(ChangeEvent<string> evt)
        {
            UpdateAudioIdList(_categoriesDropdown.index);
        }

        private void UpdateAudioIdList(int categoryIndex)
        {
            if (_categories.Length <= categoryIndex)
                return;
            _selectedCategoryIndex = categoryIndex;
            _selectedCategory = _categories[categoryIndex];
            _categoryAudioIds = new List<AudioId>();

            _audioIdVisualElementContainer.Clear();

            string[] guids = AudioIdEditorHelper.GetAllAudioIdGUIDs();
            foreach (string guid in guids)
            {
                var audioId = AudioIdEditorHelper.GetAudioIdFromGUID(guid);
                if (audioId && audioId.Category == _selectedCategory)
                {
                    _categoryAudioIds.Add(audioId); 
                    CreateAudioIdButton(audioId);
                }
            }
        }

        private void CreateAudioIdButton(AudioId audioId)
        {
            IAudioObject audioInstance = null;

            var audioIdElement = new VisualElement()
            {
                style =
                {
                    paddingLeft =2,
                    paddingTop = 4,
                    paddingBottom = 4,
                    marginLeft = 3,
                    marginTop =5,
                    marginBottom=0,
                    width = 300,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                    backgroundColor = new StyleColor(new Color(0,0,0,.1f))
                    
                }
            };

            var label = new Label(audioId.name)
            {
                style =
                {
                    paddingTop = 4,
                    flexGrow = new StyleFloat(1)
                }
            };

            var playbackButton = new Button()
            {
                text = GetPlaybackCharacter(audioInstance != null)
            };
            playbackButton.SetEnabled(Application.isPlaying);

            var pauseButton = new Button()
            {
                style =
                {                    
                    fontSize = 16,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)
                },
                text = GetPauseCharacter(audioInstance != null && audioInstance.IsPaused())
            };
            pauseButton.SetEnabled(Application.isPlaying && audioInstance != null);
            pauseButton.RegisterCallback((ClickEvent evt) =>
            {
                if (Application.isPlaying)
                {
                    if (audioInstance != null)
                    {
                        if (audioInstance.IsPaused())
                            audioInstance.Unpause(.5f);
                        else
                            audioInstance.Pause(.5f);

                        pauseButton.text = GetPauseCharacter(audioInstance != null && audioInstance.IsPaused());
                    }
                }
            });

            playbackButton.RegisterCallback((ClickEvent evt) =>
            {
                if (Application.isPlaying)
                {
                    if (audioInstance == null)
                    {
                        if (_controller.BgmCategory == audioId.Category)
                            audioInstance = _controller.PlayMusic(audioId, 1);
                        else
                            audioInstance = _controller.Play(audioId, 1);

                        if (audioInstance != null)
                        {

                            _audioObjects.Add(audioInstance);
                            pauseButton.SetEnabled(true);
                            audioInstance.AddOnEnded(() =>
                            {
                                _audioObjects.Remove(audioInstance);
                                audioInstance = null;
                                playbackButton.text = GetPlaybackCharacter(audioInstance != null);
                                pauseButton.text = GetPauseCharacter(audioInstance != null && audioInstance.IsPaused());
                                pauseButton.SetEnabled(false);                               
                            });
                        }
                    }
                    else
                    {
                        _audioObjects.Remove(audioInstance);
                        pauseButton.SetEnabled(false);
                        audioInstance.Stop();
                        audioInstance = null;
                    }

                    playbackButton.text = GetPlaybackCharacter(audioInstance != null);
                    pauseButton.text = GetPauseCharacter(audioInstance != null && audioInstance.IsPaused());
                }
            });


            var selectAssetButton = new Button(
            ()=> {
            EditorGUIUtility.PingObject(audioId);
            })
            {
                text = "F"
            };

            audioIdElement.Add(selectAssetButton);
            audioIdElement.Add(label);
            audioIdElement.Add(playbackButton);
            audioIdElement.Add(pauseButton);
            _audioIdVisualElementContainer.Add(audioIdElement);
        }

        private string GetPlaybackCharacter(bool isPlaying)
        {
            return isPlaying ? @"\u25A0" : @"\u25BA";
        }
        private string GetPauseCharacter(bool isPaused)
        {
            return isPaused ? @"\u25BA" : @"\u05F0";
        }

        private Label GetHeader(string text)
        {
            return new Label
            {
                style =
                {
                    paddingTop = 15,
                    paddingLeft = 3.5f,
                    paddingBottom = 3,
                    fontSize = 18,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)
                },
                text = text
            };
        }

    }
}