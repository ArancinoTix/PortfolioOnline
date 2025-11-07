using System;
using UnityEngine;

namespace U9.Permissions
{
    /// <summary>
    ///     Use this for stub permission requests if we don't support selected platform.
    /// </summary>
    public class StubPermissionProvider : MonoBehaviour, IPermissionsProvider
    {
        public void CheckCameraPermission(Action<bool> callback = null)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(CheckCameraPermission)}: This platform is not supported");
        }

        public void CheckLocationPermission(Action<bool> callback = null)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(CheckLocationPermission)}: This platform is not supported");
        }

        public void CheckStoragePermission(Action<bool> callback = null)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(CheckStoragePermission)}: This platform is not supported");
        }

        public void RequestCameraPermission(Action<bool> callback = null)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(RequestCameraPermission)}: This platform is not supported");
        }

        public void RequestLocationPermission(Action<bool> callback = null)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(RequestLocationPermission)}: This platform is not supported");
        }

        public void RequestStoragePermission(Action<bool> callback = null)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(RequestStoragePermission)}: This platform is not supported");
        }
        
        public void OpenSettings()
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(OpenSettings)}: This platform is not supported");
        }

        public void RequestPermission(PermissionType permissionType, Action<bool> permissionResponded)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(RequestPermission)}: This platform is not supported");
        }

        public void CheckPermission(PermissionType permissionType, Action<bool> callback)
        {
            Debug.Log($"{nameof(StubPermissionProvider)} {nameof(CheckPermission)}: This platform is not supported");
        }
    }
}
