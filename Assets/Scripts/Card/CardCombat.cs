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
    public bool selected = false;


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
        Vector3 startPos = transform.localPosition;
        float time = transitionCurve.keys[transitionCurve.length - 1].time;
        while (time > 0.0f)
        {
            transform.localPosition = startPos * (1 - transitionCurve.Evaluate(time)) + transitionCurve.Evaluate(time) * endValue;

            time -= Time.deltaTime;
            yield return null;
        }
        selected = false;
    }

    public void ResetPosition(Vector3 position)
    {
        StartCoroutine(CurveTransition(position));
    }
    public override void OnMouseClick()
    {
        return;
    }

    public void OnMouseDown()
    {   
        if(!Input.GetMouseButtonDown(1))
        {
            if (combatController.ActiveCard == this) { 
                combatController.CardUsed();
                return;
            }

            if (!combatController.CardisSelectable(this))
                return;

            if(selected == false)
            {
                selected = true;
                combatController.ActiveCard = this;
                StartCoroutine(CardFollower);
            }
        }
    }
    public override void OnMouseRightClick()
    {
        if (combatController.ActiveCard == this)
        {
            combatController.CancelCardSelection(this.gameObject);
            StopCoroutine(CardFollower);
        }
        if(selected == false)
            DisplayCard();
    }

}
