using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U9.Permissions;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;
using System;
using System.IO;

namespace U9.LocationHelper
{
    public class LocationHelper : MonoSingleton<LocationHelper>
    {
        [HeaderAttribute("Configuration")]
        [SerializeField] private float _warmUpDuration = 1f;

        [Separator] [ReadOnly] [SerializeField] private LocationSampleData _currentLocationSample = new LocationSampleData(double.NaN);

#if UNITY_EDITOR
        [HeaderAttribute("Editor Simulation")]
        [MessageBox("The simulated values are only used in the editor. When you start the service, the last values will be set to the ones below.")]
        [SerializeField] private double _simulatedLatitude = double.NaN;
        [SerializeField] private double _simulatedLongitude = double.NaN;
        [SerializeField] private double _simulatedAltitude = double.NaN;
        [SerializeField] private double _simulatedHeading = double.NaN;
        [MessageBox("If a samples file is provided, this will be used instead. This is only used in the editor")]

        [Separator] [SerializeField] TextAsset _samplesFile;

        private LocationSampleData _fromRecordedSample = new LocationSampleData(double.NaN);
        private LocationSampleData _toRecordedSample = new LocationSampleData(double.NaN);
        private string[] _loadedSampleStrings = new string[0];
        private int _currentLoadedSampleIndex = 0;
        private int _numberOfSamples = 0;
        private float _loadedSampleDuration = 0;
        private float _loadedSampleTimer = 0;
#endif
        [HeaderAttribute("Recording")]
        [SerializeField] private bool _autoRecordOnServiceStart = false;
        [SerializeField] private float _recordSampleInterval = .5f;
        [SerializeField] private string _recordedSampleSaveDateFormat = "yyyy-MM-dd-HH-mm-ss";
        [SerializeField] private string _recordedSampleFileNameFormat = "{0}.gpsloc";

        [Separator] [SerializeField] private string _recordedSampleFolder = "Location Recordings";
       
        private bool _locationProcessing = false;

        public double CurrentLatitude { get => _currentLocationSample.latitude; }
        public double CurrentLongitude { get => _currentLocationSample.longitude; }
        public double CurrentAltitude { get => _currentLocationSample.altitude; }
        public double CurrentHeading { get => _currentLocationSample.heading; }

        private StreamWriter _recordedSamplesWriter;

        private const double Deg2Rad = (Math.PI * 2d) / 360d;
        private const int EarthsRadius = 6371;   // earths radius = 6,371 km

        private bool _isRecording = false;
        private float _recordSampleTimer = 0;
        private float _serviceUpTime = 0;

