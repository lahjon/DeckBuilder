using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Start the game");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void HighlightText(bool highlight)
    {
        Debug.Log(this);
        // Color color = this.GetComponent<TMP_Text>().color;
        // if(highlight)
        //     color += new Color(0.1f, 0.1f, 0.1f, 0);
        // else
        //     color -= new Color(0.1f, 0.1f, 0.1f, 0);

        // this.GetComponent<TMP_Text>().color = color;
    }
}
