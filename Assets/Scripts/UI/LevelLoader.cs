using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance; 
    public Canvas canvas;
    WorldSystem world;
    public int currentScene = 0;
    
    void Awake()
    {
        world = WorldSystem.instance;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisabled()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartNewLevel();
    }

    public void LoadNewLevel(int index = 1)
    {
        StartCoroutine(LoadLevel(1.0f, index));
    }

    public void StartNewLevel(int index = 1)
    {
        StartCoroutine(StartLevel(1.0f));
        
    }

    IEnumerator LoadLevel(float time, int index)
    {

        canvas.gameObject.SetActive(true);
        canvas.GetComponent<CanvasGroup>().alpha = 0;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {

            canvas.GetComponent<CanvasGroup>().alpha = t;
            yield return null;
        }
        StartCoroutine(LoadNewScene(index));
    }

    IEnumerator StartLevel(float time)
    {
        world.LoadProgression();
        world.SaveProgression();
        if (world.missionManager != null && world.missionManager.mission == null)
        {
            world.missionManager.NewMission("Mission001");
        }
        canvas.gameObject.SetActive(true);
        canvas.GetComponent<CanvasGroup>().alpha = 1;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            canvas.GetComponent<CanvasGroup>().alpha = Mathf.Abs(t - 1);
            yield return null;
        }
        canvas.gameObject.SetActive(false);
    }

    IEnumerator LoadNewScene(int sceneNumber) 
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNumber);

        while (!async.isDone) {
            yield return 0;
        }  
        currentScene = sceneNumber;
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        return;
    }
}
