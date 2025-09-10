using UnityEngine;

public class NextPhaseButton : MonoBehaviour
{
    //Método para pasar de la fase del jugador
    public void OnNextPhaseButtonClick()
    {
        BattleManager.instance.NextPhase();
    }

}
