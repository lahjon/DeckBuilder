using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewActTransition : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text textAct;
    public TMP_Text textActName;
    public Animation transition;
    public bool disable;

    public void TransitionStart(int act, string anActName)
    {
        disable = true;

        switch (act)
        {
            case 1:
                canvas.gameObject.SetActive(true);
                textAct.text = string.Format("Act {0}", act);
                textActName.text = string.Format("{0}", anActName);
                WorldSystem.instance.worldStateManager.AddState(WorldState.Transition, false);
                transition.Play();
                break;

            
            default:
                Debug.Log("Default");
                WorldSystem.instance.worldStateManager.RemoveState();
                WorldSystem.instance.worldMapManager.content.SetActive(false);
                break;
        }
    }

    public void TransitionSwapMap()
    {
        WorldSystem.instance.worldStateManager.RemoveState(false);
        WorldSystem.instance.worldMapManager.content.SetActive(false);
        WorldSystem.instance.encounterManager.GenerateMap(2,2,4);
        WorldSystem.instance.worldStateManager.AddState(WorldState.ActMap, false);
        WorldSystem.instance.worldStateManager.AddState(WorldState.Transition, false);
    }

    public void TransitionEnd()
    {
        canvas.gameObject.SetActive(false);
        WorldSystem.instance.worldStateManager.RemoveState(false);
        disable = false;
        Debug.Log("Transitions Done");
    }
}
