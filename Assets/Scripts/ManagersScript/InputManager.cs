using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public Action<Vector2> onPositionUpdate;

    void Update()
    {
        if (Mouse.current != null)
        {
            onPositionUpdate?.Invoke(Mouse.current.position.ReadValue());
        }
    }
}
