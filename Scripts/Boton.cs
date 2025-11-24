using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonIrNivel : MonoBehaviour
{
    public string nombreEscena = "Intro";

    public void IrAlNivel()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
