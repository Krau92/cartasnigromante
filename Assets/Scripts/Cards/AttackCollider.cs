using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private CardAttacks cardAttacks; // Referencia al script de ataques de la carta
    [SerializeField] private int attackNumber; // Número del ataque (1 o 2)

    private void Start()
    {
        // Verificar que las referencias están asignadas
        if (cardAttacks == null)
        {
            Debug.LogError($"CardAttacks no está asignado en {gameObject.name}", this);
        }

        // Validar que attackNumber está en el rango correcto
        if (attackNumber < 1 || attackNumber > 2)
        {
            Debug.LogError($"AttackNumber debe ser 1 o 2. Valor actual: {attackNumber} en {gameObject.name}", this);
        }
    }

    private void OnMouseDown()
    {
        // Verificar que cardAttacks existe antes de llamar al método
        if (cardAttacks != null && attackNumber >= 1 && attackNumber <= 2)
        {
            cardAttacks.SelectAttack(attackNumber);
        }
    }
}
