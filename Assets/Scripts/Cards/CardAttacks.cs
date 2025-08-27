using UnityEngine;

public class CardAttacks : MonoBehaviour
{
    [SerializeField] private BoxCollider2D attack1Collider;
    [SerializeField] private BoxCollider2D attack2Collider;
    AttackData[] attacks = new AttackData[2]; // Array para almacenar los ataques de la carta
    [SerializeField] private Card card;

    //Suscribirse a los eventos de la carta
    private void OnEnable()
    {
        if (card != null)
        {
            EventManager.instance.OnCardClicked += EnableColliders;
            EventManager.instance.OnCardDeselected += DisableColliders;
            EventManager.instance.OnCardDeselected += card.UnmarkAttacks; // Desmarca los ataques cuando se deselecciona la carta
        }
    }

    //Desuscribirse de los eventos de la carta
    private void OnDisable()
    {
        if (card != null)
        {
            EventManager.instance.OnCardClicked -= EnableColliders;
            EventManager.instance.OnCardDeselected -= DisableColliders;
            EventManager.instance.OnCardDeselected -= card.UnmarkAttacks; // Desmarca los ataques cuando se deselecciona la carta
        }
    }

    //Método que desactiva los colliders de los ataques
    public void DisableColliders()
    {
        if (attack1Collider != null)
        {
            attack1Collider.enabled = false;
        }
        if (attack2Collider != null)
        {
            attack2Collider.enabled = false;
        }
    }

    //Método que activa los colliders de los ataques
    public void EnableColliders(Card card)
    {
        if (card == this.card && card.IsZoomed) // Verifica si la carta está ampliada
        {
            if (attack1Collider != null)
            {
                attack1Collider.enabled = true;
            }
            if (attack2Collider != null)
            {
                attack2Collider.enabled = true;
            }
        }
    }

    //Método para gestionar la selección de ataque
    public void SelectAttack(int attackNumber)
    {
        if (BattleManager.instance.GetSelectedCard() != this.card)
        {
            return; // Si ya hay un ataque seleccionado, no hacer nada
        }

        EventManager.instance.AttackSelected(attacks[attackNumber - 1]); // Llama al evento de selección de ataque
        card.UnmarkAttacks(); // Desmarca los ataques de la carta
        card.MarkAttack(attackNumber); // Marca el ataque seleccionado
    }

    //Método para asignar los ataques
    public void SetCardAttacks()
    {
        if (card != null)
        {
            attacks[0] = card.GetAttack(1); // Asignar el primer ataque de la carta
            attacks[1] = card.GetAttack(2); // Asignar el segundo ataque de la carta
        }
    }

}
