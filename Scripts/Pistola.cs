using UnityEngine;

public class Pistola : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float fuerzaDisparo = 25f;
    public float tiempoEntreDisparos = 0.5f;

    private float tiempoProximoDisparo = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= tiempoProximoDisparo)
        {
            Disparar();
            tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    void Disparar()
    {
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);

        Rigidbody rb = bala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = puntoDisparo.forward * fuerzaDisparo;
        }

        Destroy(bala, 5f);
    }
}
