using System;

namespace U9.Permissions
{
    /// <summary>
    /// Interface to extend permissions for Meta Quest that are not needed in other mobile platforms
    /// </summary>
    public interface IMetaPermissionsProvider
    {
        /// <summary>
        ///     Check if scene permission is already granted.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void CheckScenePermission(Action<bool> callback = null);

        /// <summary>
        ///     Request scene permission.
        /// </summary>
        /// <param name="callback">Invoke callback with true if permission is granted.</param>
        void RequestScenePermission(Action<bool> callback = null);
    }
}
