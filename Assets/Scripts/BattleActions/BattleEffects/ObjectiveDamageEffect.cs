using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class ObjectiveDamageEffect : BattleEffect
{
    public override string EffectName => "Standard Damage";
    public override void ExecuteEffect(Card user, List<Card> targets)
    {
        foreach (Card target in targets)
        {
            if (target != null && !target.IsDead)
            {
                int damage = power;
                target.cardHealth.TakeDamage(damage);
            }
        }
    }
}
