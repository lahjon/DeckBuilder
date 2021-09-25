using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuManager : Manager
{
    public GameObject canvas;
    public MenuCharacter menuCharacter;
    public MenuSettings menuSettings;
    public MenuDeck menuDeck;
    public MenuProgression menuProgression;
    public List<GameObject> tabs;
    GameObject _currentTab;
    GameObject currentTab
    {
        get => currentTab;
        set
        {
            _currentTab?.SetActive(false);
            _currentTab = value;
            _currentTab?.SetActive(true);
        }
    }
    int _idx;
    public int idx
    {
        get => _idx;
        set
        {
            _idx = (tabs.Count + value) % tabs.Count;
        }
    }
    protected override void Start()
    {
        base.Start();
        world.menuManager = this;
        tabs.ForEach(x => x.SetActive(false));
        currentTab = tabs[0];
    }

    public void ToggleMenu()
    {
        if (canvas.activeSelf)
            canvas.SetActive(false);
        else
            canvas.SetActive(true);
    }

    public void ButtonCloseMenu()
    {
        ToggleMenu();
    }

    public void ButtonNextTab()
    {
        idx++;
        currentTab = tabs[idx];
    }
    public void ButtonPreviousTab()
    {
        idx--;
        currentTab = tabs[idx];
    }
}
