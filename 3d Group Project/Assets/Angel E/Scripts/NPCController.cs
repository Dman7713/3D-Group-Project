using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ObjectList
{
    public List<string> objects; // A single list of expected objects
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

    private static HashSet<Transform> occupiedLocations = new HashSet<Transform>();

    [SerializeField] private List<ObjectList> possibleLists = new List<ObjectList>(); // Multiple lists stored properly
    private List<string> activeList = new List<string>(); // The chosen list for checking
    private HashSet<string> objectsInTrigger = new HashSet<string>(); // Track objects inside the trigger
    public bool isSatisfied { get; private set; } = false; // True when all expected objects are inside




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
            Debug.Log($"Selected list {randomIndex + 1}: [{string.Join(", ", activeList)}]");
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
            Debug.Log("NPC lacks time. Moving to exit.");
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
                objectsInTrigger.Add(expectedName);
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
                objectsInTrigger.Remove(expectedName);
                matched = true;
                break;
            }
        }

        if (matched) CheckIfSatisfied();
    }

    private void CheckIfSatisfied()
    {
        isSatisfied = objectsInTrigger.Count == activeList.Count;
        Debug.Log($"isSatisfied: {isSatisfied}");
    }

    private bool NameMatches(string objectName, string expectedName)
    {
        string pattern = $@"(^|\s|_){Regex.Escape(expectedName)}(\s|_|$|Clone)";
        return Regex.IsMatch(objectName, pattern);
    }
}