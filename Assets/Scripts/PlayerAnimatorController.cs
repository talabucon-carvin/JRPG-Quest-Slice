using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (playerController == null) return;

        // Get movement vector in world space
        Vector3 velocity = playerController.Rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);

        // Speed parameter
        float speed = horizontalVel.magnitude;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime); // smooth damp

        // Optional: directional blend (for strafing)
        Vector3 localVel = transform.InverseTransformDirection(horizontalVel);
        animator.SetFloat("MoveX", localVel.x, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveY", localVel.z, 0.1f, Time.deltaTime);

        // Jump / Grounded
        bool isGrounded = Mathf.Abs(playerController.Rb.linearVelocity.y) < 0.01f;
        animator.SetBool("IsGrounded", isGrounded);

        // Trigger jump animation
        if (!isGrounded && animator.GetBool("IsGrounded"))
        {
            animator.SetTrigger("Jump");
        }
    }
}
