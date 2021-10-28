using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance; 
    public Canvas canvas;
    WorldSystem world;
    public int currentScene = 0;

    public CharacterClassType selectedCharacterClassType;
    List<int> selectedTokens = new List<int>();
    public Slider slider;
    Tween tween;
    
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
        StartCoroutine(LoadLevel(0.2f, index));
    }

    public void StartNewLevel(int index = 1)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            // load start data to bring to overworld
            world.characterManager.selectedCharacterClassType = selectedCharacterClassType;
            world.tokenManager.selectedTokens = selectedTokens;
        }

        world.LoadProgression();
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

    IEnumerator LoadNewScene(int sceneNumber) 
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNumber);
        //tween = loadingCircle.DORotate(new Vector3(0, 0, transform.localRotation.eulerAngles.z + 180), 2f, RotateMode.FastBeyond360).SetLoops(-1);;

        slider.value = 0;

        while (!async.isDone) {
            slider.value = async.progress;
            yield return 0;
        }  

        //tween?.Kill();
        if (sceneNumber == 1)
        {
            WorldStateSystem.SetInTown(true);
        }
        canvas.gameObject.SetActive(false);
    }
}
