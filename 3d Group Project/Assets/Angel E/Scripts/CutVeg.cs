using UnityEngine;

public class CutVeg : MonoBehaviour
{
    [SerializeField]
    GameObject cutV;
    [SerializeField]
    GameObject burntV; // Variable for the burnt version of the object
    Vector3 spawnPos;
    [SerializeField]
    Vector3 spawnDiff;
    Quaternion spawnRot;
    [SerializeField]
    Vector3 spawnScale;
    [SerializeField]
    int spawnCount = 1;
    [SerializeField]
    int cutCount = 1;
    [SerializeField]
    Vector3 scaleDecrease;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cutV.transform.localScale = spawnScale;
        spawnRot = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        spawnPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Knife")
        {
            cutCount -= 1;
            transform.localScale -= scaleDecrease;

            // Check if the object has the "burnt" tag
            if (CompareTag("Burnt"))
            {
                // Spawn the burnt version of the object
                if (spawnCount <= 1)
                {
                    Instantiate(burntV, spawnPos, spawnRot); // Spawn burnt object
                    Destroy(gameObject);
                }
                else
                {
                    Instantiate(burntV, spawnPos + spawnDiff, spawnRot); // Spawn burnt object in a different position
                    spawnCount -= 1;
                }
            }
            else
            {
                // Original cut behavior for objects without the "burnt" tag
                if (cutCount >= 0)
                {
                    if (spawnCount <= 1)
                    {
                        Instantiate(cutV, spawnPos, spawnRot);
                        Destroy(gameObject);
                    }
                    else
                    {
                        Instantiate(cutV, spawnPos + spawnDiff, spawnRot);
                        spawnCount -= 1;
                    }
                }
            }
        }
    }
}
