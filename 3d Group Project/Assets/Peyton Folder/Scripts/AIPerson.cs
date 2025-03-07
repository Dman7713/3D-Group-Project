using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CustomerAI : MonoBehaviour
{
    public NavMeshAgent agent; // Assign in Inspector
    public Transform counterSpot; // Should be an EMPTY GameObject at the waiting position
    public Transform exitPoint; // The position where NPC leaves
    public float waitTime = 5f; // How long they wait for food before leaving
    public string requiredFoodTag = "Food"; // Tag for food items

    private bool isWaiting = false;
    private bool hasEaten = false;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("No NavMeshAgent found on " + gameObject.name);
                return;
            }
        }

        if (counterSpot != null)
        {
            Debug.Log(gameObject.name + " moving to counter spot at: " + counterSpot.position);
            agent.SetDestination(counterSpot.position);
        }
        else
        {
            Debug.LogError("Counter spot is not assigned!");
        }
    }

    void Update()
    {
        if (!isWaiting && !hasEaten && agent.remainingDistance > 0 && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log(gameObject.name + " reached the counter and is waiting...");
            StartCoroutine(WaitForFood());
        }
    }

    IEnumerator WaitForFood()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        if (!hasEaten) // If no food was delivered, the customer leaves
        {
            Leave();
        }
    }

    private void Leave()
    {
        if (exitPoint != null)
        {
            Debug.Log(gameObject.name + " is leaving...");
            agent.SetDestination(exitPoint.position);
            StartCoroutine(DespawnAfterExit());
        }
        else
        {
            Debug.LogError("Exit point is not assigned!");
            Destroy(gameObject);
        }
    }

    IEnumerator DespawnAfterExit()
    {
        yield return new WaitUntil(() => agent.remainingDistance > 0 && agent.remainingDistance <= agent.stoppingDistance);
        Debug.Log(gameObject.name + " despawned.");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasEaten && other.CompareTag(requiredFoodTag))
        {
            Debug.Log(gameObject.name + " ate " + other.gameObject.name);
            Destroy(other.gameObject); // "Eat" the food
            hasEaten = true;
            Leave(); // Walk away after eating
        }
    }
}
