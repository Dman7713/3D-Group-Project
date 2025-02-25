using UnityEngine;
using System.Collections.Generic;

public class HierarchyChecker : MonoBehaviour
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
            nameList.Add(obj.name);
            if (obj.transform.childCount > 0)
            {
                obj = obj.transform.GetChild(0).gameObject;
            }
            else
            {
                obj = null;
            }
        }
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
