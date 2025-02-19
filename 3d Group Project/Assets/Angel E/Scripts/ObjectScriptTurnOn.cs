using UnityEngine;

public class ObjectScriptTurnOn : MonoBehaviour
{
    PlatingObjects platingobjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // disable the plating objects script
        GetComponent<PlatingObjects>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.name == "Plate")
        {
            GetComponent<PlatingObjects>().enabled = true;
        }
        if (transform.parent == null)
        {
            GetComponent<PlatingObjects>().enabled = false;
        }
    }
}

