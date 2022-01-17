using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexGridOverworld : HexGrid
{
    public static Dictionary<Vector3Int, HexTileOverworld> tiles = new Dictionary<Vector3Int, HexTileOverworld>();
    public GameObject encounterPrefab;
    public override void Init()
    {
        CreateGrid();
        tiles = _tiles.ToDictionary(x => x.Key, x => (HexTileOverworld)x.Value);
        tiles.Values.ToList().ForEach(x => AssignEncounter(x));
    }
    void AssignEncounter(HexTileOverworld tile)
    {
        List<Encounter> encountersToOptimize = new List<Encounter>();
        if (tile.coord != Vector3Int.zero && !tile.Blocked && Random.Range(0, 10) < 2)
            encountersToOptimize.Add(tile.AddEncounter());
        HexOptimizer optimizer = new HexOptimizer();
        optimizer.SetEncounters(encountersToOptimize);
        optimizer.Run();
    }
}