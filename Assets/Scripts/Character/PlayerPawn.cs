using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PlayerPawn : MonoBehaviour
{
    public HexTileOverworld currentTile;
    public float moveSpeed;
    ScenarioManager scenarioManager;
    void Start()
    {
        scenarioManager = WorldSystem.instance.scenarioManager;
    }
    public bool MoveToLocation(HexTileOverworld nextTile)
    {
        ScenarioManager.MouseInputEnabled = false;
        List<HexTile> path = AStarSearch.StartAStarSearch(currentTile, nextTile);
        if (path == null) 
        {
            ScenarioManager.MouseInputEnabled = true;
            return false;
        }

        path.Remove(currentTile);
        StartCoroutine(Move(path, nextTile));
        currentTile = nextTile;
        return true;
    }

    IEnumerator Move(List<HexTile> path, HexTileOverworld tileReached)
    {
        foreach (HexTile tile in path)
        {
            transform.DOMove(new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z), moveSpeed);
            yield return new WaitForSeconds(moveSpeed);
        }
        ScenarioManager.MouseInputEnabled = true;
        tileReached.StartEncounter();
    }
    
}
