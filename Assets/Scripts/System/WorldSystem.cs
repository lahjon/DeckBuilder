using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class WorldSystem : MonoBehaviour
{
    public static WorldSystem instance; 
    public WorldState worldState;
    private Dictionary<string, int> characterStats;
    private GameObject characterPrefab;
    private PlayableCharacterData characterData;
    public EncounterManager encounterManager;
    public CharacterManager characterManager;
    public ShopManager shopManager;
    public MenuManager menuManager;
    public CameraManager cameraManager;
    public DeckDisplayManager deckDisplayManager;
    public TownManager townManager;
    public UIManager uiManager;
    public GameEventManager gameEventManager;
    public ObjectiveManager objectiveManager;
    public CompanionManager companionManager;
    public MissionManager missionManager;
    public EquipmentManager equipmentManager;
    public DisplayCardManager displayCardManager;
    public TokenManager tokenManager;
    public ArtifactManager artifactManager;
    public BonfireManager bonfireManager;
    public WorldMapManager worldMapManager;
    public BlacksmithManager blacksmithManager;
    public DialogueManager dialogueManager;
    public RewardManager rewardManager;
    public CombatRewardManager combatRewardManager;
    public ScenarioMapManager scenarioMapManager;
    public AbilityManager abilityManager;
    public ToolTipManager toolTipManager;
    public int act;
    public int subAct;
    public int saveAmount;
    public int loadAmount;
    public bool debugMode;

    void Awake()
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

    void Start()
    {
        act = 1;
        subAct = 1;
    }

    public void SaveProgression(bool saveTemp = true)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableWorld>()));
            
            if (saveTemp)
            {
                SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableTemp>()));
            }

            int index = (int)characterManager.selectedCharacterClassType == 0 ? 1 : (int)characterManager.selectedCharacterClassType;

            SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableCharacter>()), index);  
            
        }

        saveAmount++;
        Debug.Log("Amount saved: " + saveAmount);
    }
    public void LoadProgression()
    {
        SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveableTemp>()));
        SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveableWorld>()));

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            int index = (int)characterManager.selectedCharacterClassType == 0 ? 1 : (int)characterManager.selectedCharacterClassType;

            SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveableCharacter>()), index);

        }

        loadAmount++;
        Debug.Log("Amount loaded: " + loadAmount);
    }

    public void BossDefeated()
    {
        scenarioMapManager.currentTile.encounters.ForEach(x => x.status = EncounterHexStatus.Visited);

        if (subAct < 3)
            subAct++;
        else
        {
            subAct = 1;
            act++;
        }

        SaveProgression();
    }
    private void UpdateStartScene()
    {
        return;
    }

}
