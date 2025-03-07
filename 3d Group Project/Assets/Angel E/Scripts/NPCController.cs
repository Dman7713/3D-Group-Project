using JetBrains.Annotations;
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
    private Animator animator;
    private Rigidbody rb;

    private NavMeshAgent agent;
    private Transform exitPoint;
    private TaskLocation targetTask;
    private bool taskStarted = false;
    private bool movingToExit = false;

    [SerializeField] public float despawnTime = 5f;
    [SerializeField] private float returnThreshold = 2.5f;
    [SerializeField] private float returnCheckInterval = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float customStopDistance = 1.0f; // Custom stopping distance
    [SerializeField] private float extraCustomStopDistance = 5.0f; // Custom stopping distance

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
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Debug.LogError("NPC not placed on NavMesh at the start! Adjusting position.");
            transform.position = hit.position;
        }
        agent.Warp(transform.position);

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
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        // Get the velocity of the Rigidbody and calculate the speed
        float speed = rb.linearVelocity.magnitude;
        Debug.Log(speed);
        // Set the speed parameter in the Animator to control animations
        animator.SetFloat("speed", speed);

    }

    public void InitializeNPC(List<TaskLocation> taskLocations, Transform exit)
    {
        exitPoint = exit;
        agent = GetComponent<NavMeshAgent>();
        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Debug.LogError("NPC not placed on NavMesh at the start! Adjusting position.");
            transform.position = hit.position;
        }
        agent.Warp(hit.position);

        targetTask = GetAvailableLocation(taskLocations);
        if (targetTask != null)
        {
            occupiedLocations.Add(targetTask.location);
            agent.SetDestination(targetTask.location.position);
            StartCoroutine(CheckArrivalAtTarget());
            StartCoroutine(CheckDisplacement());
        }
        else
        {
            Debug.LogError("No available task locations!");
            Destroy(gameObject);
        }
    }

    private IEnumerator CheckArrivalAtTarget()
    {
        while (!taskStarted)
        {
            yield return new WaitForSeconds(0.2f);

            float distanceToTarget = Vector3.Distance(transform.position, targetTask.location.position);

            if (distanceToTarget <= customStopDistance)
            {
                BoxCollider boxCheck = GetComponent<BoxCollider>();
                boxCheck.enabled = true;
                agent.isStopped = true;
                taskStarted = true;

                if (targetTask.lookAtTarget != null)
                {
                    StartCoroutine(TurnToTarget(targetTask.lookAtTarget));
                }

                StartCoroutine(CompleteTaskAndMoveToExit());
                break;
            }
        }
    }

    private IEnumerator TurnToTarget(Transform lookAtTarget)
    {
        while (true)
        {
            Vector3 direction = (lookAtTarget.position - transform.position).normalized;
            direction.y = 0; // Keep NPC upright
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, lookRotation) < 1f)
            {
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

            float distanceToTarget = Vector3.Distance(transform.position, targetTask.location.position);
            if (!movingToExit && targetTask != null && distanceToTarget > returnThreshold)
            {
                if (!agent.enabled)
                {
                    agent.enabled = true;
                }
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
        float timeRemaining = despawnTime;
        Debug.Log("NPC at " + targetTask.location.name + ". Starting task.");

        while (!isSatisfied && timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        if (isSatisfied)
        {
            Debug.Log("NPC finished task. Moving to exit.");
        }
        else
        {
            Debug.Log("NPC timeout. Moving to exit.");
        }
        BoxCollider boxCheck = GetComponent<BoxCollider>();
        boxCheck.enabled = false;
        occupiedLocations.Remove(targetTask.location);
        movingToExit = true;
        agent.isStopped = false;
        agent.SetDestination(exitPoint.position);
        StartCoroutine(CheckArrivalAtExit());
    }

    
    private IEnumerator CheckArrivalAtExit()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            float distanceToExit = Vector3.Distance(transform.position, exitPoint.position);

            if (distanceToExit <= extraCustomStopDistance)
            {
                Debug.Log("NPC reached exit. Despawning.");
                Destroy(gameObject);
                break;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        string objectName = other.gameObject.name;
        bool matched = false;
        if (other.gameObject.name == plateObj)
        {
            GameObject brokePlate = other.gameObject;

            if (brokePlate != null && matched)
            {
                Destroy(brokePlate);
                Debug.Log("🛠️ 'Broke Plate' has been destroyed.");
            }
        }

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
            DestroyObjects(); // Destroy objects inside the trigger
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
