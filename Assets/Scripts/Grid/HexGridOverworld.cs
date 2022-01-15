using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexGridOverworld : MonoBehaviour
{
    public float hexScale = 0.365f;
    public float tileSize, tileGap;
    public int gridWidth;
    public Transform tileParent;
    public GameObject hexTilePrefab, hexTileBlockedPrefab, encounterPrefab;
    public Dictionary<Vector3Int, HexTileOverworld> tiles = new Dictionary<Vector3Int, HexTileOverworld>();
    public HexTileOverworld GetTile(Vector3Int cellCoordinate) => tiles.ContainsKey(cellCoordinate) ? tiles[cellCoordinate] : null;
    public void CreateGrid()
    {
        for (int i = 0; i <= gridWidth; i++)
            CreateRow(i);
        List<Encounter> encountersToOptimize = new List<Encounter>();
        foreach (HexTileOverworld tile in tiles.Values)
        {
            tile.AssignNeighboors();
            if (tile.coord != Vector3Int.zero && !tile.Blocked && Random.Range(0, 10) < 2)
            {
                encountersToOptimize.Add(tile.AddEncounter());
            }
        }
        HexOptimizer optimizer = new HexOptimizer();
        optimizer.SetEncounters(encountersToOptimize);
        optimizer.Run();
    }
    void CreateRow(int row)
    {
        if(row == 0) AddTile(Vector3Int.zero).Revealed = true;

        Vector3Int currentCoord = (Vector3Int.zero + GridDirection.SouthWest) * row;
        foreach (GridDirection dir in GridDirection.Directions)
            for (int i = 0; i < row; i++)
            {
                if (row <= 2)
                    AddTile(currentCoord += dir).Revealed = true;
                
                else
                    AddTile(currentCoord += dir, true);
            }
    }

    List<HexTileOverworld> GetTilesAtRow(int row)
    {
        if (row == 0) return new List<HexTileOverworld>() {GetTile(Vector3Int.zero)};
        List<HexTileOverworld> retList = new List<HexTileOverworld>();
        
        Vector3Int currentCoord = (Vector3Int.zero + GridDirection.SouthWest) * row;
        foreach (GridDirection dir in GridDirection.Directions)
            for(int i = 0; i < row; i++)
                retList.Add(GetTile(currentCoord += dir));

        return retList;
    }

    public Vector3 CellPosToWorldPos(Vector3Int coord)
    {
        float width = Mathf.Sqrt(3) * (tileSize + tileGap);
        float height = 1.5f * (tileSize + tileGap);
        float x = (width * coord.x * 0.5f) - (width * coord.y * 0.5f);
        float z = height * coord.z * -1;

        return new Vector3(x, transform.position.y, z);
    }

    public HexTileOverworld AddTile(Vector3Int coord, bool randomBlocked = false)
    {
        if (coord.x + coord.y + coord.z != 0)
        {
            Debug.LogWarning("Invalid coord");
            return null;
        }
        bool blocked = randomBlocked && Random.Range(0, 10) < 2;
        GameObject prefab = blocked ? hexTileBlockedPrefab : hexTilePrefab;
        HexTileOverworld tile = Instantiate(prefab, CellPosToWorldPos(coord), Quaternion.identity, tileParent).GetComponent<HexTileOverworld>();
        tile.Blocked = blocked;
        tile.gameObject.name = string.Format("Tile_{0}_{1}_{2}", coord.x, coord.y, coord.z);
        tile.coord = coord;
        tile.Init();
        tiles[coord] = tile;
        tile.transform.localScale = Vector3.one * hexScale;

        return tile;
    }
}