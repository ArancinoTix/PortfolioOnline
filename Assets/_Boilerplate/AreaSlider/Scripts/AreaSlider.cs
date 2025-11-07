using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace U9.AreaSlider
{
    public class AreaSlider : MonoBehaviour
    {
        [SerializeField] private Transform _canvasScaler;
        [SerializeField] private RectTransform _palette;
        [SerializeField] private Transform _thumb;
        [SerializeField] private bool _useCorners = false;

        [Header("Debug")]
        [SerializeField] private bool _showDebugs = false;
        [SerializeField] private Transform _debugCenterPrefab;
        [SerializeField] private Transform _debugClosestPrefab;

        [Header("Values")]
        [SerializeField] private float[] _values;

        private RectTransform _thumbRect;
        private Transform _debugTransform;

        private Vector2[] _centralPoints;
        private Vector2[] _nearestPoints;
        private List<Transform> _debugs = new List<Transform>();

        private Vector2 _spectrumXY;
        private Bounds _imageBounds; // the bounds of the palette
        private Vector3 _boundsMax; // max bounds
        private Vector3 _boundsMin; // min bounds

        private PolygonCollider2D _polyCollider;
        private bool _withinPoly = false;

        public Action<float[]> OnValuesChanged;
        public float[] Values { 
            get => _values;            
        }

        public bool ShowDebugs { 
            get => _showDebugs;
            set
            {
                _showDebugs = value;
                _debugTransform.gameObject.SetActive(_showDebugs);
            }
        }

        private void Start()
        {
            _thumbRect = _thumb.GetComponent<RectTransform>();
            InitBounds();
            InitInterpolationPoints();
            CreateDebugs(_centralPoints);

            _thumbRect.anchoredPosition = Origin();

            OnPress();
        }

        private void InitBounds()
        {
            _spectrumXY = new Vector2(_palette.rect.width * _canvasScaler.localScale.x,
                _palette.rect.height * _canvasScaler.localScale.y);
            _polyCollider = _palette.GetComponent<PolygonCollider2D>();
            _imageBounds = _polyCollider.bounds;
            _boundsMax = _imageBounds.max;
            _boundsMin = _imageBounds.min;
        }

        private void InitInterpolationPoints()
        {
            _values = new float[_polyCollider.GetTotalPointCount()];
            _centralPoints = new Vector2[_polyCollider.GetTotalPointCount()];
            _nearestPoints = new Vector2[_polyCollider.GetTotalPointCount()];

            for (int i = 0; i < _centralPoints.Length; i++)
            {
                if (i < _centralPoints.Length - 1)
                    _centralPoints[i] = (_polyCollider.points[i] + _polyCollider.points[i + 1]) / 2;
                else
                    _centralPoints[i] = (_polyCollider.points[i] + _polyCollider.points[0]) / 2;
            }      
        }

        public float GetWeight(int index)
        {
            return _values[index];
        }

        public void OnPress()
        {
            UpdateThumbPosition();
            UpdateNeareastPoints();
            UpdateWeights(true);
        }

        public void OnDrag()
        {
            UpdateThumbPosition();
            UpdateNeareastPoints();
            UpdateWeights(true);
        }

        private void UpdateThumbPosition()
        {
            _withinPoly = _polyCollider.OverlapPoint(Input.mousePosition);

            float x = Mathf.Clamp(Input.mousePosition.x, _boundsMin.x, _boundsMax.x + 1);
            float y = Mathf.Clamp(Input.mousePosition.y, _boundsMin.y, _boundsMax.y);
            Vector3 newPos = new Vector3(x, y, transform.position.z);
            if (_withinPoly)//thumb.position != newPos)// && withinPoly)
            {
                _thumb.position = newPos;

            }
        }

        private void UpdateNeareastPoints()
        {
            for (int i = 0; i < _nearestPoints.Length; i++) //
            {
                if (i < _nearestPoints.Length - 1)
                    _nearestPoints[i] = NearestPointOnLine(_polyCollider.points[i], _polyCollider.points[i + 1], _thumbRect.anchoredPosition);
                else
                    _nearestPoints[i] = NearestPointOnLine(_polyCollider.points[i], _polyCollider.points[0], _thumbRect.anchoredPosition);
            }

            for (int i = 0; i < _debugs.Count; i++)
            {
                _debugs[i].localPosition = _nearestPoints[i];
            }
        }

        private void UpdateWeights(bool triggerEvent)
        {
            var didChange = false;

            for (int i = 0; i < _values.Length; i++)//
            {
                float length = Vector2.Distance(Origin(), _useCorners ? _polyCollider.points[i] : _centralPoints[i]); //nearestPoints[i]);
                float tPos = Vector2.Distance(_thumbRect.anchoredPosition, _nearestPoints[i]);

                var newValue = Mathf.Clamp01((length - tPos) / length);
                if (!didChange)
                    didChange = _values[i] != newValue;

                _values[i] = newValue;
            }

            if(didChange && triggerEvent)
                OnValuesChanged?.Invoke(_values);
        }

       

        private Vector2 NearestPointOnLine(Vector2 pointA, Vector2 pointB, Vector2 thumb)
        {
            //Get heading
            Vector2 heading = (pointB - pointA);
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector2 lhs = thumb - pointA;
            float dotP = Vector2.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0, magnitudeMax);
            return pointA + heading * dotP;
        }

        private Vector2 Origin()
        {
            float x = 0;
            float y = 0;
            foreach (Vector2 p in _centralPoints)
            {
                x += p.x;
                y += p.y;
            }

            float cx = x / _centralPoints.Length;
            float cy = y / _centralPoints.Length;

            return new Vector2(cx, cy);
        }

        private Vector2 PixelPosition()
        {
            Vector2 spectrumScreenPosition = _palette.transform.position;
            Vector2 thumbScreenPosition = _thumb.position;
            Vector2 position = thumbScreenPosition - spectrumScreenPosition + _spectrumXY * 0.5f;
            position = new Vector2((position.x / _palette.rect.width),
                (position.y / _palette.rect.height));
            return position;
        }

        private void CreateDebugs(Vector2[] pos)
        {
            _debugTransform = new GameObject("Debug").transform;
            _debugTransform.SetParent(_palette);
            _debugTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _debugTransform.localScale = Vector3.one;

            for (int i = 0; i < pos.Length; i++)
            {
                Transform t = Instantiate(_debugCenterPrefab, _debugTransform);
                t.localPosition = pos[i];                
            }

            for (int i = 0; i < _nearestPoints.Length; i++)
            {
                Transform t = Instantiate(_debugClosestPrefab, _debugTransform);
                t.localPosition = _nearestPoints[i];
                _debugs.Add(t);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(_showDebugs != _debugTransform.gameObject.activeSelf)
            {
                ShowDebugs = _showDebugs;
            }
        }
#endif
    }
}