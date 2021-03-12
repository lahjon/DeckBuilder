using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterOverworld : Encounter
{
    private EncounterManager encounterManager;
    [HideInInspector]
    public int mapIndex;
    void Awake()
    {
        encounterManager = WorldSystem.instance.encounterManager;
    }

    public override void UpdateEncounter()
    {
        
        if(gameObject.GetComponent<Encounter>() == encounterManager.overworldEncounters[0])
        {
            SetIsVisited();
        }
        encounterType = encounterData.type;

        isVisited = isClicked = encounterData.isVisited;
        UpdateIcon();
    }

    protected override bool CheckViablePath(Encounter anEncounter)
    {
        return encounterManager.currentEncounter.neighbourEncounters.Contains(anEncounter);
    }

    public override void ButtonPress()
    {
        if(!isVisited && CheckViablePath(this) && !isClicked && WorldSystem.instance.worldState == WorldState.ActMap)
        {
            List<System.Action> visitActions = new List<System.Action>();

            switch (this.encounterType)
            {
                case EncounterType.OverworldCombatNormal:
                    // Debug.Log("Enter Combat!");
                    // SetIsVisited(this);
                    // WorldSystem.instance.EnterCombat();

                    visitActions.Clear();
                    void actionNormal() 
                    {
                        WorldSystem.instance.EnterCombat();
                    }
                    visitActions.Add(actionNormal);
                    StartCoroutine(SetVisited(visitActions, this));

                    break;
                
                case EncounterType.OverworldCombatElite:
                    // Debug.Log("Enter Elite Combat!");
                    // SetIsVisited(this);
                    // WorldSystem.instance.EnterCombat();

                    visitActions.Clear();
                    void actionElite() 
                    {
                        WorldSystem.instance.EnterCombat();
                    }
                    visitActions.Add(actionElite);
                    StartCoroutine(SetVisited(visitActions, this));

                    break;
                
                case EncounterType.OverworldCombatBoss:
                    // Debug.Log("Enter Boss Combat!");
                    // SetIsVisited(this);
                    // WorldSystem.instance.EnterCombat();

                    visitActions.Clear();
                    void actionBoss() 
                    {
                        WorldSystem.instance.EnterCombat();
                    }
                    visitActions.Add(actionBoss);
                    StartCoroutine(SetVisited(visitActions, this));

                    break;

                case EncounterType.OverworldShop:

                    visitActions.Clear();
                    void actionShop() 
                    {
                        WorldSystem.instance.shopManager.shop.gameObject.SetActive(true);
                        WorldSystem.instance.shopManager.shop.RestockShop();
                        SetIsVisited(this);
                        WorldSystem.instance.worldStateManager.AddState(WorldState.Shop);
                        Debug.Log("Enter Shop!");
                    }
                    visitActions.Add(actionShop);
                    StartCoroutine(SetVisited(visitActions, this));
                    
                    // WorldSystem.instance.shopManager.shop.gameObject.SetActive(true);
                    // WorldSystem.instance.shopManager.shop.RestockShop();
                    // SetIsVisited(this);
                    // WorldSystem.instance.worldStateManager.AddState(WorldState.Shop);
                    // Debug.Log("Enter Shop!");
                    break;

                case EncounterType.OverworldRandomEvent:
                    // SetIsVisited(this);
                    // WorldSystem.instance.worldStateManager.AddState(WorldState.Event, false);
                    // WorldSystem.instance.uiManager.encounterUI.encounterData = this.encounterData;
                    // WorldSystem.instance.uiManager.encounterUI.StartEncounter();


                    visitActions.Clear();
                    WorldSystem.instance.uiManager.encounterUI.encounterData = this.encounterData;
                    void actionEncounter()
                    {
                        WorldSystem.instance.worldStateManager.AddState(WorldState.Event, false);
                        WorldSystem.instance.uiManager.encounterUI.StartEncounter();
                    }
                    visitActions.Add(actionEncounter);
                    StartCoroutine(SetVisited(visitActions, this));
                    
                    break;
                
                default:
                    isClicked = true;
                    //CreateUI();
                    break;
            }
        }
    }
}
