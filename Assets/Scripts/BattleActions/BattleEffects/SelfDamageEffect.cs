using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SelfDamageEffect : BattleEffect
{
    public override string EffectName => "Self Damage";
    public override void ExecuteEffect(Card user, List<Card> targets)
    {
        if (user != null && !user.IsDead)
        {
            int damage = power;
            user.cardHealth.TakeDamage(damage);
        }
    }
}
