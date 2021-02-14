using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using System;

public class CombatController : StateMachine
{
    public GameObject TemplateCard;
    public BezierPath bezierPath;
    public GameObject TemplateEnemy;
    public List<EnemyData> enemyDatas = new List<EnemyData>();
    public TMP_Text lblEnergy;
    public List<Transform> trnsEnemyPositions;
    public Camera CombatCamera;
    public GameObject content;
    public Transform cardPanel;
    public Transform cardHoldPos;

    public CombatActorHero Hero; 

    public GameObject txtDeck;
    public GameObject txtDiscard;
    public int HandSize = 10;
    public int DrawCount = 5;
    public float offset = 50;
    public float handDistance = 150;
    public float handHeight = 75;
    public int energyTurn = 3;
    public float origoCardRot = 1000f;
    public float origoCardPos = 1000f;
    public float handDegreeBetweenCards = 10f;
    public CombatActorEnemy targetedEnemy;

    KeyCode[] AlphaNumSelectCards = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

    [HideInInspector]
    public int backingEnergy;
    [HideInInspector]
    public int cEnergy
    {
        get { return backingEnergy; }
        set { backingEnergy = value; lblEnergy.text = backingEnergy.ToString(); }
    }

    private List<CardData> DeckData;
    public List<CardCombat> Deck = new List<CardCombat>();
    public List<CardCombat> Hand = new List<CardCombat>();
    public List<CardCombat> Discard = new List<CardCombat>();
    public List<CardCombat> createdCards = new List<CardCombat>();
    public List<CardCombat> discardCardEndTurn = new List<CardCombat>();
    public List<CardCombat> discardCardPlayerTurn = new List<CardCombat>();

    public List<CombatActorEnemy> EnemiesInScene = new List<CombatActorEnemy>();
    private int amountOfEnemies;
    public bool mouseInsidePanel = false;

    float cardPanelwidth;

    // we could remove enemies from list but having another list will 
    // make it easier to reference previous enemies for resurection etc
    public List<CombatActorEnemy> DeadEnemiesInScene = new List<CombatActorEnemy>();

    //[HideInInspector]
    public CardCombat _activeCard;
    public CardCombat previousActiveCard;
    [HideInInspector]
    public CombatActorEnemy ActiveEnemy;
    public AnimationCurve transitionCurve;
    public bool acceptInput = true;
    private bool drawingCards = false;

    public Canvas canvas;
    private void Awake()
    {
        cardPanelwidth = cardPanel.GetComponent<RectTransform>().rect.width;
    }

    public CardCombat ActiveCard 
    {
        get
        {
            return _activeCard;
        }
        set
        {
            _activeCard = value;
            EnemiesInScene.ForEach(x => x.SetTarget(false));
        }
    }

    void Update()
    {
        for(int i = 0; i < AlphaNumSelectCards.Length && i < Hand.Count; i++)
        {
            if (Input.GetKeyDown(AlphaNumSelectCards[i]) && WorldSystem.instance.worldState == WorldState.Combat)
            {
                if(ActiveCard == Hand[i])
                    ActiveCard.OnMouseRightClick(false);
                else
                    Hand[i].OnMouseClick();

                break;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) && acceptInput == true && discardCardEndTurn.Count == 0)
        {
            PlayerInputEndTurn();
        }
    }


    public void DetectCanvasClick()
    {
        if (ActiveCard != null)
        {
            if (Input.GetMouseButton(0))
                ActiveCard.OnMouseClick();
            else
                ActiveCard.OnMouseRightClick(false);
        }
    }

    public void PlayerInputEndTurn()
    {
        // have to ahve a function for the ui button
        EndState();
    }

    public void SetUpEncounter(List<EnemyData> enemyDatas = null)
    {

        DeckData = WorldSystem.instance.characterManager.playerCardsData;
        DeckData.ForEach(x => Discard.Add(CreateCardFromData(x)));

        if(enemyDatas == null)
            enemyDatas = WorldSystem.instance.encounterManager.currentEncounter.encounterData.enemyData;

        for(int i = 0; i < enemyDatas.Count; i++)
        {
            GameObject EnemyObject = Instantiate(TemplateEnemy, trnsEnemyPositions[i].position, Quaternion.Euler(0, 0, 0), this.transform) as GameObject;
            CombatActorEnemy combatActorEnemy = EnemyObject.GetComponent<CombatActorEnemy>();
            combatActorEnemy.combatController = this;
            combatActorEnemy.ReadEnemyData(enemyDatas[i]);
            EnemiesInScene.Add(combatActorEnemy);
        }

        amountOfEnemies = EnemiesInScene.Count;

        SetState(new EnterCombat(this));
    }

    public Vector3 GetCardScale()
    {
        return Vector3.one;
    }


    public (Vector3 Position, Vector3 Angles) GetPositionInHand(CardCombat card)
    {
        return GetPositionInHand(Hand.IndexOf(card));
    }

