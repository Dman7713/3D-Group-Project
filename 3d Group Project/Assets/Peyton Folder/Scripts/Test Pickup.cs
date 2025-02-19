using UnityEngine;

public class PickupDropSystem3D : MonoBehaviour
{
    public Transform holdPosition; // Where the object will be held
    public float pickupRange = 3f; // Distance to detect objects
    public LayerMask pickupLayer;  // Assign this to a "Pickup" layer

    private GameObject heldObject;
    private Rigidbody heldObjectRb;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button down (pick up)
        {
            if (heldObject == null)
            {
                TryPickup();
            }
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button up (drop)
        {
            if (heldObject != null)
            {
                DropItem();
            }
        }

        if (heldObject != null)
        {
            // If the object is held, update its position to follow the hold position
            heldObject.transform.position = holdPosition.position;
        }
    }

    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, pickupLayer))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldObject = hit.collider.gameObject;
                heldObjectRb = heldObject.GetComponent<Rigidbody>();

                // Disable physics
                heldObjectRb.isKinematic = true;
                heldObjectRb.useGravity = false;

                // Attach object to hold position
                heldObject.transform.position = holdPosition.position;
                heldObject.transform.SetParent(holdPosition);
            }
        }
    }

    void DropItem()
    {
        // Enable physics
        heldObject.transform.SetParent(null);
        heldObjectRb.isKinematic = false;
        heldObjectRb.useGravity = true;
        heldObject = null;
        heldObjectRb = null;
    }
}
