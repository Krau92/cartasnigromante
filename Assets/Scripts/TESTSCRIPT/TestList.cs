using System.Collections.Generic;
using UnityEngine;

public class TestList : MonoBehaviour
{
    
    public AttackData pervertedAttackData;
    public List<CardHealth> targetCards;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            pervertedAttackData.targetType = TargetType.Self;
            targetCards = new TargetManager().GetTargetCards(pervertedAttackData);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pervertedAttackData.targetType = TargetType.AllAllies;
            targetCards = new TargetManager().GetTargetCards(pervertedAttackData);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            pervertedAttackData.targetType = TargetType.AllEnemies;
            targetCards = new TargetManager().GetTargetCards(pervertedAttackData);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            pervertedAttackData.targetType = TargetType.All;
            targetCards = new TargetManager().GetTargetCards(pervertedAttackData);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            pervertedAttackData.InitializeAttackData(DeckManager.instance.GetPlayerDeck()[0]);
        }
  

    }
}
