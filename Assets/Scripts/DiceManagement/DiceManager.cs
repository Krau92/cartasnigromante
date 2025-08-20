using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    //Crear singleton para el DiceManager
    public static DiceManager instance;
    private void Awake()
    {
        //Si no hay una instancia, la creamos
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //!No tengo claro que quiero hacerlo persistente entre escenas
        }
        else
        {
            //Si ya hay una instancia, destruimos este objeto
            Destroy(gameObject);
        }
    }
    //Referencia al prefab del dado
    [SerializeField] private GameObject cardDicePrefab;
    [SerializeField] private GameObject gameDicePrefab;
    [SerializeField] private Transform diceSpace; //Espacio donde se generan los dados de juego
    private List<Dice> gameDices = new(); // Lista para almacenar los dados del juego
    [SerializeField] private int dicesAmount; //Cantidad de dados de juego
    [SerializeField] private List<Sprite> diceFaces;

    void Start()
    {
        CreateGameDices(dicesAmount);
    }

    //!TESTEO
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Si se pulsa la tecla R, se lanzan todos los dados
        {
            RollAllGameDices();
        }
    }

    //!FIN TESTEO

    //Método para crear un dado
    public void CreateCardAttackDices(Transform parent, List<int> values)
    {
        if (values == null || values.Count == 0)
        {
            Debug.LogWarning("No values provided for dice creation");
            return;
        }

        for (int i = 0; i < values.Count; i++)
        {
            CreateAndConfigureCardDice(parent, i, values.Count, values[i]);
        }
    }

    //Método que crea y configura cada dado de ataque de la carta
    private void CreateAndConfigureCardDice(Transform parent, int index, int totalCount, int value)
    {
        //Instanciar el dado
        GameObject dice = Instantiate(cardDicePrefab, parent);
        //dice.transform.localScale = new Vector3(0.75f, 0.75f, 1f); // Ajustar el tamaño del dado
        RectTransform diceRect = dice.GetComponent<RectTransform>();

        if (diceRect == null)
        {
            Debug.LogError("Dice prefab must have a RectTransform component");
            return;
        }

        //Configurar el dado basado en su posición
        ConfigureCardDicePosition(diceRect, index, totalCount);

        //Configurar el valor del dado (aquí puedes agregar la lógica para mostrar el valor)
        ConfigureDiceValue(dice, value);
    }

    //Método para configurar la posición del dado en la carta
    private void ConfigureCardDicePosition(RectTransform diceRect, int index, int totalCount)
    {
        Vector2 anchorPosition = GetDiceAnchorPosition(index, totalCount);

        //Configurar anchors y pivot
        diceRect.anchorMin = anchorPosition;
        diceRect.anchorMax = anchorPosition;
        diceRect.pivot = new Vector2(0.5f, 0.5f);
        diceRect.anchoredPosition = Vector2.zero;
    }

    //Método para obtener la posición del ancla del dado según su índice y el número total de dados
    private Vector2 GetDiceAnchorPosition(int index, int totalCount)
    {
        return index switch
        {
            0 when totalCount == 1 => new Vector2(0.5f, 1f),  // Centro superior para un solo dado
            0 => new Vector2(0f, 1f),                         // Esquina superior izquierda
            1 => new Vector2(1f, 1f),                         // Esquina superior derecha
            2 when totalCount == 3 => new Vector2(0.5f, 0f), // Centro inferior para 3 dados
            2 => new Vector2(0f, 0f),                         // Esquina inferior izquierda
            3 => new Vector2(1f, 0f),                         // Esquina inferior derecha
            _ => new Vector2(0.5f, 0.5f)                      // Centro por defecto
        };
    }

    //Método que asigna el sprite de la cara del dado según el valor
    private void ConfigureDiceValue(GameObject dice, int value)
    {
        dice.GetComponent<SpriteRenderer>().sprite = diceFaces[value - 1]; // Asignar la cara del dado según el valor
    }

    //Método para eliminar todos los dados de un espacio
    public void ClearDices(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    //Método para crear los dados de juego
    public void CreateGameDices(int amount)
    {
        ClearGameDices(); // Limpiar los dados existentes

        for (int i = 0; i < amount; i++)
        {
            GameObject dice = Instantiate(gameDicePrefab, diceSpace);
            gameDices.Add(dice.GetComponent<Dice>());
            float positionX = i / (float)(amount - 1); // Calcular la posición X basada en el índice
            Vector2 anchorPosition = new(positionX, 0); // Ajustar la posición de cada dado

            //Configurar anchors y pivot
            RectTransform diceRect = dice.GetComponent<RectTransform>();
            diceRect.anchorMin = anchorPosition;
            diceRect.anchorMax = anchorPosition;
            diceRect.pivot = new Vector2(0.5f, 0.5f);
            diceRect.anchoredPosition = Vector2.zero;
        }
    }

    //Método para limpiar los dados de juego
    public void ClearGameDices()
    {
        foreach (Dice dice in gameDices)
        {
            Destroy(dice.gameObject);
        }
        gameDices.Clear();
    }

    //Método para obtener la imagen de un dado según su valor
    public Sprite GetDiceFace(int value)
    {
        if (value < 1 || value > diceFaces.Count)
        {
            Debug.LogError("Invalid dice value: " + value);
            return null;
        }
        return diceFaces[value - 1]; // Retorna la cara del dado según el valor
    }

    //Método para rollear todos los dados de juego
    public void RollAllGameDices()
    {
        foreach (Dice dice in gameDices)
        {
            dice.RollDice();
        }
    }

    //Método para deseleccionar los dados que no esten usados
    public void DeselectUnusedDices()
    {
        foreach (Dice dice in gameDices)
        {
            if (!dice.Used)
            {
                dice.ResetDice(); // Reiniciar el dado si no ha sido usado
            }
        }
    }

}