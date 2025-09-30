using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private InputHandler _input;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _maxShootDistance = 1000f;
    [SerializeField] private LayerMask _hitLayerMask = -1;
    [SerializeField] private float _gunDamage = 10f;
    [SerializeField] private float _fireRate = 10f; // Time between shots in seconds

    [Header("Gunfire Controller")]
    // --- Audio ---
    [SerializeField] private AudioClip _gunShotClip;
    [SerializeField] private AudioSource _source;
    [SerializeField] private Vector2 _audioPitch = new Vector2(.9f, 1.1f);

    // --- Muzzle ---
    [SerializeField] private GameObject _muzzlePrefab;
    [SerializeField] private GameObject _muzzlePosition;


    private float _lastShotTime = 0f;
    private Ray _lastRay; // Store the last ray for gizmo visualization
    private Vector3 _shootingDirection; // Store the current shooting direction

    private void Start()
    {
        if (_source != null) _source.clip = _gunShotClip;
    }
    private void Shoot()
    {
        // Check if enough time has passed since the last shot
        if (Time.time - _lastShotTime < 1f / _fireRate)
        {
            return; // Don't shoot if fire rate hasn't been met
        }

        ShootFX();

        _lastShotTime = Time.time; // Update the last shot time

        ShootHitscan();
    }
    private void ShootFX()
    {
        // --- Spawn muzzle flash ---
        var flash = Instantiate(_muzzlePrefab, _muzzlePosition.transform);

        // --- Handle Audio ---
        if (_source != null)
        {
            // --- Sometimes the source is not attached to the weapon for easy instantiation on quick firing weapons like machineguns, 
            // so that each shot gets its own audio source, but sometimes it's fine to use just 1 source. We don't want to instantiate 
            // the parent gameobject or the program will get stuck in a loop, so we check to see if the source is a child object ---
            if (_source.transform.IsChildOf(transform))
            {
                _source.Play();
            }
            else
            {
                // --- Instantiate prefab for audio, delete after a few seconds ---
                AudioSource newAS = Instantiate(_source);
                if ((newAS = Instantiate(_source)) != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                {
                    // --- Change pitch to give variation to repeated shots ---
                    newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(_audioPitch.x, _audioPitch.y));
                    newAS.pitch = Random.Range(_audioPitch.x, _audioPitch.y);

                    // --- Play the gunshot sound ---
                    newAS.PlayOneShot(_gunShotClip);

                    // --- Remove after a few seconds. Test script only. When using in project I recommend using an object pool ---
                    Destroy(newAS.gameObject, 4);
                }
            }
        }

        // --- Insert custom code here to shoot projectile or hitscan from weapon ---

    }
    private void ShootHitscan()
    {
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
