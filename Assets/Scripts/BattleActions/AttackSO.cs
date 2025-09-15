using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle Action", menuName = "Scriptable Objects/Battle Action")]
public class AttackSO : ScriptableObject
{
    [SerializeField] private string attackName;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private TargetTypes targetType;
    [SerializeField] private List<int> valorDados = new List<int>();

    [SerializeReference] private List<BattleEffect> battleEffects = new List<BattleEffect>();
    
    [SerializeField] private List<StatusAilment> objectiveStatusAilments = new List<StatusAilment>();
    [SerializeField] private List<StatusAilment> userStatusAilments = new List<StatusAilment>();

    // Getters públicos de solo lectura
    public string AttackName => attackName;
    public string Description => description;
    public TargetTypes Target => targetType;
    public List<int> GetValorDados() { return valorDados; }
    public List<BattleEffect> GetBattleEffects() { return battleEffects; }
    public List<StatusAilment> GetObjectiveStatusAilments() { return objectiveStatusAilments; }
    public List<StatusAilment> GetUserStatusAilments() { return userStatusAilments; }
}