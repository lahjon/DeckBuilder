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
    public AnimationCurve transitionCurveScaleDiscard;
    public AnimationCurve transitionCurveScaleDraw;
    public AnimationCurve transitionCurveTransform;
    public bool selected = false;
    public bool inTransition = false;


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
            transform.localPosition = Vector3.Lerp(startValue, endValue, time / duration);
            //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, endValue, ref velocity, duration);
            time += Time.fixedDeltaTime;
            yield return null;
        }
        transform.localPosition = endValue;
    }

    IEnumerator CurveTransition(Vector3 endValue, bool scale, bool disable, bool useLocal = true, bool invertScale = false)
    {
        inTransition = true;
        Vector3 startPos;
        if(useLocal)
        {
            startPos = transform.localPosition;
        }
        else
        {
            startPos = transform.position;
        }

        float time = transitionCurveTransform.keys[transitionCurveTransform.length - 1].time;

        while (time > 0.0f)
        {
            if(useLocal)
            {
                transform.localPosition = startPos * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endValue;
            }
            else
            {  
                transform.position = startPos * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endValue;
            }

            if (scale)
            {
                float tempScale;

                if (invertScale == true)
                {
                    tempScale = transitionCurveScaleDraw.Evaluate(time);
                }
                else
                {
                    tempScale = transitionCurveScaleDiscard.Evaluate(time);
                }

                transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            }

            time -= Time.deltaTime;
            yield return null;
        }
        selected = false;
        inTransition = false;
        combatController.CheckInTransition(false);

        transform.localScale = new Vector3(1,1,1);
        if(disable)
        {
           this.gameObject.SetActive(false);
        }
    }

    public void AnimateCardByCurve(Vector3 pos, bool scale = false, bool disable = false, bool useLocal = true, bool invertScale = false)
    {
        StopAllCoroutines();
        StartCoroutine(CurveTransition(pos, scale, disable, useLocal, invertScale));
    }

    public void AnimateCardByPathDiscard()
    {
        StopAllCoroutines();
        this.GetComponent<BezierFollow>().StartAnimation();
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
    public override void OnMouseRightClick(bool allowDisplay = true)
    {
        if (combatController.ActiveCard == this)
        {
            combatController.CancelCardSelection(this.gameObject);
            StopCoroutine(CardFollower);
        }
        if(selected == false && allowDisplay == true)
            DisplayCard();
    }

}
