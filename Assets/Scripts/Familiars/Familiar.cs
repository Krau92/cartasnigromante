using UnityEngine;
using System.Collections.Generic;

public class Familiar : MonoBehaviour
{
    public List<AttackData> attacks; //Lista de los ataques asignados a la carta
    [SerializeReference]public List<Passives> passives; //Lista de las pasivas asignadas a la carta

    void OnEnable()
    {
        //Suscribirse al evento de ataque realizado
        EventManager.instance.OnAttackExecuted += CheckPassives;
    }
    void OnDisable()
    {
        //Desuscribirse del evento de ataque realizado
        EventManager.instance.OnAttackExecuted -= CheckPassives;
    }

    private void CheckPassives()
    {
        foreach (var passive in passives)
        {
            passive.PassiveEffect();
        }
    }
}
