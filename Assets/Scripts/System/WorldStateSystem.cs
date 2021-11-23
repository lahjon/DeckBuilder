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

        worldAnimator = GetComponent<Animator>();
        overlayAnimator = transform.GetChild(0).GetComponent<Animator>();
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

    public static void SetInState(WorldState aWorldState, bool aBool = true)
    {
        // called from game event only
        switch (aWorldState)
        {
            case WorldState.OverworldEvent:
                worldAnimator.SetBool("InOverworldEvent", aBool);
                break;
            case WorldState.Combat:
                worldAnimator.SetBool("InCombat", aBool);
                break;
            case WorldState.TownReward:
                worldAnimator.SetBool("InTownReward", aBool);
                break;
            case WorldState.EventReward:
                worldAnimator.SetBool("InEventReward", aBool);
                break;
            case WorldState.Town:
                worldAnimator.SetBool("InTown", aBool);
                break;
            case WorldState.Overworld:
                worldAnimator.SetBool("InOverworld", aBool);
                break;
            case WorldState.WorldMap:
                worldAnimator.SetBool("WorldMap", aBool);
                break;
            default:
                break;
        }
    }

    public static void SetInCombat(bool aBool)
    {
        worldAnimator.SetBool("InCombat", aBool);
    }
    public static void SetInBuilding(bool aBool)
    {
        worldAnimator.SetBool("InBuilding", aBool);
    }
    public static void SetInOverworldShop(bool aBool)
    {
        worldAnimator.SetBool("InOverworldShop", aBool);
    }
    public static void SetInCombatReward(bool aBool)
    {
        worldAnimator.SetBool("InCombatReward", aBool);
    }
    public static void SetInTownReward(bool aBool)
    {
        worldAnimator.SetBool("InTownReward", aBool);
    }

    public static void SetInEventReward(bool aBool)
    {
        worldAnimator.SetBool("InEventReward", aBool);
    }
    public static void SetInTownShop(bool aBool)
    {
        worldAnimator.SetBool("InTownShop", aBool);
    }
    public static void SetInCutscene()
    {
        worldAnimator.SetTrigger("InCutscene");
    }
    public static void SetInWorldMap()
    {
        worldAnimator.SetTrigger("TriggerWorldMap");
    }
    public static void SetInOverworld()
    {
        worldAnimator.SetTrigger("TriggerOverworld");
    }
    public static void SetInTown(bool aBool)
    {
        worldAnimator.SetBool("TriggerTown", aBool);
    }
    public static void SetInChoice(bool aBool)
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
        Debug.Log("trans");
    }
    public static void SetInDisplay()
    {
        if (_currentOverlayState == OverlayState.Display)
        {
            overlayAnimator.SetTrigger("Clear");
        }
        else
        {
            overlayAnimator.SetTrigger("InDisplay");
        }
    }
    public static void SetInDialogue(bool value)
    {
        overlayAnimator.SetBool("InDialogue", value);
    }
    // public static void SetInRewardScreen()
    // {
    //     overlayAnimator.SetTrigger("InRewardScreen");
    // }
    public static void SetInEscapeMenu()
    {
        if (_currentOverlayState == OverlayState.EscapeMenu)
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
        if (_currentOverlayState == OverlayState.None)
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
        if (_currentOverlayState == OverlayState.CharacterSheet)
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
        Debug.Log("clear");
        overlayAnimator.SetTrigger("Clear");
    }
}
