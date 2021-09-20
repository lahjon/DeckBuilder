using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatCardPresenter : MonoBehaviour
{
    public float displayHeight = 550;
    public float waitDelay = 0.1f;

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
        
        if(cardsToHand.Any())
        {
            cardsToHand.ForEach(c => {
                CombatSystem.instance.Hand.Add(c.card);
                (c.card.transform.localPosition, c.card.transform.localEulerAngles) = CombatSystem.instance.GetPositionInHand(c.card);
                c.card.animator.SetTrigger("GotoHand");
                c.card.selectable = true;
                cardsLocale.Remove(c);
            });

            CombatSystem.instance.RefreshHandPositions();
        }

        cardsLocale.ForEach(c => {
            c.card.transform.localEulerAngles = Vector3.one;
            c.card.animator.SetBool("ToCardPileDiscard", c.targetLocale == CardLocation.Discard);
            if (c.targetLocale == CardLocation.Discard)
                CombatSystem.instance.Hero.discard.Add(c.card);
            else
                CombatSystem.instance.Hero.AddToDeckSemiRandom(c.card);
            });

        int nrToDeck = cardsLocale.Count(c => c.targetLocale == CardLocation.Deck);
        if(nrToDeck == cardsLocale.Count || nrToDeck == 0)
        {
            float shiftX = cardsLocale.Count % 2 == 0 ? dist / 2f : 0f;
            for (int i = 0; i < cardsLocale.Count; i++)
                cardsLocale[i].card.transform.localPosition = new Vector3(dist * (i - cardsLocale.Count / 2) + shiftX, displayHeight);
        }
        else
        {
            int cursorDiscard = 1;
            int cursorDeck = -1;
            foreach ((CardCombat card, CardLocation target) c in cardsLocale)
                c.card.transform.localPosition = new Vector3(
                                                        (c.target == CardLocation.Deck ? cursorDeck-- : cursorDiscard++)*dist,
                                                        displayHeight);        
        }

        float addWait = CombatSystem.instance.ActiveActor == CombatSystem.instance.Hero ? 0 : 1f;
        StartCoroutine(DelaySendToCardPile(cardsLocale.Select(c => c.card).ToList(), addWait));
    }

    IEnumerator DelaySendToCardPile(List<CardCombat> cards,float addWait)
    {
        yield return new WaitForSeconds(waitDelay + addWait);

        foreach (CardCombat c in cards)
            if (c.animator.GetCurrentAnimatorStateInfo(0).IsName("Foreshadowed")) c.animator.SetTrigger("Discarded");
    }
}
