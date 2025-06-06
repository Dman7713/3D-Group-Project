using UnityEngine;
using UnityEngine.Jobs;

public class PlatingObjects : MonoBehaviour
{
    [SerializeField]
    public string targetLayerName = "PickUP";
     private Vector3 originalWorldScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().freezeRotation = false;
        }
    }

    /*
    // on collision
    private void OnCollisionEnter(Collision collision)
    {
        ///originalWorldScale = collision.transform.lossyScale;
        // ENSURE THAT THE CODE WILL ONLY WORK IF THE SCRIPT IS ENABLED
        if (GetComponent<PlatingObjects>().enabled == true)
        {
            // if there is no parent to the object and it has the tag "ingredients"
            if (collision.transform.parent != transform && collision.transform.tag == "Ingredients")
            {
                ///grab collided objects rigidbody (if it has one)
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                ///if it has  one
                if (rb != null)
                {
                    ///cut all velocity
                    rb.angularVelocity = Vector3.zero;
                    rb.linearVelocity = Vector3.zero;
                    rb.freezeRotation = true;
                }
                if (rb != null && transform.name != "Plate")
                {
                    rb.isKinematic = true;
                }
                // set its parent to the scripted object
                collision.transform.SetParent(transform);
                //if the object's name is not plate and the scripted object has a parent named plate,
                if (gameObject.name != "Plate" && transform.parent.name == "Plate" && transform.parent != null)
                {
                    // set the scripted object's child to be the child of the scripted object's parent
                    collision.transform.SetParent(transform.parent);
                }
                ///collision.transform.localScale = new Vector3(originalWorldScale.x / transform.lossyScale.x, originalWorldScale.y / transform.lossyScale.y, originalWorldScale.z / transform.lossyScale.z);
            }
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        originalWorldScale = other.transform.lossyScale;
        int targetLayer = LayerMask.NameToLayer(targetLayerName);
        if (GetComponent<PlatingObjects>().enabled == true)
        { 
            if (other.transform.parent != transform && other.transform.tag == "Ingredients" && other.transform != transform.parent 
                || other.transform.parent != transform && other.transform.tag == "Pickup" && other.gameObject.layer == targetLayer && other.transform != transform.parent)
            {
                Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.angularVelocity = Vector3.zero;
                    rb.linearVelocity = Vector3.zero;
                    rb.freezeRotation = true;
                }
                if (rb != null && transform.name != "Plate")
                {
                    rb.isKinematic = true;
                }
                other.transform.SetParent(transform);
                BoxCollider[] boxes = GetComponents<BoxCollider>();
                boxes[0].enabled = false;
                boxes[0].isTrigger = false;
                GetComponent<PlatingObjects>().enabled = false;
                other.transform.localScale = new Vector3(originalWorldScale.x / transform.lossyScale.x, originalWorldScale.y / transform.lossyScale.y, originalWorldScale.z / transform.lossyScale.z);
            }
        }
    }
}
// errors:
// * plated objects displacement on pickup and drop
// * scaling issues after placing the cheese, objects narrow to fit its thinner size
