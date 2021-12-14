using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using System;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem instance;
    public CombatOverlay combatOverlay;
    public GameObject TemplateCard;
    public BezierController bezierController;
    public SelectionPath selectionPath;
    public GameObject TemplateEnemy;
    public Camera CombatCamera;
    public Transform cardPanel;
    public Transform cardHoldPos;
    public bool hasCompanion;
    public CombatStateType combatStateType;
    public Transform environmentAnchor;
    public GameObject[] environmentPrefabs;
    GameObject currentEnvironment;
    public CombatActorType actorTurn;
    public Button companionButton;
    public List<ItemEffect> effectOnCombatStart = new List<ItemEffect>();
    Queue<ItemEffect> queuedEffects = new Queue<ItemEffect>();

    public CombatCardPresenter cardPresenter;
    public bool deSelectOnMouseLeave = true;

    public CombatActorHero Hero;
    public CombatActorCompanion companion;
    public GameObject content;

    public GameObject txtDeck;
    public GameObject txtDiscard;
    public int HandSize = 10;
    public int drawCount;
    public float offset = 50;
    public float handDistance = 150;
    public float handHeight = 75;
    public float origoCardRot = 1000f;
    public float origoCardPos = 1000f;
    public float baseDegreeBetweenCards = 10f;
    public float clampedDegreeBetweenCards { get => baseDegreeBetweenCards * Mathf.Min(1.0f, (6.0f - 1.0f) / (Hand.Count - 1)); }
    public bool acceptEndTurn = true;
    public bool acceptActions = true;
    public bool acceptProcess = false;
    public bool quickPlayCards;
    public List<EnemyData> enemyDatas = new List<EnemyData>();

    public EncounterDataCombat encounterData;

    public Animator animator;

    public List<Formation> formations = new List<Formation>();
    public List<Vector3> formationPositions;

    Queue<object> drawnToResolve = new Queue<object>();

    public GameObject templateEnergyDisplay;
    public Transform energyParent;

    public Dictionary<EnergyType, int>  energyMax   = new Dictionary<EnergyType, int>();
    public Dictionary<EnergyType, int>  energyTurn  = new Dictionary<EnergyType, int>();
    Dictionary<EnergyType, int>         cEnergy     = new Dictionary<EnergyType, int>();

    Dictionary<EnergyType, CardCostDisplay> displayEnergies = new Dictionary<EnergyType, CardCostDisplay>();

    public bool forcePlayCards = false;

    public int GetEnergy(EnergyType type) => cEnergy.ContainsKey(type) ? cEnergy[type] : 0;
    public void ModifyEnergy(Dictionary<EnergyType, int> changes, bool enforceMax = false)
    {
        bool changedAny = false;
        foreach (EnergyType type in changes.Keys)
        {
            if (!displayEnergies.ContainsKey(type)) RegisterEnergyType(type);
            
            int valPre = (cEnergy.ContainsKey(type) ? cEnergy[type] : 0);
            cEnergy[type] = changes[type] + valPre;

            if (enforceMax) cEnergy[type] = Mathf.Min(cEnergy[type], energyMax[type]);

            if (cEnergy[type] - valPre != 0) changedAny = true;
            displayEnergies[type].lblEnergy.text = cEnergy[type].ToString();
        }

        if (changedAny)
        {
            EventManager.EnergyChanged();
            EventManager.EnergyInfoChanged(changes);
        }
    }

    public void ModifyEnergy(EnergyType type, int change, bool enforceMax = false) => ModifyEnergy(new Dictionary<EnergyType, int> { { type, change } }, enforceMax);

    [SerializeField] private CombatActorEnemy _targetedEnemy;

    public List<CardVisual> deckData;
    public ListEventReporter<CardCombat> Hand = new ListEventReporter<CardCombat>(EventManager.HandCountChanged);
    public List<CardCombat> createdCards = new List<CardCombat>();

    public List<CombatActorEnemy> EnemiesInScene = new List<CombatActorEnemy>();
    public bool mouseInsidePanel = false;
    public CombatDeckDisplay combatDeckDisplay;

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
    public Button openDeck, discardDeck;

    public List<CardCombat> cardsPlayedThisTurn = new List<CardCombat>();
    public List<CardCombat> playHistory = new List<CardCombat>();

    public CombatActorEnemy TargetedEnemy
    {
        get
        {   
            return _targetedEnemy;
        }
        set
        {
            if (_targetedEnemy != null)
            {
                _targetedEnemy.DisableTarget();
            }
            _targetedEnemy = value;

            if (_targetedEnemy == null && EnemiesInScene.Count > 0)
            {
                TargetedEnemy = EnemiesInScene[0];
            }
            _targetedEnemy?.EnableTarget();

            if (!(ActiveCard is null))
            {
                ActiveCard.animator.SetBool("HasTarget", !(value is null));
                ActiveCard.DamageNeedsRecalc();
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

            _activeCard = value;
            //EnemiesInScene.ForEach(x => x.SetTarget(false));

            //overwrite
            foreach (CardCombat card in Hand)
                if (card != value) card.MouseReact = (value is null);

            if (_activeCard != null)
                _activeCard.selected = true;
        }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            EventManager.OnDeckCountChangeEvent += UpdateDeckTexts;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Plumbing, Setup, Start/End turn

    public void NoteCardFinished(Card card)
    {
        cardsPlayedThisTurn.Add((CardCombat)card);
        playHistory.Add((CardCombat)card);
    }

    public void SetUpEncounter()
    {
        cardsPlayedThisTurn.Clear();
        playHistory.Clear();
        createdCards.Clear();
        Hero.maxHitPoints = CharacterStats.Health;
        Hero.hitPoints = WorldSystem.instance.characterManager.CurrentHealth;
        
        animator.SetBool("CompanionTurnStartedByPlayer", false);
        if (WorldSystem.instance.companionManager.currentCompanionData is CompanionData companionData)
        {
            companion.ReadCompanionData(companionData);
            animator.SetBool("HasCompanion", true);
            hasCompanion = true;
            companionButton.gameObject.SetActive(true);
            companionButton.interactable = false;
        }
        else    
        {
            companion.gameObject.SetActive(false);
            animator.SetBool("HasCompanion", false);
            hasCompanion = false;
            companionButton.gameObject.SetActive(false);
        }

        deckData = WorldSystem.instance.characterManager.deck.ToList();

        HashSet<EnergyType> energyTypes = new HashSet<EnergyType>();
        deckData.ForEach(d => energyTypes.UnionWith(d.energyToCostUI.Keys));
        foreach (EnergyType type in energyTypes)
            RegisterEnergyType(type);

        foreach (EnergyType type in displayEnergies.Keys.Except(energyTypes))
        {
            Destroy(displayEnergies[type]);
            displayEnergies.Remove(type);
        }

        foreach(CardVisual cv in deckData)
        {
            CardCombat card = CardCombat.Factory(cv);
            Hero.deck.Add(card);
        }

        Hero.ShuffleDeck();

        enemyDatas = encounterData.enemyData;
        formations.Where(x => x.FormationType == encounterData.formation).First().transforms.ForEach(x=> formationPositions.Add(x.position));

        for (int i = 0; i < enemyDatas.Count; i++)
        {
            GameObject EnemyObject = Instantiate(TemplateEnemy, transform);
            EnemyObject.transform.position = formationPositions[i];
            CombatActorEnemy combatActorEnemy = EnemyObject.GetComponent<CombatActorEnemy>();
            combatActorEnemy.ReadEnemyData(enemyDatas[i]);
            EnemiesInScene.Add(combatActorEnemy);
            if (i == 0)
            {
                TargetedEnemy = EnemiesInScene[0];
            }
        }
        animator.SetTrigger("StartSetup");
    }

    private void RegisterEnergyType(EnergyType type)
    {
        if (displayEnergies.ContainsKey(type)) return;
        displayEnergies[type] = Instantiate(templateEnergyDisplay, energyParent).GetComponent<CardCostDisplay>();
        displayEnergies[type].SetType(type);
        displayEnergies[type].lblEnergy.text = "0";

        if (!energyMax.ContainsKey(type)) energyMax[type] = 0;
        if (!energyTurn.ContainsKey(type)) energyTurn[type] = 0;
    }

    void CreateEnvironment()
    {
        if (currentEnvironment != null) Destroy(currentEnvironment);

        GameObject env = null;
        TileBiome biome = TileBiome.None;

        if (WorldSystem.instance.scenarioMapManager.currentTile != null)
            biome = WorldSystem.instance.scenarioMapManager.currentTile.tileBiome;

        if (biome != TileBiome.None)
        {
            GameObject[] envs = environmentPrefabs.Where(x => x.name.Contains(biome.ToString())).ToArray();
            if (envs.Count() > 0)
                env = envs[UnityEngine.Random.Range(0, envs.Count())];
        }

        if (env == null)
            env = environmentPrefabs[UnityEngine.Random.Range(0, environmentPrefabs.Count())];

        currentEnvironment = Instantiate(env, environmentAnchor);
    }

    public void StartCombat()
    {
        content.SetActive(true);
        CreateEnvironment();
        SetUpEncounter();
    }


    public void BindCharacterData()
    {
        energyTurn[EnergyType.Standard] = CharacterStats.Syphon;

        energyMax[EnergyType.Standard] = 4;
        energyMax[EnergyType.Rage] = 3;

        drawCount =  WorldSystem.instance.characterManager.defaultDrawCardAmount + CharacterStats.Wit;
        Hero.maxHitPoints = CharacterStats.Health;
        Hero.hitPoints = WorldSystem.instance.characterManager.CurrentHealth;
    }

    public void EndTurn()
    {
        EventManager.TurnEnded();
        cardsPlayedThisTurn.Clear();
        Hand.Where(c => c.HasProperty(CardSingleFieldPropertyType.Unstable)).ToList().ForEach(c => { 
            Hand.Remove(c); 
            Debug.Log(c);
            Hero.ExhaustCard(c); 
            });

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
        DeadEnemiesInScene.Add(enemy);
        EnemiesInScene.Remove(enemy);
        TargetedEnemy = null;
        ToggleTargetForward();
    }

    public void ButtonPlayCompanion()
    {
        if (combatStateType == CombatStateType.Idle)
        {
            companionButton.interactable = false;
            animator.SetBool("CompanionTurnStartedByPlayer", true);
            animator.SetTrigger("CompanionTurnStart");
        }
    }

    public void ToggleTargetForward()
    {
        if (TargetedEnemy != null && EnemiesInScene.Count > 1)
        {
            int idx = (EnemiesInScene.IndexOf(TargetedEnemy) + 1 + EnemiesInScene.Count) % EnemiesInScene.Count;
            TargetedEnemy = EnemiesInScene[idx]; 
        }
    }
    public void ToggleTargetBackwards()
    {
        if (TargetedEnemy != null && EnemiesInScene.Count > 1)
        {
            int idx = (EnemiesInScene.IndexOf(TargetedEnemy) - 1 + EnemiesInScene.Count) % EnemiesInScene.Count;
            TargetedEnemy = EnemiesInScene[idx]; 
        }
    }
    public void WinCombat()
    {
        animator.SetBool("HasWon",true);
        EventManager.CombatWon();
    }

    public void CleanUpEnemies()
    {
    while (EnemiesInScene.Any())
        {
            KillEnemy(EnemiesInScene[0]);
        }
    }

    public void CleanUpScene()
    {
        Hand.ForEach(x => Destroy(x.gameObject));
        Hand.Clear();
        Hero.deck.ForEach(x => Destroy(x.gameObject));
        Hero.deck.Clear();
        Hero.discard.ForEach(x => Destroy(x.gameObject));
        Hero.discard.Clear();
        Hero.ClearAllEffects();

        DeadEnemiesInScene.Clear();
    }

    public List<CombatActor> GetTargets(CombatActor source, CardTargetType targetType, CombatActor suppliedTarget)
    {
        List<CombatActor> targets = new List<CombatActor>();
        targetType = source.targetDistorter[targetType];

        if (targetType == CardTargetType.Self)
            targets.Add(source);
        else if (targetType == CardTargetType.AlliesExclSelf)
            targets.AddRange(source.allies);
        else if (targetType == CardTargetType.AlliesInclSelf)
        {
            targets.AddRange(source.allies);
            targets.Add(source);
        }
        else if (targetType == CardTargetType.EnemyAll)
            targets.AddRange(source.enemies);
        else if (targetType == CardTargetType.EnemySingle)
            targets.Add(suppliedTarget);
        else if (targetType == CardTargetType.EnemyRandom)
        {
            int id = UnityEngine.Random.Range(0, source.enemies.Count);
            targets.Add(source.enemies[id]);
        }
        else if (targetType == CardTargetType.All)
            targets.AddRange(ActorsInScene);

        return targets;
    }

    public int PreviewCalcDamageAllEnemies(int value)
    {
        int possibleDamage = PreviewCalcDamageEnemy(value, TargetedEnemy);
        foreach (CombatActor enemy in EnemiesInScene)
            if (possibleDamage != PreviewCalcDamageEnemy(value, enemy))
                return value;

        return possibleDamage;
    }

    public int PreviewCalcDamageEnemy(int value, CombatActor enemy)
    {
        return RulesSystem.instance.CalculateDamage(value, Hero, enemy);
    }

    public int CalculateDisplayDamage(int value)
    {
        CombatActorEnemy enemy = TargetedEnemy;
        int calcDamage = enemy is null ? PreviewCalcDamageAllEnemies(value) : PreviewCalcDamageEnemy(value, enemy);
        return calcDamage;
    }

    public void RecalcAllCardsDamage()
    {
        foreach(CardCombat card in Hand)
        {
            card.DamageNeedsRecalc();
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

        float localoffset = (Hand.Count % 2 == 0) ? clampedDegreeBetweenCards / 2 : 0;
        float degree = ((CardNr - (Hand.Count / 2)) * clampedDegreeBetweenCards + localoffset);
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
            //Debug.LogError("Current degree requested for card not in hand!");
            return -1f;
        }

        float localoffset = (Hand.Count % 2 == 0) ? clampedDegreeBetweenCards / 2 : 0;
        float degree = ((Hand.IndexOf(card) - (Hand.Count / 2)) * clampedDegreeBetweenCards + localoffset);

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
        foreach (CardCombat card in Hand)
        {
            if (card != ActiveCard)
            {
                card.transform.SetSiblingIndex(cursor);
                card.cardCollider.transform.SetSiblingIndex(Hand.Count + cursor++);
            }
        }
    }


    public void RefreshHandPositions(CardCombat excludeCard = null)
    {
        foreach (CardCombat card in Hand)
            if (card != excludeCard)
                card.animator.SetBool("NeedFan", true);
    }


    public void ReportDeath(CombatActor combatActor)
    {
        if (combatActor != Hero)
        {
            KillEnemy((CombatActorEnemy)combatActor);
            if (EnemiesInScene.Count == 0)
                WinCombat();
        }

        // if(combatActor == Hero)
        //     WorldSystem.instance.characterManager.KillCharacter();
    }
    #endregion

    #region Draw and Discard Cards, Deck Management

    public IEnumerator DrawCards(int CardsToDraw) {
        acceptProcess = false;
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

        yield return StartCoroutine(ResolveDrawnEffects());

        acceptProcess = true;
    }

    private void DrawSingleCard()
    {
        CardCombat card = (CardCombat)Hero.deck[0];
        Hero.deck.RemoveAt(0);
        Hand.Add(card);
        card.effectsOnDraw.ForEach(e => drawnToResolve.Enqueue(e));
        card.activitiesOnDraw.ForEach(e => drawnToResolve.Enqueue(e));
        if (card.HasProperty(CardSingleFieldPropertyType.Immediate)) drawnToResolve.Enqueue(card);

        card.animator.SetTrigger("StartDraw");
    }

    IEnumerator ResolveDrawnEffects()
    {
        while(drawnToResolve.Count != 0)
        {
            object obj = drawnToResolve.Dequeue();
            if(obj is StatusEffectCarrier cardEffect)
            {
                List<CombatActor> targets = GetTargets(Hero, cardEffect.Target, null);
                for (int i = 0; i < cardEffect.Times; i++)
                    foreach (CombatActor actor in targets)
                        yield return StartCoroutine(actor.RecieveEffectNonDamage(cardEffect));
            }
            else if(obj is CombatActivitySetting a)
            {
                yield return StartCoroutine(CombatActivitySystem.instance.StartByCardActivity(a));
            }
            else if(obj is CardCombat card)
            {
                Debug.Log("Enqeueing immediate card");
                card.animator.Play("Queued");
            }
        }
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
        ActiveCard = null;
        ResetSiblingIndexes();
    }

    public IEnumerator DiscardAllCards()
    {
        while (Hand.Any())
        {
            yield return StartCoroutine(DiscardCard(Hand[Hand.Count - 1]));
        }
    }

    public IEnumerator DiscardCard(CardCombat card)
    {
        Hero.DiscardCard(card);
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

    public void DetectCanvasClick()
    {
        Debug.Log("Canvas detected click");
        if (ActiveCard != null)
        {
            CancelCardSelection();
        }
    } 

    public void PlayerInputEndTurn()
    {
        // have to ahve a function for the ui button
        animator.SetBool("PlayerEndRequested",true);
    }

    public void CardClicked(CardCombat card)
    {
        if (ActiveCard == card)
            SelectedCardTriggered();
        else if (card.selectable)
            ActiveCard = card;
        else if (!card.cost.Payable())
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Cannot pay for this card!");
    }

    private bool MouseInsideArea()
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cardPanel.GetComponent<RectTransform>(), Input.mousePosition, WorldSystem.instance.cameraManager.currentCamera, out result);
        //Debug.Log(result);

        if (cardPanel.GetComponent<RectTransform>().rect.Contains(result))
        {
            //Debug.Log("True");
            return true;
        }

        //Debug.Log("False");
        return false;
    }

    public void ButtonOpenDeck()
    {
        combatDeckDisplay.OpenDeckDisplay(CardLocation.Deck);
        Debug.Log("Open Deck");
    }

    public void ButtonOpenExhaust()
    {
        combatDeckDisplay.OpenDeckDisplay(CardLocation.Exhaust);
        Debug.Log("Open Exhaust");
    }

    public void ButtonOpenDiscard()
    {
        combatDeckDisplay.OpenDeckDisplay(CardLocation.Discard);
        Debug.Log("Open Discard");
    }

    public void SelectedCardTriggered()
    {
        // if (MouseInsideArea())
        // {
        //     CancelCardSelection();
        //     return;
        // }
        
        Debug.Log("Clicky");
        ActiveCard.animator.SetTrigger("Confirmed");
        ActiveCard = null;
    }

    public void QueueEffect(ItemEffect effect)
    {
        queuedEffects.Enqueue(effect);
        animator.SetBool("EffectsQueued", true);
    }

    public IEnumerator EmptyEffectQueue()
    {
        Debug.Log("Effects queued count:" + queuedEffects.Count);
        while(queuedEffects.Count !=0)
            yield return StartCoroutine(queuedEffects.Dequeue().RunEffectEnumerator());

        animator.SetBool("EffectsQueued", false);
    }

    #endregion , user input
}
