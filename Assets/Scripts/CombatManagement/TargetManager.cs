using System.Collections.Generic;
using UnityEngine;

public class TargetManager
{
    //Método para obtener las cartas objetivo en base al tipo de objetivo del ataque
    public List<CardHealth> GetTargetCards(AttackData attackData)
    {
        List<CardHealth> targetCards = new();

        switch (attackData.targetType)
        {
            case TargetType.Self:
                //Se devuelve la lista vacía para evitar que el ataque se ejecute dos veces
                break;

            case TargetType.AllAllies:
                List<Card> alliesCards = DeckManager.instance.GetPlayerDeck();
                foreach (var card in alliesCards)
                {
                    targetCards.Add(card.cardHealth);
                }
                break;

            case TargetType.AllEnemies:
                List<Card> enemyCards = DeckManager.instance.GetEnemyDeck();
                foreach (var card in enemyCards)
                {
                    targetCards.Add(card.cardHealth);
                }
                break;

                case TargetType.All:
                List<Card> allCards = new();
                allCards.AddRange(DeckManager.instance.GetPlayerDeck());
                allCards.AddRange(DeckManager.instance.GetEnemyDeck());
                foreach (var card in allCards)
                {
                    targetCards.Add(card.cardHealth);
                }
                break;

            default:
                targetCards.Add(BattleManager.instance.GetSelectedCard().cardHealth);

                break;

        }

        return targetCards;
    }
}