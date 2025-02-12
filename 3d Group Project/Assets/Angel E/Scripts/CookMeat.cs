using UnityEngine;

public class CookMeat : MonoBehaviour
{
    [SerializeField]
    GameObject cookedM;
    [SerializeField]
    Vector3 spawnScale;
    [SerializeField]
    float cookTime;
    Vector3 spawnPos;
    Quaternion spawnRot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cookedM.transform.localScale = spawnScale;
    }

    // Update is called once per frame
    void Update()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Stovetop")
        {
            cookTime -= Time.deltaTime;
            Debug.Log(cookTime);
            if (cookTime <= 0)
            {
                Instantiate(cookedM, spawnPos, spawnRot);
                Destroy(gameObject);
            }
        }
    }
}
