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
    NewActTransition newActTransition;
    
    void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        normalColor = new Color(0.6f, 0.6f, 0.6f);
        highlightColor = new Color(0.8f, 0.8f, 0.8f);
        pressColor = new Color(1.0f, 1.0f, 1.0f);
        disabledColor = new Color(0.4f, 0.4f, 0.4f);
        scaled = Vector3.one + new Vector3(0.1f, 0.1f, 0.1f);
        newActTransition = WorldSystem.instance.worldMapManager.newActTransition;

        sortingLayer = 5;
        ColorOnUnlock();
    }

    public void Disable()
    {

    }
    void OnMouseDown()
    {
        if (unlocked && !newActTransition.disable)
        {
            transform.localScale = scaled;
        }
    }

    void OnMouseUp()
    {
        if (unlocked && overButton && !newActTransition.disable)
        {
            transform.localScale = Vector3.one;
            Confirm();
        }
    }

    void Confirm()
    {
        Debug.Log("Clicked: " + index);
        newActTransition.TransitionStart(index, actName);
    }
    void OnMouseOver()
    {
        if (unlocked && !newActTransition.disable)
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
