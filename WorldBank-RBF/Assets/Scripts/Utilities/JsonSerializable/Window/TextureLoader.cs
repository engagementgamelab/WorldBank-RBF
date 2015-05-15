using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TextureLoader {

	readonly string path;

	public TextureLoader (string path) {
		this.path = path;
	}

	public void LoadCityTextures (ParallaxLayerManager layerManager) {
        string loadPath = EditorUtility.OpenFolderPanel ("Load city textures", path, "");
        string[] folders = Directory.GetDirectories (loadPath);
        int folderCount = folders.Length;
        layerManager.LayerCount = folderCount;
        for (int i = 0; i < folderCount; i ++) {
            ParallaxLayer layer = layerManager.layers[i];
            LoadTexturesDirectory (layer, folders[i]);
        }
    }

    public void LoadTexturesDirectory (ParallaxLayer layer, string loadPath="") {
        loadPath = (loadPath == "") 
            ? EditorUtility.OpenFolderPanel ("Load layer textures", path, "layer1")
            : loadPath;
        string[] textures = Directory.GetFiles (loadPath);
        List<string> texturesToLoad = new List<string> ();
        for (int i = 0; i < textures.Length; i ++) {
            string texture = textures[i];
            if (texture.EndsWith (".png")) {
                texturesToLoad.Add (texture);
            }
        }
        LoadLayerTextures (layer, texturesToLoad);
    }

    void LoadLayerTextures (ParallaxLayer layer, List<string> textures) {
        layer.ClearImages ();
        foreach (string texturePath in textures) {
            ParallaxImage image = EditorObjectPool.Create<ParallaxImage> ();
            string path = "Assets" + texturePath.Remove (0, Application.dataPath.Length);
            image.Texture = AssetDatabase.LoadAssetAtPath (path, typeof (Texture2D)) as Texture2D;
            layer.AddImage (image);
        }
    }
}
