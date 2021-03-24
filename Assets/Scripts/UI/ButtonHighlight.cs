using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHighlight : MonoBehaviour
{
    public bool isHighlighted = false;
    public void HighlightText(bool highlight)
    {
        Color color = this.GetComponent<TMP_Text>().color;
        if(highlight && !isHighlighted)
        {
            color += new Color(0.2f, 0.2f, 0.2f, 0);
            isHighlighted = true;
        }
        else
        {
            color -= new Color(0.2f, 0.2f, 0.2f, 0);
            isHighlighted = false;
        }

        this.GetComponent<TMP_Text>().color = color;
    }
}
