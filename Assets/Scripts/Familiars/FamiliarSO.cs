using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class FamiliarSO : ScriptableObject
{
    public string familiarName;
    [SerializeField] private List<AttackSO> attackSOs; //Lista de los ataques asignados a la carta
    [SerializeField] private List<Passives> passives; //Lista de las pasivas asignadas a la carta

    public List<AttackSO> GetAttacksList() { return attackSOs; }
    public List<Passives> GetPassivesList() { return passives; }
}
