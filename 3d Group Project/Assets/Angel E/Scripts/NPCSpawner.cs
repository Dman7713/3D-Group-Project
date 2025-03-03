using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab; // NPC prefab to spawn
    public Transform startPoint; // NPC spawn position
    public List<NPCController.TaskLocation> taskLocations; // Possible task locations
    public Transform exitPoint; // The final destination before despawning
    public float spawnInterval = 5f; // Time between spawns

    private void Start()
    {
        StartCoroutine(SpawnNPCs());
    }

    private IEnumerator SpawnNPCs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        if (npcPrefab == null || taskLocations.Count == 0 || exitPoint == null)
        {
            Debug.LogError("NPCSpawner is missing references!");
            return;
        }

        GameObject npcInstance = Instantiate(npcPrefab, startPoint.position, Quaternion.identity);
        NPCController npcController = npcInstance.GetComponent<NPCController>();

        if (npcController != null)
        {
            npcController.InitializeNPC(taskLocations, exitPoint);
        }
    }
}
