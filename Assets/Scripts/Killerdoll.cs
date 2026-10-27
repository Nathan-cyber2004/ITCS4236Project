using UnityEngine;
using System.Collections.Generic;

public class KillerDollAI : MonoBehaviour
{
    // AI States
    public enum AIState 
    { 
        Patrol, 
        Investigate, 
        Chase 
    }

    [Header("AI Settings")]
    public AIState currentState = AIState.Patrol; // Start in Patrol mode
    public Transform player;
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float sightRange = 15f; 

    [Header("Breadcrumb Settings")]
     public Transform[] breadcrumbs; 
    private Transform currentBreadcrumb;
    private Transform lastBreadcrumb; 

    public Animator animator;

    private Vector3 targetLocation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        // Pick a starting breadcrumb she can clearly see
        PickNextBreadcrumb();
    }

    void FixedUpdate()
    {
        // Decision Tree for AI States
        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrolState();
                break;
            case AIState.Investigate:
                UpdateInvestigateState();
                break;
            case AIState.Chase:
                UpdateChaseState();
                break;
        }
    }

    // States

    void UpdatePatrolState()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        // Move towards the current breadcrumb
        MoveTowards(targetLocation, moveSpeed);

        // If we reach the breadcrumb in the doorway/hallway, pick the next visible one
        Vector3 flatPos = new Vector3(transform.position.x, targetLocation.y, transform.position.z);
        if (Vector3.Distance(flatPos, targetLocation) < 1.2f)
        {
            PickNextBreadcrumb();
        }

        // Only switch to Chase if player is in range AND not hidden behind a solid wall
        if (Vector3.Distance(transform.position, player.position) <= sightRange && CanSeeTarget(player.position))
        {
            currentState = AIState.Chase;
        }
    }

    void UpdateInvestigateState()
    {
        MoveTowards(targetLocation, moveSpeed);
    }

    void UpdateChaseState()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        // Move towards the player
        MoveTowards(player.position, chaseSpeed);

        // If the player is too far away or hidden behind a wall, go back to patrolling
        if (Vector3.Distance(transform.position, player.position) > sightRange * 1.5f || !CanSeeTarget(player.position))
        {
            PickNextBreadcrumb();
            currentState = AIState.Patrol;
        }
    }

    void MoveTowards(Vector3 destination, float speed)
    {
       
        Vector3 flatTarget = new Vector3(destination.x, transform.position.y, destination.z);
        transform.LookAt(flatTarget);

        Vector3 direction = (flatTarget - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
    }

    void PickNextBreadcrumb()
{
    if (breadcrumbs == null || breadcrumbs.Length == 0) return;

    List<Transform> visibleBreadcrumbs = new List<Transform>();

    // Find all breadcrumbs she can clearly see (excluding the one she is standing on)
    foreach (Transform crumb in breadcrumbs)
    {
        if (crumb != currentBreadcrumb && CanSeeTarget(crumb.position))
        {
            visibleBreadcrumbs.Add(crumb);
        }
    }

    
    // If she sees more than 1 option, prevent her from going back to the last breadcrumb she just came from
    if (visibleBreadcrumbs.Count > 1 && lastBreadcrumb != null)
    {
        visibleBreadcrumbs.Remove(lastBreadcrumb);
    }

   
    lastBreadcrumb = currentBreadcrumb;

    // Pick a random visible forward breadcrumb
    if (visibleBreadcrumbs.Count > 0)
    {
        currentBreadcrumb = visibleBreadcrumbs[Random.Range(0, visibleBreadcrumbs.Count)];
        targetLocation = currentBreadcrumb.position;
    }
    else
    {
        // Fallback if cornered
        currentBreadcrumb = breadcrumbs[Random.Range(0, breadcrumbs.Length)];
        targetLocation = currentBreadcrumb.position;
    }
}

    bool CanSeeTarget(Vector3 targetPos)
    {
        // Check from waist-height so floor colliders don't block the laser check
        Vector3 start = transform.position + Vector3.up * 0.5f;
        Vector3 end = new Vector3(targetPos.x, transform.position.y + 0.5f, targetPos.z);

        
        if (Physics.Linecast(start, end, out RaycastHit hit))
        {
            if (hit.transform == player || hit.transform.IsChildOf(transform))
                return true;
            return false; // Blocked by wall or furniture
        }
        return true; // Clear line of sight through the doorway
    }
}