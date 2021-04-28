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
    public GameObject cAnchorIntent;

    public void RecieveIntent(CardEffect block, CardEffect Damage, List<CardEffect> Effects)
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

    }

    public void SetPosition()
    {
        float height = transform.parent.GetComponent<SpriteRenderer>().size.y;
        cAnchorIntent.transform.position = transform.parent.transform.position + new Vector3(0,height,0);
    }

    public void ShowDisplay(bool enabled)
    {
        gameObject.SetActive(enabled);
    }


}
