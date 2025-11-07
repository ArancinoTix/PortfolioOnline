using System;
using System.Collections;
using UnityEngine;

namespace U9.Permissions
{
    /// <summary>
    ///     Use this for NativeGallery (storage) permission requests.
    /// </summary>
    public abstract class NativeGalleryPermissionProvider : MonoBehaviour
    {
        private Coroutine _permissionCoroutine;
        
        protected void CheckNativeGalleryPermission(Action<bool> callback)
        {
            NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
            callback?.Invoke(permission == NativeGallery.Permission.Granted);
        }

        protected void RequestNativeGalleryPermission(Action<bool> callback = null)
        {
            if(_permissionCoroutine != null)
                StopCoroutine(_permissionCoroutine);
            _permissionCoroutine = StartCoroutine(AskForNativeGalleryPermission(callback));
        }

        private IEnumerator AskForNativeGalleryPermission(Action<bool> callback = null)
        {
            NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
            
            if (!permission.Equals(NativeGallery.Permission.Granted))
            {
                NativeGallery.RequestPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
                
                yield return new WaitForEndOfFrame();   
                permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
            }
            callback?.Invoke(permission == NativeGallery.Permission.Granted);
            _permissionCoroutine = null;
        }
    }
}
