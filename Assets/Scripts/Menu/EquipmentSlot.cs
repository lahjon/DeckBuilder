using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public EquipmentData equipmentData;
    public Image image;
    public Effect effect;
    public void BindData(EquipmentData data)
    {
        if (data == null) return;

        if (!image.enabled) image.enabled = true;
        equipmentData = data;
        image.sprite = data.artwork;
        if (effect == null)
            effect = Effect.GetEffect(gameObject, data.effect, true);
        else
        {
            effect.RemoveEffect();
            Destroy(effect);
            effect = Effect.GetEffect(gameObject, data.effect, true);
        }
    }
    
}
