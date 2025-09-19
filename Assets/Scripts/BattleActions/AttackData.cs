using System;
using System.Collections.Generic;
[Serializable]
public class AttackData
{
    AttackSO battleAction; // Referencia al ataque que se está ejecutando
    public int power;
    public string attackName;
    public string description;
    bool isSelectable;

    public TargetTypes targetType;
    public List<BattleEffect> battleEffects = new();

    public List<StatusAilment> objectiveStatusAilments = new();
    public List<StatusAilment> userStatusAilments = new();
    public List<int> valorDados; // Valores de los dados para el ataque

    public void ExecuteAction(Card user, List<Card> objectives)
    {
        //!FALTA APLICACIÓN DE MODIFICADORES AL ATTACK DATA

        foreach (var effect in battleEffects)
        {
            effect.ExecuteEffect(user, objectives);
        }

        //!PENSAR EN LA GESTIÓN DE ESTADOS ALTERADOS
        /*
        foreach (var ailment in objectiveStatusAilments)
        {
            ailment.ApplyAilment(objectiveCardHealth);
        }
        foreach (var ailment in userStatusAilments)
        {
            ailment.ApplyAilment(userCardHealth);
        }
        */
    }

    public void InitializeAttackData(Card userCard)
    {
        foreach(var battleEffect in battleEffects)
        {
            if(battleEffect is ObjectiveDamageEffect || battleEffect is ObjectiveDirectDamageEffect)
                power = battleEffect.Power + userCard.PowerModifier; // Asignar el poder del usuario al efecto de daño
            
        }
        attackName = battleAction.AttackName; // Asignar el nombre del ataque
        description = battleAction.Description; // Asignar la descripción del ataque
        battleEffects = battleAction.GetBattleEffects(); // Asignar los efectos del ataque
        objectiveStatusAilments = battleAction.GetObjectiveStatusAilments(); // Asignar los estados alterados del ataque
        userStatusAilments = battleAction.GetUserStatusAilments(); // Asignar los estados alterados del ataque
        valorDados = battleAction.GetValorDados(); // Asignar los valores de los dados del ataque
        targetType = battleAction.Target; // Asignar el tipo de objetivo del ataque
    }

    //Método para asignar el SO del ataque
    public void SetBattleAction(AttackSO battleAction)
    {
        this.battleAction = battleAction;
    }

    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
    }
}