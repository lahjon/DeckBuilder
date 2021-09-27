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
    public System.Action exitCallback;
    public TMP_Text upgradeLevel;
    public Toggle toggleUpgrade;
    public GameObject upgradeTools;

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
        titleText.text = "Pick a card to upgrade";
        exitCallback = callback;
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
        foreach(CardDisplay display in sourceToCard.Values)
        {
            switch (callbackType)
            {
                case 0:
                    display.gameObject.SetActive(true);
                    display.OnClick = () => DisplayCard(display);
                    break;
                case 1:
                    display.gameObject.SetActive(display.upgradable);
                    display.OnClick = () => {
                        if (cardToSource[display].UpgradeCard() == true)
                        {
                            display.Mimic(cardToSource[display]);
                            CloseDeckDisplay();
                            exitCallback?.Invoke();
                            exitCallback = null;
                        }
                        else
                            world.uiManager.UIWarningController.CreateWarning("Card is fully upgraded! Select another card!");
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

    public void DisplayCard(CardVisual aCard)
    {
        if(selectedCard == null)
        {
            aCard.OnMouseExit();

            previousPosition = aCard.transform.position;
            selectedCard = aCard;

            placeholderCard.Clone(aCard);
            inspectCard.SetActive(true);
            scroller.enabled = false;
            upgradeLevel.text = (placeholderCard.timesUpgraded + 1).ToString();
            selectedCard.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.1f);
        }
        else
            DeactivateDisplayCard();
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
        upgradeTools.SetActive(toggleUpgrade.isOn);

        if(!toggleUpgrade.isOn && selectedCard != null && inspectCard.activeSelf)
            ResetPlaceHolderCard();
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

    public void ButtonClose()
    {
        world.deckDisplayManager.CloseDeckDisplay();
    }

}
