using UnityEngine;

public class CookMeat : MonoBehaviour
{
    /// <summary>
    /// [SerializeField]
    /// </summary>
    ///AudioSource sounds;
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
        ///cookedM.transform.localScale = spawnScale;
    }

    // Update is called once per frame
    void Update()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Stovetop")
        {
            cookTime -= Time.deltaTime;
            Debug.Log(cookTime);
            if (cookTime == 0 || cookTime < 0)
            {
                ///sounds.mute = true;
                Instantiate(cookedM, spawnPos, spawnRot);
                cookedM.transform.localScale = spawnScale;
                Destroy(gameObject);
            }

        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stovetop")
        {
            sounds.Play();
            sounds.mute = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
            sounds.mute = true;
    }
    */
}
