using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    public float bobSpeed = 4.0f;  // Speed of bobbing
    public float bobAmount = 0.05f; // How much the camera moves

    private float timer = 0.0f;
    private Vector3 originalPosition;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            timer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(timer) * bobAmount;
            transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + bobOffset, originalPosition.z);
        }
        else
        {
            // Reset position when not moving
            timer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5);
        }
    }
}
