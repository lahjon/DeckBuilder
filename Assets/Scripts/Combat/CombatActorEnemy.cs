using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;


public class CombatActorEnemy : CombatActor
{
    public IntentDisplay intentDisplay;
    public GameObject test;
    public TMP_Text txtMoveDisplay;
    public EnemyData enemyData;
    public GameObject CanvasMoveDisplay;
    public string enemyName;

    public GameObject cardTemplate;

    public GameObject AnchorMoveDisplay;

    public Canvas canvasEffects;
    public Canvas canvasIntent;
    public GameObject target;

    public bool stochasticReshuffle = true;

    public Card hand;

    float toolTiptimer = 0;
    float toolTipDelay = 1f;
    bool toolTipShowing = false;
    [SerializeField] Enemy enemyScript;
    public GameObject targetRing;


    public override void SetupAlliesEnemies(){
        allies.AddRange(CombatSystem.instance.EnemiesInScene.Where(x=> x != this));
        enemies.Add(CombatSystem.instance.Hero);
    }

    void OnDestroy()
    {
        DOTween.Kill(this);
    }

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

        deck = new ListEventReporter<Card>();
        discard = new ListEventReporter<Card>();

        foreach(CardData cardData in enemyData.deck)
        {
            GameObject cardObject = Instantiate(cardTemplate, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0));
            cardObject.transform.SetParent(gameObject.transform);
            Card card = cardObject.GetComponent<Card>();
            card.owner = this; 
            card.cardData = cardData;
            card.BindCardData();
            deck.Add(card);
        }

        enemyName = enemyData.enemyName;
        stochasticReshuffle = enemyData.stochasticReshuffle;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemyData.artwork;

        if (enemyData.characterArt != null)
        {
            GameObject enemyArt = Instantiate(enemyData.characterArt);
            enemyArt.transform.SetParent(this.gameObject.transform);
            enemyArt.transform.localPosition = Vector3.zero ;
            spriteRenderer.color = new Color(1,1,1,0);
            collision = gameObject.AddComponent<BoxCollider2D>();
            collision.size = enemyArt.GetComponent<BoxCollider2D>().size;
            collision.offset = enemyArt.GetComponent<BoxCollider2D>().offset;

            Enemy enemy;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<Enemy>(out enemy))
                {
                    enemyScript = enemy;
                }
            }
        }
        else
        {
            spriteRenderer.sprite = enemyData.artwork;
            collision = gameObject.AddComponent<BoxCollider2D>();
        }

        target.transform.localPosition += new Vector3(0, collision.size.y / 2, 0);
        AnchorToolTip.transform.localPosition = new Vector3(collision.size.x / 2, collision.size.y * 0.75f);
        intentDisplay.SetPosition();
       
        maxHitPoints = enemyData.StartingHP;
        hitPoints = enemyData.StartingHP;
        if(enemyData.shuffleInit) ShuffleDeck();
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
        if (card.Attacks.Any()) displayDamage = RulesSystem.instance.CalculateDamage(card.Attacks[0].Value.value, this, CombatSystem.instance.Hero);
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

    public void OnDeath()
    {
        collision.enabled = false;
        Debug.Log("Enemy Dead!");

        foreach (CardEffect re in effectTypeToRule.Values)
            re.OnActorDeath();

        List<EffectType> effects = effectTypeToRule.Keys.ToList();
        foreach (EffectType e in effects)
        {
            effectTypeToRule[e]?.Dismantle();
        }
        

        if(hand != null) Destroy(hand.gameObject);
        hand = null;
        deck.ForEach(x => Destroy(x.gameObject));
        deck.Clear();
        discard.ForEach(x => Destroy(x.gameObject));
        discard.Clear();

        foreach (CombatActor actor in allies)
            actor.allies.Remove(this);

        foreach (CombatActor actor in enemies)
            actor.enemies.Remove(this);

        StartDeathAnimation();
    
        EventManager.EnemyKilled(enemyData);
    }

    void StartDeathAnimation()
    {
        float timeDelay = 0.5f;
        SpriteRenderer spriteRenderer;
        if (enemyScript != null)
        {
            enemyScript.material.DOFloat(0, "Dissolve", 1.0f).OnComplete( () =>
                {DOTween.To(() => 0, x => { }, 0, timeDelay).OnComplete( () => Destroy(gameObject));}
                );
        }
        else if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            spriteRenderer.DOColor(new Color(1,1,1,0), 1.0f).OnComplete( () =>
                {DOTween.To(() => 0, x => { }, 0, timeDelay).OnComplete( () => Destroy(gameObject));}
                );
        }
        
    }   

    public void EnableTarget()
    {
        targetRing.SetActive(true);
    }

    public void DisableTarget()
    {
        targetRing.SetActive(false);
    }

    public void OnMouseOver()
    {
        toolTiptimer += Time.deltaTime;
        if(!toolTipShowing && toolTiptimer > toolTipDelay)
        {
            toolTipShowing = true;
        }
        // if (CombatSystem.instance.ActiveCard != null  && CombatSystem.instance.ActiveCard.targetRequired)
        //     SetTarget(true);
        

        //if(CombatSystem.instance.TargetedEnemy != this) CombatSystem.instance.TargetedEnemy = this;
    }

    public void OnMouseUp()
    {
        CombatSystem.instance.TargetedEnemy = this;
    }


    public void OnMouseExit()
    {
        toolTiptimer = 0;
        toolTipShowing = false;
        //SetTarget(false);
        //if (CombatSystem.instance.TargetedEnemy == this) CombatSystem.instance.TargetedEnemy = null;
    }

    public override void RecalcDamage()
    {
        UpdateIntentDisplay(hand);
    }

    public override int GetStat(StatType stat) => 0;
}
