using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour 
{

	public Mesh hexMesh;
	public bool useColors, useUv;
	[System.NonSerialized] List<Vector3> vertices;
	[System.NonSerialized] List<Color> colors;
	[System.NonSerialized] List<int> triangles;
	[System.NonSerialized] List<Vector2> uvs;

	void Awake() 
    {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
	}
	public void Clear()
	{
		hexMesh.Clear();
		vertices = ListPool<Vector3>.Get();
		if (useColors)
			colors = ListPool<Color>.Get();
		if (useUv)
			uvs = ListPool<Vector2>.Get();
		triangles = ListPool<int>.Get();
	}
	public void Apply()
	{
		hexMesh.SetVertices(vertices);
		ListPool<Vector3>.Add(vertices);

		if (useColors) 
		{
			hexMesh.SetColors(colors);
			ListPool<Color>.Add(colors);
		}

		hexMesh.SetTriangles(triangles, 0);
		ListPool<int>.Add(triangles);
		hexMesh.RecalculateBounds();

		if (useUv) 
		{
			hexMesh.SetUVs(0, uvs);
			ListPool<Vector2>.Add(uvs);
		}
		hexMesh.RecalculateNormals();
		hexMesh.Optimize();
	}
	public void PlanarUVProjection()
	{
		int i = 0;
		Bounds bounds = hexMesh.bounds;
		List<Vector2> newUvs = new List<Vector2>();
		while(i < hexMesh.vertices.Count())
		{
			//newUvs.Add(new Vector2(hexMesh.vertices[i].x / bounds.size.x, hexMesh.vertices[i].z / bounds.size.x));
			newUvs.Add(new Vector2(hexMesh.vertices[i].x / 100, hexMesh.vertices[i].z / 100));
			i++;
		}
		hexMesh.SetUVs(0, newUvs);
	}

	public void AddTriangleUnperturbed (Vector3 v1, Vector3 v2, Vector3 v3) 
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) 
    {
		int vertexIndex = vertices.Count;
		vertices.Add(HexMetrics.Perturb(v1));
		vertices.Add(HexMetrics.Perturb(v2));
		vertices.Add(HexMetrics.Perturb(v3));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
    public void AddTriangleColor(Color c1, Color c2, Color c3) 
    {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}
    public void AddTriangleColor(Color color) 
    {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
    {
		int vertexIndex = vertices.Count;
		vertices.Add(HexMetrics.Perturb(v1));
		vertices.Add(HexMetrics.Perturb(v2));
		vertices.Add(HexMetrics.Perturb(v3));
		vertices.Add(HexMetrics.Perturb(v4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	public void AddQuadColor(Color c1, Color c2, Color c3, Color c4) 
    {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}
	public void AddQuadColor (Color c1, Color c2) {
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}
	public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3) 
	{
		uvs.Add(uv1);
		uvs.Add(uv2);
		uvs.Add(uv3);
	}
	
	public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4) 
	{
		uvs.Add(uv1);
		uvs.Add(uv2);
		uvs.Add(uv3);
		uvs.Add(uv4);
	}
	public void AddQuadUV(float uMin, float uMax, float vMin, float vMax) 
	{
		uvs.Add(new Vector2(uMin, vMin));
		uvs.Add(new Vector2(uMax, vMin));
		uvs.Add(new Vector2(uMin, vMax));
		uvs.Add(new Vector2(uMax, vMax));
	}
}