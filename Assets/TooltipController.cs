using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipController : MonoBehaviour
{
    public GameObject templateTooltip;
    public List<TMP_Text> txt_tips = new List<TMP_Text>();

    private int cursor = 0;

    public void ResetToolTips()
    {
        cursor = 0;
    }        

    public void AddTipText(string tip)
    {
        if (tip == "") return;
        if(cursor == txt_tips.Count)
        {
            GameObject go = Instantiate(templateTooltip, this.transform);
            TMP_Text text = go.GetComponentInChildren<TMP_Text>();
            txt_tips.Add(text);
            text.transform.parent.gameObject.SetActive(false);
        }

        txt_tips[cursor++].text = tip;
    }

    public void ShowHide(bool show)
    {
        for (int i = 0; i < cursor; i++)
            txt_tips[i].transform.parent.gameObject.SetActive(show);
    }

}
