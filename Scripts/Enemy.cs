using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Persecución")]
    public float velocidad = 6f;
    public float distanciaDeteccion = 20f;

    [Header("Ataque")]
    public float distanciaAtaque = 2.2f;
    public float daño = 10f;
    public float cooldownAtaque = 1.5f;

    [Header("Vida")]
    public float vidaMaxima = 75f;
    private float vidaActual;

    [Header("Animaciones - Arrastra aquí los Animation Clips")]
    public AnimationClip animacionIdle;
    public AnimationClip animacionCaminar;
    public AnimationClip animacionAtaque;
    public AnimationClip animacionMuerte;

    [Header("Configuración de Rotación")]
    [Tooltip("Ajusta este valor si tu modelo mira en dirección incorrecta (0, 90, 180 o -90)")]
    public float offsetRotacion = 0f;
    public float velocidadRotacion = 10f;

    [Header("Debug")]
    public bool mostrarLogsAnimacion = false;

    private Transform jugador;
    private Rigidbody rb;
    private Animator anim;
    private bool estaMuerto = false;
    private bool puedeAtacar = true;
    private bool estaAtacando = false;
    private string estadoActual = "";

    // Control de animaciones
    private enum EstadoZombie { Idle, Caminando, Atacando, Muerto }
    private EstadoZombie estadoActualZombie = EstadoZombie.Idle;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jugador == null)
            Debug.LogError("[Enemy] No se encontró Player (tag 'Player').");

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        vidaActual = vidaMaxima;

        // Configurar Rigidbody - CRÍTICO para movimiento correcto
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX |
                            RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f; // Evitar rotación automática
            rb.mass = 50f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        VerificarAnimaciones();
    }

    void VerificarAnimaciones()
    {
        Debug.Log($"[Enemy] {gameObject.name} - Animaciones asignadas:");
        Debug.Log($"  Idle: {(animacionIdle != null ? animacionIdle.name : "NO ASIGNADA ❌")}");
        Debug.Log($"  Caminar: {(animacionCaminar != null ? animacionCaminar.name : "NO ASIGNADA ❌")}");
        Debug.Log($"  Ataque: {(animacionAtaque != null ? animacionAtaque.name : "NO ASIGNADA ❌")}");
        Debug.Log($"  Muerte: {(animacionMuerte != null ? animacionMuerte.name : "NO ASIGNADA ❌")}");
    }

    void Update()
    {
        if (estaMuerto || jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Determinar estado según distancia
        if (distancia <= distanciaAtaque)
        {
            Atacar();
        }
        else if (distancia <= distanciaDeteccion)
        {
            Perseguir();
        }
        else
        {
            Idle();
        }
    }

    void Perseguir()
    {
        if (estaAtacando) return;

        // Cambiar a animación de caminar
        CambiarEstado(EstadoZombie.Caminando);

        // Calcular dirección hacia el jugador
        Vector3 direccion = (jugador.position - transform.position).normalized;
        direccion.y = 0; // Mantener en plano horizontal

        // Mover al zombie
        Vector3 nuevaPosicion = transform.position + direccion * velocidad * Time.deltaTime;
        rb.MovePosition(nuevaPosicion);

        // Rotar suavemente hacia la dirección de movimiento
        if (direccion != Vector3.zero)
        {
            RotarHacia(direccion);
        }
    }

    void Idle()
    {
        if (estaAtacando) return;
        CambiarEstado(EstadoZombie.Idle);
    }

    void Atacar()
    {
        // Detener movimiento
        rb.linearVelocity = Vector3.zero;

        // Cambiar a estado de ataque
        CambiarEstado(EstadoZombie.Atacando);

        // Rotar hacia el jugador
        Vector3 direccionJugador = (jugador.position - transform.position).normalized;
        direccionJugador.y = 0;

        if (direccionJugador != Vector3.zero)
        {
            RotarHacia(direccionJugador);
        }

        // Ejecutar ataque si está disponible
        if (puedeAtacar && !estaAtacando)
        {
            StartCoroutine(AtaqueCoroutine());
        }
    }

    void RotarHacia(Vector3 direccion)
    {
        // Crear rotación objetivo
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        // Aplicar offset si el modelo está rotado incorrectamente
        rotacionObjetivo *= Quaternion.Euler(0, offsetRotacion, 0);

        // Rotar suavemente
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            velocidadRotacion * Time.deltaTime
        );
    }

    void CambiarEstado(EstadoZombie nuevoEstado)
    {
        // Si ya está en ese estado, no hacer nada
        if (estadoActualZombie == nuevoEstado) return;

        estadoActualZombie = nuevoEstado;

        // Reproducir animación correspondiente
        switch (nuevoEstado)
        {
            case EstadoZombie.Idle:
                ReproducirAnimacion(animacionIdle, "Idle");
                break;
            case EstadoZombie.Caminando:
                ReproducirAnimacion(animacionCaminar, "Caminar");
                break;
            case EstadoZombie.Atacando:
                ReproducirAnimacion(animacionAtaque, "Ataque");
                break;
            case EstadoZombie.Muerto:
                ReproducirAnimacion(animacionMuerte, "Muerte");
                break;
        }
    }

    void ReproducirAnimacion(AnimationClip clip, string tipoAccion)
    {
        if (anim == null || clip == null) return;

        // Solo cambiar si es diferente
        if (estadoActual == clip.name) return;

        try
        {
            anim.CrossFade(clip.name, 0.2f);
            estadoActual = clip.name;

            if (mostrarLogsAnimacion)
                Debug.Log($"[Enemy] Reproduciendo: {clip.name} ({tipoAccion})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Enemy] ❌ ERROR al reproducir {clip.name}: {e.Message}");
        }
    }

    System.Collections.IEnumerator AtaqueCoroutine()
    {
        puedeAtacar = false;
        estaAtacando = true;

        // Esperar un momento antes de hacer daño (tiempo de animación)
        yield return new WaitForSeconds(0.5f);

        // Verificar si sigue en rango y hacer daño
        if (jugador != null && Vector3.Distance(transform.position, jugador.position) <= distanciaAtaque + 0.5f)
        {
            Debug.Log("[Enemy] ¡Ataque conectado!");
            SceneManager.LoadScene("Derrota");
        }

        // Esperar cooldown antes de permitir otro ataque
        yield return new WaitForSeconds(cooldownAtaque);

        puedeAtacar = true;
        estaAtacando = false;
    }

    public void RecibirDaño(float daño)
    {
        if (estaMuerto) return;

        vidaActual -= daño;
        float vidaNormalizada = Mathf.Clamp01(vidaActual / vidaMaxima);

        // Actualizar barra de vida
        VidaEnemigo barra = GetComponentInChildren<VidaEnemigo>();
        if (barra != null)
            barra.SetNormalizedHealth(vidaNormalizada);

        Debug.Log($"[Enemy] Vida: {vidaActual}/{vidaMaxima} ({vidaNormalizada:P0})");

        if (vidaActual <= 0)
            Morir();
    }

    void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        // Cambiar estado
        CambiarEstado(EstadoZombie.Muerto);

        // Notificar al GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.SumarZombieEliminado(1);

        Debug.Log($"[Enemy] {gameObject.name} murió");

        // Desactivar física
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        // Desactivar collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Destruir después de la animación
        float tiempoMuerte = animacionMuerte != null ? animacionMuerte.length : 4f;
        Destroy(gameObject, tiempoMuerte);
    }

    public void RalentizarTemporalmente(float factor = 0.5f, float duracion = 1f)
    {
        if (!estaMuerto)
            StartCoroutine(RalentizarCoroutine(factor, duracion));
    }

    private System.Collections.IEnumerator RalentizarCoroutine(float factor, float duracion)
    {
        float velocidadOriginal = velocidad;
        velocidad *= factor;
        yield return new WaitForSeconds(duracion);
        velocidad = velocidadOriginal;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // Dibujar dirección "adelante" del modelo
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * 2f);
    }
}