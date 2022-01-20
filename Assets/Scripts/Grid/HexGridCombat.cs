using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexGridCombat : HexGridController
{
    public static Dictionary<Vector3Int, HexTileCombat> tiles = new Dictionary<Vector3Int, HexTileCombat>();
    void Start()
    {
        //Init();
    }
    public override void Init()
    {
        CreateGrid();
        tiles = _tiles.ToDictionary(x => x.Key, x => (HexTileCombat)x.Value);
    }
}