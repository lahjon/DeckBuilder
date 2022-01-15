using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexGridCombat : MonoBehaviour
{
    public static Dictionary<Vector3Int, HexTileCombat> tiles = new Dictionary<Vector3Int, HexTileCombat>();
    public static HexTileCombat GetTile(Vector3Int coord) => tiles.ContainsKey(coord) ? tiles[coord] : null;
}