        public Action onLocationUpdate { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Is the service running?
        /// </summary>
        public bool IsRunning
        {
            get => Status == LocationServiceStatus.Running && _locationProcessing;
        }

        /// <summary>
        /// Is the surface warmed up and ready?
        /// </summary>
        public bool IsReady
        {
            get => _serviceUpTime >= _warmUpDuration;
        }

        public bool HasLocationData { get { return !double.IsNaN(CurrentLatitude) && !double.IsNaN(CurrentLongitude); } }

        /// <summary>
        /// The current status of the service
        /// </summary>
        public LocationServiceStatus Status
        {
#if UNITY_EDITOR
            get => _locationProcessing ? LocationServiceStatus.Running : LocationServiceStatus.Stopped;
#else
            get => Input.location.status;
#endif
        }

        /// <summary>
        /// Checks if we have a data sample within the given time frame
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool HasLocationDataFromWithinTheLast(float seconds)
        {
            return HasLocationData && !float.IsNaN(_currentLocationSample.timeFromServiceStart) && _serviceUpTime - _currentLocationSample.timeFromServiceStart < seconds;
        }


        [DisableInEditMode]
        [Button]
        /// <summary>
        /// Requests the location service to start.
        /// This will check the permissions provider and request if not granted.
        /// If permission fails, the permission provider will handle the error.
        /// </summary>
        /// <param name="onServiceResponded">Returns true if running</param>
        public void StartService(Action<bool> onServiceResponded = null)
        {
            if(IsRunning)
            {
                onServiceResponded?.Invoke(true);
            }
            else
            {
                PermissionsProviderController.Instance.HasLocationPermission(PermissionChecked);
            }

            void PermissionChecked(bool granted)
            {
                if (!granted)
                    PermissionsProviderController.Instance.RequestLocationPermission(PermissionRequested);
                else
                    PermissionRequested(true);
            }

            void PermissionRequested(bool granted)
            {
                if (!granted)
                    onServiceResponded?.Invoke(false);
                else
                {
                    Debug.Log("[LOCATION HELPER] Service started");
                    Input.location.Start();
                    _locationProcessing = true;
                    _serviceUpTime = 0;
                    onServiceResponded?.Invoke(true);

#if UNITY_EDITOR
                    if (_samplesFile != null)
                    {
                        _loadedSampleStrings = _samplesFile.text.Split(Environment.NewLine);
                        _currentLoadedSampleIndex = 0;
                        _numberOfSamples = _loadedSampleStrings.Length;
                        SetLoadedSamples();
                    }
#endif

                    if (_autoRecordOnServiceStart)
                        StartRecording();
                }    
            }
        }
        [DisableInEditMode]
        [Button]
        /// <summary>
        /// Stops the service
        /// </summary>
        public void StopService()
        {
            StopRecording();

#if UNITY_EDITOR
            _loadedSampleStrings = new string[0];
#endif

            Debug.Log("[LOCATION HELPER] Service stopped");
            Input.location.Stop();
            _locationProcessing = false;
            _serviceUpTime = 0;
        }

        [DisableInEditMode]

        [Separator]
        [Button]
        /// <summary>
        /// Starts recording the location samples to a file.
        /// </summary>
        /// <param name="resetIfAlreadyRecording">Begins a new recording if already recording</param>
        public void StartRecording(bool resetIfAlreadyRecording = false)
        {
            if(IsRunning)
            {
                if (_isRecording && !resetIfAlreadyRecording)
                    return;

                StopRecording();

                string saveFolder = Path.Combine(Application.persistentDataPath, _recordedSampleFolder);
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);

                string saveFilePath = Path.Combine(
                    saveFolder,
                    string.Format(_recordedSampleFileNameFormat, DateTime.Now.ToString(_recordedSampleSaveDateFormat)));

                _recordedSamplesWriter = new StreamWriter(saveFilePath);
                
                _recordSampleTimer = _recordSampleInterval;
                _isRecording = true;

                Debug.Log("[LOCATION HELPER] Recording started");
                Debug.Log("[LOCATION HELPER] Recording to: " + saveFilePath);
            }
        }

        /// <summary>
        /// Writes the current location sample if ready
        /// </summary>
        private void WriteLocationSample()
        {
            if(_isRecording)
            {
                _recordSampleTimer += Time.unscaledDeltaTime;

                if (_recordSampleTimer >= _recordSampleInterval)
                {
                    _recordSampleTimer = 0;

                    string sample = _currentLocationSample.ToString();
                    _recordedSamplesWriter.WriteLine(sample);
                }               
            }
        }

        [DisableInEditMode][Button]
        /// <summary>
        /// Stops recording a location samples
        /// </summary>
        public void StopRecording()
        {
            if(_isRecording)
            {
                _isRecording = false;

                if (_recordedSamplesWriter != null)
                {
                    _recordedSamplesWriter.Flush();
                    _recordedSamplesWriter.Close();
                    _recordedSamplesWriter = null;
                }

                Debug.Log("[LOCATION HELPER] Recording stopped");
            }
        }

#if UNITY_EDITOR
        private void SetLoadedSamples()
        {
            if (_numberOfSamples > 0)
            {
                int fromIndex = Mathf.Clamp(_currentLoadedSampleIndex, 0, _numberOfSamples - 1);
                int toIndex = Mathf.Clamp(_currentLoadedSampleIndex + 1, 0, _numberOfSamples - 1);

                _fromRecordedSample = new LocationSampleData(_loadedSampleStrings[fromIndex]);
                _toRecordedSample = new LocationSampleData(_loadedSampleStrings[toIndex]);

                if (!_toRecordedSample.IsValid())
                    _fromRecordedSample = _toRecordedSample;

                _loadedSampleTimer = 0;
                _loadedSampleDuration = _toRecordedSample.timeFromServiceStart - _fromRecordedSample.timeFromServiceStart;

                if (_loadedSampleDuration <= 0 || float.IsNaN(_loadedSampleDuration))
                    _loadedSampleDuration = 1;
            }
        }
#endif

        [Separator]
        [Button]
        private void OpenSaveFolder()
        {
            string saveFolder = Path.Combine(Application.persistentDataPath, _recordedSampleFolder);
            Application.OpenURL(saveFolder);
        }

