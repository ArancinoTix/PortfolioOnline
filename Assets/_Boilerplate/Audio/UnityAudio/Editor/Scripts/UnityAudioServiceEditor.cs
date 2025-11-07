using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using U9.Audio.Data;
using U9.Audio.Service;
using UnityEditor;
using U9.Audio.Editor;

namespace U9.Audio.UnityAudio.Editor
{
    [CustomEditor(typeof(UnityAudioService))]
    public class UnityAudioServiceEditor : UnityEditor.Editor
    {
        private class DataElement
        {
            public VisualElement visualElement;
            public Foldout foldoutField;
            public Label label;

            public DataElement(VisualElement visualElement, Foldout foldout, Label label)
            {
                this.visualElement = visualElement;
                this.foldoutField = foldout;
                this.label = label;
            }
        }

        private class StrayElement
        {
            public VisualElement visualElement;
            public Label label;
            public AudioId id;

            public StrayElement(VisualElement visualElement, AudioId id, Label label)
            {
                this.visualElement = visualElement;
                this.id = id;
                this.label = label;
            }
        }

        private SerializedProperty _sourcePrefab;
        private SerializedProperty _musicFade;
        private SerializedProperty _maxPooledSourceCount;
        private SerializedProperty _availableAudioClips;

        private List<IAudioObject> _audioObjects;

        private AudioController _controller;

        private AudioCategory[] _categories;
        private DropdownField _categoriesDropdown;
        private AudioCategory _selectedCategory;

        private Foldout _strayIdVisualElementContainer;
        private VisualElement _audioDataVisualElementContainer;
        private List<DataElement> _dataElements;
        private List<StrayElement> _strayElements;
        private static int _selectedCategoryIndex = 0;
        private int _otherCategoryOptionCount = 0;
        private TextField _searchField;

        private VisualElement _createIdRoot;
        private Button _deleteButton;

        void OnEnable()
        {
            _sourcePrefab = serializedObject.FindProperty("_sourcePrefab");
            _musicFade = serializedObject.FindProperty("_musicFade");
            _maxPooledSourceCount = serializedObject.FindProperty("_maxPooledSourceCount");
            _availableAudioClips = serializedObject.FindProperty("_availableAudioClips");
            _dataElements = new List<DataElement>();
            _audioObjects = new List<IAudioObject>();
        }
        private void OnDisable()
        {
            if (_audioObjects != null)
            {
                foreach (var obj in _audioObjects)
                {
                    if (obj != null)
                        obj.Stop();
                }

                _audioObjects.Clear();
            }

            if (_dataElements != null)
                _dataElements.Clear();
        }

        public override VisualElement CreateInspectorGUI()
        {
            _controller = FindAnyObjectByType<AudioController>();

            VisualElement myInspector = new VisualElement();

            var classField = new ObjectField("Script")
            {
                value = MonoScript.FromMonoBehaviour((MonoBehaviour)target),
                objectType = GetType(),
                allowSceneObjects = false
            };
            classField.SetEnabled(false);
            myInspector.Add(classField);

            //Absolutely required as this data is tied to the controller
            if (_controller == null)
                myInspector.Add(GetError("No audio controller found in the scene, please add one before continuing to edit this service"));

            DrawProperties(myInspector);
            DrawCategories(myInspector);
            DrawCreateID(myInspector);


            myInspector.Add(GetHeader("Stray Audio IDs"));
            _strayIdVisualElementContainer = new Foldout();
            _strayIdVisualElementContainer.value = false;
            myInspector.Add(_strayIdVisualElementContainer);
            DrawStrayIDs();

            myInspector.Add(GetHeader("Audio Data"));
            DrawSearchBox(myInspector);
            _audioDataVisualElementContainer = new VisualElement();
            myInspector.Add(_audioDataVisualElementContainer);

            CreateDataElements();
            UpdateAudioDataList(_selectedCategoryIndex);

            UpdateVisibleStrayIds();
            return myInspector;
        }

