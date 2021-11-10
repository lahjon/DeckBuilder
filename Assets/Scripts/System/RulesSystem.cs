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
        actionsStartTurnEnum.Add(ResetRemainingEnergy);
        actionsStartTurnEnum.Add(DrawCardsNewTurn);

    }


    public IEnumerator StartTurn()
    {
        //Debug.Log("actionsStartTurnEnum: " + actionsStartTurnEnum.Count);
        for (int i = 0; i < actionsStartTurnEnum.Count; i++)
            yield return StartCoroutine(actionsStartTurnEnum[i].Invoke());

        //Debug.Log("Leaving StartTurn Enum");
    }


    public IEnumerator ResetRemainingEnergy()
    {
        //Debug.Log("Start Energy Reset");
        CombatSystem.instance.cEnergy = CombatSystem.instance.energyTurn;
        yield return new WaitForSeconds(1);
        //Debug.Log("Leaving Energy Reset");
    }

    public IEnumerator DrawCardsNewTurn()
    {
        yield return StartCoroutine(CombatSystem.instance.DrawCards(CombatSystem.instance.drawCount));
    }


    public int CalculateDamage(int startingValue, CombatActor source, CombatActor target){

        float x = startingValue;
        x += source.strengthCombat;
        x += source.GetStat(StatType.Strength);

        x = Mathf.Max(x, 0f);

        if (x == 0f) return 0;

        foreach (Func<float> func in source.dealAttackMult)
            x *= func();

        if (target != null)
            foreach (Func<float> func in source.dealAttackActorMods[target])
            x *= func();

        if(target != null)
            foreach (Func<float> func in target.takeAttackMult)
                x *= func();

        return (int)x;
    }




}
