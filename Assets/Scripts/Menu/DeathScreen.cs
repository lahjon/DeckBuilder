using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text text;
    public Button button;

    public void ButtonConfirm()
    {
        WorldSystem.instance.SaveProgression(false);
        LevelLoader.instance.LoadNewLevel(0);
    }
}
