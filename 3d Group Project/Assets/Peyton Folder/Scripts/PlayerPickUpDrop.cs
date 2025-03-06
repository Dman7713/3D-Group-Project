using UnityEngine;

public class PlayerPickAndDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform; // Target position for the grabbed object
    [SerializeField] private LayerMask pickUpLayerMask;

    private GrabObject grabbedObject;

    private void Update()
    {
        if (Input.GetMouseButton(0)) // Hold left mouse button to pick up
        {
            if (grabbedObject == null)
            {
                float pickUpDistance = 2f;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, pickUpDistance))
                {
                    // Check if the first object hit is on the pickUpLayerMask
                    if ((pickUpLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
                    {
                        if (hit.transform.TryGetComponent(out GrabObject objectToGrab))
                        {
                            // Ensure there's no object blocking the item
                            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit blockHit, pickUpDistance, pickUpLayerMask))
                            {
                                // If we hit something that's not the object we're grabbing, block the grab
                                if (blockHit.transform != hit.transform)
                                {
                                    return; // Block the pick up
                                }
                            }

                            grabbedObject = objectToGrab;
                            grabbedObject.Grab(objectGrabPointTransform);

                            // Move object to the hold position
                            grabbedObject.transform.position = objectGrabPointTransform.position;

                            // Align object to face the player when it's grabbed
                            AlignObjectToPlayer(grabbedObject.transform);
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && grabbedObject != null) // Release left mouse button to drop
        {
            grabbedObject.Drop(); // Drop the object normally
            grabbedObject = null;
        }

        // If an object is grabbed, continue to face the player even if the camera moves
        if (grabbedObject != null)
        {
            AlignObjectToPlayer(grabbedObject.transform);
        }
    }

    private void AlignObjectToPlayer(Transform objectTransform)
    {
        if (objectTransform.CompareTag("Item") || objectTransform.CompareTag("Knife"))
        {
            objectTransform.forward = -playerCameraTransform.forward;

            if (objectTransform.CompareTag("Item"))
            {
                objectTransform.Rotate(Vector3.right, -60f);
            }
            else if (objectTransform.CompareTag("Knife"))
            {
                objectTransform.Rotate(Vector3.right, -50f);
                objectTransform.Rotate(Vector3.down, 10f);
            }
        }
    }
}
