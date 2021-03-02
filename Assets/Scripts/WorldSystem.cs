using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class WorldSystem : MonoBehaviour, ISaveableTemp
{
    public static WorldSystem instance; 
    public WorldState worldState;
    private Dictionary<string, int> characterStats;
    private GameObject characterPrefab;
    private CharacterData characterData;
    public EncounterManager encounterManager;
    public CharacterManager characterManager;
    public ShopManager shopManager;
    public CameraManager cameraManager;
    public DeckDisplayManager deckDisplayManager;
    public CombatManager combatManager;
    public TownManager townManager;
    public UIManager uiManager;
    public WorldStateManager worldStateManager;
    public GameEventManager gameEventManager;
    public ProgressionManager progressionManager;
    public MissionManager missionManager;
    public int act;
    public Character character;
    public CharacterClassType characterClassType;

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
        LoadProgression();
        if(worldState != WorldState.MainMenu)
            UpdateStartScene();
        act = 1;

    }

        IEnumerator WaitForStart()
    {
        while (WorldSystem.instance.characterManager == null && WorldSystem.instance.townManager == null)
        {
            yield return null;
        }
        
    }

    public void EnterCombat(List<EnemyData> enemyDatas = null)
    {
        combatManager.combatController.gameObject.SetActive(true);
        worldStateManager.AddState(WorldState.Combat);
        cameraManager.CameraGoto(WorldState.Combat, true);
        encounterManager.encounterTier = encounterManager.currentEncounter.encounterData.tier;
        List<EnemyData> eData = enemyDatas;
        if (enemyDatas == null)
        {
            combatManager.combatController.SetUpEncounter();
        }
        else
        {
            combatManager.combatController.SetUpEncounter(eData);
        }
    }

    public void SaveProgression()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableWorld>()));
            SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableTemp>()));
            SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveableCharacter>()), (int)character.classType);
        }

    }
    public void LoadProgression()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SaveDataManager.LoadJsonData(this.GetComponents<ISaveableTemp>());
        }
        else
        {
            SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveableWorld>()));
            SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveableTemp>()));
        }
    }

    public void EndCombat(bool endAct = false)
    {
        WorldSystem.instance.worldStateManager.RemoveState(true);
        Debug.Log("EndCombat Removing card!");
        combatManager.combatController.content.gameObject.SetActive(true);
        combatManager.combatController.gameObject.SetActive(false);
        if (endAct)
        {
            SaveProgression();
            GoToTown();
            return;
        }
    }

    private void GoToTown()
    {
        worldState = WorldState.Town;
        encounterManager.UpdateAllTownEncounters(act);
        cameraManager.CameraGoto(WorldState.Town, true);
    }
    private void UpdateStartScene()
    {
        return;
    }

    public void Reset()
    {
        characterManager.Reset();
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        return;
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            characterClassType = a_SaveData.characterClassType;
        }
    }
}
