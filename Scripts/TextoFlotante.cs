using TMPro;
using UnityEngine;

public class TextoFlotante : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI textoTMP;
    [HideInInspector] public CanvasGroup canvasGroup;

    void Awake()
    {
        // Busca automáticamente los componentes
        textoTMP = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (textoTMP == null)
            Debug.LogError("ERROR: No se encontró TextMeshProUGUI dentro del prefab TextoFlotante.");

        if (canvasGroup == null)
            Debug.LogError("ERROR: No se encontró CanvasGroup dentro del prefab TextoFlotante.");
    }

    public void SetTexto(string texto)
    {
        textoTMP.text = texto;
    }
}
