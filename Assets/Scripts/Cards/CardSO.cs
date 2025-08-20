using System.Collections.Generic;
using UnityEngine;
//Ponerlo el primero del menu conceptual
[CreateAssetMenu(fileName = "New Card", menuName = "Scriptable Objects/Card", order = -1000)]
public class CardSO : ScriptableObject
{
    //Atributos de la carta, editables en el editor pero de solo lectura desde código
    [Header("Header Attributes")]
    [SerializeField] private string cardName;
    [SerializeField] private int basePE;
    [SerializeField] private int basePV;

    [Header("Images")]
    [SerializeField] private Sprite cardTier;
    [SerializeField] private Sprite cardImage;
    [SerializeField] private Sprite cardBase;
    // [SerializeField] private Sprite cardBack;

    [Header("Attacks")]
    [SerializeField] private int power;
    [SerializeField] private List<AttackSO> attacksList; //Esta lista deben ser los ataques SO

    // Propiedades públicas de solo lectura para acceder a los datos
    public string CardName => cardName;
    public int BasePE => basePE;
    public int BasePV => basePV;
    public int Power => power;
    public Sprite CardTier => cardTier;
    public Sprite CardImage => cardImage;
    // public Sprite CardBack => cardBack;
    public Sprite CardBase => cardBase;
    public List<AttackSO> GetAttacksList() { return attacksList; }
}
