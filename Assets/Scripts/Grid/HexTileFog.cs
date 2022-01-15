using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexTileFog : MonoBehaviour
{
    public HexTileOverworld hexTile;
    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;
    static ScenarioManager scenarioManager;

    void Start() 
    {
        if (scenarioManager == null)
            scenarioManager = WorldSystem.instance.scenarioManager;
    }

    public void Reveal(bool instant = false)
    {
        meshCollider.enabled = false;
        if (instant)
            gameObject.SetActive(false);
        else
            meshRenderer.material.DOColor(new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0), 1f).OnComplete(() => gameObject.SetActive(false));
    }
    void OnMouseEnter()
    {
        Select();
    }

    void OnMouseExit()
    {
        Deselect();
    }

    void Select()
    {
        scenarioManager.gridSelector.Reveal(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
    }

    void Deselect()
    {
        scenarioManager.gridSelector.Hide();
    }
    void OnMouseUp()
    {
        if (ScenarioManager.MouseInputEnabled && hexTile.neighbours.Any(x => x.Revealed) && scenarioManager.RequestActionPoints(1))
        {
            for (int i = 0; i < hexTile.neighbours.Count; i++)
            {
                if (hexTile.neighbours[i] is HexTileOverworld tile && !tile.Revealed) tile.Revealed = true;
            }
            hexTile.Revealed = true; 
            Deselect();
        }
        else
            scenarioManager.gridSelector.FailAction();
    }
}