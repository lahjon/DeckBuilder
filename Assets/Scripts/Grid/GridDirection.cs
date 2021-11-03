using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public struct GridDirection : IEquatable<GridDirection>
{

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

    public static GridDirection East = new GridDirection(DirectionName.East);
    public static GridDirection NorthEast = new GridDirection(DirectionName.NorthEast);
    public static GridDirection NorthWest = new GridDirection(DirectionName.NorthWest);
    public static GridDirection West = new GridDirection(DirectionName.West);
    public static GridDirection SouthWest = new GridDirection(DirectionName.SouthWest);
    public static GridDirection SouthEast = new GridDirection(DirectionName.SouthEast);

    public static List<GridDirection> directions = new List<GridDirection>()
    {
        East, NorthEast, NorthWest, West, SouthWest, SouthEast
    };

    public Vector3Int coords;
    public DirectionName Name;
    public int nr;

    public GridDirection(DirectionName name)
    {
        Name = name;
        nr = directionNames.IndexOf(name);
        coords = directionsCoords[nr];
    }

    public GridDirection(int directionNr)
    {
        nr = directionNr;
        coords = directionsCoords[nr];
        Name = directionNames[nr];
    }

    public GridDirection(Vector3Int coords)
    {
        this.coords = coords;
        nr = directionsCoords.IndexOf(coords);
        Name = directionNames[nr];
    }

    public bool IsOpposing(GridDirection other)
    {
        return coords == -1*other.coords;
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
    public bool Equals(GridDirection other) => Name == other.Name;
    public bool Equals(Vector3Int other) => coords == other;
    public override int GetHashCode() => Name.GetHashCode();

    public static Vector3Int operator +(GridDirection a, GridDirection b) => a.coords + b.coords;


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

