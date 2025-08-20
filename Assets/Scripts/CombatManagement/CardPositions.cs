using System.Collections.Generic;
using UnityEngine;

public class CardPositions : MonoBehaviour
{
    [SerializeField] Transform playerInitialPosition;
    [SerializeField] Transform playerFinalPosition;

    [SerializeField] Transform enemyInitialPosition;
    [SerializeField] Transform enemyFinalPosition;

    //Método para calcular la posición de cada carta del jugador
    public Vector3 GetPlayerCardPosition(List<Card> cardList, int index)
    {
        float totalWidth = playerFinalPosition.position.x - playerInitialPosition.position.x;
        float espaceBetweenCards = totalWidth / (cardList.Count - 1);

        return new Vector3(
            playerInitialPosition.position.x + index * espaceBetweenCards,
            playerInitialPosition.position.y,
            playerInitialPosition.position.z
        );
    }

    //Método para calcular la posición de cada carta del enemigo
    public Vector3 GetEnemyCardPosition(List<Card> cardList, int index)
    {
        float totalWidth = enemyFinalPosition.position.x - enemyInitialPosition.position.x;
        float espaceBetweenCards = totalWidth / (cardList.Count - 1);

        return new Vector3(
            enemyInitialPosition.position.x + index * espaceBetweenCards,
            enemyInitialPosition.position.y,
            enemyInitialPosition.position.z
        );
    }
}
