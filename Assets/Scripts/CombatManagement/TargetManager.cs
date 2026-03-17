using System.Collections.Generic;
using UnityEngine;

public class TargetManager
{
    //Método para obtener las cartas objetivo en base al tipo de objetivo del ataque
    public List<Card> GetTargetCards(AttackData attackData)
    {
        List<Card> targetCards = new();

        switch (attackData.targetType)
        {
            case TargetTypes.Self:
                //Se devuelve la lista vacía para evitar que el ataque se ejecute dos veces
                break;

            case TargetTypes.Enemy:
                if (BattleManager.instance.selectedObjectiveCard != null &&
                   BattleManager.instance.selectedObjectiveCard.isEnemyCard)
                {
                    targetCards.Add(BattleManager.instance.selectedObjectiveCard);
                }
                break;

            case TargetTypes.Ally:
                if (BattleManager.instance.selectedObjectiveCard != null &&
                   !BattleManager.instance.selectedObjectiveCard.isEnemyCard)
                {
                    targetCards.Add(BattleManager.instance.selectedObjectiveCard);
                }
                break;

            case TargetTypes.AllAllies:
                List<Card> alliesCards = DeckManager.instance.GetPlayerDeck();
                foreach (var card in alliesCards)
                {
                    targetCards.Add(card);
                }
                break;

            case TargetTypes.AllEnemies:
                List<Card> enemyCards = DeckManager.instance.GetEnemyDeck();
                foreach (var card in enemyCards)
                {
                    targetCards.Add(card);
                }
                break;

            case TargetTypes.All:
                List<Card> allCards = new();
                allCards.AddRange(DeckManager.instance.GetPlayerDeck());
                allCards.AddRange(DeckManager.instance.GetEnemyDeck());
                foreach (var card in allCards)
                {
                    targetCards.Add(card);
                }
                break;

            default:
                targetCards.Add(BattleManager.instance.selectedObjectiveCard);

                break;

        }

        return targetCards;
    }
}