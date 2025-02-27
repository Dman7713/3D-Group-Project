using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab; // Reference to the NPC prefab
    public Transform spawnPoint; // The point where the NPC will spawn
    public List<NPCController.TaskLocation> taskLocations; // List of possible locations and targets for the NPCs
    public float spawnInterval = 5f; // Interval between NPC spawns

    private void Start()
    {
        // Start spawning NPCs at random intervals
        StartCoroutine(SpawnNPCs());
    }

    private IEnumerator SpawnNPCs()
    {
        while (true) // Infinite loop for continuous NPC spawning
        {
            // Wait for the spawn interval before spawning a new NPC
            yield return new WaitForSeconds(spawnInterval);

            // Instantiate the NPC at the spawn point
            GameObject npc = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

            // Get the NPCController component from the newly instantiated NPC
            NPCController npcController = npc.GetComponent<NPCController>();

            // Assign the task locations to the NPCController
            npcController.taskLocations = taskLocations;

            // You can set other properties of the NPCController here, like despawn time or initial state
        }
    }
}
