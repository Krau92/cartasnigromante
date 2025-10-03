using UnityEngine;
using System.Collections.Generic;

public class Familiar : MonoBehaviour
{
    [SerializeField] public FamiliarSO familiar; //ScriptableObject que contiene los datos del familiar
    public List<AttackData> attackList; //Lista de los ataques asignados a la carta
    public List<AttackData> attacks; //Lista de los ataques asignados a la carta
    [SerializeReference] public List<Passives> passives; //Lista de las pasivas asignadas a la carta

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

    //Método para inicializar la lista de ataques desde el ScriptableObject
    public void InitializeAttacks()
    {
        
    }
}
