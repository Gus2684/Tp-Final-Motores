using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float gravedad = -30f;
    public float salto = 8f;

    [Header("Cámara (Primera Persona)")]
    public Camera camara;
    public float sensibilidadX = 3f;
    public float sensibilidadY = 2f;
    public float minY = -75f;
    public float maxY = 30f;
    public float alturaOjos = 1.7f;
    public float fovReducido = 20f;

    private CharacterController controller;
    private float ySpeed;
    private float rotX;
    private float rotY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (camara != null)
        {
            camara.transform.localPosition = new Vector3(0, alturaOjos, 0);
            camara.fieldOfView = fovReducido;
        }
    }

    void Update()
    {
        rotX += Input.GetAxis("Mouse X") * sensibilidadX;
        rotY -= Input.GetAxis("Mouse Y") * sensibilidadY;
        rotY = Mathf.Clamp(rotY, minY, maxY);

        transform.rotation = Quaternion.Euler(0, rotX, 0);
        if (camara != null)
            camara.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

        float moverZ = Input.GetAxis("Vertical");
        float moverX = Input.GetAxis("Horizontal");

        Vector3 movimientoHorizontal = transform.forward * moverZ + transform.right * moverX;
        if (movimientoHorizontal.magnitude > 1f)
            movimientoHorizontal.Normalize();

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                ySpeed = salto;
            }
            else if (ySpeed < 0)
            {
                ySpeed = -2f;
            }
        }
        else
        {
            ySpeed += gravedad * Time.deltaTime;
        }

        Vector3 movimientoFinal = movimientoHorizontal * velocidad;
        movimientoFinal.y = ySpeed;

        controller.Move(movimientoFinal * Time.deltaTime);

        Debug.DrawRay(transform.position, Vector3.down * 1.1f, controller.isGrounded ? Color.green : Color.red);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Perder();
        }
    }
}