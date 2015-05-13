using UnityEngine;
using System.Collections;

public class ParallaxLayerDesigner : ScriptableObject {

	public EditorObjectDrawer<ParallaxLayer> objectDrawer = 
        new EditorObjectDrawer<ParallaxLayer> ();

	public void OnGUI () {
		if (objectDrawer.Target == null) return;
		int layerIndex = objectDrawer.Target.Index + 1;
		GUILayout.Label ("Layer " + layerIndex);
		objectDrawer.DrawObjectProperties ();
	}
}
