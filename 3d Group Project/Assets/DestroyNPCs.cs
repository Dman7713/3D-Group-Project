using UnityEngine;

public class DestroyNPCs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NPC")
        {
                Destroy(other);
                Debug.Log("NPC Destroyed");
        }
    }
}
