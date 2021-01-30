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

    }


    public IEnumerator StartTurn()
    {
        Debug.Log("StartTurnEnum with " + actionsStartTurnEnum.Count + " actions in list");
        for (int i = 0; i < actionsStartTurnEnum.Count; i++)
            yield return StartCoroutine(actionsStartTurnEnum[i].Invoke());
        Debug.Log("Leaving StartTurn Enum");
    }


    IEnumerator HeroRemoveAllBlock()
    {
        combatController.Hero.healthEffects.RemoveAllBlock();
        yield return new WaitForSeconds(1);
    }

    IEnumerator EnemyRemoveAllBlock()
    {
        combatController.EnemiesInScene.ForEach(x => x.healthEffects.RemoveAllBlock());
        yield return new WaitForSeconds(1);
    }

    IEnumerator ResetRemainingEnergy()
    {
        Debug.Log("Start Energy Reset");
        combatController.cEnergy = combatController.energyTurn;
        yield return new WaitForSeconds(1);
        Debug.Log("Leaving Energy Reset");
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



}
