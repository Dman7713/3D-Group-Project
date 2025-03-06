using UnityEngine;
using UnityEngine.AI;

public class AIWalkToCounter : MonoBehaviour
{
    public float speed = 3f; // Speed of the AI
    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent
    private Transform target; // The target object (Counter)

    void Start()
    {
        // Get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Optionally, set speed from the inspector
        navMeshAgent.speed = speed;

        // Find the object with the "Counter" tag
        GameObject counter = GameObject.FindGameObjectWithTag("Counter");

        if (counter != null)
        {
            target = counter.transform;
            // Set the destination for the AI to move towards the counter
            navMeshAgent.SetDestination(target.position);
        }
        else
        {
            Debug.LogWarning("No object with 'Counter' tag found!");
        }
    }

    void Update()
    {
        // Optionally, you can check if the AI has reached the target
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            Debug.Log("AI has reached the counter!");
        }
    }
}