        /// <summary>
        /// Draws the general properties
        /// </summary>
        /// <param name="myInspector"></param>
        private void DrawProperties(VisualElement myInspector)
        {
            myInspector.Add(GetHeader("Properties"));
            myInspector.Add(new PropertyField(_sourcePrefab));
            myInspector.Add(new PropertyField(_maxPooledSourceCount));
            myInspector.Add(new PropertyField(_musicFade));
        }

        /// <summary>
        /// Draw the categories
        /// </summary>
        /// <param name="myInspector"></param>
        private void DrawCategories(VisualElement myInspector)
        {
            myInspector.Add(GetHeader("Category"));

            //Gets the available Categories
            _categories = _controller.GetAllCategories();
            List<string> categoryChoices = new List<string>();

            //Add our base options
            categoryChoices.Add("- ALL -");
            categoryChoices.Add("- UNASSIGNED -");
            _otherCategoryOptionCount = 2;

            //Add the others
            for (int i = 0; i < _categories.Length; i++)
                categoryChoices.Add(_categories[i].name);

            _categoriesDropdown = new DropdownField(categoryChoices, _selectedCategoryIndex);
            _categoriesDropdown.RegisterValueChangedCallback(CategoryDropdownSet);
            myInspector.Add(_categoriesDropdown);
        } 
        
        /// <summary>
        /// Draws a search box for finding specific entries
        /// </summary>
        /// <param name="root"></param>
        private void DrawSearchBox(VisualElement root)
        {
            var label = new Label()
            {
                style =
                {
                    paddingTop = 3,
                    paddingLeft = 4
                },
                text = "Find"
            };
            var searchRoot = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                    marginBottom = 7
                }
            };

