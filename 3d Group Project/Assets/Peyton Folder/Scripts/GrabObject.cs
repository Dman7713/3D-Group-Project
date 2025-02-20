using UnityEngine;

public class GrabObject : MonoBehaviour
{
    private Rigidbody objectRigidBody;
    private Transform objectGrabPointTransform;
    [SerializeField] float lerpSpeed = 10f;
    private int originalLayer;
    private int pickedUpLayer;

    private void Awake()
    {
        objectRigidBody = GetComponent<Rigidbody>();
        originalLayer = gameObject.layer; // Store the original layer
        pickedUpLayer = LayerMask.NameToLayer("IsPickedUp"); // Get the IsPickedUp layer index
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidBody.useGravity = false;
        objectRigidBody.isKinematic = true; // Make the object kinematic when grabbed
        gameObject.layer = pickedUpLayer; // Change to IsPickedUp layer
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidBody.useGravity = true;
        objectRigidBody.isKinematic = false; // Make the object dynamic again when dropped
        gameObject.layer = originalLayer; // Revert to original layer
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidBody.MovePosition(newPosition);
        }
    }
}
