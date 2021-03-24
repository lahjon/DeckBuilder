using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Token : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool unlocked = false;
    [HideInInspector]
    public bool selected = false;
    [HideInInspector]
    public bool active = false;
    public int cost;
    Color colorUnselected = new Color(0.5f, 0.5f, 0.5f);
    Color colorSelected = new Color(1.0f, 1.0f, 1.0f);
    Color colorDisabled = new Color(0.0f, 0.0f, 0.0f);
    Button button;
    ColorBlock color;

    [TextArea(5,5)]
    public string description;
    [HideInInspector]
    public TokenManager tokenManager;
    bool initialized = false;
    Vector3 position = new Vector3();
    bool inAnimation = false;


    void Start()
    {
        if (!initialized)
        {
            Init();
        }
        if (active)
        {
            ActiveToken();
        }
    }
    public virtual void Init()
    {
        tokenManager = WorldSystem.instance.tokenManager;
        button = this.GetComponent<Button>();
        color = button.colors;
        position = transform.position;
        initialized = true;
    }
    public virtual void ActiveToken()
    {
        color.normalColor = colorSelected;
        button.colors = color;
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
        tokenManager.RemoveSelectedToken(this.name);
        selected = false;
    }

    public virtual void SetSelected(bool init = false)
    {
        if (!init)
        {
            tokenManager.availableTokenPoints -= cost;
            tokenManager.AddSelectedToken(this.name);
        }
        color.normalColor = colorSelected;
        button.colors = color;
        selected = true;
    }

    public virtual void ToggleSelect()
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
            }
        }
        else
        {
            foreach (GameObject token in tokenManager.tokenMenu.allTokens)
            {
                if (token.name == name)
                {
                    Token aToken = token.GetComponent<Token>();
                    aToken.SetUnselected();
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

    public abstract void AddActivity();
    public abstract void RemoveActivity();

}
