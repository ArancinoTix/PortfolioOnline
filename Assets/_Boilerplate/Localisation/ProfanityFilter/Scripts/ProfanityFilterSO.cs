using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Localization
{

    [CreateAssetMenu(menuName = "Localisation/ProfanityFilter")]
    public class ProfanityFilterSO : ScriptableObject
    {
        [SerializeField] private string m_pattern = string.Empty;
        [SerializeField] private string m_whiteList;

        public string Pattern { get => m_pattern; }

        public bool ValidateComplex(string input, bool whiteList)
        {
            var strings = Regex.Split(input, @"\s+");

            foreach (var s in strings)
            {
                if (string.IsNullOrEmpty(m_whiteList))
                {
                    if (Regex.IsMatch(s, m_pattern, RegexOptions.IgnoreCase))
                        return false;
                }

                if (!Regex.Match(s, m_whiteList, RegexOptions.IgnoreCase).Success)
                {
                    if (Regex.IsMatch(s, m_pattern, RegexOptions.IgnoreCase))
                        return false;
                }
            }

            return true;
        }

        public string Replace(string input, bool useWhiteList = false, char replaceChar = '*')
        {
            string output = "";

            var strings = Regex.Split(input, @"\s+");
            foreach (var s in strings)
            {
                if (!Regex.Match(s, m_whiteList, RegexOptions.IgnoreCase).Success)
                {
                    output += Regex.Replace(s, m_pattern,
                    (m) =>
                    {
                        return new String('*', m.Length);
                    },
                    RegexOptions.IgnoreCase) + " ";
                }
                else
                {
                    output += s + " ";
                }
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ProfanityFilterSO))]
    public class ProfanityFilterSOEditor : Editor
    {
        private static StringBuilder stringBuilder = new StringBuilder();
        private string badWordsUsed;

        SerializedProperty m_pattern;
        SerializedProperty m_whiteList;


        string m_testPhrase = "";
        string m_validatePhrase = "";

        void OnEnable()
        {
            m_pattern = serializedObject.FindProperty("m_pattern");

            m_whiteList = serializedObject.FindProperty("m_whiteList");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle uIStyle = new GUIStyle(EditorStyles.helpBox);
            serializedObject.Update();

            //EditorGUILayout.PropertyField(m_localeName);

            EditorGUILayout.LabelField("Regex pattern:");
            m_pattern.stringValue = EditorGUILayout.TextArea(m_pattern.stringValue, uIStyle);

            if (!string.IsNullOrEmpty(badWordsUsed))
            {
                EditorGUILayout.LabelField("Bad words used:");
                EditorGUILayout.TextArea(badWordsUsed, uIStyle);
            }

            if (GUILayout.Button("Generate"))
            {
                m_pattern.stringValue = Generate();
            }

            EditorGUILayout.LabelField("Whitelist:");
            m_whiteList.stringValue = EditorGUILayout.TextArea(m_whiteList.stringValue, uIStyle);

            EditorGUILayout.LabelField("Test Phrase");
            m_testPhrase = EditorGUILayout.TextArea(m_testPhrase, uIStyle);

            if (GUILayout.Button("Test"))
            {
                m_testPhrase = ((ProfanityFilterSO)serializedObject.targetObject).Replace(m_testPhrase, true);
                Debug.Log(m_testPhrase);
            }

            EditorGUILayout.LabelField("Validate Phrase");
            m_validatePhrase = EditorGUILayout.TextArea(m_validatePhrase, uIStyle);
            if (GUILayout.Button("Validate"))
            {
                Debug.Log(((ProfanityFilterSO)serializedObject.targetObject).ValidateComplex(m_validatePhrase, true));
            }



            serializedObject.ApplyModifiedProperties();
        }

        private string Generate()
        {
            HashSet<string> keys = new HashSet<string>();

            string filePath = EditorUtility.OpenFilePanel("File location of bad words list", Application.dataPath, "");
            // return if path is empty
            if (string.IsNullOrEmpty(filePath)) { Debug.Log("No path was providded"); return ""; }

            string[] seperators = { "\n" };
            string[] words;

            stringBuilder.Clear();

            string forbiddenWords = File.ReadAllText(filePath);

            stringBuilder.Append(@"(?i)");

            words = forbiddenWords.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                if (keys.Contains(words[i])) continue;
                keys.Add(words[i]);

                if (i < words.Length - 1)
                {
                    stringBuilder.Append(words[i] + "|");
                }
                else
                {
                    stringBuilder.Append(words[i]);
                }

                badWordsUsed += words[i] + "\n";
            }

            stringBuilder.Append(@"(?-i)");
            return Regex.Replace(stringBuilder.ToString(), @"\t|\n|\r", "");
        }

    }
#endif
}