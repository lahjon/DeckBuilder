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

    public void RecieveIntent(CardEffect block, CardEffect Damage, List<CardEffect> EffectsSelf, List<CardEffect> Effects)
    {
        for (int i = 0; i < Icons.Length; i++)
            Icons[i].gameObject.SetActive(false);
        lblIntent.text = "";

        int cursor = 0;


        if(Damage.Value != 0)
        {
            Icons[cursor].sprite = AttackSprite;
            Icons[cursor++].gameObject.SetActive(true);
            lblIntent.text = Damage.Value.ToString() + (Damage.Times != 1 ? "x" + Damage.Times.ToString() : "");
        }



        if(block.Value != 0)
        {
            Icons[cursor].sprite = DefendSprite;
            Icons[cursor++].gameObject.SetActive(true);
        }


        if(EffectsSelf.Count != 0 || Effects.Count != 0)
        {
            Icons[cursor].sprite = OtherSprite;
            Icons[cursor++].gameObject.SetActive(true);
        }
    }


}
