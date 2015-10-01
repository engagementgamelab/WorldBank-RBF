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

	public TextureLoader (string path) {
		this.path = path;
	}

    public Dictionary<int, List<string>> GetTextureDirectories (string loadPath="") {
        
        #if UNITY_EDITOR
        if (loadPath == "")
            loadPath = EditorUtility.OpenFolderPanel ("Load city textures", path, "");
        #endif

        if (loadPath == "") return null;

        List<string> layerFolders = new List<string> (Directory.GetDirectories (loadPath));
        layerFolders = layerFolders.Where (folder => folder.Contains ("layer")).ToList ();
        layerFolders = OrderStrings (layerFolders);

        Dictionary<int, List<string>> texturePaths = new Dictionary<int, List<string>> ();
        for (int i = 0; i < layerFolders.Count; i ++) {
            List<string> textures = OrderStrings (Directory.GetFiles (layerFolders[i]).ToList ()).FindAll (x => x.EndsWith (".png"));
            texturePaths.Add (i, textures);
        }

        return texturePaths;
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
