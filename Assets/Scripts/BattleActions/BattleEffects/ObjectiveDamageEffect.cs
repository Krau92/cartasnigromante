using UnityEngine;

[CreateAssetMenu(fileName = "Objective Damage Effect", menuName = "Scriptable Objects/Battle Effects/Objective Damage Effect")]
public class ObjectiveDamageEffect : BattleEffect
{

    public override void ExecuteEffect()
    {
        BattleManager.instance.ApplyObjectivesDamage();
    }
}
