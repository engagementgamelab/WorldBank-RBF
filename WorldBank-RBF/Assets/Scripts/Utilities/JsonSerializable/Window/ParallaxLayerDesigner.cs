using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ParallaxLayerDesigner : ScriptableObject {

	string PATH { get { return Application.dataPath + "/Textures/Cities/"; } }

	public EditorObjectDrawer<ParallaxLayer> objectDrawer = 
        new EditorObjectDrawer<ParallaxLayer> ();

    TextureLoader textureLoader;

    void OnEnable () {
    	if (textureLoader == null) {
    		textureLoader = new TextureLoader (PATH);
    	}
    }

	public void OnGUI () {
		if (objectDrawer.Target == null) return;
		int layerIndex = objectDrawer.Target.Index + 1;
		GUILayout.Label ("Layer " + layerIndex);
		if (GUILayout.Button ("Load layer textures from directory")) {
			textureLoader.LoadTexturesDirectory (objectDrawer.Target);
		}
		objectDrawer.DrawObjectProperties ();
		if (GUILayout.Button ("Add Element")) {
			ParallaxElement element = EditorObjectPool.Create<ParallaxElement> ();
			objectDrawer.Target.AddImage (element);
		}

		// TODO: Draw all parallax elements attached to the layer
	}
}
