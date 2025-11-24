using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerVictoria : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene("Level1");
    }
}
