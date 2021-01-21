using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardCombat : Card
{
    IEnumerator CardFollower;
    [HideInInspector]
    public CombatController combatController;
    public RectTransform cardPanel;

    void Awake()
    {
        CardFollower = FollowMouseIsSelected();
    }

    

    public override void OnMouseEnter()
    {
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            transform.SetAsLastSibling();
        }
    }

    public override void OnMouseExit()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            combatController.ResetSiblingIndexes();
        }
    }

    public override void ResetScale()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }

    private IEnumerator FollowMouseIsSelected()
    {
        while (true)
        {
            Vector2 outCoordinates;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cardPanel, Input.mousePosition, null, out outCoordinates);
            GetComponent<RectTransform>().localPosition = outCoordinates;
            yield return new WaitForSeconds(0);
        }
    }

    IEnumerator LerpPosition(GameObject card, Vector3 endValue, float duration)
    {
        float time = 0;
        Vector3 startValue = card.transform.localPosition;

        while (time < duration)
        {
            card.transform.localPosition = Vector3.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        card.transform.localPosition = endValue;
    }

    public void ResetPosition(GameObject card, Vector3 position)
    {
        StartCoroutine(LerpPosition(card, position, 0.3f));
    }

    public override void OnMouseClick()
    {
        if (combatController.ActiveCard == this && cardData.Effects.Count(x => x.Target == CardTargetType.ALL) == cardData.Effects.Count) { 
            combatController.CardUsed(this);
            return;
        }

        if (!combatController.CardisSelectable(this))
        {
            combatController.CancelCardSelection(this.gameObject);
            StopCoroutine(CardFollower);
            return;
        }

        combatController.ActiveCard = this;
        StartCoroutine(CardFollower);
    }
    public override void OnMouseRightClick()
    {
        combatController.CancelCardSelection(this.gameObject);
        StopCoroutine(CardFollower);
    }

}
