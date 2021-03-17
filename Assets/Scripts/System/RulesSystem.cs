using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

public class RulesSystem : MonoBehaviour
{
    public static RulesSystem instance;

    List<Func<IEnumerator>> actionsStartTurnEnum = new List<Func<IEnumerator>>();

    CombatController combatController;

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

    public void Start()
    {
        combatController = WorldSystem.instance.combatManager.combatController;
        actionsStartTurnEnum.Add(ResetRemainingEnergy);
        actionsStartTurnEnum.Add(DrawCardsNewTurn);

    }


    public void CarryOutCard(CardData cardData, CombatActor source, CombatActor target)
    {

        if (cardData.Damage.Value != 0)
        {  
            int damage = cardData.Damage.Times * CalculateDamage(cardData.Damage.Value, source, target);
            target.healthEffects.TakeDamage(damage);
        }
        for (int i = 0; i < cardData.Effects.Count; i++)
        {
            target.healthEffects.RecieveEffectNonDamageNonBlock(cardData.Effects[i]);
        }
    }


    public IEnumerator StartTurn()
    {
        Debug.Log("actionsStartTurnEnum: " + actionsStartTurnEnum.Count);
        for (int i = 0; i < actionsStartTurnEnum.Count; i++)
            yield return StartCoroutine(actionsStartTurnEnum[i].Invoke());

        Debug.Log("Leaving StartTurn Enum");
    }


    public IEnumerator ResetRemainingEnergy()
    {
        Debug.Log("Start Energy Reset");
        combatController.cEnergy = combatController.energyTurn;
        yield return new WaitForSeconds(1);
        Debug.Log("Leaving Energy Reset");
    }

    public IEnumerator DrawCardsNewTurn()
    {
        yield return StartCoroutine(combatController.DrawCards(combatController.drawCount));
    }


    public int CalculateDamage(int startingValue, CombatActor source, CombatActor target){

        float x = startingValue;
        foreach (Func<float, float> func in source.dealAttackMods)
        {
            Debug.Log("Modified damage for attacker");
            x = func(x);
        }
        foreach (Func<float, float> func in target.takeAttackMods)
        {
            Debug.Log("Modified damage for reciever");
            x = func(x);
        }

        return (int)x;
    }




}
