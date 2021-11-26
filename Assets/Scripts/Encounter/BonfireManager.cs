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
        tween?.Kill();
        tween = bonfireImage.transform.DOScale(1.3f, 2f).SetLoops(-1, LoopType.Yoyo);
        canvas.gameObject.SetActive(true);
        float hp = ((float)CharacterStats.Health) * healPercentage;
        text.text = string.Format("Rest to heal {0} HP", hp.ToString());
    }

    public void LeaveBonfire()
    {
        tween?.Kill();
        canvas.gameObject.SetActive(false);
    }
    void Rest()
    {
        // heal % of max hp?

        float hp = CharacterStats.Health * healPercentage;
        world.characterManager.Heal((int)hp);
        WorldStateSystem.SetInBonfire(false);
    }

    public void ButtonRest()
    {
        Rest();
    }
    public void ButtonUpgrade()
    {
        world.deckDisplayManager.OpenUpgrade(ButtonLeave);
    }

    public void ButtonLeave()
    {
        WorldStateSystem.SetInBonfire(false);
    }
}
