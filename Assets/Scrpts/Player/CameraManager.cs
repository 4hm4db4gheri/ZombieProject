using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private Transform cinemachineCameraTarget;
    [SerializeField] private CinemachineCamera thirdPersonCamera;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private Transform cameraFollowTransform;

    [Header("Input")]
    [SerializeField] private InputHandler _input;

    [Header("Camera Rotation Settings")]
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -30.0f;
    [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
    [SerializeField] private float CameraAngleOverride = 0.0f;

    [Header("Camera Rotation Values")]
    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    [Header("Camera Sensetivity")]
    [SerializeField] private float _horizontalSensetivity = 10.0f;
    [SerializeField] private float _verticalSensetivity = 10.0f;

    [Header("Input System")]
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }

    #region Unity Lifecycle
    private void Start()
    {
        InitializeCamera();
        SetupInputSystem();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }
    #endregion

    #region Initialization
    private void InitializeCamera()
    {
        _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        thirdPersonCamera.Priority = 1;
        aimCamera.Priority = 0;
    }

    private void SetupInputSystem()
    {
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
        Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }
    #endregion

    #region Input Events
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            thirdPersonCamera.Priority = 0;
            aimCamera.Priority = 1;
        }
        else if (context.canceled)
        {
            thirdPersonCamera.Priority = 1;
            aimCamera.Priority = 0;
        }
    }
    #endregion

    #region Camera Rotation
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * _horizontalSensetivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * _verticalSensetivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    #endregion

    #region Utility Methods
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    #endregion

}
