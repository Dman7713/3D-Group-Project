using UnityEngine;
using System.Collections.Generic;

public class GrabPlate : MonoBehaviour
{
    private Rigidbody objectRigidBody;
    private Transform objectGrabPointTransform;
    private int originalLayer;
    private int pickedUpLayer;
    private List<Transform> grabbedFood = new List<Transform>();

    public float lerpSpeed = 10f;
    public Transform holdPoint; // Where the plate will be held
    public float foodDetectionRadius = 0.5f;

    private bool isHolding = false;

    private void Awake()
    {
        objectRigidBody = GetComponent<Rigidbody>();
        originalLayer = gameObject.layer;
        pickedUpLayer = LayerMask.NameToLayer("IsPickedUp");

        // Find or assign the hold point
        if (holdPoint == null)
        {
            holdPoint = GameObject.Find("HoldPoint")?.transform;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click
        {
            if (!isHolding)
            {
                TryPickup();
            }
            else
            {
                Drop();
            }
        }
    }

    private void TryPickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f)) // Adjust distance if needed
        {
            if (hit.collider.gameObject == gameObject)
            {
                Grab(holdPoint);
            }
        }
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        isHolding = true;

        objectRigidBody.useGravity = false;
        objectRigidBody.isKinematic = true;
        gameObject.layer = pickedUpLayer;

        // Parent the food objects to the plate
        DetectAndAttachFood();
    }

    public void Drop()
    {
        isHolding = false;
        objectGrabPointTransform = null;

        objectRigidBody.useGravity = true;
        objectRigidBody.isKinematic = false;
        gameObject.layer = originalLayer;

        // Unparent the food objects
        foreach (var food in grabbedFood)
        {
            if (food != null)
            {
                food.SetParent(null);
            }
        }
        grabbedFood.Clear();
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, objectGrabPointTransform.position, lerpSpeed * Time.deltaTime);

            // Update the position of food objects relative to the plate
            foreach (var food in grabbedFood)
            {
                if (food != null)
                {
                    food.position = transform.position; // Keep food on the plate
                }
            }
        }
    }

    // Detect and parent food objects to the plate
    private void DetectAndAttachFood()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, foodDetectionRadius);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Food")) // Ensure food has the "Food" tag
            {
                Transform foodTransform = col.transform;
                if (foodTransform != null)
                {
                    foodTransform.SetParent(transform); // Parent the food to the plate
                    grabbedFood.Add(foodTransform);
                }
            }
        }
    }

    // Trigger for detecting food
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Transform foodTransform = other.transform;
            if (foodTransform != null)
            {
                foodTransform.SetParent(transform); // Parent the food to the plate
                grabbedFood.Add(foodTransform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Transform foodTransform = other.transform;
            if (foodTransform != null)
            {
                foodTransform.SetParent(null); // Unparent the food
                grabbedFood.Remove(foodTransform);
            }
        }
    }
}
