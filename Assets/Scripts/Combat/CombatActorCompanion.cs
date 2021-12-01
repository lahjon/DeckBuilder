using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;


public class CombatActorCompanion : CombatActor
{
    public IntentDisplay intentDisplay;
    public CompanionData companionData;
    public TMP_Text txtMoveDisplay;
    public GameObject CanvasMoveDisplay;
    public string companionName;

    public GameObject cardTemplate;

    public GameObject AnchorMoveDisplay;

    public Canvas canvasEffects;
    public Canvas canvasIntent;

    public bool stochasticReshuffle = true;

    public Card hand;

    float toolTiptimer = 0;
    float toolTipDelay = 1f;
    bool toolTipShowing = false;



    public override void SetupAlliesEnemies(){
        enemies.AddRange(CombatSystem.instance.EnemiesInScene);
        allies.Add(CombatSystem.instance.Hero);
    }

    public void ReadCompanionData(CompanionData inCompanionData = null)
    {
        if (!(inCompanionData is null))
            companionData = inCompanionData;

        gameObject.SetActive(true);

        SetupCamera();

        deck = new ListEventReporter<Card>();
        discard = new ListEventReporter<Card>();

        foreach(CardData cardData in companionData.deck)
        {
            GameObject cardObject = Instantiate(cardTemplate, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0));
            cardObject.transform.SetParent(gameObject.transform);
            Card card = cardObject.GetComponent<Card>();
            card.owner = this; 
            card.cardData = cardData;
            card.BindCardData();
            deck.Add(card);
        }

        companionName = companionData.companionName;
        stochasticReshuffle = companionData.stochasticReshuffle;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = companionData.artwork;

        if (companionData.characterArt != null)
        {
            GameObject enemyArt = Instantiate(companionData.characterArt);
            enemyArt.transform.SetParent(this.gameObject.transform);
            enemyArt.transform.localPosition = Vector3.zero ;
            spriteRenderer.color = new Color(1,1,1,0);
            collision = gameObject.AddComponent<BoxCollider2D>();
            collision.size = enemyArt.GetComponent<BoxCollider2D>().size;
            collision.offset = enemyArt.GetComponent<BoxCollider2D>().offset;
        }
        else
        {
            spriteRenderer.sprite = companionData.artwork;
            collision = gameObject.AddComponent<BoxCollider2D>();
        }

        AnchorToolTip.localPosition = new Vector3(collision.size.x / 2, collision.size.y * 0.75f);
        intentDisplay.SetPosition();
       
        if(companionData.shuffleInit) ShuffleDeck();
    }

    public void SetupCamera()
    {
        canvasIntent.worldCamera = WorldSystem.instance.cameraManager.combatCamera;
        canvasIntent.planeDistance = WorldSystem.instance.uiManager.planeDistance;

        canvasEffects.worldCamera = WorldSystem.instance.cameraManager.combatCamera;
        canvasEffects.planeDistance = WorldSystem.instance.uiManager.planeDistance;
    }

    public void UpdateIntentDisplay(Card card)
    {
        int displayDamage = 0;
        if (card.Attacks.Any()) displayDamage = RulesSystem.instance.CalculateDamage(card.Attacks[0].Value.value, this, CombatSystem.instance.TargetedEnemy);
        intentDisplay.RecieveIntent(card, displayDamage);   
    }

    public void ShowMoveDisplay(bool enabled)
    {
        intentDisplay.ShowDisplay(enabled);
    }

    public void DrawCard()
    {
        if (deck.Count == 0)
        {
            if (stochasticReshuffle)
            {
                deck.AddRange(discard);
                ShuffleDeck();
            }
            else
                for (int i = discard.Count - 1; i >= 0; i--)
                    deck.Add(discard[i]);

            discard.Clear();
        }

        hand = deck[0];
        deck.RemoveAt(0);

        UpdateIntentDisplay(hand);
    }

    public void OnMouseOver()
    {
        toolTiptimer += Time.deltaTime;
        if(!toolTipShowing && toolTiptimer > toolTipDelay)
        {
            toolTipShowing = true;
        }
    }

    public void OnMouseExit()
    {
        toolTiptimer = 0;
        toolTipShowing = false;
    }

    public override void RecalcDamage()
    {
        UpdateIntentDisplay(hand);
    }

    public override int GetStat(StatType stat)
    {
        return 0;
    }
}
