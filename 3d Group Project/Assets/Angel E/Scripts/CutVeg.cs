using UnityEngine;

public class CutVeg : MonoBehaviour
{
    [SerializeField]
    GameObject rawV;
    [SerializeField]
    GameObject cutV;
    Vector3 spawnPos;
    [SerializeField]
    Vector3 spawnDiff;
    Quaternion spawnRot;
    [SerializeField]
    Vector3 spawnScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cutV.transform.localScale = spawnScale;
    }

    // Update is called once per frame
    void Update()
    {
        spawnPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Knife")
        {
            SpawnVeg();
            Destroy(rawV);
        }
    }

    private void SpawnVeg()
    {
        Instantiate(cutV, spawnPos, spawnRot);
        Instantiate(cutV, spawnPos + spawnDiff, spawnRot);
        Instantiate(cutV, spawnPos + spawnDiff, spawnRot);
        Instantiate(cutV, spawnPos + spawnDiff, spawnRot);
    }
}
