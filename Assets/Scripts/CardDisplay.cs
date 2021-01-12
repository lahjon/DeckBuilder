using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{

    public Card card;

    public Text nameText;
    public Text descriptionText;

    public Image artworkImage;

    public Text costText;
    public Text damageText;
    public Text blockText;

    public void UpdateDisplay()
    {
        nameText.text = card.name;
        //descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;

        costText.text = card.cost.ToString();

        descriptionText.text = "";

        for(int i = 0; i < card.Effects.Count; i++)
        {
            descriptionText.text += card.Effects[i].Type.ToString() + ":" + card.Effects[i].Value;
            if (card.Effects[i].Times != 1) descriptionText.text += " " + card.Effects[i].Times + " times.";
            if (i != card.Effects.Count - 1) descriptionText.text += "\n";
        }
    }

    public void OnMouseManual()
    {
        Debug.Log("KUKAR2");
    }

    public void OnMouseText()
    {
        Debug.Log("I am text");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
