using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    public Canvas canvas;
    public GameObject abandonWindow;
    public GameObject mainMenuWindow;

    public void Activate()
    {
        canvas.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        canvas.gameObject.SetActive(false);
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
