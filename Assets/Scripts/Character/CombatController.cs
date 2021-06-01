using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using System;

public class CombatController : MonoBehaviour
{
    public CombatOverlay combatOverlay;
    public GameObject TemplateCard;
    public BezierPath discardPath;
    public SelectionPath selectionPath;
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
    public int drawCount;
    public float offset = 50;
    public float handDistance = 150;
    public float handHeight = 75;
    public int energyTurn;
    public float origoCardRot = 1000f;
    public float origoCardPos = 1000f;
    public float handDegreeBetweenCards = 10f;

    public AnimationCurve transitionCurve;
    public bool acceptEndTurn = true;
    public bool acceptActions = true;
    public Canvas canvas;
    private CombatActorEnemy _activeEnemy;

    public EncounterDataCombat encounterData;

    public Animator animator;

    public List<Vector3> formationPositions;


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
    public List<CardCombat> Hand = new List<CardCombat>();
    public List<CardCombat> createdCards = new List<CardCombat>();

    public List<CombatActorEnemy> EnemiesInScene = new List<CombatActorEnemy>();
    public bool mouseInsidePanel = false;

    // we could remove enemies from list but having another list will 
    // make it easier to reference previous enemies for resurection etc
    public List<CombatActorEnemy> DeadEnemiesInScene = new List<CombatActorEnemy>();
    public List<CombatActor> ActorsInScene { get { 
            List<CombatActor> retList = new List<CombatActor>(EnemiesInScene);
            retList.Add(Hero);
            return retList;
        } }

    public Queue<(CardCombat card, CombatActor target)> CardQueue =
        new Queue<(CardCombat card, CombatActor target)>();

    [SerializeField]
    public Card         InProcessCard;
    public CombatActor  InProcessTarget;

    public Queue<CardCombat> HeroCardsWaiting = new Queue<CardCombat>();
    public Queue<CombatActorEnemy> enemiesWaiting = new Queue<CombatActorEnemy>();


    public CombatActor ActiveActor;

    //[HideInInspector]
    public CardCombat _activeCard;


    public CombatActorEnemy TargetedEnemy
    {
        get
        {
            return _activeEnemy;
        }
        set
        {
            _activeEnemy = value;
            if (!(ActiveCard is null))
            {
                ActiveCard.animator.SetBool("HasTarget", !(value is null));
                SetCardCalcDamage(ActiveCard, value);
            }

        }
    }

    public CardCombat ActiveCard 
    {
        get
        {
            return _activeCard;
        }
        set
        {
            // pre stuff
            if (_activeCard != null && value != _activeCard)
                _activeCard.selected = false;

            //overwrite
            _activeCard = value;
            EnemiesInScene.ForEach(x => x.SetTarget(false));
            foreach (CardCombat card in Hand)
                if (card != value) card.MouseReact = (value is null);
            if (_activeCard != null)
                _activeCard.animator.SetBool("HasTarget", _activeEnemy != null);
        }
    }


    #region Plumbing, Setup, Start/End turn
    private void OnDisable()
    {
        Debug.Log("cc disabled");
    }

    public void SetUpEncounter()
    {
        Hero.combatController = this;
        Hero.maxHitPoints = WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Health);
        Hero.hitPoints = WorldSystem.instance.characterManager.currentHealth;

        DeckData = WorldSystem.instance.characterManager.playerCardsData;

        foreach(CardData cd in DeckData)
        {
            CardCombat card = CardCombat.CreateCardCombatFromData(cd);
            Hero.deck.Add(card);
        }

        Hero.ShuffleDeck();

        enemyDatas = encounterData.enemyData;
        formationPositions = encounterData.formation.GetLocalPositions();

        //enemyDatas.ForEach(x => Debug.Log(x));

