using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSystem : MonoBehaviour
{
    public static WorldSystem instance; 
    public Character aCharacter;
    public WorldStates aState = WorldStates.MainMenu;
    private int currentScene = 0;
    private Dictionary<string, int> characterStats;
    private GameObject characterPrefab;
    private CharacterData characterData;
    public EncounterManager encounterManager;

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
        
        // update the world system
        instance.aCharacter = newCharacter;
        instance.aState = WorldStates.Transition;
    }

    public void LoadByIndex(int sceneIndex) {
        StartCoroutine(LoadNewScene(sceneIndex));
    }

    private void UpdateStartScene()
    {
        instance.aCharacter.MoveToStart(encounterManager.GetStartEncounter());
    }

    IEnumerator LoadNewScene(int sceneNumber) {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNumber);

        while (!async.isDone) {
            yield return 0;
        }  
        Debug.Log("Done loading new scene!");
        currentScene = sceneNumber;
        // TODO: break this into switch or find fancier solution
        UpdateStartScene();
    }
}
