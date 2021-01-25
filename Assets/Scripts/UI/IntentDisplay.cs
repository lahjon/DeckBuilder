using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntentDisplay : MonoBehaviour
{
    public Image[] Icons = new Image[3];

    public Sprite AttackSprite;
    public Sprite DefendSprite;
    public Sprite OtherSprite;

    public TMP_Text lblIntent;

    public void RecieveIntent(List<CardEffect> EffectsSelf, List<CardEffect> Effects)
    {
        for (int i = 0; i < Icons.Length; i++)
            Icons[i].gameObject.SetActive(false);
        lblIntent.text = "";

        int cursor = 0;

        foreach(CardEffect e in Effects)
        {
            if(e.Type == EffectType.Damage)
            {
                Icons[cursor].sprite = AttackSprite;
                Icons[cursor++].gameObject.SetActive(true);
                lblIntent.text = e.Value.ToString() + (e.Times != 1 ? "x" + e.Times.ToString() : "");
            }
        }

        foreach(CardEffect e in EffectsSelf)
        {
            if(e.Type == EffectType.Block)
            {
                Icons[cursor].sprite = DefendSprite;
                Icons[cursor++].gameObject.SetActive(true);
            }
        }

        if(EffectsSelf.Count + Effects.Count > cursor)
        {
            Icons[cursor].sprite = OtherSprite;
            Icons[cursor++].gameObject.SetActive(true);
        }
    }


}
