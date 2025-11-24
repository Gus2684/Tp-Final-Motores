using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonVictoria : MonoBehaviour
{
    public void ReiniciarNivel()
    {
        SceneManager.LoadScene("Menu");
    }
}
