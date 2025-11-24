using UnityEngine;

/// <summary>
/// Script para diagnosticar problemas con enemigos que desaparecen
/// Attachar al prefab del enemigo temporalmente
/// </summary>
public class DiagnosticoEnemigos : MonoBehaviour
{
    private Vector3 posicionInicial;
    private float tiempoVida = 0f;
    private bool cayendo = false;

    void Start()
    {
        posicionInicial = transform.position;
        Debug.Log($"[DIAGNOSTICO] {gameObject.name} spawneado en {posicionInicial}");

        // Verificar componentes
        VerificarComponentes();
    }

    void Update()
    {
        tiempoVida += Time.deltaTime;

        // Detectar si está cayendo
        if (transform.position.y < posicionInicial.y - 1f && !cayendo)
        {
            cayendo = true;
            Debug.LogWarning($"[DIAGNOSTICO] {gameObject.name} está CAYENDO! Y actual: {transform.position.y}, Y inicial: {posicionInicial.y}");
        }

        // Detectar si cayó muy bajo
        if (transform.position.y < -10f)
        {
            Debug.LogError($"[DIAGNOSTICO] {gameObject.name} cayó al vacío en Y={transform.position.y}. Destruyendo...");
            Destroy(gameObject);
        }
    }

    void VerificarComponentes()
    {
        // Verificar Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"[DIAGNOSTICO] {gameObject.name} NO TIENE RIGIDBODY!");
        }
        else
        {
            Debug.Log($"[DIAGNOSTICO] {gameObject.name} Rigidbody: isKinematic={rb.isKinematic}, useGravity={rb.useGravity}, mass={rb.mass}");
        }

        // Verificar Collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"[DIAGNOSTICO] {gameObject.name} NO TIENE COLLIDER!");
        }
        else
        {
            Debug.Log($"[DIAGNOSTICO] {gameObject.name} Collider tipo: {col.GetType().Name}, enabled={col.enabled}, isTrigger={col.isTrigger}");
        }

        // Verificar Layer
        Debug.Log($"[DIAGNOSTICO] {gameObject.name} Layer: {LayerMask.LayerToName(gameObject.layer)}, Tag: {gameObject.tag}");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[DIAGNOSTICO] {gameObject.name} colisionó con {collision.gameObject.name} (Layer: {LayerMask.LayerToName(collision.gameObject.layer)})");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DIAGNOSTICO] {gameObject.name} trigger con {other.gameObject.name}");
    }

    void OnDestroy()
    {
        Debug.Log($"[DIAGNOSTICO] {gameObject.name} destruido después de {tiempoVida:F2} segundos en posición {transform.position}");
    }

    void OnDrawGizmos()
    {
        // Dibujar una esfera en la posición del enemigo
        Gizmos.color = cayendo ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Línea desde posición inicial
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(posicionInicial, transform.position);
        }
    }
}
