using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventMain : MonoBehaviour
{

    public virtual bool TriggerEvent()
    {
        return true;
        //this.gameObject.SetActive(false);
    }
}