            _searchField = new TextField()
            {
                style =
                {
                    flexGrow = new StyleFloat(1),
                    marginRight = 5
                }
            };
            _searchField.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                UpdateAudioDataList(_selectedCategoryIndex);
                UpdateVisibleStrayIds();
            });

            searchRoot.Add(label);
            searchRoot.Add(_searchField);
            root.Add(searchRoot);
        }

        /// <summary>
        /// Draw the window to create a new ID
        /// </summary>
        /// <param name="root"></param>
        private void DrawCreateID(VisualElement root)
        {
            root.Add(GetHeader("Create ID"));
            _createIdRoot = AudioIdEditorHelper.DrawCreateID(root, AttemptIdCreation);
        }

        /// <summary>
        /// Called by the ID creation. Attempts to create the new ID
        /// </summary>
        /// <param name="name"></param>
        void AttemptIdCreation(string name)
        {
            if (_selectedCategory == null)
            {
                //Cannot have a blank category
                Debug.LogError("Failed to make new ID, category is null");
            }
            else
            {
                var newId = AudioIdEditorHelper.Create(name, _selectedCategory);

                CreateNewIdData(newId);
            }
        }

        private void CreateNewIdData(AudioId newId)
        {
            if (newId == null)
            {
                //Failed to make ID
                Debug.LogError("Failed to make new ID as it is null");
            }
            else
            {
                //Success
                //Make new data entry
                _availableAudioClips.InsertArrayElementAtIndex(_availableAudioClips.arraySize);

                var audioData = _availableAudioClips.GetArrayElementAtIndex(_availableAudioClips.arraySize - 1);

                //Set default values
                audioData.FindPropertyRelative("_audioId").objectReferenceValue = newId;
                audioData.FindPropertyRelative("_clip").objectReferenceValue = null;
                audioData.FindPropertyRelative("_loop").boolValue = false;
                audioData.FindPropertyRelative("_priority").intValue = 128;
                audioData.FindPropertyRelative("_pitch").floatValue = 1;
                audioData.FindPropertyRelative("_stereoPan").floatValue = 0;
                audioData.FindPropertyRelative("_volume").floatValue = 1;
                audioData.FindPropertyRelative("_spatialBlend").floatValue = 0;

                //Apply
                serializedObject.ApplyModifiedProperties();

                //Draw the UI
                DrawAudioData(audioData, _selectedCategory, true);

                _searchField.value = string.Empty;

                UpdateAudioDataList(_selectedCategoryIndex);
                DrawStrayIDs();
                UpdateVisibleStrayIds();
            }
        }

        /// <summary>
        /// Called when a user picks from the category dropdown
        /// </summary>
        /// <param name="evt"></param>
        private void CategoryDropdownSet(ChangeEvent<string> evt)
        {
            UpdateAudioDataList(_categoriesDropdown.index);
            UpdateVisibleStrayIds();
        }

        /// <summary>
        /// Create all the existing data elements
        /// </summary>
        private void CreateDataElements()
        {
            _audioDataVisualElementContainer.Clear();
            _deleteButton = null;

            if (_dataElements != null)
                _dataElements.Clear();

            int numberOfAudioData = _availableAudioClips.arraySize;

            for (int i = 0; i < numberOfAudioData; i++)
            {
                var audioData = _availableAudioClips.GetArrayElementAtIndex(i);
                DrawAudioData(audioData, _selectedCategory);
            }
        }

        private void DrawStrayIDs()
        {
            _strayElements = new List<StrayElement>();
            _strayIdVisualElementContainer.Clear();

            //Get the used list
            AudioId[] usedIds = new AudioId[_availableAudioClips.arraySize];
            for (int i = 0, ni = usedIds.Length; i < ni; i++)
            {
                usedIds[i] = (AudioId)_availableAudioClips.GetArrayElementAtIndex(i).FindPropertyRelative("_audioId").objectReferenceValue;
            }

            //Gather all known ids
            string[] guids = AudioIdEditorHelper.GetAllAudioIdGUIDs();
            List<AudioId> unusedIds = new List<AudioId>();

            for (int i = 0, ni = guids.Length; i < ni; i++)
            {
                var id = AudioIdEditorHelper.GetAudioIdFromGUID(guids[i]);

                if (id != null)
                    unusedIds.Add(id);
            }

            //Now compare
            for (int i = unusedIds.Count - 1; i >= 0; i--)
            {
                var id = unusedIds[i];
                for (int ii = 0, nii = usedIds.Length; ii < nii; ii++)
                {
                    //If we match, remove as it is in use
                    if (usedIds[ii] == id)
                    {
                        unusedIds.RemoveAt(i);
                        break;
                    }
                }
            }

            //finally draw the UI
            for (int i = unusedIds.Count - 1; i >= 0; i--)
            {
                //Container for the header
                var strayRowElement = new VisualElement()
                {
                    style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                     paddingLeft =2,
                    paddingTop = 4,
                    paddingBottom = 4,
                    marginLeft = 0,
                    marginTop =5,
                    marginBottom=0,
                    backgroundColor = new StyleColor(new Color(0,0,0,.1f))
                }
                };

                var id = unusedIds[i];

                //Create the name label
                var label = new Label(id.name)
                {
                    style =
                {
                    paddingTop = 3,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold),
                    flexGrow = new StyleFloat(1)
                }
                };

                var selectAssetButton = new Button(
                 () =>
                 {
                     EditorGUIUtility.PingObject(id);
                 })
                {
                    style =
                {
                },
                    text = "F"
                };

                var useButton = new Button(
                () =>
                {
                    CreateNewIdData(id);
                })
                {
                    style =
                {
                },
                    text = "Use"
                };

                strayRowElement.Add(selectAssetButton);
                strayRowElement.Add(label);
                strayRowElement.Add(useButton);
                _strayIdVisualElementContainer.Add(strayRowElement);

                _strayElements.Add(new StrayElement(strayRowElement, id, label));
            }
        }

        private void UpdateVisibleStrayIds()
        {
            string searchText = _searchField.text.ToLower();

            foreach (StrayElement element in _strayElements)
            {
                bool isInCategory = false;

                //0 or 1, we can be shown
                if (_selectedCategoryIndex == 0 || _selectedCategoryIndex == 1)
                    isInCategory = true;               
                else
                    isInCategory = element.id.Category == _selectedCategory;

                if (isInCategory && !string.IsNullOrEmpty(_searchField.text))
                {
                    isInCategory = element.label.text.ToLower().Contains(searchText);
                }

                //Remove the element if not valid for the category
                if (!isInCategory)
                {
                    if (_strayIdVisualElementContainer.Contains(element.visualElement))
                        _strayIdVisualElementContainer.Remove(element.visualElement);
                }
                //Or add the element if valid for the category
                else
                {
                    _strayIdVisualElementContainer.Add(element.visualElement);
                }
            }
        }

        /// <summary>
        /// Updates the audio list to display the relevant data
        /// </summary>
        /// <param name="categoryIndex"></param>
        private void UpdateAudioDataList(int categoryIndex)
        {
            //Only allow new IDs if we selected a category
            _createIdRoot.SetEnabled(categoryIndex >= 2);

            //Set out category
            _selectedCategoryIndex = categoryIndex;

            if(categoryIndex <_otherCategoryOptionCount)
                _selectedCategory = null;
            else
                _selectedCategory = _categories[categoryIndex-_otherCategoryOptionCount];

            string searchText = _searchField.text.ToLower();

            //For all elements
            for (int i = _dataElements.Count-1; i >=0; i--)
            {
                //Get the element
                var element = _dataElements[i];
                var dataProperty = _availableAudioClips.GetArrayElementAtIndex(i);
                var audioIdProperty = dataProperty.FindPropertyRelative("_audioId");
                AudioId idObject = (AudioId)audioIdProperty.objectReferenceValue;

                bool isInCategory = false;

                //0 is ALL, we can be shown
                if (_selectedCategoryIndex == 0)
                    isInCategory = true;
                //1 is UNASSIGNED, and if we have no id, we can be shown
                else if (_selectedCategoryIndex == 1 && idObject == null)
                    isInCategory = true;
                else if(idObject != null)
                    isInCategory = idObject.Category == _selectedCategory;

                if(isInCategory && !string.IsNullOrEmpty(_searchField.text))
                {
                    isInCategory = element.label.text.ToLower().Contains(searchText);
                }

                //Enabled us if allowed
                element.visualElement.SetEnabled(isInCategory);

                //Reset our expansion
                if (!isInCategory)
                {
                    dataProperty.isExpanded = false;
                    element.foldoutField.value = false;
                }

                //Remove the element if not valid for the category
                if (!isInCategory)
                {
                    if(_audioDataVisualElementContainer.Contains(element.visualElement))
                        _audioDataVisualElementContainer.Remove(element.visualElement);
                }
                //Or add the element if valid for the category
                else
                {
                    _audioDataVisualElementContainer.Add(element.visualElement);
                }
            }
        }

        /// <summary>
        /// Draws the given data
        /// </summary>
        /// <param name="audioData"></param>
        /// <param name="selectedCategory"></param>
        /// <param name="insertAtTheStart"></param>
        /// <returns></returns>
        private void DrawAudioData(SerializedProperty audioData, AudioCategory selectedCategory, bool insertAtTheStart = false)
        {
            //Get the data
            var audioIdProperty = audioData.FindPropertyRelative("_audioId");
            var clipProperty = audioData.FindPropertyRelative("_clip");
            var loopProperty = audioData.FindPropertyRelative("_loop");
            var priorityProperty = audioData.FindPropertyRelative("_priority");
            var pitchProperty = audioData.FindPropertyRelative("_pitch");
            var stereoPanProperty = audioData.FindPropertyRelative("_stereoPan");
            var volumeProperty = audioData.FindPropertyRelative("_volume");
            var spatialBlendProperty = audioData.FindPropertyRelative("_spatialBlend");

            //Get our current ID if we have one
            AudioId idObject = (AudioId)audioIdProperty.objectReferenceValue;

            //Data container for an instance of audio 
            IAudioObject audioInstance = null;

            //Container for the header
            var audioDataHeaderElement = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                }
            };
            
            //Name of ID
            AudioId id = (AudioId)audioIdProperty.objectReferenceValue;
            AudioClip clip = (AudioClip)clipProperty.objectReferenceValue;
            var labelText = GetAudioDataName(id, clip);

            //Create the name label
            var label = new Label(labelText)
            {
                style =
                {
                    paddingTop = 4,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold),
                    flexGrow = new StyleFloat(1)
                }
            };

            //Play button
            var playbackButton = new Button()
            {
                text = GetPlaybackCharacter(audioInstance != null)
            };
            playbackButton.SetEnabled(Application.isPlaying);

            //Pause button
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

            //Pause reaction
            pauseButton.RegisterCallback((ClickEvent evt) =>
            {
                if (Application.isPlaying)
                {
                    if (audioInstance != null)
                    {
                        //Pause or unpause
                        if (audioInstance.IsPaused())
                            audioInstance.Unpause(.5f);
                        else
                            audioInstance.Pause(.5f);

                        pauseButton.text = GetPauseCharacter(audioInstance != null && audioInstance.IsPaused());
                    }
                }
            });

            //Playback reaction
            playbackButton.RegisterCallback((ClickEvent evt) =>
            {
                if (Application.isPlaying)
                {
                    //If we do not have an instance
                    if (audioInstance == null)
                    {
                        AudioId id = (AudioId)audioIdProperty.objectReferenceValue;

                        if (id != null)
                        {
                            //Play it
                            if (_controller.BgmCategory == id.Category)
                                audioInstance = _controller.PlayMusic(id, 1);
                            else
                                audioInstance = _controller.Play(id, 1);

                            if (audioInstance != null)
                            {
                                _audioObjects.Add(audioInstance);

                                //enable the pause
                                pauseButton.SetEnabled(true);

                                //Wait for the instance to finish
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
                    }
                    else
                    {
                        //If we do have an instance, stop it
                        _audioObjects.Remove(audioInstance);
                        pauseButton.SetEnabled(false);
                        audioInstance.Stop();
                        audioInstance = null;
                    }

                    //Update icons
                    playbackButton.text = GetPlaybackCharacter(audioInstance != null);
                    pauseButton.text = GetPauseCharacter(audioInstance != null && audioInstance.IsPaused());
                }
            });

            //Create the sub fields. We make the them individually, as we need to know when the values change
            //We need to use the BindProperty method as we may add new audio datas after the visual tree is created
            var idPropertyField = new PropertyField();
            idPropertyField.BindProperty(audioIdProperty);

            var clipField = new PropertyField();
            clipField.BindProperty(clipProperty);

            var loopField = new PropertyField();
            loopField.BindProperty(loopProperty);

            var priorityField = new PropertyField();
            priorityField.BindProperty(priorityProperty);

            var volumeField = new PropertyField();
            volumeField.BindProperty(volumeProperty);

            var pitchField = new PropertyField();
            pitchField.BindProperty(pitchProperty);

            var stereoPanField = new PropertyField();
            stereoPanField.BindProperty(stereoPanProperty);

            var spatialBlendField = new PropertyField();
            spatialBlendField.BindProperty(spatialBlendProperty);

            //Events to update label copy
            idPropertyField.RegisterValueChangeCallback(
              (SerializedPropertyChangeEvent evt) =>
              {
                  AudioId id = (AudioId)audioIdProperty.objectReferenceValue;
                  AudioClip clip = (AudioClip)clipProperty.objectReferenceValue;

                  label.text = GetAudioDataName(id, clip);
              });

            clipField.RegisterValueChangeCallback(
              (SerializedPropertyChangeEvent evt) =>
              {
                  AudioId id = (AudioId)audioIdProperty.objectReferenceValue;
                  AudioClip clip = (AudioClip)clipProperty.objectReferenceValue;

                  label.text = GetAudioDataName(id, clip);
              });

            //Create a foldout for the subfields
            var audioDataFoldoutElement = new Foldout();
            audioDataFoldoutElement.Add(idPropertyField);
            audioDataFoldoutElement.Add(clipField);
            audioDataFoldoutElement.Add(loopField);
            audioDataFoldoutElement.Add(volumeField);

            //Fold out for data we normally would not edit
            var extraDataFoldoutElement = new Foldout();
            extraDataFoldoutElement.value = false;
            extraDataFoldoutElement.Add(priorityField);
            extraDataFoldoutElement.Add(pitchField);
            extraDataFoldoutElement.Add(stereoPanField);
            extraDataFoldoutElement.Add(spatialBlendField);
            audioDataFoldoutElement.Add(extraDataFoldoutElement);

            //Update the foldout
            audioDataFoldoutElement.value = audioData.isExpanded;
            audioDataFoldoutElement.RegisterValueChangedCallback((ChangeEvent<bool> evt) => { audioData.isExpanded = evt.newValue; });

            //Add all the elements
            audioDataHeaderElement.Add(playbackButton);
            audioDataHeaderElement.Add(pauseButton);
            audioDataHeaderElement.Add(label);

            //General container for the data
            var audioDataElement = new VisualElement()
            {
                style =
                {
                    paddingLeft =2,
                    paddingTop = 4,
                    paddingBottom = 4,
                    marginLeft = 3,
                    marginTop =5,
                    marginBottom=0,
                    backgroundColor = new StyleColor(new Color(0,0,0,.1f))
                }
            };

            audioDataElement.Add(audioDataHeaderElement);
            audioDataElement.Add(audioDataFoldoutElement);

            //Add to the beginning if needed
            if (insertAtTheStart)
            {
                audioData.isExpanded = true;
                audioDataFoldoutElement.value =true;
                _audioDataVisualElementContainer.Insert(0, audioDataElement);
            }
            else
                _audioDataVisualElementContainer.Add(audioDataElement);

            _dataElements.Add(new DataElement(audioDataElement, audioDataFoldoutElement, label));

            //Delete button
            var deleteButton = new Button()
            {
                style =
                {
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)
                },
                text = "Del"
            };

            //delete reaction
            deleteButton.RegisterCallback((ClickEvent evt) =>
            {
                if(AttemptDelete(deleteButton, audioData))
                {
                    if (audioInstance != null)
                    {
                        _audioObjects.Remove(audioInstance);
                        audioInstance.Stop();
                        audioInstance = null;
                    }
                    audioData = null;
                }
            });
            audioDataHeaderElement.Add(deleteButton);
        }

        /// <summary>
        /// Attempt to delete the given property
        /// </summary>
        /// <param name="deleteButton"></param>
        /// <param name="audioProperty"></param>
        /// <returns></returns>
        private bool AttemptDelete(Button deleteButton, SerializedProperty audioProperty)
        {
            //If we match buttons, this is our second press
            if(_deleteButton == deleteButton)
            {
                //Reset the button
                if(_deleteButton != null)
                    _deleteButton.text = "Del";

                _deleteButton = null;
            
                //Find the matching property
                for (int i = 0, ni = _availableAudioClips.arraySize; i < ni; i++)
                {
                    if (SerializedProperty.EqualContents(_availableAudioClips.GetArrayElementAtIndex(i), audioProperty))
                    {
                        //Delete it
                        _availableAudioClips.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();

                        //Refresh the visuals
                        CreateDataElements();
                        UpdateAudioDataList(_selectedCategoryIndex);
                        DrawStrayIDs();
                        break;
                    }
                }
                return true;
            }
            //If this is our first press, update the label
            else
            {
                if (_deleteButton != null)
                    _deleteButton.text = "Del";

                _deleteButton = deleteButton;
                if (_deleteButton != null)
                    _deleteButton.text = "Confirm Del";

                return false;
            }
        }

        /// <summary>
        /// Returns a formated name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        private string GetAudioDataName(AudioId id, AudioClip clip)
        {
            var idName = id != null ? id.name : "UNASSIGNED";
            var clipName = clip != null ? clip.name : "NULL";

            return $"{idName} - {clipName}";
        }

        /// <summary>
        /// Unicode chars for play, pause and stop
        /// </summary>
        /// <param name="isPlaying"></param>
        /// <returns></returns>
        private string GetPlaybackCharacter(bool isPlaying)
        {
            return isPlaying ? @"\u25A0" : @"\u25BA";
        }

        /// <summary>
        /// Unicode chars for play, pause and stop
        /// </summary>
        /// <param name="isPlaying"></param>
        /// <returns></returns>
        private string GetPauseCharacter(bool isPaused)
        {
            return isPaused ? @"\u25BA" : @"\u05F0";
        }

        /// <summary>
        /// Formated header label
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Formated error label
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Label GetError(string text, float size = 18)
        {
            return new Label
            {
                style =
                {
                    paddingTop = 15,
                    paddingLeft = 3.5f,
                    paddingBottom = 3,
                    fontSize = size,
                    color = Color.red,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)
                },
                text = text
            };
        }
    }
}
