using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

        if(card.Attacks.Any())
        {
            attackSprite.SetActive(true);
            lblIntent.text = displayDamage.ToString() + (card.Attacks[0].Times.value != 1 ? "x" + card.Attacks[0].Times.value.ToString() : "");
            if (!card.effectsOnPlay.Any())
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
        else if(card.effectsOnPlay.Any()|| card.activitiesOnPlay.Any())
        {
            otherSprite.gameObject.SetActive(true);
            otherSprite.GetComponent<Intent>().tooltipDescription = string.Format("The enemy intends to use an unknown effect!");
        }
        else if(card.Blocks[0].Value != 0)
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
