using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject rerollButton;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject cancelSelectionButton;

    //Botón de mostrar reroll
    public void ShowRerollButton()
    {
        rerollButton.SetActive(true);
    }

    //Botón de mostrar end turn
    public void ShowEndTurnButton()
    {
        endTurnButton.SetActive(true);
    }

    //Botón de ocultar reroll
    public void HideRerollButton()
    {
        rerollButton.SetActive(false);
    }

    //Botón de ocultar end turn
    public void HideEndTurnButton()
    {
        endTurnButton.SetActive(false);
    }
    //Botón de mostrar cancel selection
    public void ShowCancelSelectionButton()
    {
        cancelSelectionButton.SetActive(true);
    }
    //Botón de ocultar cancel selection
    public void HideCancelSelectionButton()
    {
        cancelSelectionButton.SetActive(false);
    }

    void OnEnable()
    {
        EventManager.instance.OnPlayerPhaseChanged += HandlePlayerPhaseChanged;
        EventManager.instance.OnBattlePhaseChanged += HandleBattlePhaseChanged;
    }

    void OnDisable()
    {
        EventManager.instance.OnPlayerPhaseChanged -= HandlePlayerPhaseChanged;
        EventManager.instance.OnBattlePhaseChanged -= HandleBattlePhaseChanged;
    }

    //Metodo para manejar la UI según la fase de la batalla
    private void HandleBattlePhaseChanged(BattlePhases newPhase)
    {

    }

    //Método para gestionar la UI según la fase del jugador
    private void HandlePlayerPhaseChanged(PlayerPhases newPhase)
    {
        switch (newPhase)
        {
            case PlayerPhases.Idle:

                HideCancelSelectionButton();

                break;
        }
    }
}


