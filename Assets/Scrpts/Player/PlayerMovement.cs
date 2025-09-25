using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerModel;
    [SerializeField] private InputHandler _input;

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

    [Header("Aim Rotation Settings")]
    [SerializeField] private float _aimRotationSpeed = 5f;
    [SerializeField] private MousePosition3D _mousePosition3D;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerSpeed = moveSpeed; // Initialize playerSpeed to default move speed
    }

    private void HandleInput()
    {
        // Handle sprint input
        if (_input != null)
        {
            if (_input.sprint)
            {
                playerSpeed = sprintSpeed;
            }
            else
            {
                playerSpeed = moveSpeed;
            }

            // Handle jump input
            if (_input.jump && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
            }
        }
    }

    private void HandlePlayerRotation(Vector3 moveDirection)
    {
        // If aiming, rotate player to face the aim direction
        if (_input != null && _input.aim && _mousePosition3D != null)
        {
            Vector3 aimDirection = _mousePosition3D.GetAimDirection();
            // Flatten the aim direction to XZ plane (ignore Y component)
            Vector3 aimDirectionXZ = new Vector3(aimDirection.x, 0, aimDirection.z).normalized;

            if (aimDirectionXZ.magnitude > 0.1f)
            {
                // Calculate target rotation based on aim direction
                float targetAngle = Mathf.Atan2(aimDirectionXZ.x, aimDirectionXZ.z) * Mathf.Rad2Deg;

                // Smoothly rotate towards the aim direction using Lerp
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                playerModel.rotation = Quaternion.Lerp(playerModel.rotation, targetRotation, _aimRotationSpeed * Time.deltaTime);
            }
        }
        // If not aiming but moving, rotate based on movement direction
        else if (moveDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(playerModel.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _turnSmoothTime);
            playerModel.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void HandleMovementAnimation()
    {
        if (_input != null && _input.move.magnitude > 0.1f)
        {
            animator.SetBool("IsMoving", true);
            // Only show sprinting animation if player is actually moving AND sprinting
            animator.SetBool("IsSprinting", playerSpeed == sprintSpeed);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsSprinting", false); // Turn off sprinting animation when not moving
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
        Vector2 moveInput = _input != null ? _input.move : Vector2.zero;
        Vector3 moveDirection = cameraForwardXZ * moveInput.y + cameraRightXZ * moveInput.x;

        // Handle player rotation
        HandlePlayerRotation(moveDirection);

        // Move the character
        characterController.Move(playerSpeed * Time.deltaTime * moveDirection);

        // Apply gravity
        velocity.y += gravity * weight * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Update()
    {
        HandleInput();
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

