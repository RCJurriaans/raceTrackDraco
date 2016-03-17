using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (RaceTrack))]
public class RaceTrackEditor : Editor {
	
	public override void OnInspectorGUI(){
		RaceTrack mapGen = (RaceTrack)target;
		
		if (DrawDefaultInspector ()) {
			if(mapGen.autoUpdate){
				mapGen.generateTrack();
			}
		}
		
		if(GUILayout.Button("Generate")){
			mapGen.generateTrack();
		}
	}
}
