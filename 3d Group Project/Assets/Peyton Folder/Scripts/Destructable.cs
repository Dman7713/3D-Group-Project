using UnityEngine;

public class PlateShatter : MonoBehaviour
{
    public GameObject shatteredPlatePrefab; // Prefab of the broken plate pieces
    public float floorShatterThreshold = 5f; // Minimum velocity to shatter on floor
    public float wallShatterThreshold = 7f; // Minimum velocity to shatter on walls

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Get the object's layer and tag
        int objectLayer = collision.gameObject.layer;
        string objectTag = collision.gameObject.tag;

        float impactForce = rb.linearVelocity.magnitude; // Measure impact force

        // Only allow shattering from Floor layer or Wall tag
        if (objectLayer == LayerMask.NameToLayer("Floor"))
        {
            if (impactForce > floorShatterThreshold)
            {
                Shatter();
            }
        }
        else if (objectTag == "Wall")
        {
            if (impactForce > wallShatterThreshold)
            {
                Shatter();
            }
        }
    }

    void Shatter()
    {
        Instantiate(shatteredPlatePrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
