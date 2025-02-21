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
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && grabbedObject != null) // Release left mouse button to drop
        {
            DropItem();
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
}
