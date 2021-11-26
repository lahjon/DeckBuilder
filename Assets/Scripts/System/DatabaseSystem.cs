using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DatabaseSystem : MonoBehaviour
{
    public static DatabaseSystem instance;

    [Header("=========== Cards ============")]
    public List<StartingCardSet>        StartingCards   = new List<StartingCardSet>();
    public List<CardData>               cards           = new List<CardData>();
    public List<CardFunctionalityData>  cardModifiers   = new List<CardFunctionalityData>();

    [Header("=========== Encounters ============")]
    public List<EncounterDataCombat> encountersCombat = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> encounterEvent = new List<EncounterDataRandomEvent>();
    public List<EnemyData> enemies = new List<EnemyData>();

    [Header("=========== Character ============")]
    public List<ProfessionData> professionDatas = new List<ProfessionData>();
    public List<PlayableCharacterData> allCharacterDatas = new List<PlayableCharacterData>();

    [Header("=========== Icons ============")]
    public List<Sprite> allOverworldIcons = new List<Sprite>();
    public List<Sprite> allCurrencyIcons = new List<Sprite>();
    public List<HexTile> hexTiles = new List<HexTile>();

    [Header("=========== Items ============")]
    public List<AbilityData> abilityDatas = new List<AbilityData>();
    public List<ArtifactData> arifactDatas = new List<ArtifactData>();

    [Header("=========== Progression ============")]
    public List<ScenarioData> scenarios = new List<ScenarioData>();
    public List<MissionData> missions = new List<MissionData>();

    [Header("=========== CurrentScenario ============")]
    public List<EncounterDataCombat> encountersCombatToDraw = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> encountersEventToDraw = new List<EncounterDataRandomEvent>();

    [Header("=========== Dialogue ============")]
    public List<DialogueData> dialogues = new List<DialogueData>();




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

    public List<CardData> GetCardsByID(List<string> cardIds)
    {
        return cards.Where(x => cardIds.Contains(x.id)).ToList();
    }

    public CardData GetCardByID(string cardId)
    {
        return cards.FirstOrDefault(x => x.id == cardId);
    }


    public List<CardData> GetCardsByName(List<string> cardNames)
    {
        List<CardData> retList = new List<CardData>();

        Dictionary<string, int> counts = new Dictionary<string, int>();
        foreach(string name in cardNames)
        {
            if (counts.ContainsKey(name))
                counts[name]++;
            else
                counts[name] = 1;
        }

        foreach (CardData card in cards)
            if (counts.ContainsKey(card.name))
                for (int i = 0; i < counts[card.name];i++)
                    retList.Add(card);

        return retList;
    }

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

    public CardData GetRandomCard(CardFilter cardFilter)
    {
        int idx;

        if (cardFilter == null)
        {
            idx = Random.Range(0, cards.Count);
            return cards[idx];
        }
        else
        {
            List<CardData> classCards = cards.Where(x => CardFilter.Filterer(x, cardFilter)).ToList();
            if (classCards.Count == 0) return null;
            idx = Random.Range(0, classCards.Count);
            return classCards[idx];
        }
    }

    public Sprite GetOverWorldIcon(OverworldEncounterType type)
    {
        return allOverworldIcons.Where(x => x.name == string.Format("Overworld{0}",type.ToString())).First();
    }

    public List<CardData> GetStartingProfessionCards(ProfessionType profession)
    {
        return StartingCards.Where(x => x.profession == profession).SelectMany(x => x.startingCards).ToList();
    }
    public List<CardData> GetStartingDeck(bool baseProf)
    {
        if (baseProf)
        {
            return StartingCards.Where(x => x.characterClass == WorldSystem.instance.characterManager.selectedCharacterClassType && x.profession == ProfessionType.Base).First().startingCards;
        }
        else
        {
            return StartingCards.Where(x => x.characterClass == WorldSystem.instance.characterManager.selectedCharacterClassType && x.profession == WorldSystem.instance.characterManager.professionType).First().startingCards;
        }
    }


    public void ResetEncountersCombatToDraw(CombatEncounterType? type)
    {
        if (type is null)
        {
            encountersCombatToDraw.Clear();
            encountersCombatToDraw.AddRange(encountersCombat);
        }
        else
        {
            encountersCombatToDraw = encountersCombatToDraw.Where(e => e.type != type).ToList(); //paranoia;
            encountersCombatToDraw.AddRange(encountersCombat.Where(e => e.type == type));
        }
    }

    public void ResetEncountersEventToDraw()
    {
        encountersEventToDraw.Clear(); //Paranoia
        encountersEventToDraw.AddRange(encounterEvent.Where(e => e.FindInRandom));
    }

    public EncounterDataCombat GetRndEncounterCombat(OverworldEncounterType type)
    {
        if (!encountersCombatToDraw.Any(e => (int)e.type == (int)type)) ResetEncountersCombatToDraw((CombatEncounterType)type);
        List<EncounterDataCombat> encounters = encountersCombatToDraw.Where(e => e.type == (CombatEncounterType)type).ToList();
        int id = Random.Range(0, encounters.Count);
        EncounterDataCombat data = encounters[id];
        encounters.RemoveAt(id);
        return data;
    }

    public EncounterDataRandomEvent GetRndEncounterChoice()
    {
        if (!encountersEventToDraw.Any()) ResetEncountersEventToDraw();
        int id = Random.Range(0, encountersEventToDraw.Count);
        EncounterDataRandomEvent data = encountersEventToDraw[id];
        encountersEventToDraw.RemoveAt(id);
        return data;
    }
}

