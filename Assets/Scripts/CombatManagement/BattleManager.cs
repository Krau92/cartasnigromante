using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //!Añadir al manager de multiobjetivos que devuelva una lista de objetivos
    private DeckManager deckManager; // Referencia al DeckManager
    private Card selectedCard; // Carta seleccionada
    private AttackData selectedAttack; // Ataque seleccionado
    [SerializeField] private int totalRerolls; // Cantidad total de rerolls
    private int availableRerolls; // Cantidad disponible de rerolls
    public int AvailableRerolls => availableRerolls; // Propiedad para obtener la cantidad de rerolls disponibles

    [SerializeField] private Card selectedObjectiveCard; // Carta objetivo
    [SerializeField] private Dice selectedDice; //? Dados seleccionados??? en plural?

    //Manager del ataque
    private AttackData executedAttack;
    private Card casterCard;
    private CardHealth casterCardHealth;
    private List<CardHealth> objectivesCardsHealth = new();
    //private List<Dice> usedDices = new(); //? De momento solo se añadirá uno cada vez hasta que se implementen ataques diferentes

    public enum BattlePhases
    {
        DeclaringEnemy,
        PlayerAttack,
        EnemyAttack,
        ApplyEffects
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



    #region Managing player attack

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

    //Método para obtener el ataque seleccionado
    public AttackData GetSelectedAttack()
    {
        return selectedAttack;
    }

    //Método que devuelve si hay un ataque seleccionado
    public Card GetSelectedCard()
    {
        return selectedCard;
    }

    //Método para seleccionar un dado
    public void SelectDice(Dice dice)
    {
        //Si no se está en la fase de ataque del jugador, no se puede seleccionar ningún dado
        if (battlePhase != BattlePhases.PlayerAttack)
        {
            selectedDice = null; // Limpiar la referencia al dado seleccionado para evitar problemas
            return;
        }

        //Permitir seleccionar dados para rerrollear si no hay ataque seleccionado
        if (selectedAttack.attackName == null && availableRerolls != 0)
        {
            if (dice.Used) return; // Si el dado ya ha sido usado, no hacer nada

            //Gestión de bloqueo/desbloqueo del dado
            if (dice.Locked) dice.UnlockDice();
            else if (!dice.Locked) dice.LockDice();

            selectedDice = null; // Limpiar la referencia al dado seleccionado para evitar problemas
            return;
        }

        //Si no hay ataque seleccionado, no se puede seleccionar ningún dado
        if (selectedAttack.attackName == null) return; // Si no hay ataque seleccionado, no hacer nada

        //Si ya hay un dado seleccionado, se deselecciona para cambiarlo por el siguiente
        if (selectedDice != null)
            selectedDice.DeselectDice(); // Reiniciar el dado seleccionado

        selectedDice = dice; // Asignar el dado seleccionado
        selectedDice.MarkAsSelected(); // Marcar el dado como seleccionado

        CheckDiceAssignment(); // Comprobar si el dado seleccionado es válido
    }

    //Método para rerrollear los dados
    public void RerollDices()
    {
        if (availableRerolls > 0)
        {
            availableRerolls--; // Disminuir la cantidad de rerolls disponibles
            DiceManager.instance.RerollDices(); // Llamar al método de rerrollear dados en el DiceManager

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

    //Método para obtener el dado seleccionado
    public Dice GetSelectedDice()
    {
        return selectedDice;
    }

    //Método para comprobar si el dado seleccionado es válido
    private void CheckDiceAssignment()
    {
        if (selectedAttack == null || selectedDice == null)
        {
            Debug.LogWarning("No card or dice selected.");
            return;
        }

        //!Se deberá modificar cuando se añadan ataques de mas de un dado

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
            //!Falta checkeo de objetivo válido y cambiar la gestión de ejecución de ataque
            selectedObjectiveCard = card; // Asignar la carta objetivo
                                          //! De momento llamo directamente a asignar la lista de objetivos, más adelante saltar este paso si el objetivo es todos, etc.
            AssignAttackData(selectedAttack, selectedCard, objectivesCardsHealth);
            ExecuteAttack(); // Ejecutar el ataque
        }

    }

    //Método para asignar los datos correctos del ataque a ejecutar
    public void AssignAttackData(AttackData attack, Card caster, List<CardHealth> objectives)
    {
        executedAttack = attack;
        casterCard = caster;
        casterCardHealth = caster.cardHealth;
        objectivesCardsHealth = objectives;
    }

    //Método para ejecutar el ataque
    void ExecuteAttack()
    {
        selectedDice.UseDice(); // Marcar el dado como usado
        selectedAttack.ExecuteAction(casterCard); // Ejecutar el ataque
        ResetAttackManaging(); // Reiniciar las selecciones después del ataque
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
        selectedObjectiveCard = null; // Limpiar la referencia a la carta objetivo
        //usedDices.Clear(); // Limpiar la lista de dados usados
        objectivesCardsHealth.Clear(); // Limpiar la lista de salud de cartas objetivo
        casterCardHealth = null; // Limpiar la referencia a la salud de la carta lanzadora
        executedAttack = null; // Limpiar la referencia al ataque ejecutado
        GameManager.instance.UnzoomCard(); // Quitar el zoom de la carta
    }

    #endregion

    #region  Managing attack effects

    //Método para hacer daño directo a cartas (direct damage)
    public void ApplyObjectivesDirectDamage()
    {
        if (selectedObjectiveCard != null && selectedAttack != null)
        {
            objectivesCardsHealth.Clear();
            //!Obtener y asignar lista de objetivos con el manager de objetivos
            //objectivesCardHealth = objectivesManager.ObtainObjectives(selectedAttack);

            foreach (var objective in objectivesCardsHealth)
            {
                objective.TakeDirectDamage(selectedAttack.power);
            }
        }
    }

    //Método para hacer auto daño directo
    public void ApplySelfDirectDamage()
    {
        if (selectedCard != null && selectedAttack != null)
        {
            CardHealth selectedCardHealth = selectedCard.cardHealth;
            selectedCardHealth.TakeDirectDamage(selectedAttack.power);
        }
    }

    //Método para aplicar daño a cartas (daño basico)
    public void ApplyObjectivesDamage()
    {
        if (selectedObjectiveCard != null && selectedAttack != null)
        {
            objectivesCardsHealth.Clear();
            //!Obtener y asignar lista de objetivos con el manager de objetivos
            //objectivesCardHealth = objectivesManager.ObtainObjectives(selectedAttack);

            foreach (var objective in objectivesCardsHealth)
            {
                objective.TakeDamage(selectedAttack.power);
            }
        }
    }

    //Método para hacer daño a la carta seleccionada (selfdamage)
    public void ApplySelfDamage()
    {
        if (selectedCard != null && selectedAttack != null)
        {
            CardHealth selectedCardHealth = selectedCard.cardHealth;
            selectedCardHealth.TakeDamage(selectedAttack.power);
        }
    }

    //Método para curar daño a la carta objetivo (heal)
    public void ApplyHealObjectivesDamage()
    {
        if (selectedObjectiveCard != null && selectedAttack != null)
        {
            objectivesCardsHealth.Clear();
            //!Obtener y asignar lista de objetivos con el manager de objetivos
            //objectivesCardHealth = objectivesManager.ObtainObjectives(selectedAttack);

            foreach (var objective in objectivesCardsHealth)
            {
                objective.Heal(selectedAttack.power);
            }
        }
    }

    //Método para autocurarse
    public void ApplySelfHeal()
    {
        if (selectedCard != null && selectedAttack != null)
        {
            CardHealth selectedCardHealth = selectedCard.cardHealth;
            selectedCardHealth.Heal(selectedAttack.power);
        }
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
                StartCoroutine(DeclaringEnemyCoroutine()); // simular la declaración del enemigo
                
                break;

            case BattlePhases.PlayerAttack:
                //!Mostrar carteles de interfaz para cada fase
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

        ExecutePhase();
    }
    #endregion
}
