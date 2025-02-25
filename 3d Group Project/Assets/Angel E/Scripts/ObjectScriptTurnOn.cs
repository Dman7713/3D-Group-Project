using Unity.VisualScripting;
using UnityEngine;

public class ObjectScriptTurnOn : MonoBehaviour
{
    PlatingObjects platingobjects;
    [SerializeField]
    public string targetLayerName = "PickUP";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // disable the plating objects script on start, excludes the plate
        if (transform.name != "Plate")
        {
            GetComponent<PlatingObjects>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int PickupLayer = LayerMask.NameToLayer(targetLayerName);
        GameObject parent = transform.parent.gameObject;
        // once an ingredient is child to the plate or an ingredient enable the scriptS
        if (transform.parent.name == "Plate" || transform.parent.tag == "Ingredients" || transform.parent.tag == "Pickup" && parent.layer == PickupLayer)
        {
            GetComponent<PlatingObjects>().enabled = true;
        }
        if (transform.parent == null && transform.name != "Plate")
        {
            GetComponent<PlatingObjects>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
        }

        if (transform.childCount == 1)
        {
            GetComponent<PlatingObjects>().enabled = false;
        }
    }
}

