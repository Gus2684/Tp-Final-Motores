using UnityEngine;
using TMPro;

public class UI_ZombieCounter : MonoBehaviour
{
    public TextMeshProUGUI texto;

    void Update()
    {
        texto.text = "Zombies: " + ZombieCounter.instancia.zombiesMuertos.ToString();
    }
}
