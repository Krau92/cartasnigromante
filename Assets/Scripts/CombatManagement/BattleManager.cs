using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private DeckManager deckManager; // Referencia al DeckManager
    private Card selectedCard; // Carta seleccionada
    private AttackData selectedAttack; // Ataque seleccionado

    [SerializeField] private Card objectiveCard; // Carta objetivo
    [SerializeField] private Dice selectedDice; //? Dados seleccionados??? en plural?

    public enum BattlePhases //!Más adelante pensar donde gestiono los estados alterados (pueden ser dos fases diferentes, una para player otra para enemy)
    {
        DeclaringEnemy,
        PlayerAttack,
        EnemyAttack
    }

    private BattlePhases battlePhase; // Fase de batalla actual


    // Singleton para poder acceder al BattleManager desde cualquier parte del código
    public static BattleManager instance;
    private void Awake()
    {
        // Asegurarse de que solo haya una instancia de BattleManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Suscripciones a eventos
    private void OnEnable()
    {
        EventManager.instance.OnCardClicked += SelectCard;
        EventManager.instance.OnCardClicked += SelectObjectiveCard;
        EventManager.instance.OnCardDeselected += ResetAttackManaging;
        EventManager.instance.OnAttackSelected += SelectAttack;
        EventManager.instance.OnDiceSelected += SelectDice;
    }

    //Desuscripciones a eventos
    private void OnDisable()
    {
        EventManager.instance.OnCardClicked -= SelectCard;
        EventManager.instance.OnCardClicked -= SelectObjectiveCard;
        EventManager.instance.OnCardDeselected -= ResetAttackManaging;
        EventManager.instance.OnAttackSelected -= SelectAttack;
        EventManager.instance.OnDiceSelected -= SelectDice;
    }


    private void Start()
    {
        deckManager = FindAnyObjectByType<DeckManager>();
        ResetAttackManaging(); // Asegurarse de que no haya ataques seleccionados al iniciar
        StartBattle(); // Iniciar la batalla al comienzo del juego
    }



    #region Player attack managing

    //Método para seleccionar una carta
    public void SelectCard(Card card)
    {
        if (battlePhase != BattlePhases.PlayerAttack)
        {
            selectedCard = null; // Limpiar la referencia a la carta seleccionada para evitar problemas
            return;
        }

        //Si ya hay una carta seleccionada, se asigna la que está seleccionada al zoom
        if (GameManager.instance.GetZoomedCard() == null) // Si no hay carta con zoom
        {
            selectedCard = card; // Asignar la carta seleccionada
        }
        else
            selectedCard = GameManager.instance.GetZoomedCard(); // Obtener la carta que tiene el zoom
    }

    //Método para seleccionar un ataque
    public void SelectAttack(AttackData attack)
    {
        Debug.Log("Ataque seleccionado en battlemanager: " + attack.attackName);
        if (battlePhase != BattlePhases.PlayerAttack || selectedCard == null)
        {
            selectedAttack = null; // Limpiar la referencia al ataque seleccionado para evitar problemas
            return;
        }

        selectedAttack = attack;
    }

    //Método que devuelve si hay un ataque seleccionado
    public Card GetCardSelected()
    {
        return selectedCard;
    }

    //Método para seleccionar un dado
    public void SelectDice(Dice dice)
    {
        if (battlePhase != BattlePhases.PlayerAttack || selectedAttack == null)
        {
            selectedDice = null; // Limpiar la referencia al dado seleccionado para evitar problemas
            return;
        }

        if (dice.Used) // Si el dado ya ha sido usado, no hacer nada
            return;

        if (selectedDice != null)
            selectedDice.ResetDice(); // Reiniciar el dado seleccionado

        selectedDice = dice; // Asignar el dado seleccionado
        selectedDice.MarkAsSelected(); // Marcar el dado como seleccionado

        CheckDiceAssignment(); // Comprobar si el dado seleccionado es válido
    }

    //Método para comprobar si el dado seleccionado es válido
    private void CheckDiceAssignment()
    {
        if (selectedAttack == null || selectedDice == null)
        {
            Debug.LogWarning("No card or dice selected.");
            return;
        }

        foreach (int diceValue in selectedAttack.valorDados)
        {
            if (selectedDice.value == diceValue)
            {
                return;
            }
        }
        DeselectDice(); // Si el dado no es válido, reiniciar la selección
        //!Añadir texto de aviso
    }

    //Método para deseleccionar el dado
    public void DeselectDice()
    {
        if (selectedDice != null)
        {
            selectedDice.DeselectDice(); // Reiniciar el dado
            selectedDice = null; // Limpiar la referencia al dado seleccionado
        }
    }

    //Método para seleccionar una carta objetivo
    public void SelectObjectiveCard(Card card)
    {
        if (selectedAttack != null && selectedCard != null && selectedDice != null)
        {
            //!Falta checkeo de objetivo válido
            objectiveCard = card; // Asignar la carta objetivo
            ExecuteAttack(); // Ejecutar el ataque
        }

    }

    //Método para ejecutar el ataque
    void ExecuteAttack()
    {
        selectedDice.UseDice(); // Marcar el dado como usado
        selectedAttack.ExecuteAction(objectiveCard, selectedCard); // Ejecutar el ataque
        ResetAttackManaging(); // Reiniciar las selecciones después del ataque
        GameManager.instance.UnzoomCard(); // Quitar el zoom de la carta
    }


    //Método para deseleccionar todo
    public void ResetAttackManaging()
    {
        if (selectedCard != null)
            selectedCard.UnmarkAttacks(); // Desmarcar los ataques de la carta seleccionada

        if (selectedDice != null)
            DeselectDice(); // Desmarcar el dado seleccionado

        selectedCard = null; // Limpiar la referencia a la carta seleccionada
        selectedAttack = null; // Limpiar la referencia al ataque seleccionado
        objectiveCard = null; // Limpiar la referencia a la carta objetivo
    }

    #endregion

    #region Battle phase management
    //Método para iniciar el combate
    public void StartBattle()
    {
        //Setear la primera fase de batalla
        battlePhase = BattlePhases.DeclaringEnemy;
        ExecutePhase();
    }

    //Método para ejecutar la fase de batalla actual
    private void ExecutePhase() //! Falta toda la lógica
    {
        switch (battlePhase)
        {
            case BattlePhases.DeclaringEnemy:
                Debug.Log("Enemy is declaring its attack.");
                NextPhase();
                break;

            case BattlePhases.PlayerAttack:
                DiceManager.instance.RollAllGameDices();
                //! Mostrar botón de pasar de fase
                break;

            case BattlePhases.EnemyAttack:
                Debug.Log("Enemy attack phase");
                NextPhase();
                break;
        }
    }

    //Método para pasar a la siguiente fase de batalla
    public void NextPhase()
    {
        battlePhase++;
        ExecutePhase();
    }
    #endregion
}
