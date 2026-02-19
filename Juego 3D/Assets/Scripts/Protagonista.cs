using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public TMP_Text numeroObjetos;
    public TMP_Text dialogo;

    [Header("Pantalla de GameOver")]
    public Image panelFinJuego;
    public Button salirJuego;

    [Header("Pantalla de GameOver")]
    public Image panelGameOver;
    public Button reintentar;
    private bool estaMuerto;

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

        reintentar.onClick.AddListener(Reintentar);
        salirJuego.onClick.AddListener(SalirJuego);
        yRotation = transform.eulerAngles.y;
        xRotation = playerCamera.transform.localEulerAngles.x;
        if (xRotation > 180) xRotation -= 360;
        objetosRecogidos = 0;
        ActualizarObjetos(0);
        dialogo.enabled = false;
    }

    public void ActualizarObjetos(int objeto)
    {
        objetosRecogidos += objeto;
        numeroObjetos.text = Convert.ToString(objetosRecogidos + "/6");
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

    public void FinDeJuego()
    {
        panelFinJuego.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        estaMuerto = true;
    }
    public void GameOver()
    {
        estaMuerto = true;
        panelGameOver.gameObject.SetActive(true);
        numeroObjetos.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SalirJuego()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
    public void Reintentar()
    {
        objetosRecogidos = 0;
        ActualizarObjetos(0);
        panelGameOver.gameObject.SetActive(false);
        numeroObjetos.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        estaMuerto = false;
    }

    void HandleMovement()
    {
        if (!estaMuerto)
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinJuego"))
        {
            if (objetosRecogidos == 6)
            {
                FinDeJuego();
            }
            else
            {
                dialogo.enabled = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FinJuego"))
        {
            dialogo.enabled = false;
        }
    }
}
