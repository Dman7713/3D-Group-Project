using UnityEngine;

public class PlayerPickAndDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform; // Grab point is the target drop point
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask; // Detects obstacles like walls

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
            DropItem();
        }

        // If an object is grabbed, continue to face the player even if the camera moves
        if (grabbedObject != null)
        {
            AlignObjectToPlayer(grabbedObject.transform);
        }
    }

    private void DropItem()
    {
        // Cast a ray from the camera towards the grab point to check for obstacles
        RaycastHit hit;
        Vector3 directionToDropPoint = objectGrabPointTransform.position - playerCameraTransform.position;
        if (Physics.Raycast(playerCameraTransform.position, directionToDropPoint, out hit, directionToDropPoint.magnitude, obstacleLayerMask))
        {
            // Drop at the hit point if the raycast hits an obstacle (like a wall)
            grabbedObject.Drop(hit.point);
        }
        else
        {
            // Otherwise, drop at the grab point (if no obstacles in the way)
            grabbedObject.Drop(objectGrabPointTransform.position);
        }

        grabbedObject = null;
    }

    private void AlignObjectToPlayer(Transform objectTransform)
    {
        // Check if the object has the "Item" tag
        if (objectTransform.CompareTag("Item"))
        {
            // Continuously make the object's forward direction face away from the player
            objectTransform.forward = -playerCameraTransform.forward;

            // OPTIONAL: If the object has a specific handle direction, adjust it
            // Adjust rotation depending on the model (e.g., handle always points up or to the side)
            objectTransform.Rotate(Vector3.right, -60f); // This may need adjustment based on your model
        }
        if (objectTransform.CompareTag("Knife"))
        {
            // Continuously make the object's forward direction face away from the player
            objectTransform.forward = -playerCameraTransform.forward;

            // OPTIONAL: If the object has a specific handle direction, adjust it
            // Adjust rotation depending on the model (e.g., handle always points up or to the side)
            objectTransform.Rotate(Vector3.right, -50f); // This may need adjustment based on your model
            objectTransform.Rotate(Vector3.down, 10f);
        }
    }
}
