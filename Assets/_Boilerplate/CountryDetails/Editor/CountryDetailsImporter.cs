using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace U9.CountryDetails.Editor
{
    public class CountryDetailsImporter : EditorWindow
    {
        private static GUIStyle _centeredBoldLabel;

        private TextAsset _inputTextAsset;

        /// <summary>
        ///     The absolute path of output folder. It ends with the folder name, there is no slash at the end of a string.
        /// </summary>
        private string _outputAbsolutePath = string.Empty;

        /// <summary>
        ///     Style for a label with normal sized, bold font and centered text.
        /// </summary>
        private static GUIStyle CenteredBoldLabel
        {
            get
            {
                if (_centeredBoldLabel == null)
                {
                    _centeredBoldLabel = new GUIStyle("BoldLabel");
                    _centeredBoldLabel.alignment = TextAnchor.MiddleCenter;
                }

                return _centeredBoldLabel;
            }
        }

        [MenuItem("Tools/Localization/Country Details Importer")]
        public static void OpenWindow()
        {
            var window = GetWindow<CountryDetailsImporter>();
            window.titleContent = new GUIContent("CSV Convert");
            window.Show();
        }

        private void OnGUI()
        {
            DrawInputOutputParamsSection();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DrawActionsSection();
        }

        private void DrawInputOutputParamsSection()
        {
            EditorGUILayout.LabelField("Input / output params", CenteredBoldLabel);

            EditorGUILayout.BeginVertical("box");
            {
                // Draw the input field for Text Asset.
                _inputTextAsset = (TextAsset)EditorGUILayout.ObjectField("Input Text Asset", _inputTextAsset, typeof(TextAsset), false);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    // Draw button which opens system dialog to select an output folder.
                    if (GUILayout.Button("Select folder", GUILayout.MaxWidth(150)))
                    {
                        string path = !string.IsNullOrEmpty(_outputAbsolutePath) ?  _outputAbsolutePath : Application.dataPath;
                        string newPath = EditorUtility.OpenFolderPanel("Output folder", path, "");

                        if (!string.IsNullOrEmpty(newPath))
                        {
                            _outputAbsolutePath = newPath;
                        }
                    }

                    EditorGUILayout.LabelField("Output path:", AbsolutePathToProjectPath(_outputAbsolutePath));

                    // Draw the Focus button. It pings the output path folder in Unity project view.
                    if (GUILayout.Button("F", GUILayout.MaxWidth(25)))
                    {
                        var folderAsset = AssetDatabase.LoadAssetAtPath<Object>(AbsolutePathToProjectPath(_outputAbsolutePath));
                        EditorGUIUtility.PingObject(folderAsset);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawActionsSection()
        {
            EditorGUILayout.LabelField("Convert action", CenteredBoldLabel);

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.HelpBox("Creates details objects from data", MessageType.Info);

                if (GUILayout.Button("Convert to localization"))
                {
                    CreateDetails();
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        ///     Convert the absolute file path to the local, Unity Project related path, which starts with "Assets/...".
        /// </summary>
        /// <param name="absolutePath">Must be a properly built absolute path, like: D:/Repository/UnityProject/Assets/...</param>
        /// <returns></returns>
        private static string AbsolutePathToProjectPath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return absolutePath;

            // The absolute path to any file must start with the Application.dataPath value, which ends with the "Assets" word.
            // So, to get project path, everything what is local is replaced by the beginning of a project path which is the "Assets" word.
            return absolutePath.Replace(Application.dataPath, "Assets");
        }

        private void CreateDetails()
        {
            if(_inputTextAsset != null)
            {
                string[] countryStringRows = _inputTextAsset.text.Split(System.Environment.NewLine);
                string localPath = AbsolutePathToProjectPath(_outputAbsolutePath);

                string availablePath = Path.Combine(localPath, "_AvailableCountryDetails.asset");
                AvailableCountryDetailsOptions availableOptions = AssetDatabase.LoadAssetAtPath<AvailableCountryDetailsOptions>(availablePath);

                if (availableOptions == null)
                {
                    availableOptions = ScriptableObject.CreateInstance<AvailableCountryDetailsOptions>();
                    AssetDatabase.CreateAsset(availableOptions, availablePath);
                }

                List<CountryDetailsOption> options = new List<CountryDetailsOption>(); ;

                foreach (string row in countryStringRows)
                {
                    string[] countryDetailComponents = row.Split('\t');

                    //Expecting Country Name, Code and phone code
                    if(countryDetailComponents.Length == 3)
                    {
                        string assetPath = Path.Combine(localPath, $"{countryDetailComponents[0]}.asset");
                        CountryDetailsOption detailsOption = AssetDatabase.LoadAssetAtPath<CountryDetailsOption>(assetPath);

                        if (detailsOption == null)
                        {
                            detailsOption = ScriptableObject.CreateInstance<CountryDetailsOption>();
                            AssetDatabase.CreateAsset(detailsOption, assetPath);
                        }

                        detailsOption.SetDetails(countryDetailComponents[1], countryDetailComponents[2]);
                        EditorUtility.SetDirty(detailsOption);
                        options.Add(detailsOption);
                    }
                }

                availableOptions.AddOptions(options.ToArray());
                EditorUtility.SetDirty(availableOptions);

                AssetDatabase.SaveAssets();
            }
        }

    }
}
