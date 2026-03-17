# Turn-Based Card Game Prototype

![Gameplay Demo](Readme_Images/Cartes.gif)

## Overview
A turn-based card combat prototype developed in **Unity (C#)**. This was my very first solo project. Since RPG structures are complex, I started studying clean code and how to make scalable architectures. As it's an experimental prototype, there are both solid and improvable architectural decisions. It helped me a lot to understand how to split gameplay logic into single-responsibility classes.

It also introduced me to using polymorphism in video games to adapt behaviors depending on the type of child classes injected. As it was a learning prototype, I decided to leave this project as is to focus on smaller and more manageable games.

It's important to know the context: when I started this project, I had just 1 year of programming experience. Therefore, it's more of an experiment on how to write clean code and explore what possibilities Unity offers to structure a combat system with complex abilities and side effects.

## Technical decisions
### Creating a designer-friendly card system
The first priority was to make card creation easy for anyone without programming experience, focusing on allowing game designers to create and test cards without coding. The use of Data-Driven objects (ScriptableObjects in Unity) made it possible to structure all the information needed to create a card. The same structure was also used for the attacks assigned to the card. This approach allows the designer to add as many effects as desired and modify their power to balance the game.

**CODE EXAMPLE:**

```csharp
using System.Collections.Generic;
using UnityEngine;

// Keep it at the top of the creation menu
[CreateAssetMenu(fileName = "New Card", menuName = "Scriptable Objects/Card", order = -1000)]
public class CardSO : ScriptableObject
{
    // Card attributes: editable in the inspector but read-only from code
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

    // Public read-only properties to access the data
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

## Using polymorphism and inheritance to make the code scalable
In order to make this easy card creation work and keep the gameplay logic robust, I used inheritance and polymorphism. This allows each attack to be configured with as many effects as the designer wants.

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

```csharp
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

With this setup, any battle effect can be assigned, and its logic can be triggered simply by calling `ExecuteEffect(Card user, List<Card> targets);`.

## Future improvements
* **Targeting Logic (Strategy Pattern):** The `TargetManager` currently uses a massive `switch` statement to find valid targets.
* **Dependency Web:** The `BattleManager` and `DeckManager` are coupled via instances. I would use the `EventManager` or Dependency Injection to decouple the turn sequence from the deck management.
* **Hardcoded Values:** Remove magic numbers on the initial card place and replace them with relative anchors.