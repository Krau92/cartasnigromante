using System;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Scriptable Objects/Battle Effects/Damage Effect")]
public class DamageEffect : BattleEffect
{

    public override void ExecuteEffect(AttackData attackData, Card objectiveCard, Card userCard)
    {
        CardHealth objectiveCardHealth = objectiveCard.cardHealth;
        
        objectiveCardHealth.TakeDamage(attackData.power);
    }
}
