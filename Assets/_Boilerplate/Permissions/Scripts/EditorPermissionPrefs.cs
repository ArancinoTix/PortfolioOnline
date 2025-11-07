using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Permissions
{
#if UNITY_EDITOR
    /// <summary>
    ///     This class is used to simulate permissions on the editor.
    /// </summary>
    public class EditorPermissionPrefs
    {
        public static void SetPermission(PermissionType permissionType, PermissionValue permissionValue)
        {
            PlayerPrefs.SetInt($"{permissionType}", (int)permissionValue);
            PlayerPrefs.Save();
        }

        public PermissionValue GetPermission(PermissionType permissionType)
        {
            return (PermissionValue)PlayerPrefs.GetInt($"{permissionType}", (int)PermissionValue.Ask);
        }

        public static PermissionValue GetPermissionStatic(PermissionType permissionType)
        {
            return (PermissionValue) PlayerPrefs.GetInt($"{permissionType}", (int) PermissionValue.Ask);
        }

        /// <summary>
        ///     Use it to reset all permissions to Ask permission for testing purposes.
        /// </summary>
        public static void ResetAllPermissions()
        {
            SetPermission(PermissionType.Camera, PermissionValue.Ask);
            SetPermission(PermissionType.Location, PermissionValue.Ask);
            SetPermission(PermissionType.Storage, PermissionValue.Ask);
            SetPermission(PermissionType.Scene, PermissionValue.Ask);
        }
        
        /// <summary>
        ///     Use it to reset AllowOnce permissions to Ask permission on start for all permissions types.
        /// </summary>
        public void ResetAllowOncePermissions()
        {
            ResetAllowOncePermission(PermissionType.Camera);
            ResetAllowOncePermission(PermissionType.Location);
            ResetAllowOncePermission(PermissionType.Storage);
        }
        
        /// <summary>
        ///     Use it to reset AllowOnce permission to Ask permission on start.
        /// </summary>
        /// <param name="permissionType"></param>
        private void ResetAllowOncePermission(PermissionType permissionType)
        {
            if (GetPermission(permissionType) == PermissionValue.AllowOnce)
            {
                SetPermission(permissionType, PermissionValue.Ask);
            }
        }
    }
        
    public enum PermissionValue
    {
        Ask,
        AlwaysAllow,
        AllowOnce,
        Deny
    }
#endif
}
