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
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out GrabObject objectToGrab))
                    {
                        grabbedObject = objectToGrab;
                        grabbedObject.Grab(objectGrabPointTransform);

                        // Align object to face the player when it's grabbed
                        AlignObjectToPlayer(grabbedObject.transform);
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
