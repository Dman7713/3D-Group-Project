using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private Vector3 originalWorldScale; 
    private List<GameObject> stackedIngredients = new List<GameObject>(); // List of all stacked objects
    private Transform topIngredient; // The last stacked ingredient
    private float stackHeight = 0.1f; // Adjust this for better spacing

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredients") && !stackedIngredients.Contains(collision.gameObject))
        {
            AttachToPlate(collision.gameObject);
        }
    }

    private void AttachToPlate(GameObject ingredient)
    {
        originalWorldScale = ingredient.transform.lossyScale;
        // Remove Rigidbody to prevent unwanted physics interactions
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Set parent to plate
        ingredient.transform.SetParent(transform);

        // Correctly position the ingredient based on the top object
        Vector3 newPosition = (topIngredient != null)
            ? topIngredient.position + new Vector3(0, GetObjectHeight(topIngredient) / 2 + GetObjectHeight(ingredient.transform) / 2, 0) // Stacking logic
            : transform.position + new Vector3(0, GetObjectHeight(ingredient.transform) / 2, 0); // First ingredient on plate

        ingredient.transform.position = newPosition;

        // Rotate the object -90 degrees before attaching it
        ingredient.transform.rotation = Quaternion.Euler(-90, 0, 0);

        // Update stack tracking
        stackedIngredients.Add(ingredient);
        topIngredient = ingredient.transform;
        ingredient.transform.localScale = new Vector3(originalWorldScale.x / transform.lossyScale.x, originalWorldScale.y / transform.lossyScale.y, originalWorldScale.z / transform.lossyScale.z);
    }

    private float GetObjectHeight(Transform obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds.size.y; // Get the actual height of the object
        }
        return stackHeight; // Default fallback height
    }
}
