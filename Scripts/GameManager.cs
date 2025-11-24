using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias UI")]
    public TextMeshProUGUI textoContadorZombies;

    [Header("Estadísticas")]
    private int zombiesEliminados = 0;
    private int totalZombiesEnEscena = 0;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Contar zombies al inicio
        totalZombiesEnEscena = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        ActualizarUI();

        Debug.Log($"[GameManager] Total de zombies en escena: {totalZombiesEnEscena}");
    }

    public void SumarZombieEliminado(int cantidad = 1)
    {
        zombiesEliminados += cantidad;
        ActualizarUI();

        Debug.Log($"[GameManager] Zombies eliminados: {zombiesEliminados}/{totalZombiesEnEscena}");

        // Verificar victoria si se eliminaron todos
        if (zombiesEliminados >= totalZombiesEnEscena)
        {
            Debug.Log("🎉 ¡VICTORIA! Todos los zombies eliminados");
            // Aquí puedes cargar escena de victoria si tienes
            // SceneManager.LoadScene("Victoria");
        }
    }

    // Método Perder que faltaba (llamado desde Movimiento.cs)
    public void Perder()
    {
        Debug.Log("[GameManager] ¡DERROTA! El jugador ha caído");
        SceneManager.LoadScene("Derrota");
    }

    void ActualizarUI()
    {
        if (textoContadorZombies != null)
        {
            textoContadorZombies.text = $"Zombies: {zombiesEliminados}/{totalZombiesEnEscena}";
        }
    }

    // Métodos útiles
    public int GetZombiesEliminados() => zombiesEliminados;
    public int GetTotalZombies() => totalZombiesEnEscena;
    public float GetPorcentajeCompletado() => totalZombiesEnEscena > 0 ? (float)zombiesEliminados / totalZombiesEnEscena : 0f;
}
