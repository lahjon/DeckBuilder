using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public abstract class HexTile : MonoBehaviour
{
    public int id;
    public Vector3Int coord;
    public GameObject graphics;
    public List<HexTile> neighbours = new List<HexTile>();
    [SerializeField]protected bool _blocked;
    public virtual bool Blocked
    {
        get => _blocked;
        set => _blocked = value;
    }
    public virtual bool Traverseable => !Blocked;
    public virtual bool Revealed
    {
        get => _revealed;
        set => _revealed = value;
    }
    [SerializeField]protected bool _revealed;
    public int cost;
    public abstract void Init();
    protected abstract void OnMouseEnter();
    protected abstract void OnMouseExit();
    protected abstract void OnMouseUp();
    public abstract void AssignNeighboors();

}