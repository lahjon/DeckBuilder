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
    
    Dictionary<CombatActorEnemy, List<Func<CombatActorEnemy, IEnumerator>>> enemyToActionsStartTurn = new Dictionary<CombatActorEnemy, List<Func<CombatActorEnemy, IEnumerator>>>();

    Dictionary<CombatActor, List<Func<float,float>>> actorToGiveAttackMods = new Dictionary<CombatActor, List<Func<float, float>>>();
    Dictionary<CombatActor, List<Func<float,float>>> actorToTakeDamageMods = new Dictionary<CombatActor, List<Func<float, float>>>();

    Dictionary<EffectType, Action<CombatActor, CardEffect>> cardEffectToAction = new Dictionary<EffectType, Action<CombatActor, CardEffect>>();

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

        actionsStartTurnEnum.Add(HeroRemoveAllBlock);
        actionsStartTurnEnum.Add(ResetRemainingEnergy);


        cardEffectToAction[EffectType.Vurnerable] = ApplyVurnerable;
        cardEffectToAction[EffectType.Poison] = ApplyPoison;
    }

    void ApplyVurnerable(CombatActor combatActor, CardEffect cardEffect)
    {
        combatActor.healthEffects.RecieveEffect(cardEffect);
        Debug.Log("Combatactor recieved Vurnerable");
        actorToTakeDamageMods[combatActor].Add(VurnerableDamage);
    }

    void ApplyPoison(CombatActor combatActor, CardEffect cardEffect)
    {
    }

    public void CarryOutCardSelf(CardData cardData, CombatActor source)
    {
        if (cardData.Block.Value != 0)
        {
            source.healthEffects.RecieveBlock(cardData.Block.Value * cardData.Block.Times);
        }
        for (int i = 0; i < cardData.SelfEffects.Count; i++)
        {
            cardEffectToAction[cardData.SelfEffects[i].Type](source, cardData.SelfEffects[i]);
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
            cardEffectToAction[cardData.Effects[i].Type](target, cardData.Effects[i]);
        }
    }



    public void SetupEnemyStartingRules()
    {
        actorToGiveAttackMods[combatController.Hero] = new List<Func<float, float>>();
        actorToTakeDamageMods[combatController.Hero] = new List<Func<float, float>>();
        foreach(CombatActorEnemy e in combatController.EnemiesInScene)
        {
            enemyToActionsStartTurn[e] = new List<Func<CombatActorEnemy, IEnumerator>>();
            enemyToActionsStartTurn[e].Add(EnemyRemoveAllBlock);
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

        combatController.EndState();
    }

    public IEnumerator StartTurn()
    {
        Debug.Log("StartTurnEnum with " + actionsStartTurnEnum.Count + " actions in list");
        for (int i = 0; i < actionsStartTurnEnum.Count; i++)
            yield return StartCoroutine(actionsStartTurnEnum[i].Invoke());
        Debug.Log("Leaving StartTurn Enum");
    }


    public IEnumerator EnemyStartTurn(CombatActorEnemy enemy)
    {
        for (int i = 0; i < enemyToActionsStartTurn[enemy].Count; i++)
            yield return StartCoroutine(enemyToActionsStartTurn[enemy][i].Invoke(enemy));
        enemy.TakeTurn();
        Debug.Log("Took Turn");
        yield return new WaitForSeconds(1.5f);
    }


    IEnumerator HeroRemoveAllBlock()
    {
        combatController.Hero.healthEffects.RemoveAllBlock();
        yield return new WaitForSeconds(1);
    }

    IEnumerator EnemyRemoveAllBlock(CombatActorEnemy enemy)
    {
        enemy.healthEffects.RemoveAllBlock();
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator ResetRemainingEnergy()
    {
        Debug.Log("Start Energy Reset");
        combatController.cEnergy = combatController.energyTurn;
        yield return new WaitForSeconds(1);
        Debug.Log("Leaving Energy Reset");
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




    #region Relic Functions


    public void ToggleBarricade()
    {
        Func<IEnumerator> HeroBlock = HeroRemoveAllBlock;
        if (actionsStartTurnEnum.Contains(HeroBlock))
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Toggled Barricade ON");
            actionsStartTurnEnum.Remove(HeroBlock);
        }
        else
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Toggled Barricade OFF");
            actionsStartTurnEnum.Add(HeroBlock);
        }
    }




    #endregion

    #region Card Functions


    #endregion

    #region Effect Functions

    public float VurnerableDamage(float x)
    {
        return x * 1.5f;
    }

    #endregion

}
