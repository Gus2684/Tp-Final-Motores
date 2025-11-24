using UnityEngine;

public class CamaraJugador : MonoBehaviour
{
    public float sensibilidad = 5f;
    public float limiteVertical = 80f;

    private float rotacionX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -limiteVertical, limiteVertical);
        transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}