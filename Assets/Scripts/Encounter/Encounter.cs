using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public abstract class Encounter : MonoBehaviour
{
    [HideInInspector]
    public List<Encounter> neighbourEncounters;
    public EncounterData encounterData;
    public OverworldEncounterType _encounterType;
    public OverworldEncounterType encounterType { get { return _encounterType; } set { _encounterType = value; UpdateIcon(); } }
    public bool selectable = false;
    public Dictionary<GameObject, List<Encounter>> roads = new Dictionary<GameObject, List<Encounter>>();
    protected delegate void VisitAction();
    private EncounterManager encounterManager;


    void Awake()
    {
        encounterManager = WorldSystem.instance.encounterManager;
    }

    public abstract IEnumerator Entering(System.Action VisitAction, Encounter enc = null);


    public abstract void SetLeaving(Encounter nextEnc);
    private void UpdateIcon()
    {
        Sprite icon = encounterType.GetIcon();
        GetComponent<SpriteRenderer>().sprite = icon;
    }

    public void UpdateEncounter()
    {
        if (gameObject.GetComponent<Encounter>() == encounterManager.overworldEncounters[0])
        {
            StartCoroutine(Entering(() => { }));
        }
        encounterType = OverworldEncounterType.CombatNormal;
        UpdateIcon();
    }

    public void ButtonPress()
    {
        //Debug.Log("Encounter pressed");
        if (selectable)
        {
            if (encounterType == OverworldEncounterType.CombatNormal || encounterType == OverworldEncounterType.CombatElite || encounterType == OverworldEncounterType.CombatBoss)
                StartCoroutine(Entering(() => WorldStateSystem.SetInCombat(true), this));
            else if (encounterType == OverworldEncounterType.Shop)
                StartCoroutine(Entering(() => WorldStateSystem.SetInShop(true), this));
            else if (encounterType == OverworldEncounterType.RandomEvent)
            {
                WorldSystem.instance.uiManager.encounterUI.encounterData = (EncounterDataRandomEvent)encounterData;
                StartCoroutine(Entering(() => WorldStateSystem.SetInEvent(true), this));
            }
        }
    }
}
