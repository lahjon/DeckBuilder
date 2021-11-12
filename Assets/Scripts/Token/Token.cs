using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Token : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool unlocked = false;
    [HideInInspector]
    public bool selected = false;
    [HideInInspector]
    public bool active = false;
    public Sprite artwork;
    [HideInInspector] public int cost;
    public int id;
    Color colorUnselected = new Color(0.5f, 0.5f, 0.5f);
    Color colorSelected = new Color(1.0f, 1.0f, 1.0f);
    Color colorDisabled = new Color(0.0f, 0.0f, 0.0f);
    Button button;
    ColorBlock color;

    [HideInInspector] public string description;
    Vector3 position = new Vector3();
    //bool inAnimation = false;
    static TokenManager tokenManager;
    public ItemEffect itemEffect;

    public virtual void Init(bool isActive = false)
    {
        tokenManager = WorldSystem.instance.tokenManager;
        button = this.GetComponent<Button>();
        color = button.colors;
        position = transform.position;
        this.GetComponent<Image>().sprite = artwork;
        color.normalColor = colorUnselected;
        if (isActive)
        {
            ActiveToken();
        }
        if (unlocked)
        {
            UnlockToken();
        }
        else
        {
            LockToken();
        }
    }
    public virtual void ActiveToken()
    {
        color.normalColor = colorSelected;
        button.colors = color;
        active = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (unlocked)
        {
            tokenManager.tokenMenu.DisplayText(this);  
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (unlocked)
        {
            tokenManager.tokenMenu.StopDisplayText();
        }
    }

    public virtual void SetUnselected()
    {
        tokenManager.availableTokenPoints += cost;
        color.normalColor = colorUnselected;
        button.colors = color;
        tokenManager.RemoveSelectedToken(this);
        selected = false;
    }

    public virtual void SetSelected(bool init = false)
    {

        tokenManager.availableTokenPoints -= cost;

        tokenManager.AddSelectedToken(this, init);

        color.normalColor = colorSelected;
        button.colors = color;
        selected = true;
    }

    public virtual void ButtonToggleSelect()
    {
        if (!active)
        {
            if (selected)
            {
                SetUnselected();
            }
            else if (cost <= tokenManager.availableTokenPoints)
            {
                SetSelected();
            } 
            else
            {
                Debug.Log("Cant AFFORD");
                // add animation or sound or something
            }
        }
        else
        {
            foreach (Token token in tokenManager.allTokens)
            {
                if (token.id == id)
                {
                    token.SetUnselected();
                    break;
                }
            }
        }
        tokenManager.tokenMenu.UpdatePoints();
    }

    public void UnlockToken()
    {
        this.GetComponent<Button>().interactable = true;
        unlocked = true;
    }

    public void LockToken()
    {
        this.GetComponent<Button>().interactable = false;
        unlocked = false;
    }

}
