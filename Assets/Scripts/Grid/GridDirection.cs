using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class GridDirection : IEquatable<GridDirection>
{
    public Vector3Int coords;
    public DirectionName name;
    public int nr;

    public GridDirection(DirectionName name)
    {
        this.name = name;
        nr = directionNames.IndexOf(name);
        coords = directionsCoords[nr];
    }

    public GridDirection(int directionNr)
    {
        nr = directionNr;
        coords = directionsCoords[nr];
        name = directionNames[nr];
    }

    public GridDirection(Vector3Int coords)
    {
        this.coords = coords;
        nr = directionsCoords.IndexOf(coords);
        name = directionNames[nr];
    }

    public bool IsOpposing(GridDirection other)
    {
        return coords + other.coords == new Vector3Int(0, 0, 0);
    }

    public GridDirection GetOpposing()
    {
        return new GridDirection(coords * -1);
    }

    public static implicit operator Vector3Int(GridDirection dir) => dir.coords;
    public static implicit operator int(GridDirection dir) => dir.nr;

    public GridDirection Turned(int sixths)
    {
        return new GridDirection((nr + 36 + sixths) % 6);
    }

    public override bool Equals(object obj) => obj is GridDirection other && Equals(other);
    public bool Equals(GridDirection other) => name == other.name;
    public override int GetHashCode() => name.GetHashCode();

    private static List<Vector3Int> directionsCoords = new List<Vector3Int>()
    {
        new Vector3Int(1,-1,0),
        new Vector3Int(1,0,-1),
        new Vector3Int(0,1,-1),
        new Vector3Int(-1,1,0),
        new Vector3Int(-1,0,1),
        new Vector3Int(0,-1,1),
    };

    private static List<DirectionName> directionNames = new List<DirectionName>() {
        DirectionName.East,
        DirectionName.NorthEast,
        DirectionName.NorthWest,
        DirectionName.West,
        DirectionName.SouthWest,
        DirectionName.SouthEast
    };

    public enum DirectionName
    {
        East,
        NorthEast,
        NorthWest,
        West,
        SouthWest,
        SouthEast
    }
}

