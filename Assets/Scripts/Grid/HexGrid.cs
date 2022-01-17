using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public abstract class HexGrid : MonoBehaviour
{
    public float hexScale = 0.365f;
    public float tileSize, tileGap;
    public int gridWidth;
    public Transform tileParent;
    public GameObject hexTilePrefab, blockedGraphics, normalGraphics;
    protected static Dictionary<Vector3Int, HexTile> _tiles = new Dictionary<Vector3Int, HexTile>();
    public static HexTile GetTile(Vector3Int coord) => _tiles.ContainsKey(coord) ? _tiles[coord] : null;
    public virtual void CreateGrid()
    {
        for (int i = 0; i <= gridWidth; i++)
            CreateRow(i);
        foreach (HexTile tile in _tiles.Values)
            tile.AssignNeighboors();
    }
    public abstract void Init();
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
    public virtual HexTile AddTile(Vector3Int coord, bool randomBlocked = false)
    {
        if (coord.x + coord.y + coord.z != 0)
        {
            Debug.LogWarning("Invalid coord");
            return null;
        }
        bool blocked = randomBlocked && Random.Range(0, 10) < 2;
        HexTile tile = Instantiate(hexTilePrefab, CellPosToWorldPos(coord), Quaternion.identity, tileParent).GetComponent<HexTile>();
        tile.Blocked = blocked;
        if (blocked)
            tile.graphics = Instantiate(blockedGraphics, tile.transform);
        else
            tile.graphics = Instantiate(normalGraphics, tile.transform);
        tile.gameObject.name = string.Format("Tile_{0}_{1}_{2}", coord.x, coord.y, coord.z);
        //tile.graphics.GetComponent<MeshCollider>().enabled = false;
        tile.coord = coord;
        tile.Init();
        _tiles[coord] = tile;
        tile.transform.localScale = Vector3.one * hexScale;

        return tile;
    }

    public Vector3 CellPosToWorldPos(Vector3Int coord)
    {
        float width = Mathf.Sqrt(3) * (tileSize + tileGap);
        float height = 1.5f * (tileSize + tileGap);
        float x = (width * coord.x * 0.5f) - (width * coord.y * 0.5f);
        float z = height * coord.z * -1;

        return new Vector3(x, transform.position.y, z);
    }
}