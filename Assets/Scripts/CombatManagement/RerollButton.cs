using UnityEngine;

public class RerollButton : MonoBehaviour
{
    public void OnRerollButtonClick()
    {
        BattleManager.instance.RerollDices();
    }
}
