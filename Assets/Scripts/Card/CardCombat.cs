using System.Collections;
using UnityEngine;


public class CardCombat : Card
{
    IEnumerator CardFollower;
    IEnumerator CurrentAnimation;
    [HideInInspector]
    public CombatController combatController;
    public RectTransform cardPanel;
    public AnimationCurve transitionCurveScaleDiscard;
    public AnimationCurve transitionCurveScaleDraw;
    public AnimationCurve transitionCurveTransform;
    
    public bool inTransition = false;
    public bool selectable = true;
    public ParticleSystem animationSystem;
    GameObject animationObject;

    public bool MouseReact = false;

    [SerializeField]
    private bool _selected = false;

    Vector3 selectedBaseSize = new Vector3(1.1f, 1.1f, 1.1f);

    public bool selected 
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if(selected == true)
            {
                SetTransOnMouseOver();
                if (targetRequired == true)
                {
                    selectable = false;
                    MouseReact = false;
                    transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                    //transform.localPosition += new Vector3(0.0f, 1f, 0.0f);
                    transform.SetAsLastSibling();
                }
                else
                {
                    selectable = false;
                    MouseReact = false;
                    StartCoroutine(CardFollower);
                }
            }
            else
            {
                (Vector3, Vector3) TransInfo = combatController.GetPositionInHand(combatController.Hand.IndexOf(this));
                transform.localPosition = TransInfo.Item1;
                transform.localEulerAngles = TransInfo.Item2;
                transform.localScale = Vector3.one;             
            }  
        }
    }

    void Awake()
    {
        CardFollower = FollowMouseIsSelected();
    }
    
    public void CreateAnimation()
    {
        if (cardData.animationPrefab != null)
        {
            GameObject child = cardData.animationPrefab.transform.GetChild(0).gameObject;
            animationObject = Instantiate(cardData.animationPrefab, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            animationSystem = child.GetComponent<ParticleSystem>();
            animationObject.transform.position = this.transform.position;
            animationSystem.Stop();
        }
    }
    public void AnimateCardUse()
    {
        StopCoroutine(CardFollower);

        if (cardData.animationPrefab != null) // has animation
        {
            CreateAnimation();

            if (!(CurrentAnimation is null)) StopCoroutine(CurrentAnimation);
            CurrentAnimation = LerpTransition(combatController.cardHoldPos.localPosition, new Vector3(0, 0, 0), 0.2f);
            StartCoroutine(CurrentAnimation );
            //StartCoroutine(WaitForAnimation());
            
            Debug.Log("Playing Animation");
        }
        else // no animation
        {
            Debug.Log("No Animation");
            animationSystem = null;
        }

        StartCoroutine(WaitForAnimation(animationSystem));
    }

    public void StartLerpPosition(Vector3 newPos, Vector3 newRot)
    {
        if(!(CurrentAnimation is null)) StopCoroutine(CurrentAnimation);
        CurrentAnimation = LerpTransition(newPos, newRot,  0.2f);
        StartCoroutine(CurrentAnimation);
    }

    IEnumerator WaitForAnimation(ParticleSystem particleSystem = null)
    {
        if (particleSystem != null)
        {
            if(!animationSystem.isPlaying) 
                animationSystem.Play();
            yield return new WaitForSeconds(particleSystem.main.duration);
            DestroyImmediate(animationObject);
        }
        else
        {
                yield return new WaitForSeconds(0.4f);
        }

        StartCoroutine(combatController.DiscardCard(this));
    }


    public override void OnMouseEnter()
    {
        if (MouseReact && combatController.acceptSelections && combatController.ActiveCard == null)
        {
            if (!(CurrentAnimation is null)) StopCoroutine(CurrentAnimation);
            SetTransOnMouseOver();
            transform.SetAsLastSibling();
        }
    }

    public override void OnMouseExit()
    {
        Debug.Log("intrans: " + inTransition + ", mouseReact: " + MouseReact);
        if(!inTransition && combatController.ActiveCard is null && MouseReact)
        {
            (Vector3, Vector3) TransInfo = combatController.GetPositionInHand(this);
            transform.localPosition = TransInfo.Item1;
            transform.localEulerAngles = TransInfo.Item2;
            transform.localScale = Vector3.one;
            combatController.ResetSiblingIndexes();
        }
    }

    public void SetTransOnMouseOver()
    {
        transform.localPosition = new Vector3(combatController.GetPositionInHand(this).Position.x, 200, 0);
        transform.localScale = selectedBaseSize;
        transform.localEulerAngles = Vector3.zero;
    }


    private IEnumerator FollowMouseIsSelected()
    {
        while (true)
        {
            float posY = Input.mousePosition.y;
            transform.localEulerAngles = Vector3.zero; 
            transform.position = WorldSystem.instance.cameraManager.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, posY, 10));
            yield return null;
        }
    }


    IEnumerator LerpTransition(Vector3 endPosition, Vector3 endAngles, float duration)
    {
        float time = 0;
        Vector3 startValue = transform.localPosition;
        Vector3 startAngles = transform.localEulerAngles;

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startValue, endPosition, time / duration);
            transform.localEulerAngles = AngleLerp(startAngles, endAngles, time / duration);
            time += Time.fixedDeltaTime;

            yield return null;
        }

        transform.localPosition = endPosition;
        transform.localEulerAngles = endAngles;
    }

    public Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        return new Vector3(xLerp, yLerp, zLerp);
    }

    IEnumerator CurveTransition(Vector3 endPos, Vector3 endAngles, Vector3 endScale, bool EndWithDisable)
    {
        inTransition = true;
        Vector3 startPos    = transform.localPosition;
        Vector3 startAngles = transform.eulerAngles;
        Vector3 startScale  = transform.localScale;
       
        float time = transitionCurveTransform.keys[transitionCurveTransform.length - 1].time;

        while (time > 0.0f)
        {
            Vector3 currentPosition = startPos * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endPos;

            if (Vector3.Distance(currentPosition, endPos) < 1)
            {
                Debug.Log("Broke early with extra time: " + time);
                break;
            }

            transform.localPosition     = startPos      * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endPos;
            transform.localScale        = startScale    * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endScale;
            transform.localEulerAngles  = AngleLerp(startAngles, endAngles, transitionCurveTransform.Evaluate(time));

            time -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition     = endPos;
        transform.localEulerAngles  = endAngles;
        transform.localScale        = endScale;

        ResetCard(true, EndWithDisable);
    }

    public void ResetCard(bool endedTransition = false, bool disable = false)
    {
        if(endedTransition)
        {
            inTransition = false;
        }


        if (disable) gameObject.SetActive(false);

        selected = false;
        MouseReact = true;
    }

    //Används i cardDraw och ångra kort
    public void AnimateCardByCurve(Vector3 pos, Vector3 angles, Vector3 scale, bool disable = false, bool forceStart = false)
    {
        selectable = !disable;
        if (!targetRequired || forceStart)
        {
            StopCoroutine(CardFollower);
            if (!(CurrentAnimation is null)) StopCoroutine(CurrentAnimation);
            CurrentAnimation = CurveTransition(pos, angles, scale, disable);
            StartCoroutine(CurrentAnimation);
        }
        else
        {
            Debug.Log("No curve, reset card");
            ResetCard();
        }
    }


    public void AnimateCardByPathDiscard()
    {
        StopCoroutine(CardFollower);
        GetComponent<BezierFollow>().StartAnimation();
    }

    public void CardAction()
    {
        Debug.Log("CardAction called");
        // Debug.Log(CheckRaycast());
        if (!Input.GetMouseButtonDown(1))
        {
            if (combatController.ActiveCard == this) { 
                combatController.CardUsed();
                Debug.Log("1");
                return;
            }

            if (combatController.ActiveCard != null)
                combatController.ActiveCard.ResetCard();

            if (combatController.CardisSelectable(this,true))
            {
                Debug.Log("2");
                return;
            }

            SelectCard();

        }
    }

    public void SelectCard()
    {
        if (!(CurrentAnimation is null)) StopCoroutine(CurrentAnimation);
        if (combatController.ActiveCard != null) combatController.ActiveCard.DeselectCard();
        transform.SetAsLastSibling();
        selected = true;
        combatController.ActiveCard = this;
    }
    public override void  OnMouseRightClick(bool allowDisplay = true)
    {
        Debug.Log("OnMouseRighclick called");
        if (combatController.ActiveCard == this)
        {
            DeselectCard();
            Debug.Log("Deselect");
        }
        if(!selected && allowDisplay && combatController.ActiveCard == null)
        {
            DisplayCard();
            Debug.Log("Display");
        }
    }

    public override void OnMouseClick()
    {
        if(combatController.ActiveCard == this)
            CardAction();
        else if(combatController.CardisSelectable(this,false))
            SelectCard();  
    }

    public void DeselectCard()
    {
        StopCoroutine(CardFollower);
        (Vector3, Vector3) origPosition = combatController.GetPositionInHand(combatController.Hand.IndexOf(this));
        AnimateCardByCurve(origPosition.Item1, origPosition.Item2, Vector3.one);

        combatController.CancelCardSelection();
    }

    public override void ResetScale()
    {
        throw new System.NotImplementedException();
    }
}
