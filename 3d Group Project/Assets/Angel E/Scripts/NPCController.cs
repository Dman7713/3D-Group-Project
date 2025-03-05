using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ObjectList
{
    public List<string> objects; // List of expected objects
    public int pointValue; // Point value for this list
}

public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform exitPoint;
    private TaskLocation targetTask;
    private bool taskStarted = false;
    private bool movingToExit = false;

    [SerializeField] public float despawnTime = 5f;
    [SerializeField] private float returnThreshold = 2.5f;
    [SerializeField] private float returnCheckInterval = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private string plateObj;

    private static HashSet<Transform> occupiedLocations = new HashSet<Transform>();

    [SerializeField] private List<ObjectList> possibleLists = new List<ObjectList>(); // Multiple lists of expected objects
    private List<string> activeList = new List<string>(); // The chosen list for checking
    private int activePointValue = 0; // Points associated with the chosen list
    private HashSet<GameObject> objectsInTrigger = new HashSet<GameObject>(); // Track objects inside the trigger
    public bool isSatisfied { get; private set; } = false; // True when all expected objects are inside
    public int totalScore { get; private set; } = 0; // Keeps track of cumulative score


    [System.Serializable]
    public class TaskLocation
    {
        public Transform location;
        public Transform lookAtTarget;
    }

    public void Start()
    {
        BoxCollider boxCheck = GetComponent<BoxCollider>();
        boxCheck.enabled = false;

        // Select a random list from possibleLists
        if (possibleLists.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleLists.Count);
            activeList = new List<string>(possibleLists[randomIndex].objects);
            activePointValue = possibleLists[randomIndex].pointValue;
            Debug.Log($"Selected list {randomIndex + 1}: [{string.Join(", ", activeList)}] | Points: {activePointValue}");
        }
        else
        {
            Debug.LogWarning("No lists available! Ensure possibleLists has entries.");
        }
    }

    public void InitializeNPC(List<TaskLocation> taskLocations, Transform exit)
    {
        agent = GetComponent<NavMeshAgent>();
        exitPoint = exit;

        // Ensure the agent is enabled
        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        // Make sure the agent is placed on the NavMesh
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            Debug.LogError("NPC not placed on NavMesh at the start!");
            return;
        }
        agent.Warp(hit.position); // Ensure the NPC is placed on a valid position on the NavMesh

        // Select a task location
        targetTask = GetAvailableLocation(taskLocations);
        if (targetTask != null)
        {
            occupiedLocations.Add(targetTask.location);
            agent.SetDestination(targetTask.location.position);
            StartCoroutine(CheckDisplacement());
        }
        else
        {
            Debug.LogError("No available task locations!");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!taskStarted && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            taskStarted = true;
            agent.isStopped = true;

            if (targetTask.lookAtTarget != null)
            {
                StartCoroutine(TurnToTarget(targetTask.lookAtTarget));
            }

            StartCoroutine(CompleteTaskAndMoveToExit());
        }
    }

    private IEnumerator TurnToTarget(Transform lookAtTarget)
    {
        Debug.Log("NPC turning to face: " + lookAtTarget.name);

        while (true)
        {
            Vector3 direction = (lookAtTarget.position - transform.position).normalized;
            direction.y = 0; // Keep NPC upright
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, lookRotation) < 1f)
            {
                Debug.Log("NPC finished turning to " + lookAtTarget.name);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator CheckDisplacement()
    {
        while (true)
        {
            yield return new WaitForSeconds(returnCheckInterval);

            if (!movingToExit && targetTask != null && Vector3.Distance(transform.position, targetTask.location.position) > returnThreshold)
            {
                if (!agent.enabled)
                {
                    agent.enabled = true;
                }
                Debug.Log("NPC was moved! Returning to task location.");
                agent.SetDestination(targetTask.location.position);
            }
        }
    }

    private TaskLocation GetAvailableLocation(List<TaskLocation> taskLocations)
    {
        List<TaskLocation> availableLocations = new List<TaskLocation>();

        foreach (TaskLocation location in taskLocations)
        {
            if (!occupiedLocations.Contains(location.location))
            {
                availableLocations.Add(location);
            }
        }

        if (availableLocations.Count > 0)
        {
            return availableLocations[Random.Range(0, availableLocations.Count)];
        }
        return null;
    }

    private IEnumerator CompleteTaskAndMoveToExit()
    {
        BoxCollider boxCheck = GetComponent<BoxCollider>();
        boxCheck.enabled = true;
        float timeRemaining = despawnTime;
        Debug.Log("NPC at " + targetTask.location.name + ". Starting task.");

        while (!isSatisfied && timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        if (isSatisfied == true)
        {
            Debug.Log("NPC finished task. Moving to exit.");
        }
        else
        {
            Debug.Log("NPC timeout. Moving to exit.");
        }
        boxCheck.enabled = false;
        occupiedLocations.Remove(targetTask.location);
        movingToExit = true;
        agent.isStopped = false;
        agent.SetDestination(exitPoint.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        Debug.Log("NPC reached exit. Despawning.");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        string objectName = other.gameObject.name;
        bool matched = false;

        foreach (string expectedName in activeList)
        {
            bool doesMatch = NameMatches(objectName, expectedName);
            Debug.Log($"Does '{objectName}' match '{expectedName}'? → {doesMatch}");

            if (doesMatch)
            {
                objectsInTrigger.Add(other.gameObject);
                matched = true;
                break;
            }
        }

        if (matched) CheckIfSatisfied();
    }

    private void OnTriggerExit(Collider other)
    {
        string objectName = other.gameObject.name;
        bool matched = false;

        foreach (string expectedName in activeList)
        {
            bool doesMatch = NameMatches(objectName, expectedName);
            Debug.Log($"Does '{objectName}' match '{expectedName}'? → {doesMatch}");

            if (doesMatch)
            {
                objectsInTrigger.Remove(other.gameObject);
                matched = true;
                break;
            }
        }

        if (matched) CheckIfSatisfied();
    }

    private void CheckIfSatisfied()
    {
        // Ensure all expected objects are in the trigger
        foreach (string expectedName in activeList)
        {
            bool found = false;

            foreach (GameObject obj in objectsInTrigger)
            {
                if (NameMatches(obj.name, expectedName))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogWarning("❌ Check canceled: Missing an expected object!");
                return; // Stop the check immediately if any expected object is missing
            }
        }

        // Ensure NO extra objects are present and that no invalid objects are in the trigger
        foreach (GameObject obj in objectsInTrigger)
        {
            bool isValid = false;

            foreach (string expectedName in activeList)
            {
                if (NameMatches(obj.name, expectedName))
                {
                    isValid = true;
                    break;
                }
            }

            if (!isValid)
            {
                Debug.LogWarning($"❌ Check canceled: Extra object '{obj.name}' detected! It's not in the active list.");
                return; // Stop the check if any object is not in the active list
            }
        }

        // Ensure the number of objects in the trigger matches the active list
        if (objectsInTrigger.Count != activeList.Count)
        {
            Debug.LogWarning("❌ Check canceled: The number of objects does not match the expected count!");
            return; // Stop the check if the counts don't match
        }

        // If we pass both checks, set isSatisfied to true
        isSatisfied = true;

        if (isSatisfied)
        {
            MoneyCounter.Instance.AddScore(activePointValue);
            Debug.Log($"✅ Check Complete! Awarded {activePointValue} points.");
            DestroyObjects();
        }
    }

    private void DestroyObjects()
    {
        // Destroy only objects that were inside the trigger
        foreach (GameObject obj in objectsInTrigger)
        {
            Destroy(obj);
        }

        objectsInTrigger.Clear();

        // Find and destroy "Broke Plate" if it exists
        GameObject brokePlate = GameObject.Find("Broke Plate");
        if (brokePlate != null)
        {
            Destroy(brokePlate);
            Debug.Log("🛠️ 'Broke Plate' has been destroyed.");
        }
    }

    private bool NameMatches(string objectName, string expectedName)
    {
        string pattern = $@"(^|\s|_){Regex.Escape(expectedName)}(\s|_|$|Clone)";
        return Regex.IsMatch(objectName, pattern);
    }
}


// B.Bun = 1
// C.Patty = 1
// Cheese = 1
// C.Tomato = 1
// C.Lettuce = 1
// T.Bun = 1