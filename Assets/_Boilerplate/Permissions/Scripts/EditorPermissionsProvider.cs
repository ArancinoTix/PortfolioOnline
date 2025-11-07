using System;
using UnityEngine;

namespace U9.Permissions
{
#if UNITY_EDITOR
    /// <summary>
    ///     Use this class for testing permissions in editor.
    /// </summary>
    public class EditorPermissionsProvider : MonoBehaviour, IPermissionsProvider
    {
        public void CheckCameraPermission(Action<bool> callback)
        {
            var isGranted = CheckPermission(PermissionType.Camera);
            callback?.Invoke(isGranted);
        }

        public void CheckLocationPermission(Action<bool> callback)
        {
            var isGranted = CheckPermission(PermissionType.Location);
            callback?.Invoke(isGranted);
        }

        public void CheckStoragePermission(Action<bool> callback)
        {
            var isGranted = CheckPermission(PermissionType.Storage);
            callback?.Invoke(isGranted);
        }
        
        private EditorPermissionPrefs _editorPermissionPrefs;
        
        private void Start()
        {
            _editorPermissionPrefs = new EditorPermissionPrefs();
            _editorPermissionPrefs.ResetAllowOncePermissions();
        }

        public void RequestStoragePermission(Action<bool> callback = null)
        {
            RequestPermission(PermissionType.Storage, callback);
        }
        public void RequestCameraPermission(Action<bool> callback = null)
        {
            RequestPermission(PermissionType.Camera, callback);
        }

        public void RequestLocationPermission(Action<bool> callback = null)
        {
            RequestPermission(PermissionType.Location, callback);
        }
        
        public void OpenSettings()
        {
            EditorPermissionSettingsPopupWindow.ShowWindow(_editorPermissionPrefs);
        }
        
        private bool CheckPermission(PermissionType permissionType)
        {
            return _editorPermissionPrefs.GetPermission(permissionType) == PermissionValue.AlwaysAllow ||
                   _editorPermissionPrefs.GetPermission(permissionType) == PermissionValue.AllowOnce;
        }

        private void RequestPermission(PermissionType permissionType, Action<bool> callback = null)
        {
            if (_editorPermissionPrefs.GetPermission(permissionType) == PermissionValue.Deny)
            {
                callback?.Invoke(false);
                return;
            }
            if (_editorPermissionPrefs.GetPermission(permissionType) == PermissionValue.AlwaysAllow ||
                _editorPermissionPrefs.GetPermission(permissionType) == PermissionValue.AllowOnce)
            {
                callback?.Invoke(true);
                return;
            }

            EditorPermissionPopupWindow.ShowWindow(permissionType, _editorPermissionPrefs, callback);
        }

        void IPermissionsProvider.RequestPermission(PermissionType permissionType, Action<bool> callback)
        {
            switch (permissionType)
            {
                case PermissionType.Camera:
                    RequestCameraPermission(callback);
                    break;
                case PermissionType.Location:
                    RequestLocationPermission(callback);
                    break;
                case PermissionType.Storage:
                    RequestStoragePermission(callback);
                    break;
            }
        }

        public void CheckPermission(PermissionType permissionType, Action<bool> callback)
        {
            switch (permissionType)
            {
                case PermissionType.Camera:
                    CheckCameraPermission(callback);
                    break;
                case PermissionType.Location:
                    CheckLocationPermission(callback);
                    break;
                case PermissionType.Storage:
                    CheckStoragePermission(callback);
                    break;
            }
        }
    }
#endif
}