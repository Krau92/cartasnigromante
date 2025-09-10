using UnityEngine;

public class TestDices : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Si se pulsa la tecla R, se lanzan todos los dados
        {
            DiceManager.instance.RollAllGameDices();
        }
    }
}