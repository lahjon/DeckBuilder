using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ProfessionTooltip : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    void Start()
    {
        DisableTooltip();
    }

    public void DisableTooltip()
    {
        gameObject.SetActive(false);
    }
    public void EnableTooltip(ProfessionData data)
    {
        if (data != null)
        {
            gameObject.SetActive(true);
            titleText.text = data.professionName;
            descriptionText.text = data.professionDescription;
        }
    }
}
