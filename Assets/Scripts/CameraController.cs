using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target; // the player
    public Vector3 offset = new Vector3(0, 2f, -4f); // camera offset
    public float lookSpeed = 2f; // mouse sensitivity
    public float minPitch = -35f;
    public float maxPitch = 60f;

    private Vector2 lookInput;
    private float yaw;   // horizontal rotation
    private float pitch; // vertical rotation
    private PlayerInputActions controls;

    private void Awake()
    {
        controls = new PlayerInputActions();

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void LateUpdate()
    {
        if (!target) return;

        // Update rotation based on mouse
        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Rotate around the target
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Move camera
        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // aim at player head
    }
}
