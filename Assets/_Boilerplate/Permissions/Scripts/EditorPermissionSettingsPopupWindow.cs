using UnityEditor;
using UnityEngine;

namespace U9.Permissions
{
#if UNITY_EDITOR
    public class EditorPermissionSettingsPopupWindow : EditorWindow
    {
        private static EditorPermissionPrefs _editorPermissionPrefs;
        
        public static void ShowWindow(EditorPermissionPrefs editorPermissionPrefs)
        {
            EditorApplication.isPaused = true;
            _editorPermissionPrefs = editorPermissionPrefs;
            EditorPermissionSettingsPopupWindow window = GetWindow<EditorPermissionSettingsPopupWindow>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 300);
            window.titleContent = new GUIContent("Permission Settings");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Permission Settings");
            GUILayout.Space(10);
            if (GUILayout.Button("Clear All Permissions"))
            {
                EditorPermissionPrefs.ResetAllPermissions();
                this.Close();
            }  
            GUILayout.Space(10);

            DrawSetting("Camera", PermissionType.Camera);

            GUILayout.Space(10);

            DrawSetting("Location", PermissionType.Location);

            GUILayout.Space(10);

            DrawSetting("Storage", PermissionType.Storage);

            GUILayout.Space(10);
            if (GUILayout.Button("Go Back"))
            {
                this.Close();
            }
        }

        private void DrawSetting(string permissionName, PermissionType permissionType)
        {
            GUILayout.Label(permissionName);
            GUILayout.BeginHorizontal();

            var currentValue = EditorPermissionPrefs.GetPermissionStatic(permissionType);

            GUI.enabled = currentValue != PermissionValue.AlwaysAllow || currentValue == PermissionValue.AllowOnce;
            if (GUILayout.Button("Allow"))
            {
                EditorPermissionPrefs.SetPermission(permissionType, PermissionValue.AlwaysAllow);
                this.Close();
            }

            GUILayout.Space(10);

            GUI.enabled = currentValue != PermissionValue.Deny;
            if (GUILayout.Button("Deny"))
            {
                EditorPermissionPrefs.SetPermission(permissionType, PermissionValue.Deny);
                this.Close();
            }

            GUILayout.Space(10);

            GUI.enabled = true;
            if (GUILayout.Button("Reset"))
            {
                EditorPermissionPrefs.SetPermission(permissionType, PermissionValue.Ask);
                this.Close();
            }

            GUILayout.EndHorizontal();
        }
        
        private void OnDestroy()
        {
            EditorApplication.isPaused = false;
        }
    }
#endif
}