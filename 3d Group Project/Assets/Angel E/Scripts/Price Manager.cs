using System.Collections.Generic;
using UnityEngine;

// This is just a data container, so it doesn't need to inherit from MonoBehaviour
[System.Serializable]
public class GameObjectEntry
{
    public GameObject gameObject;
    public int value;
}

public class GameObjectIntList : MonoBehaviour
{
    // The list that stores GameObjects and their associated integers
    public List<GameObjectEntry> gameObjects = new List<GameObjectEntry>();

    // Method to update the value of a specific GameObject
    public void SetValue(GameObject obj, int newValue)
    {
        foreach (GameObjectEntry entry in gameObjects)
        {
            if (entry.gameObject == obj)
            {
                entry.value = newValue;
                Debug.Log($"Updated {obj.name} to {newValue}");
                return;
            }
        }
        Debug.LogWarning($"{obj.name} not found in the list!");
    }

    // Print all GameObjects and their values
    public void PrintList()
    {
        foreach (var entry in gameObjects)
        {
            Debug.Log($"{entry.gameObject.name}: {entry.value}");
        }
    }
}
