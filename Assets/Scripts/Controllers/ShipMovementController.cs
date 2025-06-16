// using UnityEngine;

// public class ShipMovementController : MonoBehaviour
// {
//     [Header("Speeds")]
//     public float moveSpeed = 30f;    // units per second
//     public float rollSpeed = 90f;    // degrees per second

//     Camera cam;

//     void Start()
//     {
//         // Cache camera (used for direction & yaw)
//         cam = GetComponentInChildren<Camera>();

//         // Zero out any starting rotation so you face forward
//         transform.rotation = Quaternion.identity;
//         if (cam != null)
//             cam.transform.localRotation = Quaternion.identity;
//     }

//     void Update()
//     {
//         // 1) Movement relative to camera
//         float h  = Input.GetAxis("Horizontal");     // A/D
//         float v  = Input.GetAxis("Vertical");       // W/S
//         float up = Input.GetKey(KeyCode.Space)   ? 1f :
//                    Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;
//         Vector3 dir = (cam.transform.right * h
//                      + cam.transform.forward * v
//                      + cam.transform.up * up).normalized;
//         transform.position += dir * (moveSpeed * Time.deltaTime);

//         // 2) Yaw align body with camera's yaw
//         if (cam != null)
//         {
//             float camYaw = cam.transform.eulerAngles.y;
//             transform.rotation = Quaternion.Euler(0f, camYaw, 0f);
//         }

//         // 3) Manual roll (Q/E)
//         float rollInput = Input.GetKey(KeyCode.Q) ? 1f :
//                           Input.GetKey(KeyCode.E) ? -1f : 0f;
//         if (Mathf.Abs(rollInput) > 0.01f)
//         {
//             // Rotate around local forward axis
//             transform.Rotate(Vector3.forward, rollInput * rollSpeed * Time.deltaTime, Space.Self);
//         }
//     }
// }
