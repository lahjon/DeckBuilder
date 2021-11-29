using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterVariablesUI : MonoBehaviour
{
    public GameObject leftBar, topBar, currencyBar;
    Vector3 startPos;
    Vector3 offset = new Vector3(-0.4f,0,0);
    float moveSpeed = 0.2f;
    bool active;

    void Start()
    {
        startPos = leftBar.transform.position;
        
        active = true;
        EventManager.OnNewWorldStateEvent += ToggleBar;
    }

    void ToggleBar(WorldState worldState)
    {
        if (worldState == WorldState.Overworld)
            topBar.SetActive(true);
        if (worldState == WorldState.Town)
            topBar.SetActive(false);
    }

    public void ShowBar()
    {
        //Debug.Log("Show HUD");
        LeanTween.move(leftBar, startPos, moveSpeed).setEaseOutCubic().setOnComplete(() => active = true);
    }

    public void HideBar()
    {
        //Debug.Log("Hide HUD");
        active = false;
        LeanTween.move(leftBar, startPos + offset, moveSpeed).setEaseOutCubic();
    }
    public void ButtonDisplayDeck()
    {
        if (active)
        {
            if (WorldStateSystem.instance.currentOverlayState != OverlayState.Display)
            {
                WorldSystem.instance.deckDisplayManager.OpenDisplay();
            }
            else
            {
                WorldSystem.instance.deckDisplayManager.CloseDeckDisplay();
            }
        }
    }
}
