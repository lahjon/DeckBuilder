using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics 
{
	public const float outerRadius = 10f;
	public const float innerRadius = outerRadius * 0.866025404f;
    public const float solidFactor = 0.8f;
	public const float elevationStep = 3f;
	public const float blendFactor = 1f - solidFactor;
	public const int terracesPerSlope = 2;
	public const float horizontalTerraceStepSize = 1f / terraceSteps;
	public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
	public const int terraceSteps = terracesPerSlope * 2 + 1;
	public const float cellPerturbStrength = 4f;
	public const float noiseScale = 0.003f;
	public const float elevationPerturbStrength = 1.5f;
	public const int chunkSizeX = 5, chunkSizeZ = 5;
	public const float waterElevationOffset = -0.5f;
	public const float waterSurfaceLevel = 1.6f;
	public static Texture2D noiseSource;
    public static Vector3[] corners =
    {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
	};
    public static Vector3 GetFirstCorner(HexDirection direction) 
    {
		return corners[(int)direction];
	}

	public static Vector3 GetSecondCorner(HexDirection direction) 
    {
		return corners[(int)direction + 1];
	}
    public static Vector3 GetFirstSolidCorner(HexDirection direction) 
    {
		return corners[(int)direction] * solidFactor;
	}
	public static Vector3 GetBridge(HexDirection direction) 
	{
		return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
	}
	public static Vector3 GetSecondSolidCorner(HexDirection direction) 
    {
		return corners[(int)direction + 1] * solidFactor;
	}
	public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step) 
	{
		float h = step * HexMetrics.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}
	public static Color TerraceLerp(Color a, Color b, int step) 
	{
		float h = step * HexMetrics.horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}
	public static HexEdgeType GetEdgeType(int elevation1, int elevation2) 
	{
		if (elevation1 == elevation2) 
		{
			return HexEdgeType.Flat;
		}
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1) 
		{
			return HexEdgeType.Slope;
		}

		return HexEdgeType.Cliff;
	}
	public static Vector4 SampleNoise(Vector3 position) 
	{
		return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
	}
	public static Vector3 Perturb(Vector3 position) 
	{
		Vector4 sample = SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
		return position;
	}
}


[System.Serializable] public struct HexCoordinates 
{
	public int X => coords.x;
	public int Z => coords.z;
    public int Y => coords.y;
    public Vector3Int coords;

	public HexCoordinates(int x, int z) 
    {
        coords = new Vector3Int(x, (-x - z), z);
	}
    public static HexCoordinates FromOffsetCoordinates(int x, int z) 
    {
		return new HexCoordinates(x - z / 2, z);
	}

	public override string ToString() 
    {
		return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines() 
    {
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}
}

public enum HexDirection 
{
	NE, 
	E, 
	SE, 
	SW, 
	W, 
	NW
}
public enum HexEdgeType 
{
	Flat, 
	Slope, 
	Cliff
}
public static class HexDirectionExtensions 
{
    public static HexDirection Opposite(this HexDirection direction) 
    {
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
    public static HexDirection Previous(this HexDirection direction) 
    {
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}

	public static HexDirection Next(this HexDirection direction) 
    {
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}
}

public struct EdgeVertices 
{
	public Vector3 v1, v2, v3, v4;
	public EdgeVertices(Vector3 corner1, Vector3 corner2) 
	{
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
		v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
		v4 = corner2;
	}
	public static EdgeVertices TerraceLerp (
		EdgeVertices a, EdgeVertices b, int step)
	{
		EdgeVertices result;
		result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
		result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
		result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
		result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
		return result;
	}
	
}

public static class ListPool<T> 
{
	static Stack<List<T>> stack = new Stack<List<T>>();
	public static List<T> Get () 
	{
		if (stack.Count > 0) 
			return stack.Pop();
		else
			return new List<T>();
	}
	public static void Add (List<T> list) 
	{
		list.Clear();
		stack.Push(list);
	}
}