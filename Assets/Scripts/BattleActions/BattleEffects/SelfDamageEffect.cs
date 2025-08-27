using UnityEngine;

[CreateAssetMenu(fileName = "Self Damage Effect", menuName = "Scriptable Objects/Battle Effects/Self Damage Effect")]
public class SelfDamageEffect : BattleEffect
{

    public override void ExecuteEffect()
    {
        BattleManager.instance.ApplySelfDamage();
    }
}
