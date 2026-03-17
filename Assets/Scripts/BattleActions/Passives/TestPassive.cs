using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class TestPassive : Passives
{
    public override string name => "Test Passive";
    public override void PassiveEffect()
    {
        List<Card> enemyDeck = DeckManager.instance.GetEnemyDeck();
        foreach (var card in enemyDeck)
        {
            if (card.cardHealth.CurrentPV < card.GetMaxPV())
            {
                Debug.Log("Passive activated on card: " + card);
            }
        }
    }
}

