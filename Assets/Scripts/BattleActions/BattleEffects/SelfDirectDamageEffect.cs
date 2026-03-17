using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SelfDirectDamageEffect : BattleEffect
{
    public override string EffectName => "Self Direct Damage";
    public override void ExecuteEffect(Card user, List<Card> targets)
    {
        if (user != null && !user.IsDead)
        {
            int damage = power;
            user.cardHealth.TakeDirectDamage(damage);
        }
    }
}
