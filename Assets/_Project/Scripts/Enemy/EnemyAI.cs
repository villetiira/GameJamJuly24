using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace keijo
{

    public class EnemyAI : MonoBehaviour
    {
        [Header("Component References")]
        public Animator animator;
        //public GameManager gameManager;
        public Transform enemyEyes;
        public NavMeshAgent agent;
        public AudioSource audioSource;

        [Space(5f)]
        [Header("Stats")]
        public float patrolRange = 100;
        public float alertPatrolRange = 15;
        public float alertTimer = 10f;
        public float maxAlertTimer = 10f;
        public float walkSpeed = 4f;
        public float runSpeed = 7f;
        public int damage = 50;

        [Space(5f)]
        [Header("Audio")]
        public AudioClip attackSound;
        public AudioClip idleSound1;
        public AudioClip idleSound2;
        public AudioClip angrySound;
        public AudioClip deathSound;
        public AudioClip hitSound;
        public AudioClip specialSound;
        public AudioClip footStepSound;


        [Space(5f)]
        [Header("Debugging")]
        /* 
        * States: 
        * 0 : patrol 
        * 1 : combat
        * 2 : alert
        * 3 : dead
         */
        public int currentState = 0;
        public bool attacking = false;

        public Transform targetPlayer;
        public Vector3 spawnPosition;
        public Vector3 lastSightingPos;
        bool playerInRange = false;

        // Define layer mask to include all layers except the "Structure" and "Enemies" layers
        int layerMask = (1 << 11) | (1 << 12);

        void Awake()
        {
            //gameManager = FindFirstObjectByType<GameManager>();
        }

        public virtual void Start()
        {
            SwitchToPatrolState();
        }

        public virtual void Update()
        {
            switch (currentState)
            {
                case 0:
                    RunPatrolState();
                    break;
                case 1:
                    RunCombatState();
                    break;
                case 2:
                    RunAlertState();
                    break;
                case 3:
                    RunDeadState();
                    break;
                default:
                    Debug.Log("Unrecognized state on" + gameObject.name);
                    break;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                SwitchToDeadState();
            }
        }

        public virtual void SwitchToPatrolState()
        {
            Debug.Log(gameObject.name + " Switched to Patrol state");
            currentState = 0;
            agent.speed = walkSpeed;
        }
        public virtual void SwitchToCombatState()
        {
            Debug.Log(gameObject.name + " Switched to Combat state");
            currentState = 1;
            agent.speed = runSpeed;
        }
        public virtual void SwitchToAlertState()
        {
            Debug.Log(gameObject.name + " Switched to Alert state");
            currentState = 2;
            patrolRange = alertPatrolRange;
            alertTimer = maxAlertTimer;
            lastSightingPos = targetPlayer.position;
        }
        public virtual void SwitchToDeadState()
        {
            currentState = 3;
            Debug.Log("entered dead state!");

            //stop movement
            agent.isStopped = true;
            animator.SetFloat("MoveSpeed", 0);

            // Play death animation
            animator.SetTrigger("Death");

            // Play death sound
            audioSource.PlayOneShot(deathSound);

            foreach (Collider collider in GetComponents<Collider>())
            {
                collider.enabled = false;
            }
        }

        public virtual void RunPatrolState()
        {
            // move around
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 point;
                if (RandomPoint(spawnPosition, patrolRange, out point))
                {
                    agent.SetDestination(point);
                }
            }
            // set animation
            if (agent.velocity.normalized.magnitude > 0)
            {
                animator.SetFloat("MoveSpeed", agent.velocity.normalized.magnitude / 2); // half the velocity to walk
            }

        }

        public virtual void RunAlertState()
        {
            // if player not spotted within timer, go back to patrol state
            if (alertTimer < 0)
            {
                // otherwise go back to patrolling immediately
                alertTimer = maxAlertTimer;
                SwitchToPatrolState();
            }
            alertTimer -= Time.deltaTime;
        }

        public virtual void RunCombatState()
        {

            if (attacking) return;

            // check target player status
            /*if (targetPlayer.GetComponentInParent<PlayerStats>().isDead)
            {
                // current target dead, go to patrol state
                SwitchToPatrolState();
                return;
            }*/

            // if target in sight and in range, attack the target
            if (playerInRange)
            {
                StartAttacking();
                return;
            }

            // set movement animation based on speed
            if (agent.velocity.normalized.magnitude > 0)
            {
                animator.SetFloat("MoveSpeed", agent.velocity.normalized.magnitude);
            }

        }

        public virtual void RunDeadState()
        {

        }

        public bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {

            Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
            {
                //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
                //or add a for loop like in the documentation
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }




        public virtual void OnCollisionEnter(Collision collision)
        {
            if(currentState != 1)
            {
                // touched!
                if (collision.gameObject.CompareTag("Player"))
                {
                    targetPlayer = collision.transform.GetComponent<PlayerController>().cameraFollowPoint;
                    SwitchToCombatState();
                }
            }
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("trigger entered!");
                playerInRange = true;
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("trigger exited!");
                playerInRange = false;
            }
        }

        public void StartAttacking()
        {
            Debug.Log("Attacking!");
            attacking = true;

            // Stop moving
            agent.SetDestination(transform.position);
            animator.SetFloat("MoveSpeed", 0);

            // play animation
            animator.SetTrigger("Attack");
        }

        public void AttackAnimationFinished()
        {
            Debug.Log("finished attacking");
            attacking = false;
        }

        public void Attack()
        {
            if (playerInRange)
            {
                targetPlayer.GetComponent<PlayerController>().TakeDamage(damage);
            }
        }

        public void Hit(Transform player)
        {
            Debug.Log(gameObject.name + " is hit by " + player.name + "!");
            if(!attacking)
            {
                animator.SetTrigger("Hit");
            }
            // Switch to Follow state if not already following
            if (currentState != 1 || currentState != 3) // check if already in combat state or dead
            {
                targetPlayer = player.GetComponent<PlayerController>().cameraFollowPoint;
                SwitchToCombatState();
            }
        }

        public void Die()
        {

            // Disable AI to stop movement
            SwitchToDeadState();
        }

        public void PlayAudio(string soundName)
        {
            Debug.Log("playing audio: "+ soundName);
            switch(soundName)
            {
                case "Attack1":
                    audioSource.PlayOneShot(attackSound);
                    break;
                case "Idle1":
                    audioSource.PlayOneShot(idleSound1);
                    break;
                case "Idle2":
                    audioSource.PlayOneShot(idleSound2);
                    break;
                case "Angry":
                    audioSource.PlayOneShot(angrySound);
                    break;
                case "Death":
                    audioSource.PlayOneShot(deathSound);
                    break;
                case "Hit":
                    audioSource.PlayOneShot(hitSound);
                    break;
                case "Special":
                    audioSource.PlayOneShot(specialSound);
                    break;
                case "FootStep":
                    audioSource.PlayOneShot(footStepSound);
                    break;
                default:
                    Debug.Log("Unrecognized sound: ' " + soundName + " ' played on :" + gameObject.name);
                    break;
            }
        }

        public void SetTargetPlayer(Transform player)
        {
            targetPlayer = player;
        }

        public bool PlayerIsInFront(Vector3 playerPos)
        {
            //get direction to player without vertical
            Vector3 directionToPlayer = playerPos - enemyEyes.position;
            directionToPlayer.y = 0f;

            //get enemy forward direction without vertical
            Vector3 forwardDirection = transform.forward;
            forwardDirection.y = 0f;

            //check if player is in 45 degree cone
            return Vector3.Angle(forwardDirection, directionToPlayer) <= 90 * 0.5f;
        }

        public bool HasLineOfSight(Vector3 playerPos)
        {
            // Perform raycast from start to target with the specified layer mask
            RaycastHit hit;
            Debug.DrawLine(enemyEyes.position, playerPos, Color.red, 0.1f);
            if (Physics.Raycast(enemyEyes.position, playerPos - enemyEyes.position, out hit, layerMask))
            {
                // No obstacles blocking the line of sight
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }

            // If no direct line of sight, check using spherecast
            Vector3 directionToPlayer = playerPos - enemyEyes.position;

            if (Physics.SphereCast(enemyEyes.position, 0.5f, directionToPlayer.normalized, out hit, 10))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // Player is in line of sight, react to player presence
                    return true; // Exit early since player is visible
                }
            }

            return false;
        }

    }
}