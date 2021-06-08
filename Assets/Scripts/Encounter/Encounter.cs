using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public abstract class Encounter : MonoBehaviour
{
    [HideInInspector]
    public List<Encounter> neighbourEncounters;
    public List<EncounterRoad> roads = new List<EncounterRoad>();
    public OverworldEncounterType _encounterType;
    public OverworldEncounterType encounterType { get { return _encounterType; } set { _encounterType = value; GetComponent<SpriteRenderer>().sprite = encounterType.GetIcon(); } }
    public bool selectable = false;
    protected delegate void VisitAction();
    private EncounterManager encounterManager;


    void Awake()
    {
        encounterManager = WorldSystem.instance.encounterManager;
    }

    public abstract IEnumerator Entering(System.Action VisitAction);

    public abstract void SetLeaving(Encounter nextEnc);

    public void UpdateEncounter()
    {
        if (gameObject.GetComponent<Encounter>() == encounterManager.overworldEncounters[0])
        {
            StartCoroutine(Entering(() => { }));
        }
        encounterType = OverworldEncounterType.CombatNormal;
    }

    public void ButtonPress()
    {
        //Debug.Log("Encounter pressed");
        if (selectable && WorldSystem.instance.gridManager.hexMapController.zoomStep == 0)
        {
            if (encounterType == OverworldEncounterType.CombatNormal || encounterType == OverworldEncounterType.CombatElite || encounterType == OverworldEncounterType.CombatBoss)
                StartCoroutine(Entering(() => WorldStateSystem.SetInCombat(true)));
            else if (encounterType == OverworldEncounterType.Shop)
                StartCoroutine(Entering(() => WorldStateSystem.SetInShop(true)));
            else if (encounterType == OverworldEncounterType.RandomEvent)
            {
                //WorldSystem.instance.uiManager.encounterUI.encounterData = (EncounterDataRandomEvent)encounterData;
                StartCoroutine(Entering(() => WorldStateSystem.SetInEvent(true)));
            }
        }
    }
}