    public (Vector3 Position,Vector3 Angles) GetPositionInHand(int CardNr)
    {
        float localoffset = (Hand.Count % 2 == 0) ? handDegreeBetweenCards/2 : 0;
        Vector3 newPos = GetPositionInHandCircle((CardNr - (Hand.Count / 2)) *handDegreeBetweenCards + localoffset);
        Vector3 origo = new Vector3(0, -origoCardRot, 0);

        Vector3 direction = newPos - origo;
        float angle = Vector3.Angle(newPos - origo, new Vector3(0, 1, 0))*(newPos.x > 0 ? -1 :1);
        Vector3 angles = new Vector3(0, 0, angle);

        return (newPos, angles);
    }

    // 0 degree will yield top of curve.
    private Vector3 GetPositionInHandCircle(float degree)
    {
        degree = degree * Mathf.Deg2Rad;
        return new Vector3(origoCardRot*Mathf.Sin(degree),origoCardRot*Mathf.Cos(degree) - origoCardRot + handHeight,0); 
    }

    internal void ResetSiblingIndexes()
    {
        for (int i = 0; i < Hand.Count; i++)
            Hand[i].transform.SetSiblingIndex(i);
    }


    private void DrawCards(int x)
    {
        for(int i = 0; i < x; i++)
        {
            if(Deck.Count == 0)
            {
                Deck.AddRange(Discard);
                Discard.Clear();
                ShuffleDeck();
            }

            if (Deck.Count != 0)
            {
                Deck[0].ResetCard();
                Hand.Add(Deck[0]);
                Deck.RemoveAt(0);
            }
        }

        StartCoroutine(WaitForAnimationsDraw());
    }

    private void ShuffleDeck()
    {
        for(int i = 0; i < Deck.Count; i++)
        {
            CardCombat temp = Deck[i];
            int index = UnityEngine.Random.Range(i, Deck.Count);
            Deck[i] = Deck[index];
            Deck[index] = temp;
        }
    }

    public bool CardisSelectable(CardCombat card, bool silentCheck = true)
    {
        bool selectable = card.cardData.cost <= cEnergy && card.selectable && !drawingCards;
        if (!silentCheck && card.cardData.cost > cEnergy)
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough energy!");    
        }

