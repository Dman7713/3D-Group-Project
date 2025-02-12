using UnityEngine;

public class CutVeg : MonoBehaviour
{
    [SerializeField]
    GameObject cutV;
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
            cutCount -= 1;
            transform.localScale -= scaleDecrease;
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
