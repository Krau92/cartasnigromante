using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SelfHealEffect : BattleEffect
{
    public override string EffectName => "Self Heal";
    public override void ExecuteEffect(Card user, List<Card> targets)
    {
        if (user != null && !user.IsDead)
        {
            int healAmount = power;
            user.cardHealth.Heal(healAmount);
        }
    }


}
