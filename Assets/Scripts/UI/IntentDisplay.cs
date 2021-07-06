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


    public void RecieveIntent(Card card, int displayDamage)
    {
        attackSprite.SetActive(false);
        defendSprite.SetActive(false);
        otherSprite.SetActive(false);
        lblIntent.text = "";

        if(card.Damage.Value != 0)
        {
            attackSprite.SetActive(true);
            lblIntent.text = displayDamage.ToString() + (card.Damage.Times != 1 ? "x" + card.Damage.Times.ToString() : "");
            if (card.effectsOnPlay.Count <= 0)
            {
                attackSprite.GetComponent<Intent>().image.sprite = attackIcons[0];
                attackSprite.GetComponent<Intent>().tooltipDescription = string.Format("The enemy intends to attack!");
            }
            else
            {
                attackSprite.GetComponent<Intent>().image.sprite = attackIcons[1];
                attackSprite.GetComponent<Intent>().tooltipDescription = string.Format("The enemy intends to attack and use an unknown effect!");
            }
        }
        else if(card.effectsOnPlay.Count > 0 || card.activitiesOnPlay.Count > 0)
        {
            otherSprite.gameObject.SetActive(true);
            otherSprite.GetComponent<Intent>().tooltipDescription = string.Format("The enemy intends to use an unknown effect!");
        }
        else if(card.Block.Value != 0)
        {
            defendSprite.gameObject.SetActive(true);
            defendSprite.GetComponent<Intent>().tooltipDescription = string.Format("The enemy is intending to block!");
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
            // attackSprite.GetComponent<Intent>().DisableTween();
            // defendSprite.GetComponent<Intent>().DisableTween();
            // otherSprite.GetComponent<Intent>().DisableTween();

        }

        gameObject.SetActive(enabled);
    }

}
