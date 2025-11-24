using UnityEngine;

public class EnemyTerrainSpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject enemyPrefab;
    public int cantidadEnemigos = 10;

    [Header("Límites del área de spawn")]
    public Vector2 spawnMin = new Vector2(0, 0);      // X min, Z min
    public Vector2 spawnMax = new Vector2(100, 100);  // X max, Z max

    [Header("Altura y Verificación")]
    public float alturaSpawn = 50f;
    public bool usarRaycast = true;
    public float alturaFija = 0.5f;
    public float distanciaMinEntreEnemigos = 3f;
    public float distanciaMinimaDelJugador = 10f;

    [Header("Debug")]
    public bool mostrarDebug = true;
    public bool debugDetallado = true;

    [Header("Configuración de Rotación Inicial")]
    public bool rotacionAleatoria = true;

    private Transform jugador;

    void Start()
    {
        Debug.Log("[EnemySpawner] ====== INICIANDO SPAWNER ======");

        // Buscar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jugador = playerObj.transform;
            Debug.Log($"[EnemySpawner] ✅ Jugador encontrado en: {jugador.position}");
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] ⚠️ No se encontró jugador con tag 'Player'");
        }

        // Verificar prefab
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] ❌ NO HAY PREFAB ASIGNADO! Asigna el prefab en el Inspector");
            return;
        }
        else
        {
            Debug.Log($"[EnemySpawner] ✅ Prefab asignado: {enemyPrefab.name}");
        }

        Debug.Log($"[EnemySpawner] Configuración:");
        Debug.Log($"  - Cantidad: {cantidadEnemigos}");
        Debug.Log($"  - Área: X({spawnMin.x} a {spawnMax.x}), Z({spawnMin.y} a {spawnMax.y})");
        Debug.Log($"  - Usar Raycast: {usarRaycast}");
        Debug.Log($"  - Distancia mínima jugador: {distanciaMinimaDelJugador}m");

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        int spawnedCount = 0;
        int intentos = 0;
        int maxIntentos = cantidadEnemigos * 20;

        // Contadores de razones de fallo
        int fallosSinSuelo = 0;
        int fallosCercaJugador = 0;
        int fallosCercaOtroEnemy = 0;

        while (spawnedCount < cantidadEnemigos && intentos < maxIntentos)
        {
            intentos++;

            // Generar posición aleatoria
            float randomX = Random.Range(spawnMin.x, spawnMax.x);
            float randomZ = Random.Range(spawnMin.y, spawnMax.y);

            Vector3 posicionFinal;

            if (usarRaycast)
            {
                Vector3 posicionInicial = new Vector3(randomX, alturaSpawn, randomZ);
                RaycastHit hit;

                if (debugDetallado && intentos <= 5)
                    Debug.Log($"[EnemySpawner] Intento {intentos}: Raycast desde {posicionInicial}");

                if (Physics.Raycast(posicionInicial, Vector3.down, out hit, 200f))
                {
                    posicionFinal = hit.point + Vector3.up * 0.5f;

                    if (debugDetallado && intentos <= 5)
                        Debug.Log($"[EnemySpawner] ✅ Suelo encontrado en: {hit.point}, Spawneando en: {posicionFinal}");

                    if (mostrarDebug)
                        Debug.DrawLine(posicionInicial, hit.point, Color.green, 10f);
                }
                else
                {
                    fallosSinSuelo++;
                    if (debugDetallado && intentos <= 5)
                        Debug.LogWarning($"[EnemySpawner] ❌ No se encontró suelo en {posicionInicial}");
                    continue;
                }
            }
            else
            {
                posicionFinal = new Vector3(randomX, alturaFija, randomZ);
                if (debugDetallado && intentos <= 5)
                    Debug.Log($"[EnemySpawner] Usando altura fija: {posicionFinal}");
            }

            // Verificar distancia con jugador
            if (jugador != null)
            {
                float distanciaJugador = Vector3.Distance(posicionFinal, jugador.position);
                if (distanciaJugador < distanciaMinimaDelJugador)
                {
                    fallosCercaJugador++;
                    if (debugDetallado && intentos <= 5)
                        Debug.Log($"[EnemySpawner] ⚠️ Muy cerca del jugador ({distanciaJugador:F1}m < {distanciaMinimaDelJugador}m)");
                    continue;
                }
            }

            // Verificar distancia con otros enemigos
            if (HayEnemigoCerca(posicionFinal))
            {
                fallosCercaOtroEnemy++;
                if (debugDetallado && intentos <= 5)
                    Debug.Log($"[EnemySpawner] ⚠️ Muy cerca de otro enemigo");
                continue;
            }

            // ✅ POSICIÓN VÁLIDA - SPAWNEAR
            Quaternion rotacionInicial = rotacionAleatoria ?
                Quaternion.Euler(0, Random.Range(0f, 360f), 0) :
                Quaternion.identity;

            try
            {
                GameObject enemigo = Instantiate(enemyPrefab, posicionFinal, rotacionInicial);
                enemigo.name = "Zombie_" + spawnedCount;

                Debug.Log($"[EnemySpawner] ✅ ENEMIGO {spawnedCount + 1} SPAWNEADO en {posicionFinal}");

                // Configurar Rigidbody
                Rigidbody rb = enemigo.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotationX |
                                    RigidbodyConstraints.FreezeRotationZ;
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    rb.linearDamping = 0f;
                    rb.angularDamping = 0f;
                    rb.mass = 50f;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                    Debug.Log($"[EnemySpawner]   Rigidbody configurado correctamente");
                }
                else
                {
                    Debug.LogError($"[EnemySpawner] ❌ {enemigo.name} NO TIENE RIGIDBODY!");
                }

                // Verificar Collider
                Collider col = enemigo.GetComponent<Collider>();
                if (col == null)
                {
                    Debug.LogWarning($"[EnemySpawner]   Añadiendo CapsuleCollider...");
                    CapsuleCollider capsule = enemigo.AddComponent<CapsuleCollider>();
                    capsule.height = 2f;
                    capsule.radius = 0.5f;
                    capsule.center = new Vector3(0, 1f, 0);
                }
                else
                {
                    Debug.Log($"[EnemySpawner]   Collider: {col.GetType().Name}");
                }

                // Asignar tag
                if (!enemigo.CompareTag("Enemy"))
                {
                    enemigo.tag = "Enemy";
                    Debug.Log($"[EnemySpawner]   Tag 'Enemy' asignado");
                }

                // Verificar script Enemy
                Enemy scriptEnemy = enemigo.GetComponent<Enemy>();
                if (scriptEnemy == null)
                {
                    Debug.LogError($"[EnemySpawner] ❌ {enemigo.name} NO TIENE SCRIPT 'Enemy'!");
                }
                else
                {
                    Debug.Log($"[EnemySpawner]   Script Enemy encontrado");
                }

                spawnedCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[EnemySpawner] ❌ ERROR al instanciar enemigo: {e.Message}");
            }
        }

        // Resumen final
        Debug.Log("[EnemySpawner] ====== RESUMEN DE SPAWN ======");
        Debug.Log($"✅ Enemigos spawneados: {spawnedCount}/{cantidadEnemigos}");
        Debug.Log($"📊 Total de intentos: {intentos}");

        if (spawnedCount < cantidadEnemigos)
        {
            Debug.LogWarning($"⚠️ Solo se spawnearon {spawnedCount}/{cantidadEnemigos} enemigos");
            Debug.Log($"Razones de fallo:");
            Debug.Log($"  - Sin suelo: {fallosSinSuelo}");
            Debug.Log($"  - Cerca del jugador: {fallosCercaJugador}");
            Debug.Log($"  - Cerca de otro enemigo: {fallosCercaOtroEnemy}");

            if (fallosSinSuelo > intentos * 0.5f)
            {
                Debug.LogError("❌ PROBLEMA: Muchos raycast no encuentran suelo!");
                Debug.LogError("SOLUCIONES:");
                Debug.LogError("1. Asegúrate que tu terreno tiene un Collider");
                Debug.LogError("2. Aumenta 'Altura Spawn' a un valor más alto (ej: 100)");
                Debug.LogError("3. O desactiva 'Usar Raycast' y usa altura fija");
            }
        }
        else
        {
            Debug.Log($"🎉 ¡TODOS LOS ENEMIGOS SPAWNEADOS EXITOSAMENTE!");
        }

        Debug.Log("[EnemySpawner] ====== FIN SPAWN ======");
    }

    bool HayEnemigoCerca(Vector3 posicion)
    {
        Collider[] colliders = Physics.OverlapSphere(posicion, distanciaMinEntreEnemigos);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
                return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 centro = new Vector3(
            (spawnMin.x + spawnMax.x) / 2f,
            0,
            (spawnMin.y + spawnMax.y) / 2f
        );
        Vector3 tamaño = new Vector3(
            spawnMax.x - spawnMin.x,
            1f,
            spawnMax.y - spawnMin.y
        );
        Gizmos.DrawWireCube(centro, tamaño);

        if (jugador != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(jugador.position, distanciaMinimaDelJugador);
        }
    }
}

