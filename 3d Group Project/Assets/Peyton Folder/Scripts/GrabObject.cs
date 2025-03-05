using UnityEngine;

public class GrabObject : MonoBehaviour
{
    private Rigidbody objectRigidBody;
    private Transform objectGrabPointTransform;
    private int originalLayer;
    private int pickedUpLayer;

    // Store the layer indices for Player, GrabPoint, and Wall
    private int playerLayer;
    private int grabPointLayer;
    private int wallLayer;

    // Lerp speed and damping
    public float lerpSpeed = 10f;
    public float heldDamping = 5f; // Damping when held

    private Vector3 lastPosition;
    private Vector3 objectVelocity;

    // Reference to the HoldPoint object (will be found dynamically)
    private Transform holdPointTransform;
    private AudioSource audioSource;

    private void Awake()
    {
        objectRigidBody = GetComponent<Rigidbody>();
        originalLayer = gameObject.layer; // Store the original layer
        pickedUpLayer = LayerMask.NameToLayer("IsPickedUp"); // Get the IsPickedUp layer index

        // Get the Player, GrabPoint, and Wall layer indices
        playerLayer = LayerMask.NameToLayer("Player");
        grabPointLayer = LayerMask.NameToLayer("GrabPoint");
        wallLayer = LayerMask.NameToLayer("Wall");

        // Find the HoldPoint object dynamically in the hierarchy
        holdPointTransform = GameObject.Find("HoldPoint")?.transform;

        // Check if HoldPoint was found and assign the AudioSource
        if (holdPointTransform != null)
        {
            audioSource = holdPointTransform.GetComponent<AudioSource>(); // Get the AudioSource from HoldPoint
        }
        else
        {
            Debug.LogWarning("HoldPoint object not found in the scene!");
        }
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidBody.useGravity = false; // Disable gravity when grabbed
        objectRigidBody.isKinematic = true; // Enable kinematic mode when held
        gameObject.layer = pickedUpLayer; // Change to IsPickedUp layer

        // Ignore collisions with the Player and GrabPoint layers, but not the Wall layer
        Physics.IgnoreLayerCollision(pickedUpLayer, playerLayer, true);
        Physics.IgnoreLayerCollision(pickedUpLayer, grabPointLayer, true);
        Physics.IgnoreLayerCollision(pickedUpLayer, wallLayer, false); // Ensure collision with walls remains

        lastPosition = transform.position; // Initialize last position for velocity tracking

        // Play the grab sound from the HoldPoint's AudioSource if it exists
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidBody.useGravity = true; // Re-enable gravity when dropped
        objectRigidBody.isKinematic = false; // Disable kinematic mode when dropped
        gameObject.layer = originalLayer; // Revert to original layer

        // Re-enable collisions with the Player and GrabPoint layers
        Physics.IgnoreLayerCollision(pickedUpLayer, playerLayer, false);
        Physics.IgnoreLayerCollision(pickedUpLayer, grabPointLayer, false);

        // Apply stored velocity from swinging motion but dampened for balance
        objectRigidBody.linearVelocity = objectVelocity * 0.75f; // Apply reduced velocity for a natural throw effect
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // Smoothly move the object towards the grab point using Lerp
            Vector3 targetPosition = objectGrabPointTransform.position;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            objectRigidBody.MovePosition(newPosition); // Use MovePosition to update position using Rigidbody physics

            // Calculate velocity for throw physics
            objectVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
            lastPosition = transform.position;
        }
    }
}
