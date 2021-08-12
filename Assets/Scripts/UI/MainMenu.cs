using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public Canvas canvas;
    public void PlayGame()
    {
        
        LevelLoader.instance.LoadNewLevel();
        Debug.Log("Start the game");
    }
    public void QuitGame()
    {
        //Application.Quit();
        Debug.Log("Quit the game");
    }
}
