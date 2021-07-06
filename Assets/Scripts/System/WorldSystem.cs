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
    public CameraManager cameraManager;
    public DeckDisplayManager deckDisplayManager;
    public TownManager townManager;
    public UIManager uiManager;
    public GameEventManager gameEventManager;
    public ProgressionManager progressionManager;
    public MissionManager missionManager;
    public TokenManager tokenManager;
    public ArtifactManager artifactManager;
    public BonfireManager bonfireManager;
    public WorldMapManager worldMapManager;
    public DialogueManager dialogueManager;
    public RewardManager rewardManager;
    public LevelManager levelManager;
    public GridManager gridManager;
    public UseItemManager useItemManager;
    public ToolTipManager toolTipManager;
    public int act;
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

            int index = (int)characterManager.selectedCharacterClassType;

            if (index > 0)
            {
                SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableCharacter>()), index);  
            }
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
            int index = (int)characterManager.selectedCharacterClassType;
            if (index > 0)
            {
                SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveableCharacter>()), index);
            }
        }

        loadAmount++;
        Debug.Log("Amount loaded: " + loadAmount);
    }

    public void BossDefeated()
    {
        gridManager.currentTile.encounters.ForEach(x => x.status = EncounterHexStatus.Visited);
        WorldStateSystem.SetInOverworld(false);
        WorldStateSystem.SetInTown(true);
        SaveProgression();
    }
    private void UpdateStartScene()
    {
        return;
    }

}
