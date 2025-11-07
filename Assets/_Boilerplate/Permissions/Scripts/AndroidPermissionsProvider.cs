using System;
using UnityEngine;
using UnityEngine.Android;

namespace U9.Permissions
{
    /// <summary>
    ///     Use this for Android permission requests.
    /// </summary>
    public class AndroidPermissionsProvider : MobilePermissionProvider
    {
#if UNITY_ANDROID

        public override void CheckCameraPermission(Action<bool> callback)
        {
            bool isGranted = CheckAndroidPermission(Permission.Camera);
            callback?.Invoke(isGranted);
        }

       public override void CheckLocationPermission(Action<bool> callback)
        {
            bool isGranted = CheckAndroidPermission(Permission.FineLocation);
            callback?.Invoke(isGranted);
        }

        public override void CheckStoragePermission(Action<bool> callback)
        {
            CheckNativeGalleryPermission(callback);
        }

        public override void RequestLocationPermission(Action<bool> callback = null)
        {
            RequestAndroidPermission(Permission.FineLocation, callback);
        }

        public override void RequestCameraPermission(Action<bool> callback = null)
        {
            RequestAndroidPermission(Permission.Camera, callback);
        }

        public override void RequestStoragePermission(Action<bool> callback = null)
        {
            RequestNativeGalleryPermission(callback);
        }

        public override void OpenSettings()
        {
            AndroidRuntimePermissions.OpenSettings();
        }

        protected static bool CheckAndroidPermission(string permissionString)
        {
            return AndroidRuntimePermissions.CheckPermission(permissionString);
        }

        protected static void RequestAndroidPermission(string permissionString, Action<bool> callback=null)
        {
            PermissionCallbacks callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += (string result) => {
                callback?.Invoke(false);
            };
            callbacks.PermissionGranted += (string result) => {
                callback?.Invoke(true);
            };
            Permission.RequestUserPermission(permissionString, callbacks);
        }

#else

        public override void CheckCameraPermission(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public override void CheckLocationPermission(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public override void CheckStoragePermission(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public override void RequestStoragePermission(Action<bool> callback = null)
        {
            throw new NotImplementedException();
        }
        
        public override void RequestCameraPermission(Action<bool> callback = null)
        {
            throw new NotImplementedException();
        }

        public override void RequestLocationPermission(Action<bool> callback = null)
        {
            throw new NotImplementedException();
        }

        public override void OpenSettings() { }

#endif
    }

}