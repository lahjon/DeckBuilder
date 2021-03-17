using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionScreen : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text textAct;
    public TMP_Text textActName;
    public Animation transition;
    void Awake()
    {
        WorldStateSystem.instance.transitionScreen = this;
        transition = this.GetComponent<Animation>();
    }

    public void EnterActTransitionStart()
    {
        int index = WorldSystem.instance.worldMapManager.actIndex;
        string actName = WorldSystem.instance.worldMapManager?.actNames[index];

        WorldStateSystem.SetInTransition(true);
        textAct.text = string.Format("Act {0}", index);
        textActName.text = string.Format("{0}", actName);
        transition.Play("EnterActTransition");
    }
    public void EnterActTransitionMidSwap()
    {
        WorldSystem.instance.encounterManager.GenerateMap(2,2,4);
        Debug.Log("MID");
    }
    public void NormalTransitionStart() 
    {
        WorldStateSystem.SetInTransition(true);
        transition.Play("NormalTransition");
    }

    void TriggerCallback()
    {
        WorldStateSystem.TriggerClear();
    }
}
