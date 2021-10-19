using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class DebugUI : MonoBehaviour
{
    public GameObject canvas;
    public CardData cardData;
    public int strength;
    public string tokenReward;
    public string artifactReward = "TestArtifact1";
    public TMP_Text worldState;
    public TMP_Text overlayState;
    public TMP_Text worldTier;
    public TMP_Dropdown dropdownEnemies;
    public TMP_Dropdown dropdownEncounter;
    public TMP_Dropdown dropdownCard;

    WorldSystem world;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            ToggleDebugMenu();
        }
    }

    void Start()
    {
        world = WorldSystem.instance;
        List<string> optionsEnemies = new List<string>();
        DatabaseSystem.instance.encountersCombat.ForEach(x => optionsEnemies.Add(x.name));
        dropdownEnemies.AddOptions(optionsEnemies);

        List<string> optionsEncounters = new List<string>();
        DatabaseSystem.instance.encounterEvent.ForEach(x => optionsEncounters.Add(x.name));
        dropdownEncounter.AddOptions(optionsEncounters);

        List<string> optionsCard = new List<string>();
        DatabaseSystem.instance.cards.ForEach(x => optionsCard.Add(x.name));
        dropdownCard.AddOptions(optionsCard);
    }

    public void UpdateCharacterDebugHUD()
    {
        worldState.text = WorldStateSystem.instance.currentWorldState.ToString();
        overlayState.text = WorldStateSystem.instance.currentOverlayState.ToString();
        worldTier.text = "Act " + world.act.ToString();
    }

    public void DebugCreateWarning()
    {
        world.uiManager.UIWarningController.CreateWarning("This is a debug warning!");
    }

    public void DebugCreateNewItem()
    {
        world.useItemManager.AddItem();
    }
    public void DebugTakeDamage(int amount)
    {
        world.characterManager.TakeDamage(amount);
    }
    public void DebugHeal(int amount)
    {
        world.characterManager.Heal(amount);
    }

    public void DebugDraftCards(int amount)
    {
        WorldStateSystem.SetInCombatReward(true);
    }

    public void DebugUpgradeGearHead()
    {
        world.equipmentManager.UpgradeEquipment(EquipmentType.Head);
    }


    public void DebugAddStat(int amount)
    {
        world.characterManager.characterStats.ModifyHealth(amount);
    }

    public void DebugRemoveStat(int amount)
    {
        world.characterManager.characterStats.ModifyHealth(-amount);
    }

    public void DebugAddSpecficArtifact()
    {
        world.artifactManager.AddArtifact(artifactReward);
    }
    public void DebugAddExperience(int amount)
    {
        world.levelManager.AddExperience(amount);
    }
    public void DebugUnlockPerk()
    {
        PerkData data = world.menuManager.menuCharacter.allPerkDatas[Random.Range(0, world.menuManager.menuCharacter.allPerkDatas.Except(world.menuManager.menuCharacter.allEquippedPerksDatas).ToList().Count)];
        world.menuManager.menuCharacter.UnlockPerk(data);
    }
    public void DebugAddLevel()
    {
        world.levelManager.AddLevel();
    }

    public void DebugStartCombat()
    {
        StartCoroutine(StartCombat());
    }

    IEnumerator StartCombat()
    {
        EncounterDataCombat data = DatabaseSystem.instance.encountersCombat.FirstOrDefault(x => x.name == dropdownEnemies.options[dropdownEnemies.value].text); 
        Debug.Log(data);
        Debug.Log(dropdownEnemies.options[dropdownEnemies.value].text);
        if (data != null) 
        {
            CombatSystem.instance.encounterData = data;
            WorldStateSystem.SetInOverworld();
            yield return new WaitForSeconds(1f);
            WorldStateSystem.SetInCombat(true);
        }
        else
        {
            Debug.Log("No valid encounter data!");
        }
    }

    public void DebugStartRandomEncounterEvent()
    {
        EncounterDataRandomEvent data = DatabaseSystem.instance.encounterEvent.FirstOrDefault(x => x.name == dropdownEncounter.options[dropdownEncounter.value].text); 

        if (data == null)
        {   
            Debug.Log("No encounter named: " + data);
            return;
        }
        WorldSystem.instance.uiManager.encounterUI.encounterData = data;
        WorldStateSystem.SetInEvent(true);
        WorldSystem.instance.uiManager.encounterUI.StartEncounter();
    }

    public void DebugRemoveSpecificArtifact()
    {
        world.artifactManager.RemoveArtifact(artifactReward);
    }
    public void DebugAddRandomArtifact()
    {
        world.artifactManager.AddArtifact(world.artifactManager.GetRandomAvailableArtifact()?.name);
    }

    public void DebugRemoveRandomArtifact()
    {
        world.artifactManager.RemoveArtifact(world.artifactManager.GetRandomActiveArtifact());
    }
    public void DebugAddCardFromFilter()
    {
        string text = dropdownCard.options[dropdownCard.value].text;

        Debug.Log("card data: " + text);
        CardData data = DatabaseSystem.instance.GetRandomCard(new CardFilter() {name = text});

        if (data == null)
        {
            Debug.Log("No card named: " + data);
            return;
        }

        world.characterManager.AddCardToDeck(data);
    }
    public void DebugAddTokenReward()
    {
        world.tokenManager.UnlockNewToken(tokenReward);
    }

    public void DebugGetShards()
    {
        world.characterManager.shard += 5;
    }
    public void DebugInRewardScreen()
    {
        world.rewardManager.CreateReward(RewardType.Artifact, null, null);
    }

    public void DebugRemoveRandomItem()
    {
        world.useItemManager.RemoveItem();
    }

    public void DebugUnlockCardInScribe()
    {
        world.townManager.scribe.UnlockCard(cardData);
    }
    // public void DebugUpgradeCardPermanent()
    // { 

    //     string value = world.townManager.scribe.unlockedCards[Random.Range(0, world.townManager.scribe.unlockedCards.Count)].cardId;
    //     world.townManager.scribe.UpgradeCardPermanent(value);
    // }

    public void DebugRewardGetCard()
    {
        Debug.Log("Get Card");
        world.rewardManager.CreateReward(RewardType.UnlockCard, new string[1]{"name=Berserker_NewCard"});
        WorldStateSystem.SetInTownReward(true);
    }

    public void DebugResetAllData()
    {
        Directory.GetFiles(Application.persistentDataPath);
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            Debug.Log(file + " removed");
            file.Delete(); 
        }
    }

    public void DebugTriggerDeathScreen()
    {
        WorldStateSystem.SetInDeathScreen();
    }

    public void DebugGenerateWorld()
    {
        WorldStateSystem.SetInTown(false);
        WorldStateSystem.SetInOverworld();
        world.gridManager.GenerateMap();
    }

    public void DebugWinCombat()
    {
        if(WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            CombatSystem.instance.WinCombat();
    }
    public void ToggleDebugMenu()
    {
        if(canvas.activeSelf)
        {
            canvas.SetActive(false);
            world.debugMode = false;
        }
        else
        {
            canvas.SetActive(true);
            world.debugMode = true;
        }
    }

    public void DebugLoadGame()
    {
        world.LoadProgression();
    }
    public void DebugSaveGmae()
    {
        world.SaveProgression();
    }

}
