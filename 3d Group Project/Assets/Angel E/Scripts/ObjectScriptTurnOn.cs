using UnityEngine;

public class ObjectScriptTurnOn : MonoBehaviour
{
    private PlatingObjects platingObjects;
    [SerializeField]
    private string targetLayerName = "PickUP";

    void Start()
    {
        platingObjects = GetComponent<PlatingObjects>();

        // Disable the PlatingObjects script on start, excluding the object named "Plate"
        if (transform.name != "Plate" && platingObjects != null)
        {
            platingObjects.enabled = false;
        }
    }

    void Update()
    {
        int pickupLayer = LayerMask.NameToLayer(targetLayerName);
        GameObject parent = transform.parent ? transform.parent.gameObject : null;

        // Enable the script if the object is parented to "Plate" or an object with the "Ingredients" or "Pickup" tag and the correct layer
        if (parent != null && (parent.name == "Plate" || parent.CompareTag("Ingredients") || (parent.CompareTag("Pickup") && parent.layer == pickupLayer)))
        {
            if (platingObjects != null)
            {
                platingObjects.enabled = true;
            }
        }
        else if (transform.parent == null && transform.name != "Plate")
        {
            if (platingObjects != null)
            {
                platingObjects.enabled = false;
            }
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        // Check if any child object's name contains "Smoke"
        bool hasSmokeChild = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Smoke"))
            {
                hasSmokeChild = true;
                break;
            }
        }

        // if the scripted object has a child, turn off the plating script (ignore smoke child object)
        if (platingObjects != null && (transform.childCount == 1 && hasSmokeChild != true || transform.childCount == 2 && hasSmokeChild == true))
        {
            platingObjects.enabled = false;
        }
    }
}
