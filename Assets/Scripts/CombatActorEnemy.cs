using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class CombatActorEnemy : CombatActor
{
    public IntentDisplay intentDisplay;
    public TMP_Text txtMoveDisplay;
    public EnemyData enemyData;
    public GameObject CanvasMoveDisplay;
    public string enemyName;

    public GameObject cardTemplate;

    [HideInInspector]
    public List<Card> deck;
    public List<Card> discard;
    public CardData nextCard;
    public GameObject AnchorMoveDisplay;

    public Canvas canvasEffects;
    public Canvas canvasIntent;
    public Canvas canvasToolTip;
    public GameObject target;

    public TooltipController tooltipController;

    public Card hand;

    float toolTiptimer = 0;
    float toolTipDelay = 1f;
    bool toolTipShowing = false;


    public void SetTarget(bool set = false)
    {
        if(set && !target.activeSelf)
        {
            target.SetActive(true);
        }
        else if(!set && target.activeSelf)
        {
            target.SetActive(false);
        }
    }

    public void ReadEnemyData(EnemyData inEnemyData = null)
    {
        if (!(inEnemyData is null))
            enemyData = inEnemyData;

        SetupCamera();
        healthEffects.combatActor = this;

        foreach(CardData cardData in enemyData.deck)
        {
            GameObject cardObject = Instantiate(cardTemplate, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
            cardObject.transform.SetParent(this.gameObject.transform);
            Card card = cardObject.GetComponent<Card>();
            card.cardData = cardData;
            card.BindCardData();
            deck.Add(card);
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemyData.artwork;
        enemyName = enemyData.enemyName;
        tooltipController.AddTipText($"<b>{enemyName}</b>\nThis enemy has {deck.Count} cards in its deck!");

        if (enemyData.characterArt != null)
        {
            GameObject enemyArt = Instantiate(enemyData.characterArt);
            enemyArt.transform.SetParent(this.gameObject.transform);
            enemyArt.transform.localPosition = Vector3.zero + new Vector3(0, -2, 0);
        }

        healthEffects.maxHitPoints = enemyData.StartingHP;
        healthEffects.hitPoints = enemyData.StartingHP;
        healthEffects.RemoveAllBlock();
        ShuffleDeck();
        SetUIpositions();
        UpdateMoveDisplay(deck[0]);
    }

    public void SetupCamera()
    {
        canvasIntent.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvasIntent.planeDistance = WorldSystem.instance.uiManager.planeDistance;

        canvasEffects.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvasEffects.planeDistance = WorldSystem.instance.uiManager.planeDistance;

        canvasToolTip.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvasToolTip.planeDistance = WorldSystem.instance.uiManager.planeDistance;
    }

    public void UpdateMoveDisplay(Card card)
    {
        intentDisplay.RecieveIntent(card.Block, card.Damage, card.Effects);   
    }

    public void ShowMoveDisplay(bool enabled)
    {
        intentDisplay.ShowDisplay(enabled);
    }

    public void DrawCard()
    {
        if (deck.Count == 0)
        {
            deck.AddRange(discard);
            ShuffleDeck();
            discard.Clear();
        }

        hand = deck[0];
        deck.RemoveAt(0);

        UpdateMoveDisplay(hand);
    }

    public void OnDeath()
    {
        Destroy(hand.gameObject);
        hand = null;
        deck.ForEach(x => Destroy(x.gameObject));
        deck.Clear();
        discard.ForEach(x => Destroy(x.gameObject));
        discard.Clear();
        Debug.Log(string.Format("Enemy {0} died.", enemyData.enemyName));
        EventManager.EnemyKilled(this.enemyData);
    }

    public void SetUIpositions()
    {
        // Vector3 coordinates = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(AnchorMoveDisplay.transform.position);
        // CanvasMoveDisplay.transform.position = coordinates;
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int index = Random.Range(i, deck.Count);
            deck[i] = deck[index];
            deck[index] = temp;
        }
    }



    public void TakeDamage(int x)
    {
        healthEffects.TakeDamage(x);
    }

    public void OnMouseOver()
    {
        toolTiptimer += Time.deltaTime;
        if(!toolTipShowing && toolTiptimer > toolTipDelay)
        {
            toolTipShowing = true;
            tooltipController.ShowHide(true);
        }
        
        if (combatController.ActiveCard != null  && combatController.ActiveCard.targetRequired)
            SetTarget(true);
        

        if (combatController.TargetedEnemy is null) combatController.TargetedEnemy = this;
    }



    public void OnMouseExit()
    {
        toolTiptimer = 0;
        tooltipController.ShowHide(false);
        toolTipShowing = false;
        SetTarget(false);
        if (combatController.TargetedEnemy == this) combatController.TargetedEnemy = null;
    }

    public void OnMouseDown()
    {
        //combatController.CardUsed(this);
        Debug.Log("Press Enemy");
    }
}
