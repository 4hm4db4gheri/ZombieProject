using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private InputHandler _input;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _maxShootDistance = 1000f;
    [SerializeField] private LayerMask _hitLayerMask = -1;
    [SerializeField] private float _gunDamage = 10f;
    [SerializeField] private float _fireRate = 0.1f; // Time between shots in seconds

    private float _lastShotTime = 0f;
    private Ray _lastRay; // Store the last ray for gizmo visualization
    private Vector3 _shootingDirection; // Store the current shooting direction

    private void Shoot()
    {
        // Check if enough time has passed since the last shot
        if (Time.time - _lastShotTime < _fireRate)
        {
            return; // Don't shoot if fire rate hasn't been met
        }

        _lastShotTime = Time.time; // Update the last shot time

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        _lastRay = ray; // Store ray for gizmo visualization
        _shootingDirection = ray.direction; // Store shooting direction for player rotation

        // First try to hit an object, but ignore the player
        if (Physics.Raycast(ray, out RaycastHit hit, _maxShootDistance, _hitLayerMask))
        {
            // Check if we hit the player - if so, ignore this hit and continue the ray
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                // Continue the ray from the hit point to find the next object
                Vector3 newOrigin = hit.point + ray.direction * 0.1f; // Small offset to avoid self-collision
                Ray newRay = new Ray(newOrigin, ray.direction);

                if (Physics.Raycast(newRay, out RaycastHit newHit, _maxShootDistance - hit.distance, _hitLayerMask))
                {
                    Debug.Log("Hit (after player): " + newHit.transform.name);
                    // Check if the hit object implements IDamageable
                    IDamageable damageable = newHit.transform.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(_gunDamage, gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("Hit: " + hit.transform.name);
                // Check if the hit object implements IDamageable
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_gunDamage, gameObject);
                }
            }
        }
    }


    private void Update()
    {
        // Always update shooting direction based on mouse position, even when not shooting
        if (_mainCamera != null)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            _shootingDirection = ray.direction;
        }

        if (_input.shoot)
        {
            Shoot();
        }
    }

    // Public method to get the current shooting direction for player rotation
    public Vector3 GetShootingDirection()
    {
        return _shootingDirection;
    }

    // Draw the ray in the Scene view for debugging
    private void OnDrawGizmos()
    {
        if (_mainCamera != null)
        {
            // Draw the ray from camera to mouse position
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_lastRay.origin, _lastRay.direction * _maxShootDistance);

            // Draw a small sphere at the ray origin
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_lastRay.origin, 0.1f);
        }
    }
}
