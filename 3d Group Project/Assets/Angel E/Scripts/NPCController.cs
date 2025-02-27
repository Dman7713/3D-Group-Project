using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public Transform startPoint; // The spawn location
    public List<TaskLocation> taskLocations; // List of destinations (includes look-at objects)
    [SerializeField] public float despawnTime = 5f; // Countdown duration

    private NavMeshAgent agent;
    private TaskLocation targetTask;
    private static HashSet<Transform> occupiedLocations = new HashSet<Transform>();
    private bool isSatisfied = false;
    private bool taskStarted = false;

    [SerializeField] private List<string> expectedNames = new List<string>(); // Expected hierarchy names

    [System.Serializable]
    public class TaskLocation
    {
        public Transform location; // Where the NPC moves
        public Transform lookAtTarget; // What the NPC should face
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Choose a random unoccupied location
        targetTask = GetAvailableLocation();
        if (targetTask != null)
        {
            occupiedLocations.Add(targetTask.location);
            agent.SetDestination(targetTask.location.position);
        }
        else
        {
            Debug.LogError("No available task locations!");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Check if the NPC has reached the destination and hasn't started the task yet
        if (!taskStarted && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            taskStarted = true;
            agent.isStopped = true; // Stop movement

            // Make the NPC face the correct direction
            if (targetTask.lookAtTarget != null)
            {
                Vector3 direction = (targetTask.lookAtTarget.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = lookRotation;
            }

            StartCoroutine(CompleteTaskAndDespawn());
        }
    }

    private TaskLocation GetAvailableLocation()
    {
        List<TaskLocation> availableLocations = new List<TaskLocation>();

        foreach (TaskLocation task in taskLocations)
        {
            if (!occupiedLocations.Contains(task.location))
            {
                availableLocations.Add(task);
            }
        }

        if (availableLocations.Count > 0)
        {
            return availableLocations[Random.Range(0, availableLocations.Count)];
        }
        return null;
    }

    private IEnumerator CompleteTaskAndDespawn()
    {
        float timeRemaining = despawnTime;

        Debug.Log("NPC has arrived at " + targetTask.location.name + ". Starting task countdown.");

        while (!isSatisfied && timeRemaining > 0)
        {
            Debug.Log("Time Remaining: " + Mathf.Ceil(timeRemaining));
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        Debug.Log("Task completed at " + targetTask.location.name + "! NPC despawning...");

        // Free up the location
        occupiedLocations.Remove(targetTask.location);

        // Despawn NPC
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Plate")
        {
            if (other.gameObject == null) return;

            List<string> hierarchyNames = new List<string>();
            CollectNames(other.gameObject, hierarchyNames);

            if (CheckNameMatch(hierarchyNames, expectedNames))
            {
                isSatisfied = true;
                Debug.Log($"Object {other.gameObject.name}: Hierarchy matches expected names!");
            }
            else
            {
                Debug.Log($"Object {other.gameObject.name}: Hierarchy does NOT match expected names.");
            }
        }
    }

    void CollectNames(GameObject obj, List<string> nameList)
    {
        while (obj != null)
        {
            if (obj.name != "Smoke") // Ignore "Smoke"
            {
                nameList.Add(obj.name);
            }
            obj = GetValidChild(obj);
        }
    }

    GameObject GetValidChild(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.name != "Smoke")
            {
                return child.gameObject;
            }
        }
        return null;
    }

    bool CheckNameMatch(List<string> hierarchy, List<string> expected)
    {
        if (hierarchy.Count != expected.Count) return false;

        for (int i = 0; i < hierarchy.Count; i++)
        {
            if (hierarchy[i] != expected[i])
            {
                return false;
            }
        }
        return true;
    }
}
