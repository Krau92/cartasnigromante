# Turn-Based Card Game Prototype

![Gameplay Demo](Readme_Images/Cartes.gif)

## Overview
A turn-based card combat prototype developed in **Unity (C#)**. This was my very first project on my own. As RPGs structure are complex I started to study about clean code and making scalable code. As it's an experimental prototype, there are both nice and improvable architecture decisions. It helped a lot to understand how to split gameplay logic into single responsibilities classes.
It also introduced myself on using polymorfism on videogame to adapt behaviours depending on the type of children classes injected. As it was a learning prototype I decided to leave this project to focus on smaller and affordable games.
It's important to know the background, when I started this project I had just 1 year of programming experience so it's more a testing about how to make clean code and what kind of possibilities have Unity in order to structure a complex habilities and side effects combat system have.

## Technical decisions
### Creating an easy to configure system to create cards
The first priority is to made the card creation easy for everyone who doesn't have programming experience, focused on the possibility that game designers could create and test cards without coding. The use of Data-Driven objects (ScriptableObjects in Unity) made it possible structuring all the information needed to create a card. It's been also used the same structure for the attacks assigned to the card. Each attack let the designer to put as effects as wanted and modify the power to balance the game.

**CODE EXAMPLE:**

```csharp
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
    [SerializeField] private int powerModifier;
    [SerializeField] private List<AttackSO> attacksList; 

    // Propiedades públicas de solo lectura para acceder a los datos
    public string CardName => cardName;
    public int BasePE => basePE;
    public int BasePV => basePV;
    public int PowerModifier => powerModifier;
    public Sprite CardTier => cardTier;
    public Sprite CardImage => cardImage;
    // public Sprite CardBack => cardBack;
    public Sprite CardBase => cardBase;
    public List<AttackSO> GetAttacksList() { return attacksList; }
}
```

## Using polymorfism and heritance to make the code scalable
In order to make this easy card creation work and keep the gameplay logic robust I used a system of heritance and polymorfism. Using this each attack can be configure with as many effects as the designer wants.

**POLYMORFISM EXAMPLE**
```csharp
using System;
using System.Collections.Generic;

[Serializable]
public abstract class BattleEffect 
{
    public int power;
    public int Power => power;
    public abstract string EffectName { get; }
    public abstract void ExecuteEffect(Card user, List<Card> targets);
}
```

```chsarp
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class ObjectiveDamageEffect : BattleEffect
{
    public override string EffectName => "Standard Damage";
    public override void ExecuteEffect(Card user, List<Card> targets)
    {
        foreach (Card target in targets)
        {
            if (target != null && !target.IsDead)
            {
                int damage = power;
                target.cardHealth.TakeDamage(damage);
            }
        }
    }
}
```

With this example, every battle effect can be assigned and called its effect with ExecuteEffect(Card user, List<Card> targets).

## Future improvements
* **Targeting Logic (Strategy Pattern):** The `TargetManager` currently uses a massive `switch` statement to find valid targets.
* **Dependency Web:** The `BattleManager` and `DeckManager` are coupled via instances. I would use the `EventManager` or Dependency Injection to decouple the turn sequence from the deck management.
* **Hardcoded Values:** Remove magic numbers on the initial card place and replace them with relative anchors.