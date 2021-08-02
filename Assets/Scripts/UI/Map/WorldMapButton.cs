using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapButton : MonoBehaviour
{
    SpriteRenderer sprite;
    Color normalColor;
    Color highlightColor;
    Color pressColor;
    Color disabledColor;
    Vector3 scaled;
    public int index;
    public bool unlocked;
    int sortingLayer;
    bool pressed;
    bool overButton;
    public string actName;
    
    void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        normalColor = new Color(0.6f, 0.6f, 0.6f);
        highlightColor = new Color(0.8f, 0.8f, 0.8f);
        pressColor = new Color(1.0f, 1.0f, 1.0f);
        disabledColor = new Color(0.4f, 0.4f, 0.4f);
        scaled = Vector3.one + new Vector3(0.1f, 0.1f, 0.1f);

        sortingLayer = 5;
        ColorOnUnlock();
    }

    public void Disable()
    {

    }
    void OnMouseDown()
    {
        if (unlocked && WorldStateSystem.instance.currentOverlayState == OverlayState.None)
        {
            transform.localScale = scaled;
        }
    }

    void OnMouseUp()
    {
        if (unlocked && overButton && WorldStateSystem.instance.currentOverlayState == OverlayState.None)
        {
            transform.localScale = Vector3.one;
            Confirm();
        }
    }

    void Confirm()
    {
        //Debug.Log("Clicked: " + index);
        WorldSystem.instance.worldMapManager.actIndex = index;
        if (index == 0)
        {
            WorldStateSystem.SetInWorldMap(false);
            WorldStateSystem.SetInTown(true);
        }
        else
        {
            WorldStateSystem.instance.overrideTransitionType = TransitionType.EnterMap;
            WorldStateSystem.SetInOverworld(true);
        }
    }
    void OnMouseOver()
    {
        if (unlocked && WorldStateSystem.instance.currentOverlayState == OverlayState.None)
        {
            sprite.color = highlightColor;
            sprite.sortingOrder = 6;
            overButton = true;
        }
    }
    void OnMouseExit()
    {
        ColorOnUnlock();
        sprite.sortingOrder = sortingLayer;
        overButton = false;
        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
        }
    }
    void ColorOnUnlock()
    {
        if (unlocked)
        {
            sprite.color = normalColor;
        }
        else
        {
            sprite.color = disabledColor;
        }
    }
}
