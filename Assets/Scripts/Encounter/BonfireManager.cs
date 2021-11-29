using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonfireManager : Manager
{
    public Image bonfireImage;
    public Canvas canvas;
    public Tween tween;
    public float healPercentage;
    public TMP_Text text;
    bool hasGottenEmber; // added this until we fix deck display to be seperated
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        world.bonfireManager = this;
    }

    public void EnterBonfire()
    {
        hasGottenEmber = false;
        tween?.Kill();
        tween = bonfireImage.transform.DOScale(1.3f, 2f).SetLoops(-1, LoopType.Yoyo);
        canvas.gameObject.SetActive(true);
        int hp = (int)(CharacterStats.Health * healPercentage);
        text.text = string.Format("Rest to heal {0} HP", hp.ToString());
    }

    public void LeaveBonfire()
    {
        tween?.Kill();
        canvas.gameObject.SetActive(false);
    }
    void Rest()
    {
        int hp = (int)(CharacterStats.Health * healPercentage);
        world.characterManager.Heal(hp);
        WorldStateSystem.SetInBonfire(false);
    }

    public void ButtonRest()
    {
        Rest();
    }
    public void ButtonUpgrade()
    {
        if (!hasGottenEmber)
        {
            world.characterManager.characterCurrency.ember += 1;
            hasGottenEmber = true;
        }
        world.deckDisplayManager.OpenUpgrade(ButtonLeave);
    }

    public void ButtonLeave()
    {
        WorldStateSystem.SetInBonfire(false);
    }
}
