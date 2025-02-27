using UnityEngine;
using System.Collections.Generic;

public class HierarchyTriggerChecker : MonoBehaviour
{
    [SerializeField] private List<string> expectedNames = new List<string>(); // Expected hierarchy names

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Plate")
        {
            if (other.gameObject == null) return;

            List<string> hierarchyNames = new List<string>();
            CollectNames(other.gameObject, hierarchyNames);

            if (CheckNameMatch(hierarchyNames, expectedNames))
            {
                Debug.Log($"Object {other.gameObject.name}: Hierarchy matches expected names!");
                Debug.Log("Hierarchy: " + hierarchyNames + "Expected: " + expectedNames);
            }
            else
            {
                Debug.Log($"Object {other.gameObject.name}: Hierarchy does NOT match expected names.");
                Debug.Log("Hierarchy: " + hierarchyNames + "Expected: " + expectedNames);
            }
        }
    }

    void CollectNames(GameObject obj, List<string> nameList)
    {
        while (obj != null)
        {
            if (obj.name != "Smoke") // Skip objects named "Smoke"
            {
                nameList.Add(obj.name);
            }

            // Find the next child that is NOT named "Smoke"
            obj = GetValidChild(obj);
        }
    }

    GameObject GetValidChild(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.name != "Smoke") // Ignore "Smoke"
            {
                return child.gameObject;
            }
        }
        return null; // No valid child found
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
