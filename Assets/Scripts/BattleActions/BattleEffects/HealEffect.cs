using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Scriptable Objects/Battle Effects/Heal Effect")]
public class HealEffect : BattleEffect
{
    public override void ExecuteEffect(AttackData attackData, Card objectiveCard, Card userCard)
    {
        CardHealth userCardHealth = userCard.cardHealth;
        
        // Assuming the attackData contains a power value for healing
        userCardHealth.Heal(attackData.power);
    }

}
