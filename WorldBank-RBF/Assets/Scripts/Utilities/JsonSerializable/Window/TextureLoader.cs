#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class TextureLoader {

	readonly string path;
    bool local = false;

	public TextureLoader (string path, bool local=false) {
		this.path = path;
        this.local = local;
	}

	public void LoadCityTextures (ParallaxLayerManager layerManager) {
        #if UNITY_EDITOR
        string loadPath = EditorUtility.OpenFolderPanel ("Load city textures", path, "");
        if (loadPath != "") LoadCityTextures (layerManager, loadPath);
        #endif
    }

    public void LoadCityTextures (ParallaxLayerManager layerManager, string loadPath) {
        
        List<string> folders = new List<string> (Directory.GetDirectories (loadPath));
        folders = folders.Where (folder => folder.Contains ("layer")).ToList ();
        folders = OrderStrings (folders);

        int folderCount = folders.Count;
        layerManager.LayerCount = folderCount;
        for (int i = 0; i < folderCount; i ++) {
            ParallaxLayer layer = layerManager.layers[i];
            LoadTexturesDirectory (layer, folders[i]);
        }
    }

    public void LoadTexturesDirectory (ParallaxLayer layer, string loadPath="") {
        #if UNITY_EDITOR
        loadPath = (loadPath == "") 
            ? EditorUtility.OpenFolderPanel ("Load layer textures", path, "layer1")
            : loadPath;
        #endif
        List<string> textures = OrderStrings (Directory.GetFiles (loadPath).ToList ());
        List<string> texturesToLoad = new List<string> ();
        for (int i = 0; i < textures.Count; i ++) {
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
            if (local) {
                string path = "file://" + texturePath;
                Coroutine.LoadTexture (path, image);
            } else {
                #if UNITY_EDITOR
                string path = "Assets" + texturePath.Remove (0, Application.dataPath.Length);
                // image._Texture = AssetDatabase.LoadAssetAtPath (path, typeof (Texture2D)) as Texture2D;
                image.TexturePath = path;
                Debug.Log (path);
                #endif
            }
            layer.AddImage (image);
        }
    }

    List<string> OrderStrings (List<string> input) {
        
        // TODO: probably better/quicker ways of doing this
        Dictionary<string, int> orderedFolders = new Dictionary<string, int> ();
        foreach (string i in input) {
            Regex re = new Regex (@"\d+");
            MatchCollection matches = re.Matches (i);
            orderedFolders.Add (i, int.Parse (matches[matches.Count-1].Value));
        }

        List<string> output = new List<string> ();
        foreach (KeyValuePair<string, int> item in orderedFolders.OrderBy (key => key.Value)) {
            output.Add (item.Key);
        }
        return output;
    }
}
