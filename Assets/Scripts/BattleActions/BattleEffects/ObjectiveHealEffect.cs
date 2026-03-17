using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ObjectiveHealEffect : BattleEffect
{
    public override string EffectName => "ObjectiveHeal";
    public override void ExecuteEffect(Card user, List<Card> targets)
    {
        foreach (Card target in targets)
        {
            if (target != null && !target.IsDead)
            {
                int healAmount = power;
                target.cardHealth.Heal(healAmount);
            }
        }
    }

}
