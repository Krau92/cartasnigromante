using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Objective Heal Effect", menuName = "Scriptable Objects/Battle Effects/Objective Heal Effect")]
public class ObjectiveHealEffect : BattleEffect
{
    public override void ExecuteEffect()
    {
        BattleManager.instance.ApplyHealObjectivesDamage();
    }

}
