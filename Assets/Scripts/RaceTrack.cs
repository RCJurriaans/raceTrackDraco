using UnityEngine;
using System.Collections;

public class RaceTrack : MonoBehaviour {

	// We store the mid points of the track and render the mesh around it
	private RacePoint[] points;

	// Tell this system how many points you want
	// Note 32k is about max (Meshes in Unity are below 65k triangles or something)
	public int pointCount;

	// Fancy pancy stuff
	public bool autoUpdate;
	public int seed;


	// Use this for initialization
	void Start () {
		generateTrack ();
	}

	// This generates the points of the track, do whatever you want
	public void generatePoints(int seed){
		System.Random prng = new System.Random (seed);
		float crtWidth = 5;
		Vector3 loc = new Vector3(0, 0, 0);
		points[0] = new RacePoint();
		points [0].location = loc; 
		points [0].width = crtWidth;
		points [0].angle = 0;
		for(int i = 1; i < pointCount; i++){
			points[i] = points[0];
			points[i].location = new Vector3(points[i-1].location.x + prng.Next(0,4)-2, points[i-1].location.y, points[i-1].location.z + prng.Next(8,12));
			points[i].angle = (prng.Next(0,90)-45)* Mathf.PI/180f;
			crtWidth = crtWidth + prng.Next(-1, 1)/10f;
			points[i].width = crtWidth;
		}
	}

	// Generate a bunch of points and the make the mesh and display it
	public void generateTrack(){
		points = new RacePoint[pointCount];
		generatePoints (seed);
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		MeshData raceMesh = GenerateMap ();
		display.DrawMesh (raceMesh);
	}

	// For each point, make 2 vertices
	// Next you can use 4 vertices to make 2 triangles
	public MeshData GenerateMap(){
		int numberOfPoints = pointCount;
		// We have 2 vertices for each point
		int verticesCount = numberOfPoints * 2;

		// We have 2 triangles for each point, except the first batch
		// 2 triangles consist of 6 vertices total
		int triangleCount = ((numberOfPoints * 2) - 2)*6;

		// Little wrapper for mesh data
		MeshData meshData = new MeshData (verticesCount, triangleCount);
		int vertexIndex = 0;

		// Initialize point once and update it, because fancy optimization :P
		RacePoint p1;

		for ( int i = 0; i < numberOfPoints; i++ ) {
			p1 = points[i];

			// Move left and right by width/2 
			Vector3 P1 = new Vector3(p1.location.x-(p1.width/2f), p1.location.y, p1.location.z);
			Vector3 P2 = new Vector3(p1.location.x+(p1.width/2f), p1.location.y, p1.location.z);

			// Rotation around center point
			// Done by translation back to base (zero point)
			P1 = P1-p1.location;
			P2 = P2-p1.location;

			// Normal rotation matrix
			float crtAngle= p1.angle;
			float P1x = Mathf.Cos(crtAngle)*P1.x - Mathf.Sin (crtAngle)*P1.z;
			float P1z = Mathf.Sin(crtAngle)*P1.x + Mathf.Cos (crtAngle)*P1.z;
			P1 = new Vector3(P1x, P1.y, P1z);

			float P2x = Mathf.Cos(crtAngle)*P2.x - Mathf.Sin (crtAngle)*P2.z;
			float P2z = Mathf.Sin(crtAngle)*P2.x + Mathf.Cos (crtAngle)*P2.z;
			P2 = new Vector3(P2x, P2.y, P2z);

			// Move back to orginal point
			P1 = P1+p1.location;
			P2 = P2+p1.location;

			// Set vertices
			meshData.vertices[vertexIndex] = P1;
			meshData.vertices[vertexIndex+1] = P2;

			// No clue how to do proper uvs
			meshData.uvs[vertexIndex] = new Vector2(p1.location.x-(p1.width/2f), p1.location.y);
			meshData.uvs[vertexIndex+1] = new Vector2(p1.location.x+(p1.width/2f), p1.location.y);

			// First set of points we do not need trianges, 
			if( i > 0 ){
				meshData.AddTriangle(vertexIndex, vertexIndex-1, vertexIndex-2);
				meshData.AddTriangle(vertexIndex+1, vertexIndex-1, vertexIndex);
			}

			vertexIndex += 2;
		}

		return meshData;
	}
}

[System.Serializable]
public struct RacePoint{
	public Vector3 location;
	public float width;
	public float angle;
}

public class MeshData{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;
	
	int triangleIndex = 0;
	
	public MeshData(int verticesCount, int triangleCount){
		vertices = new Vector3[verticesCount];
		uvs = new Vector2[verticesCount];
		triangles = new int[triangleCount];
	}
	
	public void AddTriangle(int a, int b, int c){
		triangles [triangleIndex] = a;
		triangles [triangleIndex+1] = b;
		triangles [triangleIndex+2] = c;
		triangleIndex += 3;
	}
	
	public Mesh CreateMesh(){
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals ();
		return mesh;
	}
	
}