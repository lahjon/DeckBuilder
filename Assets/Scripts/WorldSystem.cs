using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class WorldSystem : MonoBehaviour, ISaveable
{
    public static WorldSystem instance; 
    public Character character;
    public WorldState worldState;
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
    public WorldStateManager worldStateManager;
    public int act;

    public bool uglyBool;

    List<Func<IEnumerator>> listie = new List<Func<IEnumerator>>();

    public IEnumerator TestEnumerator(int remaining)
    {
        Debug.Log($"Frame from TestEnum1: {Time.frameCount}");
        if (remaining > 0)
            yield return StartCoroutine(TestEnumerator(remaining - 1));

        Debug.Log($"Frame after start: {Time.frameCount} from TestEnumNumbered {remaining}");
    }

    public IEnumerator Testenum2()
    {
        Debug.Log($"TestEnum2 Frame: {Time.frameCount}");
        yield return null;
    }


    public IEnumerator StartWithOutParameter()
    {
        for(int i = 0; i < listie.Count; i++)
        {
            uglyBool = false;
            StartCoroutine(listie[i].Invoke());
            while (!uglyBool)
                yield return null;
        }
           

        Debug.Log($"Frame after list is run: {Time.frameCount}");
    }

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

    class BoolUgly
    {
        public bool IamDone;
    }

    IEnumerator Demo1()
    {
        yield return StartCoroutine(Demo2());
        Debug.Log($"Frame count after all resolved: {Time.frameCount}");
    }

    IEnumerator Demo2()
    {
        yield return StartCoroutine(Demo3());
    }

    IEnumerator Demo3()
    {
        yield return StartCoroutine(Demo4());
    }

    IEnumerator Demo4()
    {
        yield return StartCoroutine(Demo5());
    }

    IEnumerator Demo5()
    {
        Debug.Log($"Frame count at deepest: {Time.frameCount}");
        yield return null;
    }

    IEnumerator TestWithOut()
    {
        while (!uglyBool)
        {
            uglyBool = UnityEngine.Random.Range(0f, 1f) < 0.8;
            yield return null;
        }

        Debug.Log($"TestEnum2 Frame: {Time.frameCount}");
        yield return null;
    }

    void Start()
    {
        if(worldState != WorldState.MainMenu)
            UpdateStartScene();

        worldStateManager.AddState(WorldState.Town);
        SaveDataManager.LoadJsonData((Helpers.FindInterfacesOfType<ISaveable>()));
        act = 1;

        listie.Add(TestWithOut);
        listie.Add(TestWithOut);
        listie.Add(TestWithOut);
        listie.Add(TestWithOut);
        listie.Add(TestWithOut);
        listie.Add(TestWithOut);
        listie.Add(TestWithOut);
        listie.Add(TestWithOut);


        StartCoroutine(Demo1());
    }

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
        SaveDataManager.SaveJsonData((Helpers.FindInterfacesOfType<ISaveable>()));
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

    // public void SwapState(WorldState aWorldState, bool doTransition = true)
    // {

    //     if(aWorldState == WorldState.Overworld)
    //     {
    //         encounterManager.canvas.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         encounterManager.canvas.gameObject.SetActive(false);
    //     }
    //     previousState = instance.worldState;
    //     worldState = aWorldState;
    //     cameraManager.CameraGoto(aWorldState, doTransition);
    //     characterManager.characterVariablesUI.UpdateUI();
    // }
    // public void SwapStatePrevious()
    // {
    //     worldState = previousState;
    //     characterManager.characterVariablesUI.UpdateUI();
    // }
    private void UpdateStartScene()
    {
        GetAllReferences();
    }

    public void Reset()
    {
        characterManager.Reset();
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

    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.act = act;
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        act = a_SaveData.act;
    }
}
