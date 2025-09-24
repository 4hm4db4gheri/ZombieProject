using UnityEngine;

public class MousePosition3D : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _maxAimDistance = 1000f; // Maximum distance for sky aiming
    [SerializeField] private LayerMask _aimLayerMask = -1; // Which layers to aim at (default: all layers)

    private void Update()
    {
        Vector3 aimPosition = GetMouseWorldPosition();
        transform.position = aimPosition;
    }

    /// <summary>
    /// Gets the world position where the mouse is pointing, with fallback for sky aiming
    /// </summary>
    /// <returns>World position for aiming</returns>
    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        // First try to hit an object
        if (Physics.Raycast(ray, out RaycastHit hit, _maxAimDistance, _aimLayerMask))
        {
            return hit.point;
        }

        // If no object hit, use a point at maximum distance along the ray
        return ray.GetPoint(_maxAimDistance);
    }

    /// <summary>
    /// Gets the direction from the camera to the mouse world position
    /// </summary>
    /// <returns>Normalized direction vector</returns>
    public Vector3 GetAimDirection()
    {
        Vector3 aimPosition = GetMouseWorldPosition();
        return (aimPosition - _mainCamera.transform.position).normalized;
    }

    /// <summary>
    /// Gets the distance to the aim point
    /// </summary>
    /// <returns>Distance to the aim point</returns>
    public float GetAimDistance()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxAimDistance, _aimLayerMask))
        {
            return hit.distance;
        }

        return _maxAimDistance;
    }
}