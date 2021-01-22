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
    public AnimationCurve transitionCurve;

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
        transform.localScale = combatController.GetCardScale();
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

    IEnumerator LerpPosition(Vector3 endValue, float duration)
    {
        float time = 0;
        Vector3 startValue = transform.localPosition;
        Vector3 velocity = Vector3.zero;

        while (time < duration)
        {
            //transform.localPosition = Vector3.Lerp(startValue, endValue, time / duration);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, endValue, ref velocity, duration);
            time += Time.fixedDeltaTime;
            yield return null;
        }
        transform.localPosition = endValue;
    }
    IEnumerator CurveTransition(Vector3 endValue)
    {
        float time = transitionCurve.keys[transitionCurve.length -1].time;
        while(time > 0.0f)
        {
            float dist = Vector3.Distance(transform.localPosition, endValue);
            Vector3 dir = transform.localPosition - endValue;

            transform.localPosition += Vector3.zero;

            time -= Time.deltaTime;
            yield return null;
        }
    }

    public void ResetPosition(Vector3 position)
    {
        //StartCoroutine(LerpPosition(position, 0.2f));
        StartCoroutine(CurveTransition(position));
    }
    public override void OnMouseClick()
    {
        return;
    }

    public void OnMouseDown()
    {
        if (combatController.ActiveCard == this && cardData.Effects.Count(x => x.Target == CardTargetType.ALL) == cardData.Effects.Count) { 
            combatController.CardUsed(this);
            Debug.Log("Let Go");
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
