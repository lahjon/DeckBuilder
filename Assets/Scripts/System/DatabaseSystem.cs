using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DatabaseSystem : MonoBehaviour
{
    public static DatabaseSystem instance;

    public List<CardData> cards = new List<CardData>();

    public List<EncounterDataCombat> encountersCombat = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> encounterEvent = new List<EncounterDataRandomEvent>();
    public List<Sprite> allOverworldIcons = new List<Sprite>();

    public List<ArtifactData> artifacts = new List<ArtifactData>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public List<CardData> GetCardsByName(List<string> cardNames) => cards.Where(c => cardNames.Contains(c.name)).ToList();

    public CardData GetRandomCard(CardClassType cardClass = CardClassType.None)
    {
        int idx;

        if(cardClass == CardClassType.None)
        {
            idx = Random.Range(0,cards.Count);
            return cards[idx];
        }
        else
        {
            List<CardData> classCards = cards.Where(x => x.cardClass == cardClass).ToList();
            idx = Random.Range(0, classCards.Count);
            return classCards[idx];
        }
    }
    

    public Sprite GetOverWorldIcon(OverworldEncounterType type)
    {
        return allOverworldIcons.Where(x => x.name == "Overworld" + type.ToString()).First();
    }

    public List<CardData> GetStartingDeck(CharacterClassType character)
    {
        Debug.Log("Hej");
        return cards.Where(c => (CharacterClassType)c.cardClass == character && c.rarity == Rarity.Starting).ToList();
    }

    public EncounterDataCombat GetRndEncounterCombat(OverworldEncounterType type)
    {
        if (type == OverworldEncounterType.CombatBoss) return encountersCombat.Where(e => e.name == "BossJesterRat").First();
        List<EncounterDataCombat> encounters = encountersCombat.Where(e => (int)e.type == (int)type).ToList();
        int id = Random.Range(0, encounters.Count);
        return encounters[id];
    }

    public EncounterDataRandomEvent GetRndEncounterEvent()
    {
        List<EncounterDataRandomEvent> encounters = encounterEvent.Where(e => e.FindInRandom).ToList();
        int id = Random.Range(0, encounters.Count);
        return encounters[id];
    }
}

