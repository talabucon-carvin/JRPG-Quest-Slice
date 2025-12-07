using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera; // assign the camera

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Vector2 moveInput;

    [Header("Camera")]
    public float lookSpeed = 2f;
    private Vector2 lookInput;
    private float cameraPitch = 0f;

    private PlayerInputActions controls; // just declare
    private Rigidbody rb;

    private void Awake()
    {
        // Auto-add Rigidbody if not present
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        // Auto-add Capsule Collider if not present
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<CapsuleCollider>();
            col.height = 2f;
            col.radius = 0.5f;
            col.center = new Vector3(0, 1f, 0);
        }

        // Input system setup
        controls = new PlayerInputActions();

        // Movement
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Jump
        controls.Player.Jump.performed += ctx => Jump();

        // Look
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }


    private void OnEnable()
    {
        if (controls != null) controls.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        if (controls != null) controls.Player.Disable();
    }

    private void Update()
    {
        HandleCamera();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleCamera()
    {
        // Rotate camera pitch (up/down)
        cameraPitch -= lookInput.y * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, -35f, 60f);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // Rotate player yaw (left/right)
        transform.Rotate(Vector3.up * lookInput.x * lookSpeed);
    }

    private void HandleMovement()
    {
        // Get camera-relative directions
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        // Flatten y to stay horizontal
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;
        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.linearVelocity.y; // keep current vertical velocity
        rb.linearVelocity = velocity;
    }

    private void Jump()
    {
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f) // simple ground check
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
