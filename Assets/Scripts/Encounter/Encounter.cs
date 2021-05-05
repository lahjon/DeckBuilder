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
    private EncounterType _encounterType;
    public EncounterType encounterType { get { return _encounterType; } set { _encounterType = value; UpdateIcon(); } }
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
        List<Sprite> allIcons = DatabaseSystem.instance.iconDatabase.allIcons;
        Sprite icon = allIcons.Where(x => x.name == encounterType.ToString()).FirstOrDefault();
        //GetComponent<Image>().sprite = icon;
    }

    public void UpdateEncounter()
    {
        if (gameObject.GetComponent<Encounter>() == encounterManager.overworldEncounters[0])
        {
            StartCoroutine(Entering(() => { }));
        }
        encounterType = EncounterType.OverworldCombatNormal;
        UpdateIcon();
    }

    public void ButtonPress()
    {
        //Debug.Log("Encounter pressed");
        if (selectable)
        {
            if (encounterType == EncounterType.OverworldCombatNormal || encounterType == EncounterType.OverworldCombatElite || encounterType == EncounterType.OverworldCombatBoss)
                StartCoroutine(Entering(() => WorldStateSystem.SetInCombat(true), this));
            else if (encounterType == EncounterType.OverworldShop)
                StartCoroutine(Entering(() => WorldStateSystem.SetInShop(true), this));
            else if (encounterType == EncounterType.OverworldRandomEvent)
            {
                WorldSystem.instance.uiManager.encounterUI.encounterData = (EncounterDataRandomEvent)encounterData;
                StartCoroutine(Entering(() => WorldStateSystem.SetInEvent(true), this));
            }
        }
    }
}
