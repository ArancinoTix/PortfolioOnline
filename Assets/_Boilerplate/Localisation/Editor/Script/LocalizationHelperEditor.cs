using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;
using UnityEngine.Localization.Components;
using System.IO;
using UnityEditor.Localization;

namespace U9.FigmaConverterHelper.Editor
{
    public class LocalizationHelperEditor : UnityEditor.EditorWindow
    {
        private List<LocalizationEditorVisual> _TMPROComponents;
        private string DEFAULT_TABLE_REFERENCE = "Project";
        private const string FOLDER_PATH = "Localisation Export";
        private Button _findAllBtn;
        private Button _applyBtn;
        private Button _downloadBtn;
        private Foldout _foldoutComponent;
        private GameObject _parentToAnalyze;
        private TextField _tableField;
        int _counter = 0;

        [MenuItem("Tools/Localization/Localization Assigner")]
        public static void ShowExample()
        {
            LocalizationHelperEditor wnd = GetWindow<LocalizationHelperEditor>();
            wnd.titleContent = new GUIContent("Localization Helper Editor");
        }


        private void CreateGUI()
        {
            
            VisualElement myInspector = rootVisualElement;
            //-----------------------------------------------------------------------------------
            //TITLE
            VisualElement titleContainer = new VisualElement();
            titleContainer.style.alignItems = Align.Center;
            _foldoutComponent = new Foldout()
            {
                style =
                {
                    backgroundColor = new StyleColor(new Color(0,0,0,.5f)) ,
                    alignContent = Align.Center ,
                    marginTop = 10
                }
            };
  
            var titleLabel = new Label()
            {
                text = "LOCALIZATION HELPER",
                style =
                {
                    alignContent = Align.Center,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold),
                    flexGrow = new StyleFloat(1),
                    marginLeft = 4,
                    marginRight = 4,
                    marginBottom = 4,
                    marginTop = 10
                }
            };

            titleContainer.Add(titleLabel);
            myInspector.Add(titleContainer);
            //-----------------------------------------------------------------------------------
            // Canvas header Visual Element

