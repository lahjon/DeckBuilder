using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    public Canvas canvas;
    public GameObject abandonWindow;
    public GameObject mainMenuWindow;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && WorldSystem.instance.worldStateManager.currentState != WorldState.Transition)
        {
            if (abandonWindow.activeSelf)
            {
                abandonWindow.SetActive(false);
            }
            else if(mainMenuWindow.activeSelf)
            {
                mainMenuWindow.SetActive(false);
            }
            else
            {
                ToggleActive();
            }
        }
    }

    void ToggleActive()
    {
        if (canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(false);
        }
        else
        {
            canvas.gameObject.SetActive(true);
        }
    }

    public void ButtonAbandonRun()
    {
        FileManager.ResetTempData();
        LevelLoader.instance.LoadNewLevel(0);
    }
    public void ButtonMainMenu()
    {
        WorldSystem.instance.SaveProgression();
        LevelLoader.instance.LoadNewLevel(0);
    }
}
