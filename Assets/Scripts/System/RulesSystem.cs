using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

public class RulesSystem : MonoBehaviour
{
    public static RulesSystem instance;

    List<Action> actionsEndTurn = new List<Action>();
    List<Action> actionsStartTurn = new List<Action>();
    Dictionary<Func<IEnumerator>, int> ActionPriorities = new Dictionary<Func<IEnumerator>, int>();

    bool isRunningCoroutine = false;
    List<IEnumerator> actionsEndTurnEnumerator = new List<IEnumerator>();
    List<Func<IEnumerator>> actionsStartTurnEnum = new List<Func<IEnumerator>>();

    CombatController combatController;

    public Dictionary<CombatActorEnemy, List<Func<CombatActorEnemy, IEnumerator>>> enemyToActionsStartTurn = new Dictionary<CombatActorEnemy, List<Func<CombatActorEnemy, IEnumerator>>>();

    public Dictionary<CombatActor, List<Func<float,float>>> actorToGiveAttackMods = new Dictionary<CombatActor, List<Func<float, float>>>();
    public Dictionary<CombatActor, List<Func<float,float>>> actorToTakeDamageMods = new Dictionary<CombatActor, List<Func<float, float>>>();
    public Dictionary<CombatActor, List<Func<CombatActor, IEnumerator>>> ActorToStartTurn = new Dictionary<CombatActor, List<Func<CombatActor, IEnumerator>>>();

    private void Awake()
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





    public void CarryOutCardSelf(CardData cardData, CombatActor source)
    {
        if (cardData.Block.Value != 0)
        {
            source.healthEffects.RecieveBlock(cardData.Block.Value * cardData.Block.Times);
        }
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



    public void SetupEnemyStartingRules()
    {
        actorToGiveAttackMods[combatController.Hero] = new List<Func<float, float>>();
        actorToTakeDamageMods[combatController.Hero] = new List<Func<float, float>>();
        ActorToStartTurn[combatController.Hero] = new List<Func<CombatActor, IEnumerator>>();
        ActorToStartTurn[combatController.Hero].Add(RemoveAllBlock);

        foreach(CombatActorEnemy e in combatController.EnemiesInScene)
        {
            enemyToActionsStartTurn[e] = new List<Func<CombatActorEnemy, IEnumerator>>();
            ActorToStartTurn[e] = new List<Func<CombatActor, IEnumerator>>();
            ActorToStartTurn[e].Add(RemoveAllBlock);
            actorToGiveAttackMods[e] = new List<Func<float, float>>();
            actorToTakeDamageMods[e] = new List<Func<float, float>>();
        }
    }

    public void EnemiesStartTurn()
    {
        StartCoroutine(EnumEnemiesStartTurn());
    }

    public IEnumerator EnumEnemiesStartTurn()
    {
        foreach (CombatActorEnemy enemy in combatController.EnemiesInScene)
            yield return StartCoroutine(EnemyStartTurn(enemy));

        combatController.animator.SetBool("EnemiesWaiting", false);
        combatController.animator.SetTrigger("EnemyTookTurn");
    }

    public IEnumerator StartTurn()
    {
        
        for (int i = 0; i < ActorToStartTurn[combatController.Hero].Count; i++)
            yield return StartCoroutine(ActorToStartTurn[combatController.Hero][i].Invoke(combatController.Hero));

        Debug.Log("actionsStartTurnEnum: " + actionsStartTurnEnum.Count);
        for (int i = 0; i < actionsStartTurnEnum.Count; i++)
            yield return StartCoroutine(actionsStartTurnEnum[i].Invoke());

        Debug.Log("Leaving StartTurn Enum");
    }


    public IEnumerator EnemyStartTurn(CombatActorEnemy enemy)
    {
        for (int i = 0; i < ActorToStartTurn[enemy].Count; i++)
            yield return StartCoroutine(ActorToStartTurn[enemy][i].Invoke(enemy));

        for (int i = 0; i < enemyToActionsStartTurn[enemy].Count; i++)
            yield return StartCoroutine(enemyToActionsStartTurn[enemy][i].Invoke(enemy));

        enemy.TakeTurn();
        Debug.Log("Took Turn");
        yield return new WaitForSeconds(1.5f);
    }


    public IEnumerator RemoveAllBlock(CombatActor actor)
    {
        actor.healthEffects.RemoveAllBlock();
        yield return new WaitForSeconds(1);
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
        foreach (Func<float, float> func in actorToGiveAttackMods[source])
        {
            Debug.Log("Modified damage for attacker");
            x = func(x);
        }
        foreach (Func<float, float> func in actorToTakeDamageMods[target])
        {
            Debug.Log("Modified damage for reciever");
            x = func(x);
        }

        return (int)x;
    }




}
