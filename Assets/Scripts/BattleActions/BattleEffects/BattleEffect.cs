using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BattleEffect 
{
    public int power;
    public int Power => power;
    public abstract string EffectName { get; }
    public abstract void ExecuteEffect(Card user, List<Card> targets);
}
