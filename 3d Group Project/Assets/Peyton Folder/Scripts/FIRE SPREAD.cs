using UnityEngine;

public class FireSpreader : MonoBehaviour
{
    public GameObject firePrefab; // The fire prefab to spawn
    private ParticleSystem fireParticles; // Reference to the particle system
    private bool isOnFire = false;

    void Start()
    {
        // Get the ParticleSystem attached to this GameObject
        fireParticles = GetComponentInChildren<ParticleSystem>();
        if (fireParticles != null)
        {
            // Check if the fire particles are already playing
            isOnFire = fireParticles.isPlaying;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnFire) // Check if this object is on fire
        {
            // Spawn a new fire at the point of contact
            SpawnFireAtTouch(other.transform.position);
        }
    }

    void Update()
    {
        // Continuously check if the fire particles are playing
        if (fireParticles != null && fireParticles.isPlaying)
        {
            isOnFire = true;
        }
        else
        {
            isOnFire = false;
        }
    }

    void SpawnFireAtTouch(Vector3 position)
    {
        // Instantiate a new fire at the touched location
        Instantiate(firePrefab, position, Quaternion.identity);
    }
}
