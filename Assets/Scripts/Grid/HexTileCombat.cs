using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexTileCombat : HexTile
{
    public override void AssignNeighboors()
    {
        foreach (GridDirection dir in GridDirection.Directions)
            if (HexGridCombat.GetTile(coord + dir) is HexTile neigh)
                neighbours.Add(neigh);
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseEnter()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseExit()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMouseUp()
    {
        throw new System.NotImplementedException();
    }
}