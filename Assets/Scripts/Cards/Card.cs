using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    //Referencia a la cartaSO que genera esta carta
    [SerializeField] private CardSO cardSO;
    int power;
    //Getter de power
    public int Power => power;
    private List<AttackData> attacksList = new(); // Lista de ataques para la carta, cada uno avisa si es seleccionable
    private AttackData attack1;
    private AttackData attack2;

    public CardHealth cardHealth; // Referencia a la salud de la carta
    [SerializeField] private float zoomValue = 1.5f; // Valor de zoom para la carta
    [SerializeField] bool isEnemyCard; // Indica si la carta es del enemigo

    [Space(10)] //Espacio para separar visualmente


    //Referencias a los componentes de la carta para poder crearlas
    [Header("Card base images")]
    [SerializeField] private Image cardBase;
    // De momento no hay ningún caso en que se vea la parte trasera de la carta
    // [SerializeField] private Image cardBack;
    [SerializeField] private Image cardImage;

    [Header("Header components")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text PEText;
    [SerializeField] private TMP_Text PVText;
    [SerializeField] private SpriteRenderer tierImage;

    [Header("Attack components")]
    private CardAttacks cardAttacks; // Componente que maneja los ataques de la carta
    [Space(3)] //Espacio para separar visualmente

    [Header("Attack 1 components")]
    [SerializeField] private TMP_Text attack1NameText;
    [SerializeField] private TMP_Text attack1DamageText;
    [SerializeField] private TMP_Text attack1DescriptionText;
    [SerializeField] private Transform diceSpace1;

    [Header("Attack 2 components")]
    [SerializeField] private TMP_Text attack2NameText;
    [SerializeField] private TMP_Text attack2DamageText;
    [SerializeField] private TMP_Text attack2DescriptionText;
    [SerializeField] private Transform diceSpace2;

    //Propiedades de carta
    private bool isDead;
    public bool IsDead => isDead;  // Getter público para verificar si la carta está muerta

    private bool isZoomed = false; // Indica si la carta está ampliada
    public bool IsZoomed => isZoomed; // Getter público para verificar si la carta está ampliada

    void Start()
    {
        //Actualizar todos los componentes de la carta
        CreateCard();
    }

    //Método para crear la carta
    private void CreateCard()
    {
        cardAttacks = GetComponent<CardAttacks>();
        //Asignar los valores de la cartaSO a los componentes de la carta
        cardNameText.text = cardSO.CardName;
        PEText.text = "PE: " + cardSO.BasePE.ToString();
        PVText.text = "PV: " + cardSO.BasePV.ToString();
        tierImage.sprite = cardSO.CardTier;
        cardImage.sprite = cardSO.CardImage;
        cardBase.sprite = cardSO.CardBase;
        power = cardSO.Power;

        InitializeAttackList();
        UpdateAttackInfo();
        cardAttacks.SetCardAttacks();
        cardAttacks.DisableColliders();

        isDead = false; // Inicializar el estado de muerte de la carta
        isZoomed = false; // Inicializar el estado de zoom de la carta

        //Reiniciar el color de los ataques
        UnmarkAttacks();
    }

    //Método para inicializar ataques
    void InitializeAttackList()
    {
        attacksList.Clear();
        foreach (AttackSO attack in cardSO.GetAttacksList())
        {
            AttackData attackData = new AttackData();
            attackData.SetBattleAction(attack);
            attackData.InitializeAttackData(this);
            attacksList.Add(attackData);
        }
        attacksList[0].SetSelectable(true); // Marcar el primer ataque como seleccionable
        attacksList[1].SetSelectable(true); // Marcar el segundo ataque como seleccionable

        attack1 = attacksList[0];
        attack2 = attacksList[1];
    }

    void UpdateAttackInfo()
    {
        attack1.InitializeAttackData(this);
        attack2.InitializeAttackData(this);

        //Asignar los valores de los ataques
        attack1NameText.text = attack1.attackName;
        attack1DamageText.text = attack1.power.ToString();
        attack1DescriptionText.text = attack1.description;

        attack2NameText.text = attack2.attackName;
        attack2DamageText.text = attack2.power.ToString();
        attack2DescriptionText.text = attack2.description;

        //Crear los dados para cada ataque
        CreateDice(attack1, diceSpace1);
        CreateDice(attack2, diceSpace2);
    }

    //Método para crear los dados de un ataque
    private void CreateDice(AttackData attack, Transform diceSpace)
    {
        //Limpiar los dados del espacio
        DiceManager.instance.ClearDices(diceSpace);

        //Crear los dados para el ataque
        DiceManager.instance.CreateCardAttackDices(diceSpace, attack.valorDados);
    }

    //Método para desplazar la carta a una posición específica
    public void MoveCardToPosition(Vector3 position)
    {
        //Mover la carta a la posición especificada
        StartCoroutine(MoveCardTo(position));
    }

    //Corutina que mueve la carta a una posición específica
    private IEnumerator MoveCardTo(Vector3 position)
    {
        //Desactivar la colisión de la carta mientras se mueve
        GetComponent<BoxCollider2D>().enabled = false;

        //Mover la carta a la posición especificada
        while (Vector3.Distance(transform.position, position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * 2000f);
            yield return null;
        }
        transform.position = position;

        //Activar la colisión de la carta al finalizar el movimiento
        GetComponent<BoxCollider2D>().enabled = true;
    }

    //Método para que la carta haga el efecto de zoom 
    public void ZoomCard()
    {
        transform.localScale = new Vector3(zoomValue, zoomValue, 1f);
        if (isZoomed)
            return; // Si ya está ampliada, no hacer nada

        if (isEnemyCard)
        {
            // Si es una carta del enemigo, moverla hacia abajo
            transform.position += new Vector3(0f, -20f, 0f);
        }
        else
        {
            // Si es una carta del jugador, moverla hacia arriba
            transform.position += new Vector3(0f, 20f, 0f);
        }
        isZoomed = true; // Marcar que la carta está ampliada
    }

    //Método para que la carta vuelva a su tamaño original
    public void UnzoomCard()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        if (!isZoomed)
            return; // Si no está ampliada, no hacer nada

        if (isEnemyCard)
        {
            // Si es una carta del enemigo, moverla hacia arriba
            transform.position += new Vector3(0f, 20f, 0f);
        }
        else
        {
            // Si es una carta del jugador, moverla hacia abajo
            transform.position += new Vector3(0f, -20f, 0f);
        }
        isZoomed = false; // Marcar que la carta ya no está ampliada
    }

    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead; // Actualizar el estado de muerte de la carta
        if (isDead)
        {
            // Si la carta está muerta, desactivar su colisión
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            // Si la carta no está muerta, activar su colisión
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }


    //Método para cambiar el color del texto de un ataque
    public void ChangeAttackTextColor(int attackNumber, Color color)
    {
        if (attackNumber == 1)
        {
            attack1NameText.color = color;
            attack1DamageText.color = color;
            attack1DescriptionText.color = color;
        }
        else if (attackNumber == 2)
        {
            attack2NameText.color = color;
            attack2DamageText.color = color;
            attack2DescriptionText.color = color;
        }
    }

    //Método para desmarcar los dos ataques de la carta
    public void UnmarkAttacks()
    {
        ChangeAttackTextColor(1, Color.black);
        ChangeAttackTextColor(2, Color.black);
    }

    //Método para marcar un ataque de la carta
    public void MarkAttack(int attackNumber)
    {
        if (attackNumber == 1)
        {
            ChangeAttackTextColor(1, Color.red);
        }
        else if (attackNumber == 2)
        {
            ChangeAttackTextColor(2, Color.red);
        }
    }

    //Método para obtener un ataque de la carta
    public AttackData GetAttack(int attackNumber)
    {
        if (attackNumber == 1)
        {
            return attack1;
        }
        else if (attackNumber == 2)
        {
            return attack2;
        }
        else
        {
            return null; // Retorna null si el número de ataque es inválido
        }
    }

    //Método para obtener la vida máxima de la carta
    public int GetMaxPV()
    {
        return cardSO.BasePV;
    }

    //Método para obtener los PE base de la carta
    public int GetBasePE()
    {
        return cardSO.BasePE;
    }


    //Llamar a evento cuando se hace click en la carta
    void OnMouseDown()
    {
        // Invocar el evento pasando esta carta como parámetro
        EventManager.instance.CardClicked(this);
    }

    void OnMouseEnter()
    {
        EventManager.instance.HoverCard(this);
    }

    void OnMouseExit()
    {
        EventManager.instance.UnhoverCard(this);
    }


}
