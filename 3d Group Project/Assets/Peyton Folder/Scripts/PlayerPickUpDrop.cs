using UnityEngine;

public class PlayerPickAndDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
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
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && grabbedObject != null) // Release left mouse button to drop
        {
            grabbedObject.Drop();
            grabbedObject = null;
        }
    }
}
