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
    public bool selected = false;
    public bool inTransition = false;
    public ParticleSystem animationSystem;
    GameObject animationObject;
    private bool inAnimation = false;


    void Awake()
    {
        CardFollower = FollowMouseIsSelected();
    }
    
    void Start()
    {
    }

    public void CreateAnimation()
    {
        if (cardData.animationPrefab != null)
        {
            GameObject child = cardData.animationPrefab.transform.GetChild(0).gameObject;
            Debug.Log("Child:" + child);
            animationObject = Instantiate(cardData.animationPrefab, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            animationSystem = child.GetComponent<ParticleSystem>();
            animationObject.transform.position = combatController.targetedEnemy.transform.position;
            animationSystem.Stop();
            Debug.Log(animationSystem);
        }
    }
    public void UseCard()
    {
        if (cardData.animationPrefab != null)
        {
            CreateAnimation();
            
            StopCoroutine(CardFollower);

            Vector3 center = new Vector3(Screen.width*0.5f, Screen.height*0.5f, 0.0f);
            StartCoroutine(LerpPosition(combatController.cardHoldPos.position, 0.2f));
            StartCoroutine(WaitForAnimation());
            
            Debug.Log("Playing Animation");
            StartCoroutine(WaitForAnimation(animationSystem));
        }
        else
        {
            Debug.Log("No Animation");
            combatController.SendCardToDiscard(this.gameObject, true);
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
            combatController.SendCardToDiscard(this.gameObject, true);
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
        if(transform.localScale == Vector3.one)
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

        if(WorldSystem.instance.worldState == WorldState.Combat)
        {
            transform.SetAsLastSibling();
        }
    }

    public override void OnMouseExit()
    {
        if(transform.localScale != Vector3.one)
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
            float posY = Input.mousePosition.y;
            transform.position = WorldSystem.instance.cameraManager.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, posY, 10));
            yield return null;
        }
    }


    IEnumerator LerpPosition(Vector3 endValue, float duration, float fscale = 0.2f)
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
        selected = false;
        inTransition = false;
        combatController.CardDemarkTransition(gameObject);

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
                return;
            }

            if (!combatController.CardisSelectable(this))
            {
                return;
            }

            if(selected == false)
            {
                selected = true;
                combatController.ActiveCard = this;
                StartCoroutine(CardFollower);
                Debug.Log("Selected");
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
