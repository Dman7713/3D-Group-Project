using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField]
    private Transform playerCameraTransform;
    [SerializeField]
    private Transform objectGrabPointTranform;
    [SerializeField]
    private float pickUpDistance = 2f;
    [SerializeField]
    private LayerMask pickUpLayerMask;

    private ObjectGrabbable objectGrabbable;

    // Update is called once per frame
    private void Update()
     {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectGrabbable == null)
            {
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTranform);
                    }
                }
            }
        }
        else
        {
            objectGrabbable.Drop();
            objectGrabbable = null;
        }
    }
}
