using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatCardPresenter : MonoBehaviour
{
    public CombatController combatController;
    public float cardHeight = 550;
    public float waitDelay = 0.1f;

    Transform cardDisplayArea;
    float distanceBetweenCards; 
    float cardWidht;
    float dist { get { return distanceBetweenCards + cardWidht; } }


    public void DisplayCards(List<CardCombat> cards, List<CardLocation> targetLocations)
    {
        cardWidht = cards[0].GetComponent<RectTransform>().rect.width;
        distanceBetweenCards = cardWidht / 20f;

        cards.ForEach(c => {
            c.transform.localScale = Vector3.one;
            c.animator.Play("Foreshadowed");
        });

        List<(CardCombat card, CardLocation targetLocale)> cardsLocale = new List<(CardCombat card, CardLocation targetLocale)>();
        for (int i = 0; i < cards.Count; i++)
            cardsLocale.Add((cards[i], targetLocations[i]));

        List<(CardCombat card, CardLocation targetLocale)> cardsToHand = cardsLocale.Where(x => x.targetLocale == CardLocation.Hand).ToList();
        
        if(cardsToHand.Count > 0)
        {
            cardsToHand.ForEach(c => {
                combatController.Hand.Add(c.card);
                (c.card.transform.localPosition, c.card.transform.localEulerAngles) = combatController.GetPositionInHand(c.card);
                c.card.animator.SetTrigger("GotoHand");
                cardsLocale.Remove(c);
            });

            combatController.RefreshHandPositions();
        }

        cardsLocale.ForEach(c => {
            c.card.transform.localEulerAngles = Vector3.one;
            c.card.animator.SetBool("ToCardPileDiscard", c.targetLocale == CardLocation.Discard);
            if (c.targetLocale == CardLocation.Discard)
                combatController.Hero.discard.Add(c.card);
            else
                combatController.Hero.AddToDeckSemiRandom(c.card);
            });

        int nrToDeck = cardsLocale.Count(c => c.targetLocale == CardLocation.Deck);
        if(nrToDeck == cardsLocale.Count || nrToDeck == 0)
        {
            float shiftX = cardsLocale.Count % 2 == 0 ? dist / 2f : 0f;
            for (int i = 0; i < cardsLocale.Count; i++)
                cardsLocale[i].card.transform.localPosition = new Vector3(dist * (i - cardsLocale.Count / 2) + shiftX, cardHeight);
        }
        else
        {
            int cursorDiscard = 1;
            int cursorDeck = -1;
            foreach ((CardCombat card, CardLocation target) c in cardsLocale)
                c.card.transform.localPosition = new Vector3(
                                                        (c.target == CardLocation.Deck ? cursorDeck-- : cursorDiscard++)*dist,
                                                        cardHeight);        
        }

        StartCoroutine(DelaySendToCardPile(cardsLocale.Select(c => c.card).ToList()));
    }

    IEnumerator DelaySendToCardPile(List<CardCombat> cards)
    {
        yield return new WaitForSeconds(waitDelay);

        foreach (CardCombat c in cards)
            if (c.animator.GetCurrentAnimatorStateInfo(0).IsName("Foreshadowed")) c.animator.SetTrigger("Discarded");
    }
}
