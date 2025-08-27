using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //! FALTA LA LOGICA DE CREAR LOS DECKS + METODOS DE DESBLOQUEAR CARTAS
    //! Tal vez todo esto va en otra clase llamada diferente. Algo así como Deck Data??
    //? SEPARAR DECKS POR TIERS?
    [SerializeField] private List<CardSO> completeDeckSO;
    [SerializeField] private List<Card> completeDeck;


    [SerializeField] private List<Card> unlockedDeck;

    //Decks de cada combate. gestión de los decks de jugador y enemigo
    [SerializeField] private List<Card> playerDeck;
    [SerializeField] private List<Card> enemyDeck;

    [SerializeField] private CardPositions cardPositions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartFirstPlacement();

    }

    //Método para llamar la corrutina de la primera colocación de las cartas
    public void StartFirstPlacement()
    {
        StartCoroutine(FirstPlacement());
    }

    //Método para llamar a la corrutina que mueve las cartas del jugador a su posición inicial
    public void StartMovePlayerCardsToInitialPosition()
    {
        StartCoroutine(MovePlayerCardsToInitialPosition());
    }

    //Método para llamar a la corrutina que mueve las cartas del enemigo a su posición inicial
    public void StartMoveEnemyCardsToInitialPosition()
    {
        StartCoroutine(MoveEnemyCardsToInitialPosition());
    }

    //Corrutina para la primera colocación de las cartas
    IEnumerator FirstPlacement()
    {
        StartMovePlayerCardsToInitialPosition();
        yield return new WaitForSeconds(2.25f);
        StartMoveEnemyCardsToInitialPosition();

    }

    //Método para mover las cartas del jugador a su posición inicial
    IEnumerator MovePlayerCardsToInitialPosition()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            playerDeck[i].transform.position = new Vector3(0f, -500f, 0f);
            playerDeck[i].UnzoomCard(); // Asegurarse de que las cartas vuelvan a su tamaño original
        }
        for (int i = 0; i < playerDeck.Count; i++)
        {
            playerDeck[i].MoveCardToPosition(cardPositions.GetPlayerCardPosition(playerDeck, i));
            yield return new WaitForSeconds(0.5f); // Añadir un pequeño retraso para que se vea el movimiento
        }
    }

    //Método para mover las cartas del enemigo a su posición inicial
    IEnumerator MoveEnemyCardsToInitialPosition()
    {
        for (int i = 0; i < enemyDeck.Count; i++)
        {
            enemyDeck[i].transform.position = new Vector3(0f, 500f, 0f);
            enemyDeck[i].UnzoomCard(); // Asegurarse de que las cartas vuelvan a su tamaño original
        }
        for (int i = 0; i < enemyDeck.Count; i++)
        {
            enemyDeck[i].MoveCardToPosition(cardPositions.GetEnemyCardPosition(enemyDeck, i));
            yield return new WaitForSeconds(0.5f); // Añadir un pequeño retraso para que se vea el movimiento
        }
    }

    //Método para obtener el mazo del jugador
    public List<Card> GetPlayerDeck()
    {
        return playerDeck;
    }

    //Método para obtener el mazo del enemigo
    public List<Card> GetEnemyDeck()
    {
        return enemyDeck;
    }

}
