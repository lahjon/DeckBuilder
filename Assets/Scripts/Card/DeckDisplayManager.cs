using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class DeckDisplayManager : Manager
{
    public GameObject cardPrefab;
    public RectTransform content;
    public Canvas canvas;
    public GameObject deckDisplay;
    public CardVisual selectedCard;
    public ScrollRect scroller;
    public Vector3 previousPosition;
    public CardDisplay placeholderCard;
    public GameObject inspectCard;
    public Transform deckDisplayPos;
    public CardDisplay animatedCard;
    public TMP_Text titleText;
    public TMP_Text upgradeLevel;
    public Toggle toggleUpgrade;
    public GameObject upgradesViewer, upgradeConfirm;
    public System.Action confirmCallback;

    public Dictionary<CardVisual, CardVisual> sourceToCard = new Dictionary<CardVisual, CardVisual>();
    public Dictionary<CardVisual, CardVisual> cardToSource = new Dictionary<CardVisual, CardVisual>();


    protected override void Awake()
    {
        base.Awake(); 
        world.deckDisplayManager = this;
        canvas.gameObject.SetActive(true);
        inspectCard.SetActive(false);
        deckDisplay.SetActive(false);
    }

    public void OpenDisplay()
    {
        titleText.text = "All Cards";
        WorldStateSystem.SetInDisplay();
        deckDisplay.SetActive(true);
        SetCallBacks(0);
    }

    public void OpenUpgrade(System.Action callback)
    {
        titleText.text = "Pick a card to upgrade. Costs 1 ember";
        confirmCallback = callback;
        WorldStateSystem.SetInDisplay();
        deckDisplay.SetActive(true);
        SetCallBacks(1);   
    }

    /// <summary>
    /// <para>callbackTypes with int switch: </para>
    /// <para>0 = DisplayCard </para>
    /// <para>1 = UpgradeCard </para>
    /// </summary>
    public void SetCallBacks(int callbackType)
    {
        foreach(CardDisplay display in cardToSource.Keys)
        {
            switch (callbackType)
            {
                case 0:
                    display.gameObject.SetActive(true);
                    display.OnClick = () => DisplayCard(display);
                    break;
                case 1:
                    display.gameObject.SetActive(display.upgradable);
                    display.OnClick = () => 
                    {
                        DisplayCard(display, true);
                    };
                    break;
                default:
                    break;
            }
        }
    }

    public void Add(CardVisual source)
    {
        CardDisplay card = Instantiate(cardPrefab, content.transform).GetComponent<CardDisplay>();
        card.gameObject.SetActive(true);

        sourceToCard[source] = card;
        cardToSource[card] = source;
        card.Mimic(source);
    }

    public void Remove(CardVisual source)
    {
        CardVisual card = sourceToCard[source];
        cardToSource.Remove(card);
        Destroy(sourceToCard[source].gameObject);
        sourceToCard.Remove(source);
    }

    public void DisplayCard(CardVisual aCard, bool upgrade = false)
    {
        aCard.OnMouseExit();
        if (upgrade)
        {
            if (world.characterManager.characterCurrency.ember <= 0)
            {
                world.uiManager.UIWarningController.CreateWarning("Not enought Ember!");
                return;
            }
            if (!aCard.upgradable)
            {
                world.uiManager.UIWarningController.CreateWarning("Card is fully upgraded!");
                return;
            }
        }
        
        previousPosition = aCard.transform.position;
        selectedCard = aCard;

        placeholderCard.Clone(aCard);
        placeholderCard.OnClick = () => DeactivateDisplayCard();
        inspectCard.SetActive(true);
        scroller.enabled = false;
        selectedCard.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
        upgradeConfirm.SetActive(upgrade);
        if (upgrade)
        {
            toggleUpgrade.gameObject.SetActive(false);
            placeholderCard.UpgradeCard();
        }
        else
        {
            upgradeLevel.text = (placeholderCard.timesUpgraded + 1).ToString();
            toggleUpgrade.gameObject.SetActive(aCard.cardData.maxUpgrades != 0);
        }
    }

    public void ButtonConfirm()
    {
        selectedCard.UpgradeCard();
        if (!selectedCard.upgradable) selectedCard.gameObject.SetActive(false);
        world.characterManager.characterCurrency.ember--;
        DeactivateDisplayCard();
        if (world.characterManager.characterCurrency.ember <= 0) ButtonClose();
    }

    public void ResetPlaceHolderCard()
    {
        placeholderCard.Clone(selectedCard);
        upgradeLevel.text = (selectedCard.timesUpgraded + 1).ToString();
    }

    public void ButtonNextUpgrade()
    {
        if (placeholderCard != null && placeholderCard.UpgradeCard())
        {
            upgradeLevel.text = (placeholderCard.timesUpgraded + 1).ToString();
        }
    }
    public void ButtonPreviousUpgrade()
    {
        if (placeholderCard.timesUpgraded == 0) return;
        placeholderCard.cardModifiers.RemoveAt(placeholderCard.cardModifiers.Count - 1);
        List<CardFunctionalityData> mods = new List<CardFunctionalityData>(placeholderCard.cardModifiers);
        placeholderCard.Clone(placeholderCard,mods);
        upgradeLevel.text = (placeholderCard.timesUpgraded + 1).ToString();
    }
    public void ButtonToggleViewUpgrade()
    {
        upgradesViewer.SetActive(toggleUpgrade.isOn);

        if (!toggleUpgrade.isOn && selectedCard != null && inspectCard.activeSelf)
            ResetPlaceHolderCard();
        else
            ButtonNextUpgrade();
    }

    public void DeactivateDisplayCard()
    {
        inspectCard.SetActive(false);
        toggleUpgrade.isOn = false;
        scroller.enabled = true;
        selectedCard.transform.position = previousPosition;
        selectedCard = null;
    }

    public void CloseDeckDisplay()
    {
        WorldStateSystem.TriggerClear();
        inspectCard.SetActive(false);
        deckDisplay.SetActive(false);
        selectedCard = null;
    }

    public void ConfirmCallback()
    {
        if (confirmCallback != null)
        {
            confirmCallback.Invoke();
            confirmCallback = null;
        }
    }

    public void ButtonClose()
    {
        ConfirmCallback();
        world.deckDisplayManager.CloseDeckDisplay();
    }

}
