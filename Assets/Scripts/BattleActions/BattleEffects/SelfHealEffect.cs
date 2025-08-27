using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Self Heal Effect", menuName = "Scriptable Objects/Battle Effects/Self Heal Effect")]
public class SelfHealEffect : BattleEffect
{
    public override void ExecuteEffect()
    {
        BattleManager.instance.ApplySelfHeal();
    }
    

}
