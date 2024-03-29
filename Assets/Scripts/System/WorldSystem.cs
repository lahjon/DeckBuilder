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
    public ScenarioSelectionManager scenarioSelectionManager;
    public ScenarioManager scenarioManager;
    public BlacksmithManager blacksmithManager;
    public DialogueManager dialogueManager;
    public RewardManager rewardManager;
    public CombatRewardManager combatRewardManager;
    public ScenarioMapManager scenarioMapManager;
    public AbilityManager abilityManager;
    public ToolTipManager toolTipManager;
    public HUDManager hudManager;
    public LevelManager levelManager;
    public BuildManager buildManager;
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
    private void UpdateStartScene()
    {
        return;
    }

}
