using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	public void DrawMesh(MeshData meshData){
		meshFilter.sharedMesh = meshData.CreateMesh ();	
	}
}
