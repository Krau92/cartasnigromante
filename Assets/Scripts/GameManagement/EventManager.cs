using System;
using UnityEngine;

//Prioridad alta para que se cargue antes que otros scripts
[DefaultExecutionOrder(-100)]
public class EventManager : MonoBehaviour
{
    // Singleton para poder suscribirse a eventos desde cualquier parte del código
    public static EventManager instance;
    private void Awake()
    {
        // Asegurarse de que solo haya una instancia de EventManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Evento para manejar el click en una carta
    public event Action<Card> OnCardClicked;

    public void CardClicked(Card card)
    {
        // Invocar el evento de click en la carta
        OnCardClicked?.Invoke(card);
    }

    // Evento para manejar el hover sobre una carta
    public event Action<Card> OnCardHovered;
    public void HoverCard(Card card)
    {
        // Invocar el evento de hover sobre la carta
        OnCardHovered?.Invoke(card);
    }

    // Evento para manejar el unhover sobre una carta
    public event Action<Card> OnCardUnhovered;
    public void UnhoverCard(Card card)
    {
        // Invocar el evento de unhover sobre la carta
        OnCardUnhovered?.Invoke(card);
    }

    //Evento para manejar cuando se deselecciona una carta
    public event System.Action OnCardDeselected;
    public void DeselectCard()
    {
        // Invocar el evento de deselección de la carta
        Debug.Log("Deseleccionando carta");
        OnCardDeselected?.Invoke();
    }

    // Evento para manejar el click en un ataque
    public event Action<AttackData> OnAttackSelected;
    public void AttackSelected(AttackData attack)
    {
        // Invocar el evento de selección de ataque
        OnAttackSelected?.Invoke(attack);
    }

    //Evento para manejar la selección de dados
    public event Action<Dice> OnDiceSelected;
    public void DiceSelected(Dice dice)
    {
        // Invocar el evento de selección de dado
        OnDiceSelected?.Invoke(dice);
    }
}
