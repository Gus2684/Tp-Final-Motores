using UnityEngine;
using UnityEngine.SceneManagement;

public class Botoni : MonoBehaviour
{
    public string nombreEscena = "Menu";

    public void IrAlMenu()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
