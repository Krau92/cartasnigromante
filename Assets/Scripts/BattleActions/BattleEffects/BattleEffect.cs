using System;
using UnityEngine;

public abstract class BattleEffect : ScriptableObject
{
    public abstract void ExecuteEffect(AttackData attackData, Card objectiveCard, Card userCard);
}
