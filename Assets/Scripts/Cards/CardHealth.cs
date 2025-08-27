using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CardHealth : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private CardBarAnimations cardBarAnimations;

    //Propiedades de la carta
    private int maxPV;
    private int currentPV;
    private int basePE;
    private int currentPE;


    private void Start()
    {
        if (card == null)
        {
            Debug.LogError("Card reference is not set in CardHealth.");
            return;
        }

        InitializeComponent();
    }

    //Método para inicializar los textos de las barras
    private void InitializeComponent()
    {
        maxPV = card.GetMaxPV();
        currentPV = maxPV;

        basePE = card.GetBasePE();
        currentPE = basePE;

        card.SetIsDead(false); // Asegurarse de que la carta no esté muerta al iniciar

        cardBarAnimations.maxPV = maxPV;
        cardBarAnimations.basePE = basePE;
        cardBarAnimations.InitializeComponent();

    }

    //Método para recargar la barra de PE a final de turno
    public void RechargePE()
    {
        basePE = card.GetBasePE();
        currentPE = basePE;
        UpdateBars();
    }

    //Método para actualizar las barras de vida y PE
    public void UpdateBars()
    {
        cardBarAnimations.UpdatePVBar(currentPV, maxPV);
        cardBarAnimations.UpdatePEBar(currentPE, basePE);
    }

    //Método para recibir daño
    public void TakeDamage(int damage)
    {
        int remainingDamage = damage;
        if (currentPE > 0)
        {
            remainingDamage -= currentPE;
            currentPE -= damage;
            remainingDamage = Mathf.Max(0, remainingDamage);
            currentPE = Mathf.Max(0, currentPE);
        }

        currentPV -= remainingDamage;
        if (currentPV < 0) currentPV = 0; // Asegurarse de que no sea negativo
        UpdateBars();
        CheckDeath();
    }

    //Método para recibir daño directo
    public void TakeDirectDamage(int damage)
    {
        currentPV -= damage;
        if (currentPV < 0) currentPV = 0; // Asegurarse de que no sea negativo
        UpdateBars();
        CheckDeath();
    }

    //Método para recuperar vida
    public void Heal(int amount)
    {
        currentPV += amount;
        if (currentPV > maxPV) currentPV = maxPV; // Asegurarse de que no supere el máximo
        UpdateBars();
    }

    //Método para comprobar si la carta ha muerto
    private void CheckDeath()
    {
        if (currentPV <= 0)
        {
            Debug.Log("Card has died.");
            card.SetIsDead(true); // Marcar la carta como muerta
            //! Tal vez añadir el evento de muerte de la carta aquí para comprobar el final del combate
        }
    }

}
