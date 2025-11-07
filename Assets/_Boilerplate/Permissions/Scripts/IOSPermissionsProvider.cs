using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Permissions
{
    /// <summary>
    ///     Use this for IOS permission requests.
    /// </summary>
    public class IOSPermissionsProvider : MobilePermissionProvider
    {
#if UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern int _IOSPermissions_CheckCameraPermission();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern int _IOSPermissions_RequestCameraPermission();
        
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern int _IOSPermissions_CanOpenSettings();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern int _IOSPermissions_HasCamera();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void _IOSPermissions_OpenSettings();
        
        private Coroutine _permissionLocationCheckRoutine = null;

        //----------------------------------------------------------------------------------------

        public override void CheckCameraPermission(Action<bool> callback)
        {
            var isGranted = _IOSPermissions_CheckCameraPermission() == 1;
            callback?.Invoke(isGranted);
        }

        public override void CheckLocationPermission(Action<bool> callback)
        {
            StartCheckingLocationPermission(callback);
        }

        public override void CheckStoragePermission(Action<bool> callback)
        {
            CheckNativeGalleryPermission(callback);
        }
        
        public override void RequestStoragePermission(Action<bool> callback)
        {
            RequestNativeGalleryPermission(callback);
        }

        public override void RequestCameraPermission(Action<bool> callback)
        {
            var isGranted = _IOSPermissions_RequestCameraPermission();
            callback?.Invoke(isGranted == 1);
        }

        public override void RequestLocationPermission(Action<bool> callback = null)
        {
            StartCheckingLocationPermission(callback);
        }

        public override void OpenSettings() 
        { 
            _IOSPermissions_OpenSettings();
        }

        private void StartCheckingLocationPermission(Action<bool> callback = null)
        {
            if (_permissionLocationCheckRoutine != null)
                StopCoroutine(_permissionLocationCheckRoutine);
            _permissionLocationCheckRoutine = StartCoroutine(CheckingLocationPermission(callback));
        }
        
        private IEnumerator CheckingLocationPermission(Action<bool> callback = null)
        {
            Input.location.Start();
            yield return null;

            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                yield return new WaitForSeconds(1);
            }

            if (Input.location.status == LocationServiceStatus.Running)
            {
                Input.location.Stop();
                callback?.Invoke(true);
            }
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                Input.location.Stop();
                callback?.Invoke(false);
            }
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