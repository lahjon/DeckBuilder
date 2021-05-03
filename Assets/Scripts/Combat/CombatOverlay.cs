using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CombatOverlay : MonoBehaviour
{
    Canvas canvas;
    public RectTransform playerTurn, enemyTurn, combatStart, combatWin;
    
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void AnimateCombatStart()
    {
        CanvasGroup canvasGroup = combatStart.GetComponent<CanvasGroup>();
        RectTransform rect = combatStart;
        Vector2 startPosMin = rect.anchorMin;
        Vector2 startPosMax = rect.anchorMax;
        Sequence mySequence = DOTween.Sequence();
        canvasGroup.alpha = 1;
        mySequence.Append(DOTween.To(() => rect.anchorMin, x => rect.anchorMin = x, new Vector2(startPosMin.x, .5f), 1f).SetEase(Ease.InCubic));
        mySequence.AppendInterval(0.5f);
        mySequence.Append(DOTween.To(() => rect.anchorMax, x => rect.anchorMax = x, new Vector2(startPosMax.x, -.75f), 1f).SetEase(Ease.InCubic).OnComplete(() => {rect.anchorMin = startPosMin; rect.anchorMax = startPosMax; canvasGroup.alpha = 0;}));
    }
    public void AnimateEnemyTurn()
    {
        Sequence mySequence = DOTween.Sequence();
        CanvasGroup canvasGroup = enemyTurn.GetComponent<CanvasGroup>();
        
        mySequence.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 1f).SetEase(Ease.InCubic));
        mySequence.AppendInterval(0.3f);
        mySequence.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 1f).SetEase(Ease.InCubic).OnComplete(() => canvasGroup.alpha = 0));
    }

    public void AnimateVictorious()
    {
        Sequence mySequence = DOTween.Sequence();
        CanvasGroup canvasGroup = combatWin.GetComponent<CanvasGroup>();
        
        mySequence.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, .5f).SetEase(Ease.InCubic));
        mySequence.AppendInterval(0.3f);
        mySequence.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, .5f).SetEase(Ease.InCubic).OnComplete(() => canvasGroup.alpha = 0));
    }
    public void AnimatePlayerTurn()
    {
        Sequence mySequence = DOTween.Sequence();
        CanvasGroup canvasGroup = playerTurn.GetComponent<CanvasGroup>();
        
        mySequence.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 1f).SetEase(Ease.InCubic));
        mySequence.AppendInterval(0.3f);
        mySequence.Append(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 1f).SetEase(Ease.InCubic).OnComplete(() => canvasGroup.alpha = 0));
    }
}
