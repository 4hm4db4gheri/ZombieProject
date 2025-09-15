using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera thirdPersonCamera;
    [SerializeField] private CinemachineCamera aimCamera;

    private void Start()
    {
        thirdPersonCamera.Priority = 1;
        aimCamera.Priority = 0;
    }

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
}
