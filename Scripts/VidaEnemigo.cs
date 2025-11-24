using UnityEngine;
using UnityEngine.UI;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    public Image barraRellenoVerde;
    public Image barraRellenoRoja;

    [Header("Configuración")]
    public float alturaBarraSobreCabeza = 2.5f;

    private Transform camara;
    private Canvas canvas;

    void Start()
    {
        camara = Camera.main.transform;
        canvas = GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
        }

        // Inicializar barras
        if (barraRellenoVerde != null)
            barraRellenoVerde.fillAmount = 1f;

        if (barraRellenoRoja != null)
            barraRellenoRoja.fillAmount = 1f;
    }

    void LateUpdate()
    {
        // Siempre mirar a la cámara
        if (camara != null)
            transform.LookAt(transform.position + camara.forward);
    }

    public void SetNormalizedHealth(float valorNormalizado)
    {
        valorNormalizado = Mathf.Clamp01(valorNormalizado);

        if (barraRellenoVerde != null)
            barraRellenoVerde.fillAmount = valorNormalizado;

        // Cambiar a rojo cuando esté baja
        if (valorNormalizado <= 0.33f && barraRellenoRoja != null)
        {
            barraRellenoRoja.fillAmount = valorNormalizado;
            barraRellenoVerde.gameObject.SetActive(false);
            barraRellenoRoja.gameObject.SetActive(true);
        }
    }
}
