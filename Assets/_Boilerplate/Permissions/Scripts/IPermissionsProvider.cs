using System;

namespace U9.Permissions
{
    public interface IPermissionsProvider
    {
        /// <summary>
        ///     Check if camera permission is already granted.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void CheckCameraPermission(Action<bool> callback = null);
        
        /// <summary>
        ///     Check if location permission is already granted.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void CheckLocationPermission(Action<bool> callback = null);

        /// <summary>
        ///     Check if storage permission is already granted.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void CheckStoragePermission(Action<bool> callback = null);

        /// <summary>
        ///     Request camera permission.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void RequestCameraPermission(Action<bool> callback = null);

        /// <summary>
        ///     Request location permission.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void RequestLocationPermission(Action<bool> callback = null);

        /// <summary>
        ///     Request storage permission.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void RequestStoragePermission(Action<bool> callback = null);

        /// <summary>
        ///     Open settings for this application on device, so user can change permissions manually.
        /// </summary>
        void OpenSettings();

        /// <summary>
        ///     Requests any kind of permission type
        /// </summary>
        /// <param name="permissionType">Permission type</param>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void RequestPermission(PermissionType permissionType, Action<bool> callback);

        /// <summary>
        ///     Checks any kind of permission type
        /// </summary>
        /// <param name="permissionType">Permission type</param>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void CheckPermission(PermissionType permissionType, Action<bool> callback);
    }
}
