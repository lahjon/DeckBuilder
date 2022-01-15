using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class GridSelector : MonoBehaviour
{
    public Material normal, failed;
    [SerializeField] MeshRenderer meshRenderer;
    Tween tween;
    public void Reveal(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void FailAction()
    {
        meshRenderer.material = failed;
        Helpers.DelayForSeconds(.1f, () =>
        {
            meshRenderer.material = normal;
        });
    }
}