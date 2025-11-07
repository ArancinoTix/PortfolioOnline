using System;
using UnityEngine;

namespace U9.Permissions
{
    /// <summary>
    ///     Use this for extends Android permission requests relative to Meta Quest devices.
    /// </summary>
    public class MetaQuestPermissionsProvider : AndroidPermissionsProvider, IMetaPermissionsProvider
    {
#if UNITY_ANDROID

        private const string SCENE_PERMISSION = "com.oculus.permission.USE_SCENE";

        public void RequestScenePermission(Action<bool> callback = null)
        {
            RequestAndroidPermission(SCENE_PERMISSION, callback);
        }

        public void CheckScenePermission(Action<bool> callback = null)
        {
            bool isGranted = CheckAndroidPermission(SCENE_PERMISSION);
            callback?.Invoke(isGranted);
        }

        public override void CheckPermission(PermissionType permissionType, Action<bool> callback = null)
        {
            if (permissionType == PermissionType.Scene)
                CheckScenePermission(callback);
            else
                base.CheckPermission(permissionType, callback);
        }

        public override void RequestPermission(PermissionType permissionType, Action<bool> callback = null)
        {
            if (permissionType == PermissionType.Scene)
                RequestScenePermission(callback);
            else
                base.RequestPermission(permissionType, callback);
        }

        public override void OpenSettings()
        {
            if (Application.platform == RuntimePlatform.Android)
            {                
                try
                {
                    const string systemUxRoute = "systemux://settings";
                    var intent = new AndroidJavaObject("android.content.Intent");
                    intent.Call<AndroidJavaObject>("setPackage", "com.oculus.vrshell");
                    intent.Call<AndroidJavaObject>("setAction", "com.oculus.vrshell.intent.action.LAUNCH");
                    intent.Call<AndroidJavaObject>("putExtra", "intent_data", systemUxRoute);


                    using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    using var currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    var packageName = currentActivityObject.Call<string>("getPackageName");

                    var deeplinkUri = $"/applications?package={packageName}";

                    intent.Call<AndroidJavaObject>("putExtra", "uri", deeplinkUri);

                    currentActivityObject.Call("sendBroadcast", intent);
                    currentActivityObject.Call("overridePendingTransition", 0, 0);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"[PermissionChecker] Deep linking error: {exception.Message}");
                }
            }
            else
            {
                // Can't launch app in Unity editor or Oculus link
                Debug.LogWarning($"[PermissionChecker] Cannot launch native permission out of Android");
            }
        }

#else

        public void CheckScenePermission(Action<bool> callback = null)
        {
            throw new NotImplementedException();
        }

        public void RequestScenePermission(Action<bool> callback = null)
        {
            throw new NotImplementedException();
        }

#endif
    }
}