        for (int i = 0; i < enemyDatas.Count; i++)
        {
            GameObject EnemyObject = Instantiate(TemplateEnemy, transform);
            EnemyObject.transform.localPosition = formationPositions[i];
            CombatActorEnemy combatActorEnemy = EnemyObject.GetComponent<CombatActorEnemy>();
            combatActorEnemy.combatController = this;
            combatActorEnemy.ReadEnemyData(enemyDatas[i]);
            EnemiesInScene.Add(combatActorEnemy);
        }
        animator.SetTrigger("StartSetup");
    }

    public void StartCombat()
    {
        //Debug.Log("Starting combat");
        content.SetActive(true);
        SetUpEncounter();
    }


    public void BindCharacterData()
    {
        energyTurn = WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Energy);
        drawCount =  WorldSystem.instance.characterManager.defaultDrawCardAmount + WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Wit);
        Hero.maxHitPoints = WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Health);
        Hero.maxHitPoints = WorldSystem.instance.characterManager.currentHealth;
    }

    public void EndTurn()
    {
        foreach (CardCombat card in Hand)
        {
            card.MouseReact = false;
            card.selectable = false;
        }

        StartCoroutine(DiscardAllCards());
    }

    private void KillEnemy(CombatActorEnemy enemy)
    {
        enemy.OnDeath();
        if (TargetedEnemy == enemy) TargetedEnemy = null;
        DeadEnemiesInScene.Add(enemy);
        EnemiesInScene.Remove(enemy);
    }
    public void WinCombat()
    {
        animator.SetTrigger("Win");
    }

    public void CleanUp()
    {
        while (EnemiesInScene.Count > 0)
        {
            KillEnemy(EnemiesInScene[0]);
        }

        Hand.ForEach(x => Destroy(x.gameObject));
        Hand.Clear();
        Hero.deck.ForEach(x => Destroy(x.gameObject));
        Hero.deck.Clear();
        Hero.discard.ForEach(x => Destroy(x.gameObject));
        Hero.discard.Clear();
        Hero.ClearAllEffects();

        DeadEnemiesInScene.Clear();
    }

    public CombatActorEnemy GetRandomEnemy()
    {
        int id = UnityEngine.Random.Range(0, EnemiesInScene.Count);
        return EnemiesInScene[id];
    }

    public (CardEffect effect, List<CombatActor> targets) GetTargets(CombatActor source, CardEffect effect, CombatActor suppliedTarget)
    {
        List<CombatActor> targets = new List<CombatActor>();

        if (effect.Target == CardTargetType.Self)
            targets.Add(source);
        else if (effect.Target == CardTargetType.EnemyAll)
            targets.AddRange(EnemiesInScene);
        else if (effect.Target == CardTargetType.EnemySingle)
            targets.Add(suppliedTarget);
        else if (effect.Target == CardTargetType.EnemyRandom)
            targets.Add(GetRandomEnemy());
        else if(effect.Target == CardTargetType.All)
        {
            targets.Add(Hero);
            targets.AddRange(EnemiesInScene);
        }

        CardEffect returnEffect;

        if (effect.Target == CardTargetType.EnemyRandom)
        {
            returnEffect = effect.Clone();
            returnEffect.Times = 1;
        }
        else
            returnEffect = effect;

        return (returnEffect, targets);
    }

    public int PreviewCalcDamageAllEnemies(int value)
    {
        int possibleDamage = PreviewCalcDamageEnemy(value, EnemiesInScene[0]);
        foreach (CombatActor enemy in EnemiesInScene)
            if (possibleDamage != PreviewCalcDamageEnemy(value, enemy))
                return value;

        return possibleDamage;
    }

    public int PreviewCalcDamageEnemy(int value, CombatActor enemy)
    {
        return RulesSystem.instance.CalculateDamage(value, Hero, enemy);
    }

    public void SetCardCalcDamage(CardCombat card, CombatActor enemy = null)
    {
        if (card.Damage.Value == 0) return;

        int baseVal = card.Damage.Value;
        int calcDamage = enemy is null ? PreviewCalcDamageAllEnemies(baseVal) : PreviewCalcDamageEnemy(baseVal, enemy);
        if(calcDamage != card.calcDamage)
        {
            card.calcDamage = calcDamage;
            card.RefreshDescriptionText();
        }
    }

    public void RecalcAllCardsDamage()
    {
        foreach(CardCombat card in Hand)
        {
            SetCardCalcDamage(card);
        }
    }

    #endregion

    #region Positioning and Scale
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
        // Positional Info
        float localoffset = (Hand.Count % 2 == 0) ? handDegreeBetweenCards/2 : 0;
        float degree = ((CardNr - (Hand.Count / 2)) * handDegreeBetweenCards + localoffset);
        return GetTargetPositionFromDegree(degree);
    }

    public float GetCurrentDegree(CardCombat card)
    {
        if (!Hand.Contains(card))
        {
            Debug.LogError("Current degree requested for card not in hand!");
            return -1f;
        }

        float degree = Mathf.Rad2Deg*Mathf.Asin(Mathf.Abs(card.transform.localPosition.x) /origoCardPos);

        if (card.transform.localPosition.x < 0)
            degree *= -1;
        
        return degree;
    }

    public float GetTargetDegree(CardCombat card)
    {
        if (!Hand.Contains(card))
        {
            Debug.LogError("Current degree requested for card not in hand!");
            return -1f;
        }

        float localoffset = (Hand.Count % 2 == 0) ? handDegreeBetweenCards / 2 : 0;
        float degree = ((Hand.IndexOf(card) - (Hand.Count / 2)) * handDegreeBetweenCards + localoffset);

        return degree;
    }

    public void SetCardTransFromDegree(CardCombat card, float degree)
    {
        (Vector3 pos, Vector3 angles) transInfo = GetTargetPositionFromDegree(degree);
        card.transform.localPosition = transInfo.pos;
        card.transform.localEulerAngles = transInfo.angles;
    }

    public (Vector3 Position, Vector3 Angles) GetTargetPositionFromDegree(float degree)
    {
        degree *= Mathf.Deg2Rad;
        Vector3 newPos = new Vector3(origoCardPos * Mathf.Sin(degree), origoCardPos * Mathf.Cos(degree) - origoCardPos + handHeight, 0);

        //Angling info
        Vector3 origo = new Vector3(0, -origoCardRot, 0);
        float angle = Vector3.Angle(newPos - origo, new Vector3(0, 1, 0)) * (newPos.x > 0 ? -1 : 1);
        Vector3 angles = new Vector3(0, 0, angle);

        return (newPos, angles);
    }


    internal void ResetSiblingIndexes()
    {
        int cursor = 0;
        foreach(CardCombat card in Hand)
            if (card != ActiveCard)
                card.transform.SetSiblingIndex(cursor++);
    }


    public void RefreshHandPositions(CardCombat excludeCard = null)
    {
        foreach (CardCombat card in Hand)
            if (card != excludeCard)
                card.animator.SetBool("NeedFan", true);
    }


    internal void ReportDeath(CombatActor combatActor)
    {
        if (combatActor != Hero)
            KillEnemy((CombatActorEnemy)combatActor);

        if (EnemiesInScene.Count == 0)
            WinCombat();
    }
    #endregion

    #region Draw and Discard Cards, Deck Management

    public IEnumerator DrawCards(int CardsToDraw) { 
        for(int i = 0; i < CardsToDraw; i++)
        {
            if (Hand.Count == HandSize)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Hand is full yo");
                break;
            }

            if (Hero.deck.Count == 0)
            {
                Hero.deck.AddRange(Hero.discard);
                Hero.discard.Clear();
                Hero.ShuffleDeck();
            }

            if (Hero.deck.Count == 0)
            {
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("no cards to draw broooow");
                break;
            }

            DrawSingleCard();
            yield return new WaitForSeconds(0.14f);
        }
    }

    public void DrawSingleCard()
    {
        CardCombat card = (CardCombat)Hero.deck[0];
        Hero.deck.RemoveAt(0);
        Hand.Add(card);
        card.animator.SetTrigger("StartDraw");
        UpdateDeckTexts();
    }
    public void ReturnCardFromDiscard(CardCombat discardCard = null)
    {
        CardCombat card;
        if (discardCard != null)
        {
            card = (CardCombat)Hero.discard.Where(x => x == discardCard).FirstOrDefault();
        }
        else
        {
            card = (CardCombat)Hero.discard[0];
        }

        Hero.discard.Remove(card);
        Hand.Add(card);
        card.animator.SetTrigger("StartDraw");
        UpdateDeckTexts();
    }

    internal void CancelCardSelection()
    {
        ActiveCard.selected = false;
        ActiveCard = null;
        ResetSiblingIndexes();
    }

    public IEnumerator DiscardAllCards()
    {
        while (Hand.Count > 0)
        {
            yield return StartCoroutine(DiscardCard(Hand[Hand.Count - 1]));
        }
    }

    public IEnumerator DiscardCard(CardCombat card)
    {
        Hand.Remove(card);
        Hero.discard.Add(card);

        card.animator.SetTrigger("Discarded");
        RefreshHandPositions();
        yield return new WaitForSeconds(0.1f);
        UpdateDeckTexts();
    }

    public void UpdateDeckTexts()
    {
        txtDeck.GetComponent<Text>().text = "Deck:\n" + Hero.deck.Count;
        txtDiscard.GetComponent<Text>().text = "Discard:\n" + Hero.discard.Count;
    }

    #endregion

    #region CardUsage logic, user input
    void Update()
    {
        for (int i = 0; i < AlphaNumSelectCards.Length && i < Hand.Count; i++)
        {
            if (Input.GetKeyDown(AlphaNumSelectCards[i]) && WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            {
                if (ActiveCard == Hand[i])
                    ActiveCard.OnMouseRightClick(false);
                else
                    Hand[i].OnMouseClick();

                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && acceptEndTurn == true)
        {
            PlayerInputEndTurn();
        }
    }

    public void DetectCanvasClick()
    {
        //Debug.Log("Canvas detected click");
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
        animator.SetTrigger("PlayerTurnEnd");
    }

    public bool CardisSelectable(CardCombat card, bool silentCheck = true)
    {
        bool selectable = card.cost <= cEnergy && card.selectable;
        if (!silentCheck && card.cost > cEnergy)
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough energy!");    
        }

        return selectable;
    }

    private bool MouseInsideArea()
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cardPanel.GetComponent<RectTransform>(), Input.mousePosition, WorldSystem.instance.cameraManager.mainCamera, out result);
        //Debug.Log(result);

        if (cardPanel.GetComponent<RectTransform>().rect.Contains(result))
        {
            //Debug.Log("True");
            return true;
        }

        //Debug.Log("False");
        return false;
    }

    public void SelectedCardTriggered()
    {
        //Debug.Log("mouseclicked");
        if (ActiveCard is null)
            return;

        if (MouseInsideArea())
        {
            CancelCardSelection();
            return;
        }

        ActiveCard.animator.SetTrigger("MouseClicked");
        ActiveCard = null;
    }

    #endregion , user input
}
