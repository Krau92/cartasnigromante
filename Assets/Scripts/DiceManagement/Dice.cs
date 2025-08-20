using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour
{
    public int value; // Valor del dado
    Image diceImage; // Imagen del dado
    bool used;
    public bool Used => used; // Propiedad para saber si el dado ha sido usado
    bool locked; //Para bloquear el reroll
    [SerializeField] private Color selectedColor, unavailableColor, availableColor;

    private void Awake()
    {
        diceImage = GetComponent<Image>();
        value = 1; // Valor inicial del dado
        diceImage.sprite = DiceManager.instance.GetDiceFace(value); // Asignar la cara inicial del dado
    }

    //Método para rollear el dado
    public void RollDice()
    {
        if (locked || used) return; // Si el dado está bloqueado o ya se ha usado, no hacer nada
        StartCoroutine(RollDiceCoroutine());
    }

    private IEnumerator RollDiceCoroutine()
    {
        int randTime = Random.Range(4, 18); // Tiempo aleatorio en decimas de segundo
        for (int i = 0; i < randTime; i++)
        {
            value = Random.Range(1, 7); // Generar un valor aleatorio entre 1 y 6
            diceImage.sprite = DiceManager.instance.GetDiceFace(value); // Actualizar la imagen del dado
            yield return new WaitForSeconds(0.1f); // Esperar un tiempo antes de cambiar el valor nuevamente
        }
    }

    //Método para gastar el dado
    public void UseDice()
    {
        used = true; // Marcar el dado como usado
        diceImage.color = unavailableColor; // Cambiar el color del dado a no disponible
    }

    //Método para reiniciar el dado a su estado original
    public void ResetDice()
    {
        used = false; // Marcar el dado como no usado
        locked = false; // Desbloquear el dado
        diceImage.color = availableColor; // Cambiar el color del dado a disponible
    }

    //Método para deseleccionar el dado
    public void DeselectDice()
    {
        if (used) return; // Si el dado ya ha sido usado, no hacer nada
        diceImage.color = availableColor; // Cambiar el color del dado a disponible
        ResetDice(); // Reiniciar el estado del dado
    }

    //Método para seleccionar el dado
    public void SelectDice()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.DiceSelected(this); // Invocar el evento de selección de dado
        }
    }

    //Método para marcar el dado como seleccionado
    public void MarkAsSelected()
    {
        diceImage.color = selectedColor;
    }

    //Gestión del clic en el dado
    private void OnMouseDown()
    {
        if (EventManager.instance != null)
        {
            SelectDice(); // Llamar al método de selección del dado
        }
    }

}
