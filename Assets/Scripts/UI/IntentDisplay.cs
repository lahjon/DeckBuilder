using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntentDisplay : MonoBehaviour
{
    public List<Sprite> attackIcons = new List<Sprite>();

    public GameObject attackSprite, defendSprite, otherSprite;
    public GameObject intentHolder;
    public TMP_Text lblIntent;
    public GameObject cAnchorIntent;

    public void RecieveIntent(CardEffect block, CardEffect Damage, List<CardEffect> Effects)
    {
        attackSprite.SetActive(false);
        defendSprite.SetActive(false);
        otherSprite.SetActive(false);
        lblIntent.text = "";

        if(Damage.Value != 0)
        {
            attackSprite.SetActive(true);
            lblIntent.text = Damage.Value.ToString() + (Damage.Times != 1 ? "x" + Damage.Times.ToString() : "");
            if (Effects.Count <= 0)
            {
                attackSprite.GetComponent<Intent>().image.sprite = attackIcons[0];
            }
            else
            {
                attackSprite.GetComponent<Intent>().image.sprite = attackIcons[1];
            }
        }
        else if(Effects.Count > 0)
        {
            otherSprite.gameObject.SetActive(true);
        }
        else if(block.Value != 0)
        {
            defendSprite.gameObject.SetActive(true);
        }

    }

    public void SetPosition()
    {
        float height = transform.parent.GetComponent<SpriteRenderer>().size.y;
        cAnchorIntent.transform.position = transform.parent.transform.position + new Vector3(0,height,0);
    }

    public void ShowDisplay(bool enabled)
    {
        if(enabled == false)
        {
            attackSprite.GetComponent<Intent>().DisableTween();
            defendSprite.GetComponent<Intent>().DisableTween();
            otherSprite.GetComponent<Intent>().DisableTween();

        }

        gameObject.SetActive(enabled);
    }


}
