using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleFPSLook : MonoBehaviour
{
    [Header("Look")]
    public float mouseSensitivity = 0.15f; // à ajuster (InputSystem renvoie des deltas bruts)
    public float maxPitch = 80f;

    [Header("Move")]
    public float moveSpeed = 4f;

    [Header("Cursor")]
    public bool lockCursorOnStart = true;
    public Key toggleCursorKey = Key.Escape;

    private float pitch;
    private Camera cam;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        if (!cam) cam = Camera.main;
    }

    void Start()
    {
        if (lockCursorOnStart) SetCursorLocked(true);
    }

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        // Toggle curseur (utile pour cliquer le bouton X de l'UI)
        if (kb[toggleCursorKey].wasPressedThisFrame)
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            SetCursorLocked(!locked);
        }

        // Si curseur libre, on ne contrôle pas la caméra
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Mouse look
        Vector2 delta = mouse.delta.ReadValue();
        float yaw = delta.x * mouseSensitivity;
        float lookY = delta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * yaw);

        pitch -= lookY;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        if (cam)
            cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);

        // Move (WASD)
        float h = 0f;
        float v = 0f;

        if (kb[Key.A].isPressed) h -= 1f;
        if (kb[Key.D].isPressed) h += 1f;
        if (kb[Key.W].isPressed) v += 1f;
        if (kb[Key.S].isPressed) v -= 1f;

        Vector3 dir = (transform.forward * v + transform.right * h).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
