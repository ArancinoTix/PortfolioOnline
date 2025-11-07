using System;
using UnityEngine;
using UnityEngine.Android;

namespace U9.Permissions
{
    /// <summary>
    ///     Base class for Mobile permission requests.
    /// </summary>
    public abstract class MobilePermissionProvider : NativeGalleryPermissionProvider, IPermissionsProvider
    {
        public abstract void CheckCameraPermission(Action<bool> callback);

        public abstract void CheckLocationPermission(Action<bool> callback);

        public abstract void CheckStoragePermission(Action<bool> callback);

        public abstract void RequestLocationPermission(Action<bool> callback = null);

        public abstract void RequestCameraPermission(Action<bool> callback = null);

        public abstract void RequestStoragePermission(Action<bool> callback = null);

        public abstract void OpenSettings();

        public virtual void RequestPermission(PermissionType permissionType, Action<bool> callback = null)
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

        public virtual void CheckPermission(PermissionType permissionType, Action<bool> callback = null)
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
}