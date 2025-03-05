using UnityEngine;

public class FireExtinguisherParticle : MonoBehaviour
{
    public float lifetime = 2f; // Time before it disappears

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy after a few seconds
    }
}
