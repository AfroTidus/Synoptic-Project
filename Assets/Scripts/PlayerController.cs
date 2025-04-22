using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    public float lockOnRange = 5f;
    public List<Interactable> nearbyInteractables = new List<Interactable>();
    private int currentInteractableIndex = -1;
    private Interactable lockedInteractable = null;

    private bool nearSpawner =  false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckInteractables();
        CheckLockedInteractableDistance();
    }

    void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float gravity = -9.81f;
        Vector3 velocity = new Vector3(0, gravity * Time.deltaTime, 0);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move((moveDir.normalized * speed * Time.deltaTime) + velocity);
        }
        else
        {
            characterController.Move(velocity);
        }
    }

    void CheckInteractables()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearSpawner)
            {
                EventManager.TriggerEvent(EventNames.PlayerPressedSpawnKey);
            }
            else
            {
                Debug.Log("No valid interaction");
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (lockedInteractable == null)
            {
                DetectNearbyInteractables();
                if (nearbyInteractables.Count > 0)
                {
                    LockOnToInteractable(0);
                }
            }
            else
            {
                if (nearbyInteractables.Count > 1)
                {
                    CycleInteractables();
                }
                else
                {
                    UnlockInteractable();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UnlockInteractable();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (lockedInteractable != null)
            {
                // Command followers to move to lock on position
                EventManager.TriggerEvent(EventNames.FollowerMoveToPosition, lockedInteractable.transform.position);
            }
            else
            {
                Debug.Log("No interactable locked on to command followers");
            }
        }
    }

    private void CheckLockedInteractableDistance()
    {
        if (lockedInteractable != null)
        {
            float distance = Vector3.Distance(transform.position, lockedInteractable.transform.position);
            if (distance > lockOnRange)
            {
                UnlockInteractable();
            }
        }
    }

    void DetectNearbyInteractables()
    {
        nearbyInteractables.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lockOnRange);
        foreach (var hitCollider in hitColliders)
        {
            Interactable interactable = hitCollider.GetComponent<Interactable>();
            if (interactable != null)
            {
                nearbyInteractables.Add(interactable);
            }
        }
    }

    void LockOnToInteractable(int index)
    {
        if (lockedInteractable != null)
        {
            lockedInteractable.UnLocked();
            Debug.Log("Unlocked");
        }

        lockedInteractable = nearbyInteractables[index];
        lockedInteractable.Locked();
        Debug.Log("Locked On");
        currentInteractableIndex = index;
    }

    void CycleInteractables()
    {
        DetectNearbyInteractables();
        if (nearbyInteractables.Count == 0) return;

        currentInteractableIndex = (currentInteractableIndex + 1) % nearbyInteractables.Count;
        LockOnToInteractable(currentInteractableIndex);
    }

    void UnlockInteractable()
    {
        if (lockedInteractable != null)
        {
            lockedInteractable.UnLocked();
            Debug.Log("Unlocked");
            lockedInteractable = null;
            //currentInteractableIndex = -1;
            nearbyInteractables.Clear();
        }
    }

    // Collision logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spawner"))
        {
            // Trigger the event when the player enters the spawn radius
            EventManager.TriggerEvent(EventNames.PlayerEnteredSpawnRadius, other.gameObject);
            nearSpawner = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spawner"))
        {
            // Trigger the event when the player exits the spawn radius
            EventManager.TriggerEvent(EventNames.PlayerExitedSpawnRadius, other.gameObject);
            nearSpawner = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
    }
}