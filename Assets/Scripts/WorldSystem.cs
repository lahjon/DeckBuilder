using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WorldSystem : MonoBehaviour
{
    public static WorldSystem instance; 
    public Character character;
    public WorldState worldState;
    public WorldState tempWorldState;
    public WorldState previousState; //= WorldState.MainMenu;
    private int currentScene = 0;
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
    public int act = 1;

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
        if(worldState != WorldState.MainMenu)
            UpdateStartScene();

    }

    // public WorldState WorldState
    // {
    //     get
    //     {
    //         return WorldState;
    //     }
    //     set
    //     {
    //         WorldState = value;
    //     }
    // }
    // }

    public void StoreCharacter(Dictionary<string, int> storeStats, CharacterData storeCharacterData, GameObject storeCharacterPrefab)
    {
        characterStats = storeStats;
        characterData = storeCharacterData;
        characterPrefab = storeCharacterPrefab;
    }
    public void CreateCharacter()
    {
        // create new character
        GameObject newCharacterPrefab = Instantiate(characterPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        Character newCharacter = newCharacterPrefab.GetComponent<Character>();

        // set stats to character
        newCharacter.strength = characterStats["strength"];
        newCharacter.cunning = characterStats["cunning"];
        newCharacter.speed = characterStats["speed"];
        newCharacter.endurance = characterStats["endurance"];
        newCharacter.wisdom = characterStats["wisdom"];
        newCharacter.characterClass = characterData.characterClass;

        // update the world system
        character = newCharacter;
        worldState = WorldState.Transition;
    }

    public void LoadByIndex(int sceneIndex) {
        StartCoroutine(LoadNewScene(sceneIndex));
    }

    private void GetAllReferences()
    {
        // we can take all the managers and child them to world system to make sure they are
        // or we find all the references from the scene once

        GameObject[] allManagers = GameObject.FindGameObjectsWithTag("Manager");
        foreach (GameObject item in allManagers)
        {
            string newName = item.name.ToLowerFirstChar();
            if (item.name == "EncounterManager")
            {
                encounterManager = item.GetComponent<EncounterManager>();
            }
            else if (item.name == "CharacterManager")
            {
                characterManager = item.GetComponent<CharacterManager>();
                characterManager.characterVariablesUI.UpdateUI();
            }
            else if (item.name == "CameraManager")
            {
                cameraManager = item.GetComponent<CameraManager>();
            }
            else if (item.name == "DeckDisplayManager")
            {
                deckDisplayManager = item.GetComponent<DeckDisplayManager>();
            }
            else if (item.name == "ShopManager")
            {
                shopManager = item.GetComponent<ShopManager>();
            }
        }
    }

    public void EnterCombat(List<EnemyData> enemyDatas = null)
    {
        combatManager.combatController.gameObject.SetActive(true);
        SwapState(WorldState.Combat);
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

    public void EndCombat(bool endAct = false)
    {
        combatManager.combatController.content.gameObject.SetActive(true);
        combatManager.combatController.gameObject.SetActive(false);
        if (endAct)
        {
            GoToTown();
            return;
        }
        SwapState(WorldState.Overworld);
    }

    private void GoToTown()
    {
        worldState = WorldState.Town;
        encounterManager.UpdateAllTownEncounters(act);
        cameraManager.CameraGoto(WorldState.Town, true);
    }

    public void SwapState(WorldState aWorldState, bool doTransition = true)
    {
        previousState = instance.worldState;
        worldState = aWorldState;
        cameraManager.CameraGoto(aWorldState, doTransition);
        characterManager.characterVariablesUI.UpdateUI();
    }
    public void SwapStatePrevious()
    {
        worldState = previousState;
        characterManager.characterVariablesUI.UpdateUI();
    }
    private void UpdateStartScene()
    {
        GetAllReferences();
    }

    public void Reset()
    {
        characterManager.Reset();
        ExitTown(true);
    }

    public void ExitTown(bool reset = false)
    {
        // if(reset == true)
        // {
        //     act = 1;
        // }
        // else
        // {
        //     act += 1;    
        // }
        characterManager.characterVariablesUI.UpdateUI();
        SwapState(WorldState.Overworld);

        //DEBUG STUFF
        GameObject actEncounters = WorldSystem.instance.encounterManager.actEncounters[0];
        Transform t = actEncounters.transform;
        Debug.Log(t);

        Encounter e = t.GetChild(6).gameObject.GetComponent<Encounter>();
        WorldSystem.instance.encounterManager.currentEncounter = e;

    
        //encounterManager.UpdateAllOverworldEncounters(act);
        cameraManager.mainCamera.transform.position = cameraManager.actCameraPos[act - 1].transform.position;
        cameraManager.mainCamera.transform.rotation = cameraManager.actCameraPos[act - 1].transform.rotation;
    }

    IEnumerator LoadNewScene(int sceneNumber) {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNumber);

        while (!async.isDone) {
            yield return 0;
        }  
        currentScene = sceneNumber;

        switch (sceneNumber)
        {
            case 1:
                Debug.Log("Swapping to Scene 1!");
                UpdateStartScene();
                break;
            
            default:
                Debug.Log("Dunno");
                break;
        }
    }
}
