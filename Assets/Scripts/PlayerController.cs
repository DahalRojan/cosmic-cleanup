using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement and Rotation")]
    public float moveSpeed = 30f;
    public float mouseSensitivity = 150f; // Increased sensitivity for direct look control
    public float bodyTurnSpeed = 2.5f;   // How quickly the ship's body aligns with your view
    public float rollSpeed = 90f;      // Roll speed for Q/E keys

    [Header("Zapping")]
    public float zapTime = 0.02f;
    public float zapDistance = 100f;
    public Image zapProgress;
    public AudioClip zapSound;
    public GameObject zapEffectPrefab;

    // Private component references
    private Rigidbody rb;
    private Camera cam;
    private DebrisManager debrisManager;
    private DataLogger dataLogger;
    private AudioSource audioSource;

    // State variables for targeting and zapping
    private GameObject currentTarget;
    private float focusTime;

    // State variables for camera rotation
    private float cameraYaw = 0f;
    private float cameraPitch = 0f;

    // Variables for data logging
    private float lastLogTime;
    private float lastRaycastTime;
    private Vector3 lastPosition;
    private Vector3 lastVelocity;
    private Vector3 lastAngularVelocity;
    private float totalDistance;
    private float lastMouseX, lastMouseY;
    private Vector3 simulatedAcceleration;

    void Start()
    {
        // --- Get and check Rigidbody component ---
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on Player! Disabling script.");
            enabled = false;
            return;
        }
        rb.useGravity = false;

        // --- Get and check Camera component ---
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera not found on Player GameObject! Disabling script.");
            enabled = false;
            return;
        }

        // --- Find other managers and components ---
        debrisManager = FindFirstObjectByType<DebrisManager>();
        if (debrisManager == null)
        {
            Debug.LogError("DebrisManager not found in scene!");
        }

        dataLogger = FindFirstObjectByType<DataLogger>();
        if (dataLogger == null)
        {
            Debug.LogWarning("DataLogger not found in scene! Data logging will be disabled.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource component missing on Player! Adding one.");
        }

        // --- Check for required assigned objects ---
        if (zapProgress == null)
        {
            Debug.LogError("ZapProgress Image not assigned in Inspector!");
        }

        if (ObjectPool.Instance == null)
        {
            Debug.LogError("ObjectPool not found in scene! Please add an ObjectPool GameObject.");
        }

        // --- Initialize state ---
        Cursor.lockState = CursorLockMode.Locked;
        lastPosition = transform.position;
        lastVelocity = rb.linearVelocity;
        lastAngularVelocity = rb.angularVelocity;
        totalDistance = 0f;
        lastMouseX = Input.GetAxis("Mouse X");
        lastMouseY = Input.GetAxis("Mouse Y");
        cam.transform.localEulerAngles = Vector3.zero; // Start with camera looking straight ahead
    }

    void Update()
    {
        if (rb == null || cam == null) return;

        // --- NEW: Handle all camera rotation in Update for responsiveness ---
        HandleCameraLook();

        // --- Zapping logic remains in Update ---
        HandleZapping();

        // --- Data logging remains in Update ---
        HandleDataLogging();

        // --- NEW: Calculate simulated physics data based on previous frame's velocity ---
        Vector3 currentVelocity = rb.linearVelocity;
        simulatedAcceleration = (currentVelocity - lastVelocity) / Time.deltaTime;
        lastVelocity = currentVelocity;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Physics-based operations are in FixedUpdate

        // --- NEW: Handle ship movement relative to the camera's direction ---
        HandleMovement();

        // --- NEW: Handle ship body rotation to align with camera and roll ---
        HandleBodyRotation();
    }

    /// <summary>
    /// Handles the free-look camera rotation based on mouse input.
    /// This directly rotates the camera object, independent of the ship's body.
    /// </summary>
    void HandleCameraLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust camera yaw (left/right) and pitch (up/down)
        cameraYaw += mouseX;
        cameraPitch -= mouseY;

        // Clamp the pitch to prevent the camera from flipping upside down
        cameraPitch = Mathf.Clamp(cameraPitch, -89f, 89f);

        // Apply the rotation directly to the camera's local rotation
        cam.transform.localRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
    }

    /// <summary>
    /// Handles player movement based on keyboard input.
    /// Movement is now relative to the camera's orientation for intuitive control.
    /// </summary>
    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // Strafe left/right (A/D)
        float v = Input.GetAxis("Vertical");   // Forward/back (W/S)
        float up = Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftControl) ? -1f : 0f; // Up/down

        // Calculate movement direction based on where the camera is looking
        Vector3 moveDirection = (cam.transform.right * h + cam.transform.forward * v + cam.transform.up * up).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Smoothly interpolate to the target velocity for responsive, non-slidy movement
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 10f);
    }

    /// <summary>
    /// Handles the rotation of the ship's body (Rigidbody).
    /// The body will smoothly align with the camera's yaw and respond to roll input.
    /// </summary>
    void HandleBodyRotation()
    {
        // Determine the target rotation for the body (align with camera's yaw)
        Quaternion targetBodyRotation = Quaternion.Euler(0, cameraYaw, 0);

        // Smoothly rotate the Rigidbody towards the target rotation
        // This makes the ship follow where you look. Adjust bodyTurnSpeed to change responsiveness.
        rb.rotation = Quaternion.Slerp(rb.rotation, targetBodyRotation, Time.fixedDeltaTime * bodyTurnSpeed);

        // Handle roll input (Q/E)
        float rollInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rollInput = 1f;
        if (Input.GetKey(KeyCode.E)) rollInput = -1f;

        // Apply roll as a torque for a physical feel
        if (Mathf.Abs(rollInput) > 0.01f)
        {
            rb.AddRelativeTorque(Vector3.back * rollInput * rollSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Handles the logic for targeting and zapping debris.
    /// The raycast now originates from the camera's position and direction.
    /// </summary>
    void HandleZapping()
    {
        if (Time.time - lastRaycastTime >= 0.2f)
        {
            lastRaycastTime = Time.time;
            int debrisLayerMask = LayerMask.GetMask("Debris");

            // Create a ray from the camera's position, pointing forward
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

            if (Physics.SphereCast(ray, 1f, out RaycastHit hit, zapDistance, debrisLayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                Debris debrisComponent = hitObject.GetComponent<Debris>();

                if (debrisComponent == null || !hitObject.activeSelf)
                {
                    ResetTarget();
                    return;
                }

                if (hitObject == currentTarget)
                {
                    focusTime += Time.deltaTime;
                    if (zapProgress != null) zapProgress.fillAmount = focusTime / zapTime;

                    if (focusTime >= zapTime)
                    {
                        ZapDebris(hitObject);
                    }
                }
                else
                {
                    ResetTarget();
                    currentTarget = hitObject;
                    debrisComponent.SetTargeted(true);
                }
            }
            else
            {
                ResetTarget();
            }
        }
    }

    /// <summary>
    /// Handles logging data at a fixed interval. This code is unchanged.
    /// </summary>
    void HandleDataLogging()
    {
        if (dataLogger != null && Time.time - lastLogTime >= 0.5f)
        {
            float distance = Vector3.Distance(transform.position, lastPosition);
            totalDistance += distance;
            lastPosition = transform.position;

            float speed = rb.linearVelocity.magnitude;
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseDeltaX = mouseX - lastMouseX;
            float mouseDeltaY = mouseY - lastMouseY;
            float rotationalSpeed = rb.angularVelocity.magnitude * Mathf.Rad2Deg;
            float accelerationMagnitude = simulatedAcceleration.magnitude;
            Vector3 simulatedAngularVelocity = rb.angularVelocity * Mathf.Rad2Deg;

            dataLogger.LogData(Time.time, transform.position, cam.transform.eulerAngles, speed, totalDistance, mouseDeltaX, mouseDeltaY, rotationalSpeed, accelerationMagnitude, 0f, simulatedAcceleration, simulatedAngularVelocity);

            lastLogTime = Time.time;
            lastMouseX = mouseX;
            lastMouseY = mouseY;
        }
    }


    // --- All original helper methods below are preserved without changes ---

    void ZapDebris(GameObject debris)
    {
        if (debris == null || debris.GetComponent<Debris>() == null)
        {
            Debug.LogError("Debris or Debris component is null!");
            return;
        }

        debris.GetComponent<Debris>().SetTargeted(false);
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.GetObject(debris.transform.position, Quaternion.identity);
        }
        if (zapSound != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = zapSound;
            audioSource.loop = false;
            audioSource.Play();
            Invoke("StopZapSound", 4f);
        }
        if (debrisManager != null)
        {
            debrisManager.DebrisZapped(debris);
        }
        debris.SetActive(false);
        currentTarget = null;
    }

    void StopZapSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void ResetTarget()
    {
        if (currentTarget != null)
        {
            Debris debrisComponent = currentTarget.GetComponent<Debris>();
            if (debrisComponent != null)
            {
                debrisComponent.SetTargeted(false);
            }
            currentTarget = null;
        }
        focusTime = 0;
        if (zapProgress != null) zapProgress.fillAmount = 0;
    }
}