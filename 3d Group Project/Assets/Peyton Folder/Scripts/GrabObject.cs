using UnityEngine;

public class GrabObject : MonoBehaviour
{
    private Rigidbody objectRigidBody;
    private Transform objectGrabPointTransform;
    private int originalLayer;
    private int pickedUpLayer;

    // Store the layer indices for Player and GrabPoint
    private int playerLayer;
    private int grabPointLayer;

    // Lerp speed
    public float lerpSpeed = 10f;

    private void Awake()
    {
        objectRigidBody = GetComponent<Rigidbody>();
        originalLayer = gameObject.layer; // Store the original layer
        pickedUpLayer = LayerMask.NameToLayer("IsPickedUp"); // Get the IsPickedUp layer index

        // Get the Player and GrabPoint layer indices
        playerLayer = LayerMask.NameToLayer("Player");
        grabPointLayer = LayerMask.NameToLayer("GrabPoint");
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidBody.useGravity = false; // Disable gravity when grabbed
        gameObject.layer = pickedUpLayer; // Change to IsPickedUp layer

        // Ignore collisions with the Player and GrabPoint layers
        Physics.IgnoreLayerCollision(pickedUpLayer, playerLayer, true);
        Physics.IgnoreLayerCollision(pickedUpLayer, grabPointLayer, true);
    }

    public void Drop(Vector3 dropPoint) // Accept dropPoint parameter
    {
        this.objectGrabPointTransform = null;
        objectRigidBody.useGravity = true; // Re-enable gravity when dropped
        gameObject.layer = originalLayer; // Revert to original layer

        // Re-enable collisions with the Player and GrabPoint layers
        Physics.IgnoreLayerCollision(pickedUpLayer, playerLayer, false);
        Physics.IgnoreLayerCollision(pickedUpLayer, grabPointLayer, false);

        // Move the object to the drop point
        objectRigidBody.linearVelocity = (dropPoint - transform.position) * 5f; // Throw the object based on velocity
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // Smoothly move the object towards the grab point using Lerp
            Vector3 targetPosition = objectGrabPointTransform.position;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            objectRigidBody.MovePosition(newPosition); // Use MovePosition to update position using Rigidbody physics
        }
    }
}
