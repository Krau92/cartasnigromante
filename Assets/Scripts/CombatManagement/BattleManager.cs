using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //private DeckManager deckManager; // Referencia al DeckManager NO SE ESTA USANDO DE MOMENTO
    private TargetManager targetManager = new(); // Instancia del TargetManager
    public Card selectedCard { get; private set; } // Carta seleccionada
    public AttackData selectedAttack { get; private set; } // Ataque seleccionado
    [SerializeField] private int totalRerolls; // Cantidad total de rerolls
    public int availableRerolls { get; private set; } // Cantidad de rerolls disponibles

    public Card selectedObjectiveCard { get; private set; } // Carta objetivo
    [SerializeField] private List<Dice> selectedDices; // Dados seleccionados

    //Manager del ataque
    private AttackData executedAttack;
    private Card casterCard;
    private List<Card> objectivesCards = new();
    //private List<Dice> usedDices = new(); //? De momento solo se añadirá uno cada vez hasta que se implementen ataques diferentes

    private BattlePhases battlePhase; // Fase de batalla actual
    private PlayerPhases playerPhase; // Fase de jugador actual

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
        //deckManager = FindAnyObjectByType<DeckManager>();  //DE MOMENTO NO SE USA
        ResetAttackManaging(); // Asegurarse de que no haya ataques seleccionados al iniciar
        StartBattle(); // Iniciar la batalla al comienzo del juego
    }



    #region Managing player attack

    //Método para seleccionar una carta
    public void SelectCard(Card card)
    {
        if (battlePhase != BattlePhases.PlayerAttack || playerPhase != PlayerPhases.Idle)
            return;


        if (!card.isEnemyCard)
        {
            selectedCard = card;
            ChangePlayerPhase(PlayerPhases.CardSelected);
        }

    }

    //Método para seleccionar un ataque
    public void SelectAttack(AttackData attack)
    {
        if (battlePhase != BattlePhases.PlayerAttack || playerPhase != PlayerPhases.CardSelected)
            return;
        selectedAttack = attack;
        ChangePlayerPhase(PlayerPhases.AttackSelected);
    }

    //Método para seleccionar un dado
    public void SelectDice(Dice dice)
    {
        //Si no se está en la fase de ataque del jugador, no se puede seleccionar ningún dado
        if (battlePhase != BattlePhases.PlayerAttack || !(playerPhase == PlayerPhases.AttackSelected || playerPhase == PlayerPhases.Idle))
            return;

        //Permitir seleccionar dados para rerrollear si no hay ataque seleccionado
        if (playerPhase == PlayerPhases.Idle && availableRerolls != 0)
        {
            if (dice.Used) return; // Si el dado ya ha sido usado, no hacer nada

            //Gestión de bloqueo/desbloqueo del dado
            if (dice.Locked) dice.UnlockDice();
            else if (!dice.Locked) dice.LockDice();

            selectedDices.Clear(); // Limpiar la referencia al dado seleccionado para evitar problemas. Aunque ya debería estar limpia
            return;
        }
        //if(el ataque es de un solo dado)
        //Si ya hay un dado seleccionado, se deselecciona para cambiarlo por el siguiente
        if (selectedDices.Count > 0)
            selectedDices[0].DeselectDice(); // Reiniciar el dado seleccionado

        selectedDices.Clear(); // Limpiar la lista de dados seleccionados
        selectedDices.Add(dice); // Asignar el dado seleccionado
        selectedDices[0].MarkAsSelected(); // Marcar el dado como seleccionado

        //!DESCOMENTAR SI SE IMPLEMENTAN ATAQUES DE VARIOS DADOS
        /*else if(el ataque es de varios dados)
        {
            foreach(Dice dice in selectedDices)
            {
                if(dice == selectedDice) // Si el dado ya está seleccionado, deseleccionarlo
                {
                    dice.DeselectDice();
                    selectedDices.Remove(dice);
                    return;
                }
                selectedDices.Add(dice); // Añadir el dado a la lista de dados seleccionados
                dice.MarkAsSelected(); // Marcar el dado como seleccionado
            }
         }
         */

        CheckDiceAssignment(); // Comprobar si el dado seleccionado es válido
    }

    //Método para rerrollear los dados
    public void RerollDices()
    {
        if (availableRerolls > 0)
        {
            availableRerolls--; // Disminuir la cantidad de rerolls disponibles
            DiceManager.instance.RerollDices(); // Llamar al método de rerrollear dados en el DiceManager
            if (availableRerolls == 0)
                DiceManager.instance.UnlockAllDices(); // Desbloquear todos los dados si ya no quedan rerolls

        }
        else
        {
            Debug.Log("No hay rerolls disponibles.");
            //! Añadir lógica para notificar al jugador que no hay rerolls disponibles
        }
    }

    //Método para reiniciar los rerolls disponibles
    public void ResetRerolls()
    {
        availableRerolls = totalRerolls;
    }



    //Método para comprobar si el dado seleccionado es válido
    private void CheckDiceAssignment()
    {
        //if(ataque de un solo dado) {
        foreach (int diceValue in selectedAttack.valorDados)
        {
            if (selectedDices[0].value == diceValue)
            {
                ChangePlayerPhase(PlayerPhases.Targeting);
                SelectObjectiveCard(null); //Para que selecione automaticamente si el ataque no requiere de un objetivo específico
                return;
            }
        }
        DeselectDice(); // Si el dado no es válido, reiniciar la selección
        //!Añadir texto de aviso
        //}


        /* else if(ataque de varios dados)
        {
        int totalValue = 0;
        foreach(Dice dice in selectedDices)
        {
            totalValue += dice.value;
        }
        if(totalValue >= valor necesario)
            ChangePlayerPhase(PlayerPhases.Targeting);
            SelectObjectiveCard(null); //Para que selecione automaticamente si el ataque no requiere de un objetivo específico

        }
        */


    }

    //Método para deseleccionar el dado
    public void DeselectDice()
    {
        if (selectedDices != null)
        {
            foreach (Dice dice in selectedDices)
            {
                dice.DeselectDice(); // Reiniciar el dado
            }
            selectedDices.Clear(); // Limpiar la lista de dados seleccionados
        }
    }

    //Método para seleccionar una carta objetivo
    public void SelectObjectiveCard(Card card)
    {
        if (playerPhase != PlayerPhases.Targeting)
            return;

        selectedObjectiveCard = card; // Asignar la carta objetivo

        objectivesCards.Clear(); // Limpiar la lista de cartas objetivo
        objectivesCards = targetManager.GetTargetCards(selectedAttack); // Obtener las cartas objetivo en base al tipo de objetivo del ataque
        AssignAttackData(selectedAttack, selectedCard);
        //? Método que simule el efecto de lanzar el ataque
    }

    //Método para asignar los datos correctos del ataque a ejecutar
    public void AssignAttackData(AttackData attack, Card caster)
    {
        executedAttack = attack;
        casterCard = caster;
    }

    //Método para ejecutar el ataque. Se ejecuta desde el botón de la interfaz
    public void ExecuteAttack()
    {
        StartCoroutine(ExecuteAttackCoroutine());
    }

    //Corutina para ejecutar el ataque con una pequeña pausa
    private IEnumerator ExecuteAttackCoroutine()
    {
        foreach (var dice in selectedDices)
        {
            dice.UseDice(); // Marcar el dado como usado
        }
        casterCard.UseCard(); // Marcar la carta como usada
        selectedAttack.ExecuteAction(casterCard, objectivesCards); // Ejecutar el ataque
        yield return new WaitForSeconds(1f); // Esperar 1 segundo antes de reiniciar las selecciones
        EventManager.instance.AttackExecuted(); // Notificar que el ataque ha sido ejecutado
        //! Más adelante, según las pasivas, devolver algún objeto con el attack executed?
        ResetAttackManaging(); // Reiniciar las selecciones después del ataque
    }


    //Método para deseleccionar todo
    public void ResetAttackManaging()
    {
        if (selectedCard != null)
            selectedCard.UnmarkAttacks(); // Desmarcar los ataques de la carta seleccionada

        if (selectedDices != null)
        {
            foreach (Dice dice in selectedDices)
            {
                dice.DeselectDice(); // Reiniciar el dado seleccionado
            }
            selectedDices.Clear(); // Limpiar la lista de dados seleccionados
        }

        selectedCard = null; // Limpiar la referencia a la carta seleccionada
        selectedAttack = null; // Limpiar la referencia al ataque seleccionado
        selectedObjectiveCard = null; // Limpiar la referencia a la carta objetivo
        objectivesCards.Clear(); // Limpiar la lista de cartas objetivo
        casterCard = null; // Limpiar la referencia a la carta lanzadora
        executedAttack = null; // Limpiar la referencia al ataque ejecutado
        GameManager.instance.UnzoomCard(); // Quitar el zoom de la carta
        ChangePlayerPhase(PlayerPhases.Idle); // Volver a la fase de espera
    }

    #endregion


    #region Battle phase management
    //Método para iniciar el combate
    public void StartBattle()
    {
        //Setear la primera fase de batalla
        battlePhase = BattlePhases.DeclaringEnemy;
        playerPhase = PlayerPhases.Idle;
        ExecutePhase();
    }

    //Método para ejecutar la fase de batalla actual
    private void ExecutePhase() //! Falta toda la lógica
    {
        switch (battlePhase)
        {
            case BattlePhases.DeclaringEnemy:
                Debug.Log("Enemy is declaring its attack.");
                StartCoroutine(DeclaringEnemyCoroutine()); // simular la declaración del enemigo

                break;

            case BattlePhases.PlayerAttack:
                //!Mostrar carteles de interfaz para cada fase
                ChangePlayerPhase(PlayerPhases.Idle);
                ResetAttackManaging(); // Reiniciar las selecciones al inicio de la fase de ataque del jugador
                ResetRerolls(); // Reiniciar los rerolls disponibles al inicio de la fase de ataque del jugador
                DiceManager.instance.ResetDices(); // Reiniciar todos los dados antes de rollear
                DiceManager.instance.RollAllGameDices();
                //! Mostrar botón de pasar de fase

                break;

            case BattlePhases.EnemyAttack:
                Debug.Log("Enemy attack phase");
                NextPhase();
                break;

            case BattlePhases.ApplyEffects:
                //Aquí se aplicarán todos los efectos de estado, en aliados y enemigos
                DeckManager.instance.ReactivatePlayerDeck(); // Reactivar todas las cartas del mazo del jugador antes de aplicar por si los enemigos te stunean ese turno
                Debug.Log("Applying effects phase");

                //Por último comprobar si se ha ganado o perdido la partida
                //!COMPROBAR VICTORIA O DERROTA

                NextPhase();
                break;
        }
    }

    //Corutina para simular la declaración del enemigo
    private IEnumerator DeclaringEnemyCoroutine()
    {
        //!IR MOSTRANDO LA DECLARACIÓN DEL ENEMIGO
        yield return new WaitForSeconds(2f); // Esperar 2 segundos
        NextPhase();
    }

    //Método para pasar a la siguiente fase de batalla
    public void NextPhase()
    {
        battlePhase++;
        if ((int)battlePhase > Enum.GetValues(typeof(BattlePhases)).Length - 1)
        {
            battlePhase = 0; // Volver a la primera fase si se ha superado la última
        }

        EventManager.instance.BattlePhaseChanged(battlePhase);
        ExecutePhase();
    }

    //Método para cambiar la fase del jugador
    public void ChangePlayerPhase(PlayerPhases newPhase)
    {
        playerPhase = newPhase;
        EventManager.instance.PlayerPhaseChanged(playerPhase); // Notificar el cambio de fase del jugador
    }
    #endregion
}
