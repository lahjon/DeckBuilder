using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BlacksmithManager : Manager
{
    public Image blacksmithImage;
    public Canvas canvas;
    public TMP_Text text;
    public int upgradeCost;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        world.blacksmithManager = this;
    }

    public void EnterBlacksmith()
    {
        canvas.gameObject.SetActive(true);
        world.characterManager.characterCurrency.ember++;
    }
    public void ButtonBuyEmber()
    {
        if (world.characterManager.characterCurrency.gold >= upgradeCost)
        {
            world.characterManager.characterCurrency.gold -= upgradeCost;
            world.characterManager.characterCurrency.ember++;
        }
        else
        {
            world.uiManager.UIWarningController.CreateWarning("Not enough gold!");
        }
    }

    public void LeaveBlacksmith()
    {
        canvas.gameObject.SetActive(false);
    }
    public void ButtonUpgrade()
    {
        world.deckDisplayManager.OpenUpgrade(null);
    }

    public void ButtonLeave()
    {
        WorldStateSystem.SetInBlacksmith(false);
    }
}
