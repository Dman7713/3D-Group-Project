using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private List<GameObject> stackedIngredients = new List<GameObject>();  // List of all stacked objects
    private Transform topIngredient; // Tracks the last placed ingredient
    private float stackHeight = 0.1f; // Height spacing between objects

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredients") && !stackedIngredients.Contains(collision.gameObject))
        {
            AttachToPlate(collision.gameObject);
        }
    }

    private void AttachToPlate(GameObject ingredient)
    {
        // Disable Rigidbody to prevent physics from interfering
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Set parent to the plate
        ingredient.transform.SetParent(transform);

        // Determine correct stacking position
        Vector3 newPosition = topIngredient != null
            ? topIngredient.position + new Vector3(0, stackHeight, 0)  // Stack on top of last ingredient
            : transform.position + new Vector3(0, stackHeight, 0);  // First ingredient on plate

        ingredient.transform.position = newPosition;
        ingredient.transform.rotation = Quaternion.identity; // Keep upright orientation

        // Update tracking variables
        stackedIngredients.Add(ingredient);
        topIngredient = ingredient.transform;
    }
}