        private void Update()
        {
            if(IsRunning)
            {
#if UNITY_EDITOR
                if (_numberOfSamples > 0)
                {
                    _loadedSampleTimer += Time.unscaledDeltaTime;
                    float lerp = _loadedSampleTimer / _loadedSampleDuration;

                    if (_fromRecordedSample.IsValid())
                    {
                        _simulatedLatitude = Mathf.Lerp((float)_fromRecordedSample.latitude, (float)_toRecordedSample.latitude, lerp);
                        _simulatedLongitude = Mathf.Lerp((float)_fromRecordedSample.longitude, (float)_toRecordedSample.longitude, lerp);
                        _simulatedAltitude = Mathf.Lerp((float)_fromRecordedSample.altitude, (float)_toRecordedSample.altitude, lerp);
                        _simulatedHeading = Mathf.Lerp((float)_fromRecordedSample.heading, (float)_toRecordedSample.heading, lerp);
                    }

                    if (_loadedSampleTimer >=_loadedSampleDuration)
                    {
                        _currentLoadedSampleIndex++;
                        SetLoadedSamples();
                    }
                }

                UpdateGPSData(_simulatedLatitude, _simulatedLongitude, _simulatedAltitude, _simulatedHeading);
#else
                var lastData = Input.location.lastData;
                UpdateGPSData(
                    lastData.latitude, 
                    lastData.longitude, 
                    lastData.altitude, 
                    Input.compass.trueHeading);
#endif

                _serviceUpTime += Time.unscaledDeltaTime;
                
                WriteLocationSample();
            }
        }

        private void UpdateGPSData(double latitude, double longitude, double altitude, double heading)
        {
            _currentLocationSample.latitude = latitude;
            _currentLocationSample.longitude = longitude;
            _currentLocationSample.altitude = altitude;
            _currentLocationSample.heading = heading;
            _currentLocationSample.timeFromServiceStart = _serviceUpTime;
            onLocationUpdate?.Invoke();
        }


        /// <summary>
        /// Calculate own distance to point. Must run location service for this to work
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public double CurrentDistanceToPoint(double latitude, double longitude)
        {
            return DistanceToPoint(CurrentLatitude, CurrentLongitude, latitude, longitude);
        }

        /// <summary>
        /// Dirty calculation for calculating distance between two coords.
        /// </summary>
        /// <param name="latitude1"></param>
        /// <param name="longitude1"></param>
        /// <param name="latitude2"></param>
        /// <param name="longitude2"></param>
        /// <returns></returns>
        public static double DistanceToPoint(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double latitudeRadians1 = Deg2Rad * latitude1;
            double latatiudeRadians2 = Deg2Rad * latitude2;
            double differenceLatitudeRadians = Deg2Rad * (latitude2 - latitude1);
            double differenceLongitudeRadians = Deg2Rad * (longitude2 - longitude1);
            double a = Math.Pow(Math.Sin(differenceLatitudeRadians * 0.5d), 2) + (Math.Pow(Math.Sin(differenceLongitudeRadians * 0.5d), 2) * Math.Cos(latitudeRadians1) * Math.Cos(latatiudeRadians2));
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthsRadius * c * 1000;    // in meters
        }
    }   

    [System.Serializable]
    public struct LocationSampleData
    {
        public double latitude;
        public double longitude;
        public double altitude ;
        public double heading;
        public float timeFromServiceStart;

        private const string k_stringFormat = "{0}_{1}_{2}_{3}_{4}";
        private const char k_seperator = '_';

        public LocationSampleData(double dateTime)
        {
            latitude = double.NaN;
            longitude = double.NaN;
            altitude = double.NaN;
            heading = double.NaN;
            timeFromServiceStart = float.NaN;
        }

        public override string ToString()
        {
            return string.Format(k_stringFormat, latitude, longitude, altitude, heading, timeFromServiceStart);
        }

        public bool IsValid()
        {
            return !double.IsNaN(latitude)
                && !double.IsNaN(longitude)
                && !double.IsNaN(altitude)
                && !double.IsNaN(heading)
                && !double.IsNaN(timeFromServiceStart);
        }

        public LocationSampleData(string data)
        {
            string[] split = data.Split(k_seperator);
            if(split.Length ==5)
            {
                latitude = double.Parse(split[0]);
                longitude = double.Parse(split[1]);
                altitude = double.Parse(split[2]);
                heading = double.Parse(split[3]);
                timeFromServiceStart = float.Parse(split[4]);
            }
            else
            {
                latitude = double.NaN;
                longitude = double.NaN;
                altitude = double.NaN;
                heading = double.NaN;
                timeFromServiceStart = float.NaN;
            }
        }
    }
}