            VisualElement canvasSelector = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    marginTop = 5,
                    marginRight = 5,
                    marginLeft = 5 ,
                    marginBottom = 5,
                }
            };
            var canvasLabel = new Label()
            {
                text = "Selected Transform",
                style =
                {
                    alignContent = Align.Stretch,
                    marginRight = 5,
                    marginTop = 5,
                    marginBottom = 5
                }
            };
            var canvasField = new ObjectField()
            {
                style =
                {
                    alignContent = Align.Stretch,
                    flexGrow = new StyleFloat(1),
                    marginLeft = 5,
                    marginTop = 5,
                    marginBottom = 5,
                }
            };
            canvasField.RegisterValueChangedCallback((e) =>
            {
                var value = canvasField.value as GameObject;
                _foldoutComponent.Clear();
                if (value != null)
                {
                    _parentToAnalyze = canvasField.value as GameObject;
                    _findAllBtn.SetEnabled(true);
                }
                else
                {
                    _findAllBtn.SetEnabled(false);
                    _downloadBtn.SetEnabled(false);
                }
            });
            canvasSelector.Add(canvasLabel);
            canvasSelector.Add(canvasField);
            myInspector.Add(canvasSelector);
            //-----------------------------------------------------------------------------------
            VisualElement tableSelector = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    marginTop = 5,
                    marginRight = 5,
                    marginLeft = 5 ,
                    marginBottom = 5,
                }
            };
            var tableLabel = new Label()
            {
                text = "Selected Table Collection",
                style =
                {
                    alignContent = Align.Stretch,
                    marginRight = 5,
                    marginTop = 5,
                    marginBottom = 5
                }
            }; 
            _tableField = new TextField()
            {
                value = DEFAULT_TABLE_REFERENCE,
                style =
                {
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                     flexGrow = new StyleFloat(1),
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop =5,
                    marginBottom=5
                }
            };
            tableSelector.Add(tableLabel);
            tableSelector.Add(_tableField);
            myInspector.Add(tableSelector);
            //-----------------------------------------------------------------------------------
            //FIND ALL BUTTON
            _findAllBtn = new Button(
            () =>
            {
                _foldoutComponent.Clear();
                FindAllTMPComponents();
                _applyBtn.SetEnabled(true);
                _downloadBtn.SetEnabled(true);
            })
            {
                style =
            {
            },
                text = "Find All Components"
            };
            _applyBtn = new Button(
            () =>
            {
                ApplyLocalisation();
                this.Close();
            })
            {
                style =
            {
            },
                text = "Apply Localization Components"

            };

            _downloadBtn = new Button(
            () =>
            {
                DownloadCVSFile();
            })
            {
                style =
            {
            },
                text = "Download CVS File"

            };
            _downloadBtn.SetEnabled(false);
            _applyBtn.SetEnabled(false);
            _findAllBtn.SetEnabled(false);
            myInspector.Add(_findAllBtn);
            var scrollView = new ScrollView()
            {
                style =
                {
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop = 5,
                    marginBottom = 5,
                    flexGrow = new StyleFloat(1),

                }

            };



            scrollView.Add(_foldoutComponent);
            myInspector.Add(scrollView);
            myInspector.Add(_applyBtn);
            myInspector.Add(_downloadBtn);
        }

        private void DrawSingleListElement(LocalizationEditorVisual data)
        {
            if (data != null)
            {
                var dataContainer = new VisualElement()
                {
                    style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop =5,
                    marginBottom=5,
                    backgroundColor = new StyleColor(new Color(0,0,0,.1f))
                }
                };
                //-------------------------------------------------------------------------------------------------------
                //OBJ FIELD
                var objectField = new VisualElement()
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                        alignItems = new StyleEnum<Align>(Align.Stretch),
                        marginLeft = 5,
                        marginRight = 5,
                        marginTop =2,
                        marginBottom=2,
                        width = 250
                    }
                };

                var label = new Label()
                {
                    text = data.gameobject.name,
                    style =
                {
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                    flexGrow = new StyleFloat(1),
                    alignContent = Align.Stretch,
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop = 5,
                    marginBottom = 5
                }
                };
                var selectAssetButton = new Button(
                 () =>
                 {
                     EditorGUIUtility.PingObject(data.gameobject);
                 })
                {
                    style =
                {
                        position = Position.Absolute,
                        right = 0,
                        marginLeft = 5,
                        marginRight = 5,
                        marginTop = 5,
                        marginBottom = 5
            },
                    text = "F"
                };
                objectField.Add(label);
                //-------------------------------------------------------------------------------------------------------

                var textLabel = new TextField()
                {
                    value = data.suggestedCopy,
                style =
                {
                    alignItems = new StyleEnum<Align>(Align.Stretch),
                    flexGrow = new StyleFloat(1),
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop =5,
                    marginBottom=5
                }
                };
                textLabel.RegisterValueChangedCallback((e) =>
                {
                    data.suggestedCopy = e.newValue;
                    ComponentModifiedThroughEditor(data);
                });
                
                var toggle = new Toggle()
                {
                    style =
                {
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop =5,
                    marginBottom=5
                },
                    value = data.isSelected,
                };
                toggle.RegisterValueChangedCallback((e) =>
                {
                    data.isSelected = e.newValue;
                    ComponentModifiedThroughEditor(data);
                    textLabel.SetEnabled(e.newValue);
                    objectField.SetEnabled(e.newValue);
                });
                textLabel.SetEnabled(data.isSelected);
                objectField.SetEnabled(data.isSelected);
                dataContainer.Add(toggle);
                dataContainer.Add(objectField);
                dataContainer.Add(textLabel);
                dataContainer.Add(selectAssetButton);
                _foldoutComponent.Add(dataContainer);
            }
            else
                Debug.LogError("DATA == null");
        }

        public void FindAllTMPComponents()
        {
            _TMPROComponents = new List<LocalizationEditorVisual>();
            _counter = 0;
            FindAllTMPComponentsRecursivelly(_parentToAnalyze.transform);
        }
        private void FindAllTMPComponentsRecursivelly(Transform parent)
        {
            foreach (Transform child in parent)
            {
                if (child.GetComponent<TMP_InputField>() != null)
                {
                    var List = child.GetComponentsInChildren<TextMeshProUGUI>();
                    foreach (var txt in List)
                    {
                        //if it's not the text component, it's the placeholder one
                        if (txt != child.GetComponent<TMP_InputField>().textComponent)
                        {
                            AddComponentToList(txt.gameObject);
                        }
                    }
                }
                else if (child.GetComponent<TextMeshProUGUI>() != null)
                {
                    AddComponentToList(child.gameObject);
                    FindAllTMPComponentsRecursivelly(child);
                }
                else if (child.GetComponent<TMP_Dropdown>() != null)
                    return;
                else
                    FindAllTMPComponentsRecursivelly(child);
            }
        }

        private void AddComponentToList(GameObject child)
        {
            var localizeComponent = child.GetComponent<LocalizeStringEvent>();
            //No localize string component attached
            if (localizeComponent == null)
            {
                var localizationEditorVisual = new LocalizationEditorVisual(child, SuggestCopy(child.transform), _counter, false);
                DrawSingleListElement(localizationEditorVisual);
                _TMPROComponents.Add(localizationEditorVisual);

            }
            //localize string component already attached
            else
            {
                var tableCollection = LocalizationEditorSettings.GetStringTableCollection(localizeComponent.StringReference.TableReference);
                string entryName = string.Empty;
                if (tableCollection != null)
                    entryName = localizeComponent.StringReference.TableEntryReference.ResolveKeyName(tableCollection.SharedData);
                var localizationEditorVisual = new LocalizationEditorVisual(child, entryName, _counter, true, false);
                DrawSingleListElement(localizationEditorVisual);
                _TMPROComponents.Add(localizationEditorVisual);
                
            }
            _counter++;
        }

        private void ComponentModifiedThroughEditor(LocalizationEditorVisual data)
        {
            _TMPROComponents[data.index].isSelected = data.isSelected;
        }

        private void ApplyLocalisation()
        {
            LocalizeStringEvent localizedString;
            foreach (var element in _TMPROComponents)
            {
                if (element.isSelected)
                {

                    if (element.gameobject.GetComponent<LocalizeStringEvent>() == null)
                        localizedString = element.gameobject.AddComponent<LocalizeStringEvent>();
                    else
                        localizedString = element.gameobject.GetComponent<LocalizeStringEvent>();


                    var tablereferences = localizedString;
                    localizedString.SetTable(_tableField.value);
                    localizedString.SetEntry(element.suggestedCopy);
                    localizedString.RefreshString();
                }
            }
        }

        private string SuggestCopy(Transform component)
        {
            Transform componentToCheck = component;
            bool rootFound = false;
            string viewName = string.Empty;
            string name = string.Empty;
            string componentName = string.Empty;
            string componentParentName = string.Empty;
            while (!rootFound)
            {
                if(componentToCheck.parent.gameObject.GetComponent<Canvas>() == null)
                {
                    componentName = componentToCheck.parent.gameObject.name.ToLower();
                    if ((componentName.Contains("btn") || componentName.Contains("button") || componentName.Contains("field")) && componentParentName == string.Empty)
                        componentParentName = componentName;

                    componentToCheck = componentToCheck.parent;
                }  
                else
                {
                    if(componentParentName == string.Empty)
                        viewName  = componentToCheck.name.ToLower();
                    else
                        viewName = componentToCheck.name.ToLower() + "." + componentParentName;
                    rootFound = true;
                }
            }

            name = viewName.Replace(" ", "") + "." + component.gameObject.name.Replace(" ", "");
            return name;
        }

        private void DownloadCVSFile()
        {
            System.Text.StringBuilder csvContentBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < _TMPROComponents.Count; i++)
            {
                if(_TMPROComponents[i].hasLocalizationComponent && _TMPROComponents[i].suggestedCopy != string.Empty)
                {
                    string secondColumn = _TMPROComponents[i].gameobject.GetComponent<TextMeshProUGUI>().text != null ? GetFormattedString(_TMPROComponents[i].gameobject.GetComponent<TextMeshProUGUI>().text) : string.Empty;
                    csvContentBuilder.Append(_TMPROComponents[i].suggestedCopy).Append(",").Append(secondColumn).Append("\n");
                }
            }

            string folderPath = Path.Combine(Application.dataPath, FOLDER_PATH);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, "data.csv");
            File.WriteAllText(filePath, csvContentBuilder.ToString());
            Application.OpenURL(folderPath);
            Debug.Log("CSV file exported to: " + filePath);
        }
        private string GetFormattedString(string stringToFormat)
        {
            if (stringToFormat.Contains('\n') || stringToFormat.Contains(','))
            {
                string newString = "\" " + stringToFormat + "\" ";
                return newString;
            }
            else
                return stringToFormat;
        }
    }

    public class LocalizationEditorVisual
    {
        public GameObject gameobject;
        public string suggestedCopy = string.Empty;
        public bool isSelected = true;
        public bool hasLocalizationComponent;
        public int index;

        public LocalizationEditorVisual(GameObject gameObject, string suggestedCopy, int index,bool hasLocalizationComponent, bool isSelected = true)
        {
            this.gameobject = gameObject;
            this.suggestedCopy = suggestedCopy;
            this.hasLocalizationComponent = hasLocalizationComponent;
            this.index = index;
            this.isSelected = isSelected;
        }
    }
}