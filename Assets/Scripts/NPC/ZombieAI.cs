using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    /// <summary>
    /// Zombie NPC that follows and chases the player using NavMesh pathfinding.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieAI : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("The player transform to follow")]
        [SerializeField] private Transform player;

        [Header("Movement Settings")]
        [Tooltip("How close the zombie needs to be to stop moving")]
        [SerializeField] private float stoppingDistance = 2f;

        [Tooltip("How often to update the path (in seconds)")]
        [SerializeField] private float pathUpdateRate = 0.2f;

        private NavMeshAgent agent;
        private float pathUpdateTimer;

        /// <summary>
        /// Initialize the NavMeshAgent component.
        /// </summary>
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = stoppingDistance;
        }

        /// <summary>
        /// Find the player if not assigned.
        /// </summary>
        private void Start()
        {
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }
        }

        /// <summary>
        /// Update the zombie's path to follow the player.
        /// </summary>
        private void Update()
        {
            if (player == null) return;

            pathUpdateTimer += Time.deltaTime;

            if (pathUpdateTimer >= pathUpdateRate)
            {
                pathUpdateTimer = 0f;
                agent.SetDestination(player.position);
            }
        }
    }
}