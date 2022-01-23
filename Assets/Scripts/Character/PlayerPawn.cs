using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PlayerPawn : MonoBehaviour
{
    public HexCell currentTile;
    public float moveSpeed;
    ScenarioManager scenarioManager;
    float heightOffset = 4;
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = new Vector3(value.x, heightOffset, value.z);
    }
    void Start()
    {
        scenarioManager = WorldSystem.instance.scenarioManager;
    }
    public bool MoveToLocation(HexCell toTile)
    {
        if (HexGrid.instance.ReturnPath() is List<HexCell> path && path.Any())
        {
            currentTile.DisableHighlight();
            ScenarioManager.MouseInputEnabled = false;
            path.Reverse();
            path.RemoveAt(0);
            StartCoroutine(Move(path, toTile));
            return true;
        }
        return false;
    }

    IEnumerator Move(List<HexCell> path, HexCell toTile)
    {
        foreach (HexCell tile in path)
        {
            transform.DOMove(new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z), moveSpeed);
            tile.DisableHighlight();
            yield return new WaitForSeconds(moveSpeed);
        }
        ScenarioManager.MouseInputEnabled = true;
        HexGrid.instance.currentCell = toTile;
        currentTile = toTile;
        currentTile.StartEncounter();
    }

}