        return selectable;
    }
    CardCombat CreateCardFromData(CardData cardData)
    {
        GameObject CardObject = Instantiate(TemplateCard, new Vector3(-10000, -10000, -10000), Quaternion.Euler(0, 0, 0)) as GameObject;
        CardObject.transform.SetParent(cardPanel, false);
        CardObject.transform.localScale = GetCardScale();
        CardObject.GetComponent<BezierFollow>().route = bezierPath.transform;
        CardCombat Card = CardObject.GetComponent<CardCombat>();
        Card.cardData = cardData;
        Card.cardPanel = cardPanel.GetComponent<RectTransform>();
        Card.combatController = this;
        Card.BindCardData();
        createdCards.Add(Card);
        HideCard(CardObject);
        return Card;
    }


    internal void CancelCardSelection()
    {
        ActiveCard = null;
        ResetSiblingIndexes();
    }

    public void EndTurn()
    {
        Hero.healthEffects.EffectsStartTurn();
        while (Hand.Count > 0)
        {
            CardCombat card = Hand[0];
            card.MouseReact = false;
            discardCardEndTurn.Add(card);
            DiscardCard(card);
            DiscardCardToPile(card);
        }

        StartCoroutine(WaitForAnimationsDiscardEndTurn());
    }

    public void StartTurn()
    {
        // ENEMY TURN'
        
        DrawCards(DrawCount);
        Hand.ForEach(x => x.selected = false);
        Hand.ForEach(x => x.transform.localScale = new Vector3(1,1,1));
        Debug.Log("New turn started. Cards in Deck, Hand, Discard: " + Deck.Count + "," + Hand.Count + "," + Discard.Count);
        txtDeck.GetComponent<Text>().text = "Deck:\n" + Deck.Count;
        txtDiscard.GetComponent<Text>().text = "Discard:\n" + Discard.Count;

        StartCoroutine(RulesSystem.instance.StartTurn());

        EnemiesInScene.ForEach(x => x.healthEffects.EffectsStartTurn());
    }

    IEnumerator WaitForAnimationsDiscardEndTurn(Action endArgument = null)
    {
        while (discardCardEndTurn.Count > 0)
        {
            yield return new WaitForSeconds(0.01f);
        }
        discardCardEndTurn.Clear();
        if(endArgument != null)
            endArgument.Invoke();
    }

    IEnumerator WaitForAnimationsDiscardsPlayerTurn(Action endArgument = null)
    {
        while (discardCardPlayerTurn.Count > 0)
        {
            Debug.Log(discardCardPlayerTurn.Count);
            yield return new WaitForSeconds(0.01f);
        }
        WinCombat();
        discardCardPlayerTurn.Clear();
        if(endArgument != null)
            endArgument.Invoke();
    }

    IEnumerator WaitForAnimationsDraw()
    {
        drawingCards = true;
        int i = 0;
        Debug.Log(Hand.Count);
        while (i < Hand.Count)
        {
            DisplayCard(i);
            i++;
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Done");
        drawingCards = false;
        ResetSiblingIndexes();
    }

    public void CardDemarkTransition(CardCombat card)
    {
        discardCardEndTurn.Remove(card);
    }

    public void DiscardCard(CardCombat card)
    {
        Discard.Add(card);
        Hand.Remove(card);
    }

    public void DiscardCardToPile(CardCombat card)
    {

        AnimateCardDiscard(card);

        card.transform.localScale = GetCardScale();
        
        if(discardCardPlayerTurn.Count > 0)
            discardCardPlayerTurn.RemoveAt(0);
        txtDiscard.GetComponent<Text>().text = Discard.Count.ToString();
    }

    private void HideCard(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(-10000, -10000, -10000);
        gameObject.SetActive(false);
    }

    private void AnimateCardDiscard(CardCombat card)
    {
        card.inTransition = true;
        card.AnimateCardByPathDiscard();
    }

    private void AnimateCardDraw(CardCombat card, Vector3 endPos, Vector3 endAngles)
    {
        card.AnimateCardByCurve(endPos, endAngles, Vector3.one, false, true);
    }

    private void DisplayCard(int i)
    {
        Hand[i].gameObject.SetActive(true);
        Hand[i].transform.position = txtDeck.transform.position;
        Hand[i].transform.localScale = Vector3.zero;
        Hand[i].MouseReact = false;
        (Vector3, Vector3) TransInfo = GetPositionInHand(i);
        AnimateCardDraw(Hand[i], TransInfo.Item1, TransInfo.Item2);
    }

    public void ToggleInsidePanel()
    {
        if (mouseInsidePanel)
        {
            mouseInsidePanel = false;
        }
        else
        {
            mouseInsidePanel = true;
        }
    }

    private bool MouseInsideArea()
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cardPanel.GetComponent<RectTransform>(), Input.mousePosition, WorldSystem.instance.cameraManager.mainCamera, out result);
        Debug.Log(result);
        Debug.Log(cardPanel.GetComponent<RectTransform>().rect.Contains(result));

        if (cardPanel.GetComponent<RectTransform>().rect.Contains(result))
        {
            Debug.Log("True");
            return true;
        }

        Debug.Log("False");
        return false;
    }

    public void CardUsed(CombatActorEnemy enemy = null)
    {
        Debug.Log("Card used from Controller");
        targetedEnemy = enemy;
        if (ActiveCard is null)
            return;

        if((ActiveCard.cardData.targetRequired && enemy is null) || MouseInsideArea())
        {
            ActiveCard.DeselectCard();
            return;
        }

        cEnergy -= ActiveCard.cardData.cost;

        RulesSystem.instance.CarryOutCardSelf(ActiveCard.cardData, Hero);

        List<CombatActorEnemy> targetedEnemies = new List<CombatActorEnemy>();
        if (ActiveCard.cardData.targetRequired)
        {
            if (enemy != null)
                targetedEnemies.Add(enemy);
        }
        else
            targetedEnemies.AddRange(EnemiesInScene);


        foreach (CombatActorEnemy e in targetedEnemies) {
            RulesSystem.instance.CarryOutCard(ActiveCard.cardData,Hero, e);
            if (e.healthEffects.hitPoints <= 0)
                KillEnemy(e);
        } 

        discardCardPlayerTurn.Add(ActiveCard);
        ActiveCard.UseCard();
        CheckVictory();
        ActiveCard = null;
        RefreshHandPositionsAfterCardUsed();
    }

    public void RefreshHandPositionsAfterCardUsed()
    {
        for(int i = 0; i < Hand.Count; i++)
        {
            (Vector3, Vector3) TransInfo = GetPositionInHand(i);
            Hand[i].StartLerpPosition(TransInfo.Item1, TransInfo.Item2);
        }
    }

    private void KillEnemy(CombatActorEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
        DeadEnemiesInScene.Add(enemy);
        EnemiesInScene.Remove(enemy);
    }

    public void CheckVictory()
    {
        Debug.Log(DeadEnemiesInScene.Count);
        if(DeadEnemiesInScene.Count == amountOfEnemies)
        {

            StartCoroutine(WaitForAnimationsDiscardsPlayerTurn());
        }
    }

    public void WinCombat()
    {
        //called from DEBUG
        while(EnemiesInScene.Count > 0)
        {
            KillEnemy(EnemiesInScene[0]);
        }
        Debug.Log("Victory!");
        ResetCombat();
        WorldSystem.instance.uiManager.rewardScreen.GetCombatReward();
    }

    void ResetCombat()
    {
        DeadEnemiesInScene.Clear();
        Deck.Clear();
        Hand.Clear();
        Discard.Clear();
        EnemiesInScene.Clear();
        discardCardEndTurn.Clear();
        foreach (CardCombat card in createdCards)
        {
            DestroyImmediate(card);
        }
    }
}
