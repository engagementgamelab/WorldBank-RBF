using UnityEngine;
using System.Collections;

public class ParallaxLayerDesigner : ScriptableObject {

	public EditorObjectDrawer<ParallaxLayer> objectDrawer = 
        new EditorObjectDrawer<ParallaxLayer> ();

    ParallaxImageDesigner backgroundImageDesigner = null;

    void OnEnable () {
    	if (backgroundImageDesigner == null) {
    		backgroundImageDesigner = CreateInstance ("ParallaxImageDesigner") as ParallaxImageDesigner;
    	}
    }

	public void OnGUI () {
		if (objectDrawer.Target == null) return;
		backgroundImageDesigner.objectDrawer.Target = objectDrawer.Target.image;
		int layerIndex = objectDrawer.Target.Index + 1;
		GUILayout.Label ("Layer " + layerIndex);
		objectDrawer.DrawObjectProperties ();
		backgroundImageDesigner.OnGUI ();
	}
}
