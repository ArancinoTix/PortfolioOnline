using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public Vector2 MousePosition { get; private set; }

    void Update()
    {
        if (Mouse.current != null)
        {
            MousePosition = Mouse.current.position.ReadValue();
        }
    }
}
