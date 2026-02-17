using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Protagonista : MonoBehaviour
{
    [Header("Referencias")]
    public Camera playerCamera;

    [Header("Sensibilidad y Límites")]
    public float sensitivity = 2f; // Sensibilidad más baja para valores modernos
    public float smoothTime = 0.05f; // Suavizado ligero para evitar micro-saltos

    [Header("Físicas")]
    public float walkSpeed = 5f;
    public float gravity = -15f; // Un poco más fuerte para que no "flote"

    [Header("Objetos Recogidos")]
    public int objetosRecogidos;
    public GameObject[] objetos;

    private CharacterController controller;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Es vital que la cámara sea hija del Player
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Start()
    {
        foreach (GameObject obj  in objetos)
        {
            Renderer colorObjetos = obj.GetComponent<Renderer>();
            colorObjetos.material.color = Color.blue;
        }

        objetosRecogidos = 0;
    }
    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clampeamos la rotación vertical
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        // Aplicamos las rotaciones de forma limpia
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

        // Aplicar movimiento
        controller.Move(move * walkSpeed * Time.deltaTime);

        // Gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
