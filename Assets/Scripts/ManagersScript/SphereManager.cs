using UnityEngine;

public class SphereManager : MonoBehaviour
{
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 force)
    {
        _rb.AddForce(force, ForceMode.Impulse);
    }
}
