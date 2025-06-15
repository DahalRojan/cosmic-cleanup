using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraLookController : MonoBehaviour
{
    public float sensitivity = 150f;
    float yaw, pitch;

    void Update()
    {
        yaw   += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, -89f, 89f);
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
