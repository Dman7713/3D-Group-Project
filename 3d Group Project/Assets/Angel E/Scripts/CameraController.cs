using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 5f;
    private bool isCameraEnabled = true;

    void Update()
    {
        if (isCameraEnabled)
        {
            float moveX = Input.GetAxis("Mouse X") * cameraSpeed;
            float moveY = Input.GetAxis("Mouse Y") * cameraSpeed;

            transform.position += new Vector3(moveX, moveY, 0);
        }
    }

    public void DisableCamera()
    {
        isCameraEnabled = false;
    }

    public void EnableCamera()
    {
        isCameraEnabled = true;
    }
}
