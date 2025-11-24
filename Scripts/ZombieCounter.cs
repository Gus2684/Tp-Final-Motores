using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieCounter : MonoBehaviour
{
    public static ZombieCounter instancia;

    public int zombiesMuertos = 0;
    public int meta = 20;

    void Awake()
    {
        // Singleton para que exista uno solo
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SumarMuerte()
    {
        zombiesMuertos++;

        // Debug opcional para ver si suma
        Debug.Log("Zombies muertos: " + zombiesMuertos);

        if (zombiesMuertos >= meta)
        {
            SceneManager.LoadScene("Victoria");
        }
    }
}
