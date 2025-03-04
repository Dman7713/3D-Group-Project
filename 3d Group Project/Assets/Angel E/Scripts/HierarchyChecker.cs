using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TriggerSetChecker : MonoBehaviour
{
    [SerializeField] private List<List<string>> possibleLists = new List<List<string>>(); // Multiple lists of expected objects
    private List<string> activeList = new List<string>(); // The chosen list of expected objects
    private HashSet<string> objectsInTrigger = new HashSet<string>(); // Track objects inside the trigger
    public bool isSatisfied { get; private set; } = false; // True when all expected objects are in

    private void Start()
    {
        // Select a random list from possibleLists
        if (possibleLists.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleLists.Count);
            activeList = new List<string>(possibleLists[randomIndex]);
            Debug.Log($"Selected list {randomIndex + 1}: [{string.Join(", ", activeList)}]");
        }
        else
        {
            Debug.LogWarning("No lists available! Ensure possibleLists has entries.");
        }
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
