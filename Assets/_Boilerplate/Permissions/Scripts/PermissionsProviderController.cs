using System;
using UnityEngine;
using System.Collections;
using U9.Errors;
using U9.Errors.Codes;
using VisualInspector;
using AYellowpaper.SerializedCollections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace U9.Permissions
{

    public enum PermissionType
    {
        Camera,
        Location,
        Storage,
        Scene
    }

    /// <summary>
    ///     Use this to control permissions requests on any platform.
    /// </summary>
    public class PermissionsProviderController : MonoSingleton<PermissionsProviderController>
    {

        [SerializeField] private SerializedDictionary<PermissionType, ResponseCodeOption> _errorsDetails = new();
        [SerializeField] private float _delayBetweenRequests = .5f;

        public Action<bool> onPause;

        /// <summary>
        ///     This is the actual permissions provider.
        /// </summary>
        private IPermissionsProvider _permissionsProvider;

        private Coroutine _permissionRequestRoutine;
        private float _lastPauseTime = 0;

        private const float MIN_PAUSE_DURATION_REQUIRED = 1.5f;

        private void Awake()
        {
            Instance = this;
            InitPermissionsProvider();
#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
#endif
        }

        /// <summary>
        ///     Use this to initialize permissions provider for current platform.
        /// </summary>
        private void InitPermissionsProvider()
        {
            var permissionsProviderGameObject = new GameObject("Permissions Provider");
            permissionsProviderGameObject.transform.SetParent(transform.parent);
#if UNITY_EDITOR
            _permissionsProvider =
                permissionsProviderGameObject.AddComponent<EditorPermissionsProvider>();
#elif UNITY_IOS
            _permissionsProvider = 
                permissionsProviderGameObject.AddComponent<IOSPermissionsProvider>();
#elif UNITY_ANDROID && META_QUEST_TARGET
            _permissionsProvider =
                permissionsProviderGameObject.AddComponent<MetaQuestPermissionsProvider>();
            
#elif UNITY_ANDROID
            _permissionsProvider =
                    permissionsProviderGameObject.AddComponent<AndroidPermissionsProvider>();
#else
            _permissionsProvider = 
                permissionsProviderGameObject.AddComponent<StubPermissionProvider>();
#endif
        }

#if UNITY_EDITOR
        [Button]
        private void ClearPermissions()
        {
            EditorPermissionPrefs.ResetAllPermissions();
        }
#endif

        public void RequestPermissions(PermissionType[] permissions, Action<bool[]> callback = null)
        {
            if (_permissionRequestRoutine != null)
                StopCoroutine(_permissionRequestRoutine);

            _permissionRequestRoutine = StartCoroutine(RequestPermissionsRoutine(permissions, callback));
        }

        private IEnumerator RequestPermissionsRoutine(PermissionType[] permissions, Action<bool[]> callback = null)
        {
            bool[] results = new bool[permissions.Length];

            //We will request each permission
            for(int i =0, ni = permissions.Length; i<ni; i++)
            {
                var permissionType = permissions[i];
                var requestCompleted = false;

                //Check if we already have the permission
                HasPermission(permissionType,
                   (bool granted) => {
                        //Did we grant it?
                        results[i] = granted;
                       requestCompleted = true;
                   });

                while (!requestCompleted)
                {
                    yield return null;
                }

                //If not, request it
                if (results[i] == false)
                {
                    requestCompleted = false;

                    RequestPermission(permissionType,
                        (bool granted) =>
                        {
                        //Did we grant it?
                        results[i] = granted;
                            requestCompleted = true;
                        });

                    while (!requestCompleted)
                    {
                        yield return null;
                    }
                }

                yield return new WaitForSeconds(_delayBetweenRequests);
            }

            //If we reached the end, respond with the results
            callback?.Invoke(results);
        }

        public void RequestPermission(PermissionType permissionType, Action<bool> callback = null)
        {
            Debug.Log($"Requesting {permissionType} permission");

            var responseCodeOption = _errorsDetails[permissionType];

            //Request the permission
            _permissionsProvider.RequestPermission(permissionType, PermissionResponded);

            //Called if we unpause after opening settings
            void onPauseChanged(bool isPaused)
            {
                if (!isPaused)
                {
                    onPause -= onPauseChanged;
                    //Check if we granted it
                    HasLocationPermission(PermissionResponded);
                }
            }

            //Called if we ignored the error
            void ErrorIgnored()
            {
                callback?.Invoke(false);
            }

            //Called if we click open settings
            void ErrorChoiceMade(int index)
            {
                onPause += onPauseChanged;
                OpenSettings();
            }

            //Responce from native calls
            void PermissionResponded(bool granted)
            {
                if (granted)
                {
                    callback?.Invoke(granted);
                }
                else
                {
                    //If we failed, display an error if we can
                    if (responseCodeOption != null && ErrorManager.Instance != null)
                    {
                        ErrorManager.Instance.ShowError(responseCodeOption, ErrorIgnored, null, ErrorChoiceMade);
                    }
                    else
                    {
                        //If no error, just inform of the failure
                        callback?.Invoke(granted);
                    }
                }
            }
        }

        public void RequestLocationPermission(Action<bool> callback = null)
        {
            RequestPermission(PermissionType.Location, callback);
        }
        
        public void RequestCameraPermission(Action<bool> callback = null)
        {
            RequestPermission(PermissionType.Camera, callback);
        }
        
        public void RequestStoragePermission(Action<bool> callback = null)
        {
            RequestPermission(PermissionType.Storage, callback);
        }

        public void RequestScenePermission(Action<bool> callback = null)
        {
            RequestPermission(PermissionType.Scene, callback);
        }

        public void OpenSettings()
        {
            _lastPauseTime = Time.time;
            _permissionsProvider.OpenSettings();
        }

        public void HasPermission(PermissionType permissionType, Action<bool> callback = null)
        {
            _permissionsProvider.CheckPermission(permissionType, callback);
        }

        public void HasLocationPermission(Action<bool> callback = null)
        {
            _permissionsProvider.CheckLocationPermission(callback);
        }
        
        public void HasCameraPermission(Action<bool> callback = null)
        {
            _permissionsProvider.CheckCameraPermission(callback);
        }
        
        public void HasStoragePermission(Action<bool> callback = null)
        {
            _permissionsProvider.CheckStoragePermission(callback);
        }

        public void HasScenePermission(Action<bool> callback = null)
        {
            _permissionsProvider.CheckPermission(PermissionType.Scene, callback);
        }

        private void PauseStateChanged(bool isPaused)
        {
#if UNITY_ANDROID && META_QUEST_TARGET
                //There is a bug where Application Focus is triggered immediately
                if (Time.time - _lastPauseTime >= MIN_PAUSE_DURATION_REQUIRED)
                    onPause?.Invoke(isPaused);
                else if (!isPaused)
                    Debug.Log("Unpaused too early. Not intentional. Native UI is likely still visible");
            
#else            
                onPause?.Invoke(isPaused);
#endif
            }

#if UNITY_EDITOR
        private void OnPauseStateChanged(PauseState state)
        {
            PauseStateChanged(state == PauseState.Paused);
        }

#elif UNITY_ANDROID && META_QUEST_TARGET
        //Pause is not triggered on Quest. Focus is used instead
        private void OnApplicationFocus  (bool pauseStatus)
        {
                PauseStateChanged(!pauseStatus);
        }
#else    
        private void OnApplicationPause(bool pauseStatus)
        {
            PauseStateChanged(pauseStatus);
        }
#endif
    }
}
