using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ParallaxLayerDesigner : ScriptableObject {

	string PATH { get { return Application.dataPath + "/Textures/Cities/"; } }

	public EditorObjectDrawer<ParallaxLayer> objectDrawer = 
        new EditorObjectDrawer<ParallaxLayer> ();

    // ParallaxImageDesigner backgroundImageDesigner = null;

    void OnEnable () {
    	/*if (backgroundImageDesigner == null) {
    		backgroundImageDesigner = CreateInstance ("ParallaxImageDesigner") as ParallaxImageDesigner;
    	}*/
    }

	public void OnGUI () {
		if (objectDrawer.Target == null) return;
		// backgroundImageDesigner.objectDrawer.Target = objectDrawer.Target;
		int layerIndex = objectDrawer.Target.Index + 1;
		GUILayout.Label ("Layer " + layerIndex);
		if (GUILayout.Button ("Load layer textures folder")) {
			LoadTexturesDirectory ();
		}
		objectDrawer.DrawObjectProperties ();
		// backgroundImageDesigner.OnGUI ();
	}

	void LoadTexturesDirectory () {
		string loadPath = EditorUtility.OpenFolderPanel ("Load layer textures", PATH, "json");
		string[] textures = Directory.GetFiles (loadPath);
		List<string> texturesToLoad = new List<string> ();
		for (int i = 0; i < textures.Length; i ++) {
			string texture = textures[i];
			if (texture.EndsWith (".png")) {
				texturesToLoad.Add (texture);
			}
		}
		LoadTextures (texturesToLoad);
	}

	void LoadTextures (List<string> textures) {
		objectDrawer.Target.ClearImages ();
		foreach (string texturePath in textures) {
			ParallaxImage image = EditorObjectPool.Create<ParallaxImage> ();
			string path = "Assets" + texturePath.Remove (0, Application.dataPath.Length);
			image.Texture = AssetDatabase.LoadAssetAtPath (path, typeof (Texture2D)) as Texture2D;
			objectDrawer.Target.AddImage (image);
		}
	}
}
