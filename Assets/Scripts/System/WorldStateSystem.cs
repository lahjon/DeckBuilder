using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldStateSystem : MonoBehaviour
{
    public static Animator worldAnimator;
    public static Animator overlayAnimator;
    public TransitionType overrideTransitionType;
    public TransitionScreen transitionScreen;
    public static WorldStateSystem instance;
    static WorldState _currentWorldState;
    static OverlayState _currentOverlayState;
    static bool disableAllInput;

    public WorldState currentWorldState
    {
        get
        {
            return _currentWorldState;
        }
        set
        {
            _currentWorldState = value;
            EventManager.NewWorldState(_currentWorldState);
            WorldSystem.instance.uiManager?.debugUI.UpdateCharacterDebugHUD();
        }
    }
    public OverlayState currentOverlayState
    {
        get
        {
            return _currentOverlayState;
        }
        set
        {
            _currentOverlayState = value;
            EventManager.NewOverlayState(_currentOverlayState);
            WorldSystem.instance.uiManager?.debugUI.UpdateCharacterDebugHUD();
        }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        worldAnimator = this.GetComponent<Animator>();
        overlayAnimator = this.transform.GetChild(0).GetComponent<Animator>();

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SetInTown(true);
        }
    }

    public static void TriggerToMainMenu()
    {
        worldAnimator.SetTrigger("ToMainMenu");
        worldAnimator.SetBool("InTown", false);
        worldAnimator.SetBool("InCombat", false);
        worldAnimator.SetBool("InBuilding", false);
        worldAnimator.SetBool("InShop", false);
        worldAnimator.SetBool("InCutscene", false);
        worldAnimator.SetBool("InWorldMap", false);
        worldAnimator.SetBool("InOverworld", false);
        worldAnimator.SetBool("InReward", false);
        worldAnimator.SetBool("InEvent", false);
    }
    public static void SetInTown(bool aBool)
    {
        worldAnimator.SetBool("InTown", aBool);
    }
    public static void SetInCombat(bool aBool)
    {
        worldAnimator.SetBool("InCombat", aBool);
    }
    public static void SetInBuilding(bool aBool)
    {
        worldAnimator.SetBool("InBuilding", aBool);
    }
    public static void SetInShop(bool aBool)
    {
        worldAnimator.SetBool("InShop", aBool);
    }
    public static void SetInCutscene(bool aBool)
    {
        worldAnimator.SetBool("InCutscene", aBool);
    }
    public static void SetInWorldMap(bool aBool)
    {
        worldAnimator.SetBool("InWorldMap", aBool);
    }
    public static void SetInOverworld(bool aBool)
    {
        worldAnimator.SetBool("InOverworld", aBool);
    }
    public static void SetInReward(bool aBool)
    {
        worldAnimator.SetBool("InReward", aBool);
    }
    public static void SetInEvent(bool aBool)
    {
        worldAnimator.SetBool("InEvent", aBool);
    }
    public static void SetInBonfire(bool aBool)
    {
        worldAnimator.SetBool("InBonfire", aBool);
    }

    // overlay states
    public static void SetInTransition(bool aBool)
    {
        overlayAnimator.SetBool("InTransition", aBool);
    }
    public static void SetInDisplay()
    {
        if (_currentOverlayState == OverlayState.None || _currentOverlayState == OverlayState.Display)
        {
            overlayAnimator.SetTrigger("Clear");
        }
        else
        {
            overlayAnimator.SetTrigger("InDisplay");
        }
    }
    public static void SetInDialogue()
    {
        overlayAnimator.SetTrigger("InDialogue");
    }
    public static void SetInRewardScreen()
    {
        overlayAnimator.SetTrigger("InRewardScreen");
    }
    public static void SetInEscapeMenu()
    {
        if (_currentOverlayState == OverlayState.None || _currentOverlayState == OverlayState.EscapeMenu)
        {
            overlayAnimator.SetTrigger("Clear");
        }
        else
        {
            overlayAnimator.SetTrigger("InEscapeMenu");
        }
    }

    public static void SetInDeathScreen()
    {
        if (_currentOverlayState == OverlayState.None || _currentOverlayState == OverlayState.EscapeMenu)
        {
            overlayAnimator.SetTrigger("InDeathScreen");
        }
        else
        {
            overlayAnimator.SetTrigger("Clear");
        }
    }

    public static void SetInCharacterSheet()
    {
        if (_currentOverlayState == OverlayState.None || _currentOverlayState == OverlayState.CharacterSheet)
        {
            if (!overlayAnimator.GetBool("InCharacterSheet"))
            {
                overlayAnimator.SetBool("InCharacterSheet", true);
            }
            else
            {
                overlayAnimator.SetTrigger("Clear");
            }
        }
    }
    public static void TriggerClear()
    {
        overlayAnimator.SetTrigger("Clear");
    }
}
