using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    [SerializeField] private Transform _arrowPivot;
    [SerializeField] private SphereManager _spherePrefab;
    [SerializeField, Range(0, 20)] private int _sphereCount;
    private ObjectPool<SphereManager> _spheresPool;

    public void Init()
    {
        _spheresPool = new ObjectPool<SphereManager>(_spherePrefab, _sphereCount, transform); 
    }
}
