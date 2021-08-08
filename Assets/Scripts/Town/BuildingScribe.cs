using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingScribe : Building
{
    public GameObject scribe, deckManagement, cardUpgrade; // rooms
    public GameObject cardPrefab;
    public List<CardData> startingCards = new List<CardData>();
    public List<string> unlockedCards = new List<string>();
    public List<string> currentCards = new List<string>();
    public List<CardDisplay> allCards = new List<CardDisplay>();

    void Start()
    {

    }
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        StepInto(scribe);
    }
    public void ButtonEnterDeckManagement()
    {
        StepInto(deckManagement);
    }
    public void ButtonEnterCardUpgrade()
    {
        StepInto(cardUpgrade);
    }
}
