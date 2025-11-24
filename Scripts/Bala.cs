using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidad = 25f;
    public float duracion = 5f;
    public float daño = 25f;

    void Start()
    {
        Destroy(gameObject, duracion);
    }

    void Update()
    {
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Buscar el componente Enemy en el objeto o en su raíz
        Enemy enemigo = collision.transform.GetComponent<Enemy>();
        if (enemigo == null)
            enemigo = collision.transform.root.GetComponent<Enemy>();

        if (enemigo != null)
        {
            enemigo.RecibirDaño(daño);
            enemigo.RalentizarTemporalmente(0.5f, 1f);
            Debug.Log($"[Bala] Impacto en {enemigo.gameObject.name}");
        }

        Destroy(gameObject);
    }
}
