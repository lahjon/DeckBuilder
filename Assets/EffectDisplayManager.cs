using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDisplayManager : MonoBehaviour
{
    public Dictionary<EffectType, EffectDisplay> effectToDisplay = new Dictionary<EffectType, EffectDisplay>();
    public GameObject templateEffectDisplay;

    public void SetEffect(EffectType effect, int x)
    {
        if (effectToDisplay.ContainsKey(effect))
        {
            if (x == 0)
            {
                Destroy(effectToDisplay[effect].gameObject);
                effectToDisplay.Remove(effect);
            }
            else
                effectToDisplay[effect].SetLabel(x);
        }
        else if(x != 0)
        {
            GameObject effectObject = Instantiate(templateEffectDisplay, transform.position, Quaternion.Euler(0, 0, 0),this.transform) as GameObject;
            effectToDisplay[effect] = effectObject.GetComponent<EffectDisplay>();
            effectToDisplay[effect].SetLabel(x);
        }
        
    }
}
