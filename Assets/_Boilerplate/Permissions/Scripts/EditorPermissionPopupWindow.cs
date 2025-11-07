using System;
using UnityEditor;
using UnityEngine;

namespace U9.Permissions
{
#if UNITY_EDITOR
    public class EditorPermissionPopupWindow : EditorWindow
    {
        /// <summary>
        ///     Permission type that will be requested
        /// </summary>
        private static PermissionType _permissionType;
        
        /// <summary>
        ///     Callback that will be called after user select permission option
        /// </summary>
        private static Action<bool> _callback;
        
        private static EditorPermissionPrefs _editorPermissionPrefs;

        public static void ShowWindow(PermissionType permissionType, EditorPermissionPrefs editorPermissionPrefs,
            Action<bool> callback = null)
        {
            EditorApplication.isPaused = true;
            _permissionType = permissionType;
            _editorPermissionPrefs = editorPermissionPrefs;
            _callback = callback;
            EditorPermissionPopupWindow window = GetWindow<EditorPermissionPopupWindow>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 300);
            window.titleContent = new GUIContent($"Permission {permissionType.ToString()}");
        }

        private void OnGUI()
        {
            GUILayout.Label($"Permission {_permissionType.ToString()}");
            GUILayout.Space(10);
            if (GUILayout.Button("Always allow"))
            {
                EditorPermissionPrefs.SetPermission(_permissionType, PermissionValue.AlwaysAllow);
                this.Close();
                _callback?.Invoke(true);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Allow once and ask again"))
            {
                EditorPermissionPrefs.SetPermission(_permissionType, PermissionValue.AllowOnce);
                this.Close();
                _callback?.Invoke(true);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Deny and never ask again"))
            {
                EditorPermissionPrefs.SetPermission(_permissionType, PermissionValue.Deny);
                this.Close();
                _callback?.Invoke(false);
            }
        }
        
        private void OnDestroy()
        {
            EditorApplication.isPaused = false;
        }
    }
#endif
}
