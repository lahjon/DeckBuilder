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
        float hp = ((float)world.characterManager.characterStats.GetStat(StatType.Health)) * healPercentage;
        text.text = string.Format("Rest to heal " + hp.ToString() + "HP.");
    }

    public void LeaveBonfire()
    {
        tween?.Kill();
        canvas.gameObject.SetActive(false);
    }
    void Rest()
    {
        // heal % of max hp?

        float hp = ((float)world.characterManager.characterStats.GetStat(StatType.Health)) * healPercentage;
        world.characterManager.Heal((int)hp);
        WorldStateSystem.SetInBonfire(false);
    }

    // IEnumerator Rest()
    // {

    // }

    public void ButtonRest()
    {
        Rest();
    }

    public void ButtonLeave()
    {
        WorldStateSystem.SetInBonfire(false);
    }
}
