using UnityEngine;

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
    public float sightRange = 35f;

    public Animator animator;

    private Vector3 targetLocation;

    void Start()
    {
        // Pick a random starting point to patrol to
        PickRandomPatrolPoint();
    }

    void Update()
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
        // Move towards the random patrol point
        MoveTowards(targetLocation, moveSpeed);

        // If we reach the point pick new one
        if (Vector3.Distance(transform.position, targetLocation) < 1f)
        {
            PickRandomPatrolPoint();
        }

        // If the player gets too close switch to Chase state
        if (Vector3.Distance(transform.position, player.position) <= sightRange)
        {
            currentState = AIState.Chase;
        }
    }

    void UpdateInvestigateState()
    {
        // Implement later once noise or other stimuli are added to the game
        MoveTowards(targetLocation, moveSpeed);
    }

    void UpdateChaseState()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
        // Constantly move towards the player
        MoveTowards(player.position, chaseSpeed);

        // If the player runs far enough away, go back to patrolling
        if (Vector3.Distance(transform.position, player.position) > sightRange * 1.5f)
        {
            PickRandomPatrolPoint();
            currentState = AIState.Patrol;
        }
    }

    

   void MoveTowards(Vector3 destination, float speed)
    {
        // Lock the Y coordinate to the doll's current height so she doesn't fly
        Vector3 flatTarget = new Vector3(destination.x, transform.position.y, destination.z);
        transform.position = Vector3.MoveTowards(transform.position, flatTarget, speed * Time.deltaTime);
        transform.LookAt(flatTarget);
    }

    void PickRandomPatrolPoint()
    {
        // Pick a random point within a certain range for the AI to patrol to
        float randomX = Random.Range(-35f, 35f);
        float randomZ = Random.Range(-35f, 35f);
        targetLocation = new Vector3(randomX, transform.position.y, randomZ);
    }
}