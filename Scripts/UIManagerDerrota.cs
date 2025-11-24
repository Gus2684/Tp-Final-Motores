using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerDerrota : MonoBehaviour
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
