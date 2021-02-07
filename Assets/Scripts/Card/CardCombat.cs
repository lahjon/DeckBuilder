using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class CardCombat : Card
{
    IEnumerator CardFollower;
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
                    this.transform.localPosition = combatController.GetPositionInHand(combatController.GetCardNumberInHand(this));
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
            StartCoroutine(LerpTransition(combatController.cardHoldPos.position, 0.4f));
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
        if(!inTransition && WorldSystem.instance.worldState == WorldState.Combat && combatController.ActiveCard is null)
        {
                SetScaleEnlarged();
                transform.SetAsLastSibling();
        }
    }

    public override void OnMouseExit()
    {
        if(!inTransition)
        {
            if(!selected)
                ResetScale();

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

    public void SetScaleEnlarged()
    {
        transform.localScale = Vector3.one + new Vector3(0.1f, 0.1f, 0.1f);
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


    IEnumerator LerpTransition(Vector3 endValue, float duration, float fscale = 0.2f)
    {
        inAnimation = true;
        float time = 0;
        Vector3 startValue = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startValue, endValue, time / duration);
            time += Time.fixedDeltaTime;

            yield return null;
        }

        transform.position = endValue;
        inAnimation = false;
    }

    IEnumerator CurveTransition(Vector3 endValue, bool scale, bool disable, bool useLocal = true, bool invertScale = false)
    {
        inTransition = true;
        Vector3 startPos;
        startPos = useLocal ? transform.localPosition : transform.position;

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

        ResetCard(true, disable);

    }

    private void ResetCard(bool transition = false, bool disable = false)
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

    public void AnimateCardByCurve(Vector3 pos, bool scale = false, bool disable = false, bool useLocal = true, bool invertScale = false, bool drawPhase = false)
    {
        StopCoroutine(CardFollower);

        if(!targetRequired || drawPhase)
            StartCoroutine(CurveTransition(pos, scale, disable, useLocal, invertScale));
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

    // public List<RaycastResult> RaycastMouse()
    // {
        
    //     PointerEventData pointerData = new PointerEventData (EventSystem.current)
    //     {
    //         pointerId = -1,
    //     };
        
    //     pointerData.position = Input.mousePosition;

    //     List<RaycastResult> results = new List<RaycastResult>();
    //     EventSystem.current.RaycastAll(pointerData, results);
        
    //     return results;
    // }

    // public void RaycastEnemy()
    // {
    //     RaycastHit[] hits;
    //     hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

    //     for (int i = 0; i < hits.Length; i++)
    //     {
    //         RaycastHit hit = hits[i];
    //         Renderer rend = hit.transform.GetComponent<Renderer>();

    //         Debug.Log(hits[i]);
    //     }

    // }


    // public GameObject CheckRaycast()
    // {
    //     RaycastEnemy();
    //     RaycastResult rayHit = RaycastMouse().FirstOrDefault(x => x.gameObject.ToString() == "CombatActorEnemy(Clone)");
    //     if (rayHit.isValid)
    //     {
    //         GameObject enemy = rayHit.gameObject;
    //         Debug.Log("An enemy");
    //         return enemy;
    //     }
    //     return null;
    // }

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
