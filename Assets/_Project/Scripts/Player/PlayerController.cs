using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace keijo
{
    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool TargetDown;
        public bool TargetUp;
        public bool SprintDown;
        public bool SprintUp;
    }

    public class PlayerController : MonoBehaviour
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        //constants
        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        public GameManager gameManager;

        [Header("Movement Settings")]
        public float walkSpeed = 5f;
        public float sprintSpeed = 10f;
        public float jumpForce = 5f;
        public Animator animator;
        //public Animator armsAnimator;
        bool sprinting = false;

        [Header("Player stats")]
        public int health = 100;
        public bool isDead = false;

        [Header("Camera Settings")]
        public Transform cameraFollowPoint;
        public CharacterCamera characterCamera;
        public GameObject characterBody;
        public GameObject crosshair;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip takeDamageClip;

        [Header("Interacting")]
        bool interacting = false;
        Interactable interactTarget;
        float interactTimer;
        public GameObject interactPanel;
        public TMP_Text interactText;
        public TMP_Text interactTargetName;
        public Slider interactProgress;
        public UnityEvent<float> InteractProgress = new UnityEvent<float>();

        // variables
        bool isGrounded = false;
        bool daySwitching;

        private void Awake()
        {

            //initialize UI
            //armsAnimator = transform.Find("FirstPersonCamera").GetComponentInChildren<Animator>();
            animator = transform.Find("FullBody").GetComponentInChildren<Animator>();
        }

        private void Start()
        {

            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            characterCamera.gameObject.SetActive(true);

            SetLayerAllChildren(characterBody.transform, 10);
            characterCamera.SetFollowTransform(cameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            characterCamera.IgnoredColliders.Clear();
            characterCamera.IgnoredColliders.AddRange(gameObject.GetComponentsInChildren<Collider>());
            Physics.IgnoreCollision(GetComponent<Collider>(), GetComponent<Collider>());
        }

        void Update()
        {
            if (isDead || daySwitching) return;
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            UpdateGroundedStatus();

            GetInputs();

            SetInputs();

            /*if (Input.GetMouseButtonDown(0) && !attackInProgress)
            {
                Attack();
            }*/

            if (Input.GetKeyDown(KeyCode.E))
            {
                interacting = true;
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                interacting = false;
                interactTimer = 0;
                interacting = false;
                interactProgress.value = 0;
                interactProgress.gameObject.SetActive(false);
            }
            if (interacting)
            {
                Interacting();
            }

            ShowInteractables();

        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);

            // Apply inputs to the camera
            characterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);


        }

        void GetInputs()
        {
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = characterCamera.transform.rotation;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
            characterInputs.SprintDown = Input.GetKeyDown(KeyCode.LeftShift);
            characterInputs.SprintUp = Input.GetKeyUp(KeyCode.LeftShift);
            characterInputs.TargetDown = Input.GetMouseButtonDown(1);
            characterInputs.TargetUp = Input.GetMouseButtonUp(1);
        }

        void SetInputs()
        {
            if (characterInputs.SprintDown)
            {
                sprinting = true;
            }

            if (characterInputs.SprintUp)
            {
                sprinting = false;
            }

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(characterInputs.CameraRotation * Vector3.forward, Vector3.up).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(characterInputs.CameraRotation * Vector3.up, Vector3.up).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);

            Vector3 moveInputVector = cameraPlanarRotation * Vector3.ClampMagnitude(new Vector3(characterInputs.MoveAxisRight, 0f, characterInputs.MoveAxisForward), 1f);

            // ############ 3rd Person code #############
            //turn character towards the direction of movement
            /*Quaternion rotation = transform.rotation;
            if (targeting)
            {
                // Set the current rotation from camera
                Vector3 smoothedLookInputDirection = Vector3.Slerp(transform.forward, cameraPlanarDirection, 1 - Time.deltaTime).normalized;
                transform.rotation = Quaternion.LookRotation(smoothedLookInputDirection, Vector3.up);
            }
            else if (moveInputVector != Vector3.zero)
            {
                //set the current rotation from movement
                rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveInputVector), Time.deltaTime * 40f);
                transform.rotation = rotation;
            }*/
            // ############ 3rd Person code #############

            // Set the current rotation from camera
            Vector3 smoothedLookInputDirection = Vector3.Slerp(transform.forward, cameraPlanarDirection, 1 - Time.deltaTime).normalized;
            transform.rotation = Quaternion.LookRotation(smoothedLookInputDirection, Vector3.up);


            //set animations
            float currentVelocityMagnitude = sprinting ? moveInputVector.magnitude : moveInputVector.magnitude * 0.5f;
            animator.SetFloat("MoveSpeed", currentVelocityMagnitude);
            //armsAnimator.SetFloat("MoveSpeed", currentVelocityMagnitude);

            float moveSpeed = sprinting ? sprintSpeed : walkSpeed;
            gameObject.transform.position += moveInputVector * moveSpeed * Time.deltaTime;

            if (characterInputs.JumpDown && isGrounded)
            {
                gameObject.GetComponentInChildren<Rigidbody>().velocity = Vector3.up * jumpForce;
            }
        }

        void UpdateGroundedStatus()
        {
            //get the radius of the players capsule collider, and make it a tiny bit smaller than that
            float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
            //get the position (assuming its right at the bottom) and move it up by almost the whole radius
            Vector3 pos = transform.position + Vector3.up * (radius * 0.9f);
            bool status = false;
            //returns true if the sphere touches something on that layer
            Collider[] hitcollider;
            hitcollider = Physics.OverlapSphere(pos, radius);
            foreach (var coll in hitcollider)
            {
                if(!coll.gameObject.CompareTag("Player"))
                {
                    status = true;
                }
            }
            isGrounded = status;
            animator.SetBool("Grounded", status);
            //armsAnimator.SetBool("Grounded", status);
        }

        void SetLayerAllChildren(Transform root, int layer)
        {
            var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = layer;
            }
        }

        /*void Attack()
        {
            attackInProgress = true;
            animator.SetTrigger("SwordSlash");
            //armsAnimator.SetTrigger("SwordSlash");
        }*/

        public void TakeDamage(int damage)
        {
            Debug.Log("Took damage: " + damage);
            audioSource.PlayOneShot(takeDamageClip);
            health -= damage;
            if(health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Player died");
            isDead = true;
            gameManager.GameOver(true);
        }

        public void ShowInteractables()
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(characterCamera.transform.position, characterCamera.transform.TransformDirection(Vector3.forward), out raycastHit, 3f))
            {
                GameObject targetObject = raycastHit.collider.gameObject;
                Interactable interactable = targetObject.GetComponent<Interactable>();
                if(interactable && !interactPanel.activeSelf)
                {
                    interactPanel.SetActive(true);
                    interactText.text = interactable.interactTooltip;
                    interactTargetName.text = interactable.interactableName;
                }
                else if(!interactable)
                {
                    interactPanel.SetActive(false);
                }
            }
            else
            {
                interactPanel.SetActive(false);
            }
        }

        public void Interacting()
        {
            // raycast to see what player is targeting
            RaycastHit raycastHit;
            if (Physics.Raycast(characterCamera.transform.position, characterCamera.transform.TransformDirection(Vector3.forward), out raycastHit, 3f))
            {
                GameObject targetObject = raycastHit.collider.gameObject;
                Interactable interactable = targetObject.GetComponent<Interactable>();

                if(interactable)
                {
                    if (interactable == interactTarget)
                    {
                        interactProgress.gameObject.SetActive(true);
                        if (interactTarget.interactTime < interactTimer)
                        {
                            interactable.Interact(gameObject);
                            audioSource.PlayOneShot(interactable.GetInteractClip());
                            interactTimer = 0;
                            interacting = false;
                            interactProgress.value = 0;
                            interactProgress.gameObject.SetActive(false);
                        }
                        interactTimer += Time.deltaTime;
                        interactProgress.value =  interactTimer / interactTarget.interactTime;
                    }
                    else
                    {
                        interactTimer = 0;
                        interactTarget = interactable;
                        interactProgress.value = 0;
                        interactProgress.gameObject.SetActive(false);
                    }
                }
                else
                {
                    interactTimer = 0;
                    interactProgress.value = 0;
                    interactProgress.gameObject.SetActive(false);
                }
            }
        }

        public void DaySwitched()
        {
            daySwitching = true;
        }

        public void LevelLoaded()
        {
            daySwitching = false;
        }

    }
}
