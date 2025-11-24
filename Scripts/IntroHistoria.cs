using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroHistoria : MonoBehaviour
{
    public TextoFlotante prefab;
    public RectTransform canvasRect;

    [Header("Posición fija del texto")]
    public RectTransform puntoFijo;

    [Header("Tiempos")]
    public float fadeIn = 0.8f;
    public float fadeOut = 0.8f;

    [TextArea(5, 10)]
    public string[] historia;

    void Start()
    {
        // Asegurar que nada esté pausado
        Time.timeScale = 1f;

        StartCoroutine(Secuencia());
    }

    IEnumerator Secuencia()
    {
        foreach (string linea in historia)
            yield return StartCoroutine(MostrarLinea(linea));

        // Cambiar de escena al final
        SceneManager.LoadScene("Level1");
    }

    IEnumerator MostrarLinea(string texto)
    {
        // Instancia del prefab
        TextoFlotante tf = Instantiate(prefab, canvasRect);
        RectTransform tRect = tf.GetComponent<RectTransform>();

        tf.textoTMP.color = Color.black;
        tf.canvasGroup.alpha = 0f;

        // Posición del texto
        tRect.anchoredPosition = puntoFijo.anchoredPosition;
        tRect.localScale = new Vector3(0.85f, 0.85f, 1f);

        tf.SetTexto(texto);

        yield return new WaitForEndOfFrame();

        // FADE IN
        for (float t = 0; t < fadeIn; t += Time.unscaledDeltaTime)
        {
            float s = t / fadeIn;
            s = s * s * (3 - 2 * s);

            tf.canvasGroup.alpha = s;

            tRect.localScale = Vector3.Lerp(
                new Vector3(0.85f, 0.85f, 1f),
                Vector3.one,
                s
            );

            yield return null;
        }

        tf.canvasGroup.alpha = 1f;
        tRect.localScale = Vector3.one;

        // ESPERAR CLICK
        yield return StartCoroutine(EsperarClick());

        // FADE OUT
        for (float t = 0; t < fadeOut; t += Time.unscaledDeltaTime)
        {
            float s = t / fadeOut;
            s = s * s * (3 - 2 * s);

            tf.canvasGroup.alpha = 1f - s;

            tRect.localScale = Vector3.Lerp(
                Vector3.one,
                new Vector3(0.85f, 0.85f, 1f),
                s
            );

            yield return null;
        }

        Destroy(tf.gameObject);
    }

    // MÉTODO COMPATIBLE CON AMBOS SISTEMAS DE INPUT
    IEnumerator EsperarClick()
    {
        while (true)
        {
            // Input nuevo: Mouse.current
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null &&
                UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                break;
#endif

            // Input viejo: GetMouseButtonDown
            if (Input.GetMouseButtonDown(0))
                break;

            yield return null;
        }
    }
}
