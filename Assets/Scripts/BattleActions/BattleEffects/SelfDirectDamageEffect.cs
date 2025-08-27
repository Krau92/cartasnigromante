using UnityEngine;

[CreateAssetMenu(fileName = "Direct Damage Effect", menuName = "Scriptable Objects/Battle Effects/Direct Damage Effect")]
public class SelfDirectDamageEffect : BattleEffect
{

    public override void ExecuteEffect()
    {
        BattleManager.instance.ApplySelfDirectDamage();
    }
}
