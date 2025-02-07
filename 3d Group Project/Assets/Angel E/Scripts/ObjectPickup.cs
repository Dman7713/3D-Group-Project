using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public Transform holdPoint; // Position where the object will be held
    public float pickupRange = 2f;
    public float throwForce = 10f;
    public float rotationSpeed = 5f;
    public CameraController cameraController; // Reference to CameraController (set in Inspector)

    private Rigidbody2D heldObject;
    private bool isRotating = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickup();
            else
                DropObject();
        }

        if (Input.GetMouseButtonDown(1) && heldObject != null) // Right-click to throw
        {
            ThrowObject();
        }

        if (Input.GetKey(KeyCode.R) && heldObject != null) // Hold R to rotate
        {
            isRotating = true;
            cameraController?.DisableCamera(); // Stop camera movement
            RotateObject();
        }
        else if (Input.GetKeyUp(KeyCode.R)) // Release R to stop rotating
        {
            isRotating = false;
            cameraController?.EnableCamera(); // Re-enable camera movement
        }
    }

    void TryPickup()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, pickupRange);
        if (hit.collider != null && hit.collider.CompareTag("canPickUp"))
        {
            heldObject = hit.collider.GetComponent<Rigidbody2D>();
            if (heldObject != null)
            {
                heldObject.bodyType = RigidbodyType2D.Kinematic; // Disable physics while holding
                heldObject.Sleep(); // Stops all movement and rotation
                heldObject.transform.position = holdPoint.position;
                heldObject.transform.parent = holdPoint;
            }
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.bodyType = RigidbodyType2D.Dynamic; // Re-enable physics
            heldObject.WakeUp(); // Reactivates physics interactions
            heldObject.transform.parent = null;
            heldObject = null;
        }
    }

    void ThrowObject()
    {
        if (heldObject != null)
        {
            Rigidbody2D rb = heldObject;
            DropObject();
            rb.AddForce(transform.right * throwForce, ForceMode2D.Impulse); // Apply force
        }
    }

    void RotateObject()
    {
        float rotationInput = Input.GetAxis("Mouse X") * rotationSpeed;
        heldObject.transform.Rotate(Vector3.forward, -rotationInput);
    }
}
