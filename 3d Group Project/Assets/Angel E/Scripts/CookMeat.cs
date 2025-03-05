using UnityEngine;

public class CookMeat : MonoBehaviour
{
    [SerializeField] private GameObject cookedM; // Cooked burger prefab
    [SerializeField] private Vector3 spawnScale = Vector3.one;
    [SerializeField] private float cookTime = 5f; // Time in seconds to cook
    [SerializeField] private AudioSource sizzlingSound; // Sizzle sound (optional)
    [SerializeField] private ParticleSystem smokeEffect; // Smoke particle system
    [SerializeField] private float destroyTime = 10f; // Time before raw meat is destroyed after cooking starts

    private bool isCooking = false;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private float destroyTimer = 0f;
    private PlaySoundOnCollision3D stoveSoundScript; // Reference to the sound script

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
            destroyTimer += Time.deltaTime; // Track time for destruction
            Debug.Log($"Cooking... Time left: {cookTime:F2}");

            if (cookTime <= 0)
            {
                Cook();
            }

            if (destroyTimer >= destroyTime) // Destroy object after certain time
            {
                Destroy(gameObject); // Destroys the object
                Debug.Log("Raw meat destroyed due to time limit.");
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Stovetop"))
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

            // Find the sound script on the stovetop
            stoveSoundScript = other.gameObject.GetComponent<PlaySoundOnCollision3D>();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Stovetop"))
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

        // Reset stove sound script to prevent lingering sound issues
        if (stoveSoundScript != null)
        {
            stoveSoundScript.ResetCollision();
        }

        Destroy(gameObject); // Destroy raw burger after spawning cooked burger
    }
}
