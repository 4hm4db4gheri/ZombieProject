using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
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
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private Rig _aimRig;
    [SerializeField] private Animator _animator;
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
    [SerializeField] private float _horizontalSensetivity = 2f;
    [SerializeField] private float _verticalSensetivity = 2f;
    [SerializeField] private float _aimHorizontalSensetivity = 0.5f;
    [SerializeField] private float _aimVerticalSensetivity = 0.5f;
    [SerializeField] private float _aimRigSpeed = 10f;
    [Header("Input System")]
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            // Check if mouse is available and being used
            return Mouse.current != null;
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

    private void Update()
    {
        CameraAim();
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

    #region Camera 
    private void CameraAim()
    {
        if (_inputHandler.aim)
        {
            aimCamera.Priority = 1;
            thirdPersonCamera.Priority = 0;

            _aimRig.weight = Mathf.Lerp(_aimRig.weight, 1f, Time.deltaTime * _aimRigSpeed);
            _animator.SetBool("IsAiming", true);
        }
        else
        {
            aimCamera.Priority = 0;
            thirdPersonCamera.Priority = 1;

            _aimRig.weight = Mathf.Lerp(_aimRig.weight, 0f, Time.deltaTime * _aimRigSpeed);
            _animator.SetBool("IsAiming", false);
        }
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_inputHandler.look.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _inputHandler.look.x * deltaTimeMultiplier * (_inputHandler.aim ? _aimHorizontalSensetivity : _horizontalSensetivity);
            _cinemachineTargetPitch += _inputHandler.look.y * deltaTimeMultiplier * (_inputHandler.aim ? _aimVerticalSensetivity : _verticalSensetivity);
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
