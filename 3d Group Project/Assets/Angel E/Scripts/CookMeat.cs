using UnityEngine;

public class CookMeat : MonoBehaviour
{
    [SerializeField] private GameObject cookedM; // Cooked burger prefab
    [SerializeField] private Vector3 spawnScale = Vector3.one;
    [SerializeField] private float cookTime = 5f; // Time in seconds to cook
    [SerializeField] private AudioSource sizzlingSound; // Sizzle sound (optional)
    [SerializeField] private ParticleSystem smokeEffect; // Smoke particle system

    private bool isCooking = false;
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    void Start()
    {
        // Store the initial position and rotation when the raw burger is created
        spawnPos = transform.position;
        spawnRot = transform.rotation;

        if (smokeEffect != null)
        {
            smokeEffect.Stop(); // Ensure smoke is off at the start
        }
    }

    void Update()
    {
        if (isCooking)
        {
            cookTime -= Time.deltaTime;
            Debug.Log($"Cooking... Time left: {cookTime:F2}");

            if (cookTime <= 0)
            {
                Cook();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stovetop"))
        {
            isCooking = true;

            if (sizzlingSound != null)
            {
                sizzlingSound.Play();
            }

            if (smokeEffect != null)
            {
                smokeEffect.Play(); // Start smoke when cooking begins
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stovetop"))
        {
            isCooking = false;

            if (sizzlingSound != null)
            {
                sizzlingSound.Stop();
            }

            if (smokeEffect != null)
            {
                smokeEffect.Stop(); // Stop smoke when removed from stovetop
            }
        }
    }

    private void Cook()
    {
        // Ensure the cooked burger spawns at the same position and rotation
        GameObject newMeat = Instantiate(cookedM, transform.position, transform.rotation);
        newMeat.transform.localScale = spawnScale;
        Debug.Log("Meat is cooked!");

        if (smokeEffect != null)
        {
            smokeEffect.Stop(); // Stop smoke when meat is fully cooked
        }

        Destroy(gameObject); // Destroy raw burger after spawning cooked burger
    }
}
