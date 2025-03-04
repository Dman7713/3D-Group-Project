using UnityEngine;

public class FIRE : MonoBehaviour
{
    [SerializeField] private GameObject cookedM; // Cooked burger prefab
    [SerializeField] private Vector3 spawnScale = Vector3.one;
    [SerializeField] private float cookTime = 5f; // Time in seconds to cook
    [SerializeField] private AudioSource sizzlingSound; // Sizzle sound (optional)
    [SerializeField] private ParticleSystem smokeEffect; // Smoke particle system
    [SerializeField] private float destroyTime = 10f; // Time before raw meat is destroyed after cooking starts

    private AudioSource fireAlarmAudioSource; // To store the FireAlarm's AudioSource

    private bool isCooking = false;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private float destroyTimer = 0f;
    private bool fireExtinguisherUsed = false; // Flag to check if fire extinguisher touched the object

    void Start()
    {
        // Find the FireAlarm object in the hierarchy and get its AudioSource
        GameObject fireAlarm = GameObject.Find("FireAlarm");
        if (fireAlarm != null)
        {
            fireAlarmAudioSource = fireAlarm.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("FireAlarm object not found in the hierarchy.");
        }

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
        if (isCooking && !fireExtinguisherUsed) // Only update if it's still cooking and not extinguished
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

        // Check if smokeEffect is playing and trigger external sound if it is
        if (smokeEffect != null && smokeEffect.isPlaying && fireAlarmAudioSource != null)
        {
            if (!fireAlarmAudioSource.isPlaying) // Play the sound only if it's not already playing
            {
                fireAlarmAudioSource.Play();
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Stovetop") && !isCooking)
        {
            isCooking = true;

            if (sizzlingSound != null)
            {
                sizzlingSound.Play();
            }

            if (smokeEffect != null && !fireExtinguisherUsed) // Start smoke when cooking begins
            {
                smokeEffect.Play();
            }

            // Emit fire particles at the collision point(s)
            EmitFireParticles(other);
        }
        else if (other.gameObject.CompareTag("FireExtinguisher"))
        {
            // If touched by FireExtinguisher, stop the smoke effect and cooking
            fireExtinguisherUsed = true;
            StopCookingAndParticles();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Stovetop"))
        {
            // Don't stop smoke effect until the fire extinguisher is used
            if (!fireExtinguisherUsed && smokeEffect != null)
            {
                smokeEffect.Play(); // Keep the smoke effect going even when it leaves the stovetop
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

    // Method to emit fire particles at collision points
    private void EmitFireParticles(Collision collision)
    {
        if (smokeEffect != null)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // Emit fire particles at the contact point
                var emission = smokeEffect.emission;
                emission.enabled = true; // Ensure emission is enabled

                // Position the particle system at the contact point
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = contact.point; // The contact point of the collision
                emitParams.velocity = contact.normal * 2f; // Apply a force in the collision normal direction

                smokeEffect.Emit(emitParams, 1); // Emit one particle at the contact point
            }
        }
    }

    // Method to stop cooking and particle effects when fire extinguisher is used
    private void StopCookingAndParticles()
    {
        isCooking = false;

        if (sizzlingSound != null)
        {
            sizzlingSound.Stop();
        }

        if (smokeEffect != null)
        {
            smokeEffect.Stop(); // Stop smoke when the fire extinguisher is used
        }

        Debug.Log("Fire extinguished!");
    }
}
