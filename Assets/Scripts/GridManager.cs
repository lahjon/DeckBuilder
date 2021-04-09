using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class GridManager : MonoBehaviour
{
    Grid grid;    
    public Canvas canvas;
    public GameObject debugText;

    public GameObject prefab; 
    public Camera cameraNew;
    public Dictionary<Vector3Int, GameObject> tiles = new Dictionary<Vector3Int, GameObject>();
    
    void Start()
    {
        grid = GetComponent<Grid>();
        SetupGrid();
    }

    // void CreateTileMap(int column = 3, int row = 3)
    // {
    //     for
    // }

    GameObject GetTile(Vector3Int cellCoordinate)
    {
        GameObject tile;
        tiles.TryGetValue(cellCoordinate, out tile);
        Debug.Log("Deoick");

        return tile;
    }

    void AddTile(Vector3Int cellCoordinate)
    {
        Vector3 cellPos = grid.CellToWorld(cellCoordinate);
        GameObject newTile = Instantiate(prefab, cellPos, transform.rotation, transform);

        if (!tiles.ContainsKey(cellCoordinate))
        {
            tiles.Add(cellCoordinate, newTile);
        }
    }

    void SetupGrid()
    {
        Vector3Int coord = new Vector3Int(0,1,0);
        AddTile(coord);
        
    }
}
