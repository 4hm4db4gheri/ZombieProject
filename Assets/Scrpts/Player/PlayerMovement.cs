using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerModel;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    private float playerSpeed;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float weight = 1f;
    [Header("Camera Settings")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Vector2 moveInput;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerSpeed = moveSpeed; // Initialize playerSpeed to default move speed
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerSpeed = sprintSpeed;
            animator.SetBool("IsSprinting", true);
        }
        else
        {
            playerSpeed = moveSpeed;
            animator.SetBool("IsSprinting", false);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    private void HandleMovementAnimation()
    {
        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void HandleMovement()
    {
        // Check if grounded
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
        }

        // Get camera forward and right directions (flattened to XZ plane)
        Vector3 cameraForwardXZ = new Vector3(mainCamera.forward.x, 0, mainCamera.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(mainCamera.right.x, 0, mainCamera.right.z).normalized;

        // Calculate movement direction relative to camera
        Vector3 moveDirection = cameraForwardXZ * moveInput.y + cameraRightXZ * moveInput.x;

        // Only rotate player if there's movement input
        if (moveDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(playerModel.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _turnSmoothTime);
            playerModel.rotation = Quaternion.Euler(0, angle, 0);
        }

        // Move the character
        characterController.Move(playerSpeed * Time.deltaTime * moveDirection);

        // Apply gravity
        velocity.y += gravity * weight * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Update()
    {
        HandleMovement();
        HandleMovementAnimation();
    }

    private void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            Gizmos.color = Color.red;
            // Get camera forward but flatten to XZ plane (set Y to 0)
            Vector3 cameraForwardXZ = new Vector3(mainCamera.forward.x, 0, mainCamera.forward.z).normalized;
            Gizmos.DrawRay(transform.position, cameraForwardXZ * 3f);
        }
    }
}

