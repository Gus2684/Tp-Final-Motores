using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonDerrota : MonoBehaviour
{
    public void ReiniciarNivel()
    {
        SceneManager.LoadScene("Level1");
    }
}
