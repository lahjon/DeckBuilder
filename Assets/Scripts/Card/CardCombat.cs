using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class CardCombat : Card
{
    IEnumerator CardFollower;
    IEnumerator IE_LerpPos;
    [HideInInspector]
    public CombatController combatController;
    public RectTransform cardPanel;
    public AnimationCurve transitionCurveScaleDiscard;
    public AnimationCurve transitionCurveScaleDraw;
    public AnimationCurve transitionCurveTransform;
    
    public bool inTransition = false;
    public ParticleSystem animationSystem;
    GameObject animationObject;
    private bool inAnimation = false;

    [SerializeField]
    private bool _selected = false;

    public bool selected 
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if(targetRequired)
            {
                if (_selected == true)
                {
                    this.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
                    this.transform.localPosition += new Vector3(0.0f, 1f, 0.0f);
                    transform.SetAsLastSibling();
                }
                else
                {
                    (Vector3, Vector3) TransInfo = combatController.GetPositionInHand(combatController.Hand.IndexOf(this.gameObject));
                    this.transform.localPosition = TransInfo.Item1;
                    this.transform.localEulerAngles = TransInfo.Item2;
                    ResetScale();
                }
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
    public void UseCard()
    {
        combatController.DiscardCard(this.gameObject);
        
        if (cardData.animationPrefab != null) // has animation
        {
            CreateAnimation();
            
            StopCoroutine(CardFollower);

            Vector3 center = new Vector3(Screen.width*0.5f, Screen.height*0.5f, 0.0f);
            StartCoroutine(LerpTransition(combatController.cardHoldPos.localPosition, new Vector3(0,0,0), 0.4f));
            //StartCoroutine(WaitForAnimation());
            
            Debug.Log("Playing Animation");
            StartCoroutine(WaitForAnimation(animationSystem));
        }
        else // no animation
        {
            Debug.Log("No Animation");
            combatController.DiscardCardToPile(this.gameObject);
        }
    }

    public void StartLerpPosition(Vector3 newPos, Vector3 newRot)
    {
        if(!(IE_LerpPos is null)) StopCoroutine(IE_LerpPos);
        IE_LerpPos = LerpTransition(newPos, newRot, 0.5f);
        StartCoroutine(IE_LerpPos);
    }

    IEnumerator WaitForAnimation(ParticleSystem particleSystem = null)
    {
        if (particleSystem != null)
        {
            if(!animationSystem.isPlaying) 
                animationSystem.Play();
            yield return new WaitForSeconds(particleSystem.main.duration);
            DestroyImmediate(animationObject);
            combatController.DiscardCardToPile(this.gameObject);
        }
        else
        {
            while (inAnimation)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }


    public override void OnMouseEnter()
    {
        if(!inTransition && world.worldState == WorldState.Combat && combatController.ActiveCard is null)
        {
                SetTransOnMouseOver();
                transform.SetAsLastSibling();
        }
    }

    public override void OnMouseExit()
    {
        if(!inTransition)
        {
            if (!selected)
            {
                ResetScale();
                (Vector3, Vector3) TransInfo = combatController.GetPositionInHand(combatController.Hand.IndexOf(this.gameObject));
                transform.localPosition = TransInfo.Item1;
                transform.localEulerAngles = TransInfo.Item2;
            }

            if(WorldSystem.instance.worldState == WorldState.Combat && !_selected)
            {
                combatController.ResetSiblingIndexes();
            }
        }
    }

    public override void ResetScale()
    {
        transform.localScale = Vector3.one;
    }

    public void SetTransOnMouseOver()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 150, 0);
        transform.localScale = Vector3.one + new Vector3(0.1f, 0.1f, 0.1f);
        transform.localEulerAngles = Vector3.zero;
    }


    private IEnumerator FollowMouseIsSelected()
    {
        
        if (!targetRequired)
        {
            while (true)
            {
                float posY = Input.mousePosition.y;
                transform.position = WorldSystem.instance.cameraManager.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, posY, 10));
                yield return null;
            }
        }
        else
        {
            Vector3 pos = transform.localPosition += new Vector3(0.0f,0.2f,0.0f);
            Vector3 scale = transform.localScale += new Vector3(0.3f,0.3f,0.3f);
            while (true)
            {
                transform.localScale = scale;
                transform.localPosition = pos;
                yield return null;
            }
        }
    }


    IEnumerator LerpTransition(Vector3 endPosition, Vector3 endAngles, float duration)
    {
        inAnimation = true;
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
        inAnimation = false;
    }

    Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        return new Vector3(xLerp, yLerp, zLerp);
    }

    IEnumerator CurveTransition(Vector3 endValue, Vector3 endAngles, bool scale, bool disable, bool useLocal = true, bool invertScale = false)
    {
        inTransition = true;
        Vector3 startPos = useLocal ? transform.localPosition : transform.position;
        Vector3 startAngles = useLocal ? transform.eulerAngles : transform.localEulerAngles;

        float time = transitionCurveTransform.keys[transitionCurveTransform.length - 1].time;

        while (time > 0.0f)
        {
            if(useLocal)
            {
                transform.localPosition = startPos * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endValue;
                transform.localEulerAngles = startAngles * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endAngles;
            }
            else
            {  
                transform.position = startPos * (1 - transitionCurveTransform.Evaluate(time)) + transitionCurveTransform.Evaluate(time) * endValue;
                transform.localEulerAngles = AngleLerp(startAngles,endAngles, transitionCurveTransform.Evaluate(time));
            }

            if (scale)
            {
                float tempScale = invertScale ? transitionCurveScaleDraw.Evaluate(time) : transitionCurveScaleDiscard.Evaluate(time);
                transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            }

            time -= Time.deltaTime;
            yield return null;
        }

        ResetCard(true, disable);

    }

    public void ResetCard(bool transition = false, bool disable = false)
    {
        selected = false;
        Debug.Log(this.cardData);
        if(transition)
        {
            inTransition = false;
            combatController.CardDemarkTransition(gameObject);
        }

        transform.localScale = new Vector3(1,1,1);
        if(disable)
        {
           this.gameObject.SetActive(false);
        }
    }

    public void AnimateCardByCurve(Vector3 pos, Vector3 angles, bool scale = false, bool disable = false, bool useLocal = true, bool invertScale = false, bool drawPhase = false)
    {
        StopCoroutine(CardFollower);

        if(!targetRequired || drawPhase)
            StartCoroutine(CurveTransition(pos, angles,scale, disable, useLocal, invertScale));
        else
        {
            ResetCard();
        }
    }


    public void AnimateCardByPathDiscard()
    {
        StopCoroutine(CardFollower);
        this.GetComponent<BezierFollow>().StartAnimation();
    }

    public void CardAction()
    {   
        // Debug.Log(CheckRaycast());
        if(!Input.GetMouseButtonDown(1))
        {
            if (combatController.ActiveCard == this) { 
                combatController.CardUsed();
                Debug.Log("1");
                return;
            }

            if (!combatController.CardisSelectable(this))
            {
                Debug.Log("2");
                if(combatController.ActiveCard != null)
                    combatController.ActiveCard.ResetCard();
                //DeselectCard();
                SelectCard();
                return;
            }

            if(!selected && !inTransition)
            {
                SelectCard();
            }

        }
    }

    public void SelectCard()
    {
        selected = true;
        combatController.ActiveCard = this;
        Debug.Log("Selected New Card Over Another");
        if (!targetRequired)
        {
            StartCoroutine(CardFollower);
        }
    }
    public override void OnMouseRightClick(bool allowDisplay = true)
    {
        if (combatController.ActiveCard == this)
        {
            DeselectCard();
            Debug.Log("Deselect");
        }
        if(selected == false && allowDisplay == true && combatController.ActiveCard == null && combatController.previousActiveCard != this)
        {
            DisplayCard();
            Debug.Log("Display");
        }
        combatController.previousActiveCard = null;
    }

    public override void OnMouseClick()
    {
        if(combatController.ActiveCard == null && combatController.previousActiveCard != this && !this.selected)
        {
            combatController.previousActiveCard = null;
            SelectCard();
            return;
        }
        CardAction();
    }

    public void DeselectCard()
    {
        combatController.CancelCardSelection(this.gameObject);
        StopCoroutine(CardFollower);
    }

}
