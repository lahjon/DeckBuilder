using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardCollider : MonoBehaviour, IPointerClickHandler, IToolTipable
{
    public CardCombat card;

    public void OnMouseEnter() => card.OnMouseEnter();
    public void OnMouseExit() => card.OnMouseExit();
    public void OnMouseOver() => card.OnMouseOver();

    public void OnMouseUp() => card.OnMouseClick();

    public void SetOwner(CardCombat card)
    {
        this.card = card;
        name = string.Format("col_{0}", card.name);
    }

    public void MirrorCardTrans()
    {
        transform.localPosition = card.transform.localPosition;
        transform.localScale = card.transform.localScale;
        transform.localEulerAngles = card.transform.localEulerAngles;
    }

    public void SetTransform((Vector3 pos, Vector3 scale, Vector3 angles) posInfo)
    {
        transform.localPosition = posInfo.pos;
        transform.localScale = posInfo.scale;
        transform.localEulerAngles = posInfo.angles;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            card.OnMouseClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            card.OnMouseRightClick();
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        if (card == null)
            return (new List<string>(), Vector3.zero);
        else
            return card.GetTipInfo();
    }
}
