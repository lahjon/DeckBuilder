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
    public int tokenReward = 0;
    public int artifactReward = 1;
    public TMP_Text worldState;
    public TMP_Text overlayState;
    public TMP_Text worldTier;
    public TMP_Dropdown dropdownEnemies;
    public TMP_Dropdown dropdownEncounter;
    public TMP_Dropdown dropdownCard;
    public bool debugActive;
    public List<Sprite> gearImages;

    WorldSystem world;

    void Update()
    {
        if(world.debugMode && Input.GetKeyDown(KeyCode.R) && !WorldSystem.instance.displayCardManager.active)
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
        world.abilityManager.UnlockAbility(1);
    }
    public void DebugTakeDamage(int amount)
    {
        if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            CombatSystem.instance.Hero.TakeDamage(amount);
        else
            world.characterManager.LooseLife(amount);
    }
    public void DebugHeal(int amount)
    {
        if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            CombatSystem.instance.Hero.HealLife(amount);
        else
            world.characterManager.Heal(amount);
    }

    public void DebugAddEquipmentToInventory()
    {
        Equipment equipment = world.equipmentManager.CreateEquipment();
        equipment.image.sprite = gearImages[Random.Range(0, gearImages.Count)];
        equipment.name = string.Format("Equipment{0}", Random.Range(0,500));
        world.menuManager.menuInventory.AddEquipmentToInventory(equipment);
    }

    public void DebugDraftCards(int amount)
    {
        WorldStateSystem.SetInCombatReward(true);
    }

    public void DebugUpgradeGearHead()
    {
        //world.equipmentManager.UpgradeEquipment(EquipmentType.Head);
    }


    public void DebugAddStat(int amount)
    {
        ItemEffect.Factory(new ItemEffectStruct(ItemEffectType.AddStat, "Health", amount, true), new IEffectAdderStruct("Debug", amount));
    }

    public void DebugAddSpecficArtifact()
    {
        world.artifactManager.AddArtifact(artifactReward);
    }
    public void DebugAddExperience()
    {
        world.levelManager.AddExperience(5);
    }
    public void DebugUnlockPerk()
    {
        PerkData data = world.menuManager.menuInventory.allPerkDatas[Random.Range(0, world.menuManager.menuInventory.allPerkDatas.Except(world.menuManager.menuInventory.allEquippedPerks.Select(x => x.perkData)).ToList().Count)];
        world.menuManager.menuInventory.UnlockPerk(data);
    }
    public void DebugLevelUp()
    {
        world.levelManager.LevelUp();
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
        WorldStateSystem.SetInChoice(true);
        WorldSystem.instance.uiManager.encounterUI.StartEncounter();
    }

    public void DebugRemoveSpecificArtifact()
    {
        world.artifactManager.RemoveArtifact(artifactReward);
    }
    public void DebugAddRandomArtifact()
    {
        world.artifactManager.AddArtifact(world.artifactManager.GetRandomAvailableArtifact().itemId);
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

    public void DebugInRewardScreen()
    {
        //world.rewardManager.CreateRewardNormal(RewardNormalType.UnlockCard, "name=Berserker_DisplayofMight");
        world.rewardManager.CreateRewardNormal(RewardNormalType.Ability, "0");
        WorldStateSystem.SetInTownReward(true);
    }

    public void DebugGetShards()
    {
        world.characterManager.characterCurrency.shard += 5;
    }

    public void DebugRemoveRandomItem()
    {
        //world.abilityManager.RemoveAbility();
    }

    public void DebugUnlockCardInScribe()
    {
        world.townManager.scribe.UnlockCard(cardData);
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

    public void DebugSetInBonfire()
    {
        WorldStateSystem.SetInBonfire(true);
    }

    public void DebugGenerateWorld()
    {
        WorldStateSystem.SetInTown(false);
        WorldStateSystem.SetInOverworld();
        world.scenarioMapManager.GenerateMap();
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
            debugActive = false;
        }
        else
        {
            canvas.SetActive(true);
            debugActive = true;
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
