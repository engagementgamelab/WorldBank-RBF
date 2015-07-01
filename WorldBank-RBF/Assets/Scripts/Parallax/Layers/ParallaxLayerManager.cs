using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

[JsonSerializable (typeof (Models.Scene))]
public class ParallaxLayerManager : MonoBehaviour {

	static ParallaxLayerManager instance = null;
	static public ParallaxLayerManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (ParallaxLayerManager)) as ParallaxLayerManager;
				if (instance == null) {
					GameObject go = new GameObject ("ParallaxLayerManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<ParallaxLayerManager>();
				}
			}
			return instance;
		}
	}

	[ExposeInWindow]
	public string cityName;

	[HideInInspector]
	public List<ParallaxLayer> layers = new List<ParallaxLayer> ();
	public List<ParallaxLayer> Layers {
		get { return layers; }
	}

	for (int i = 0; i < ParallaxLayer.layers ; i++) {
		for (int i = 0; i <ParallaxNpc.npcs ; i++) {

			float x = transform.position.x;
		}
	}

	[SerializeField] int layerCount = 1;
	[ExposeInWindow, ExposeProperty]
	public int LayerCount {
		get { return layerCount; }
		set { 
			layerCount = Mathf.Clamp (value, 1, 10);
			RefreshLayers ();
		}
	}

	[SerializeField] float cameraStartPosition = 0f;
	[ExposeInWindow, ExposeProperty]
	public float CameraStartPosition {
		get { return cameraStartPosition; }
		set {
			cameraStartPosition = value;
			MainCamera.Instance.Positioner.XPosition = cameraStartPosition;
		}
	}

	public ParallaxLayer FurthestLayer {
		get { 
			if (layers == null || layers.Count == 0) {
				return null;
			}
			return layers[layers.Count-1]; 
		}
	}

	#if DEBUG
	public bool designerScene = false;
	string texPath = "Command+V to paste file path";
	bool showOptions = true;
	TextureLoader textureLoader = new TextureLoader ("", true);
	#endif

	public void Create (Dictionary<int, List<string>> texturePaths) {
		Clear ();
		LayerCount = texturePaths.Count;
		foreach (var path in texturePaths) {
			layers[path.Key].CreateImages (path.Value);
		}
	}

	public void Load (string path, bool useResources=true) {
		Clear ();
		ModelSerializer.Load (this, path, useResources);

		// TODO: ModelSerializer *should be* setting the layers list, but isn't for some reason
		// Also- cities would load faster if existing layers were updated rather than destroyed & instantiated
		ParallaxLayer[] pLayers = GameObject.FindObjectsOfType (typeof (ParallaxLayer)) as ParallaxLayer[];
		layers = pLayers.ToList ();
    }

    public void LoadFromSymbol (string symbol) {
    	Load ("Config/PhaseOne/Cities/" + symbol);
    }

    void Clear () {
		foreach (ParallaxLayer layer in layers)
			layer.Destroy ();
		layers.Clear ();
    }

	void RefreshLayers () {
	
        layers = EditorObjectPool.GetObjectsOfTypeInOrder<ParallaxLayer> ();

        // Remove layers
        if (layers.Count > layerCount) {
            int remove = layers.Count - layerCount;
            int removed = 0;
            while (removed < remove) {
                EditorObjectPool.Destroy<ParallaxLayer> ();
                removed ++;
            }
            layers.RemoveRange (layerCount, remove);
        }

        // Add layers
        while (layers.Count < layerCount) {
            layers.Add (EditorObjectPool.Create<ParallaxLayer> () as ParallaxLayer);
        }
    }


    #if UNITY_EDITOR
	public void Reset () {
		cityName = "";
	}
	#endif

	#if DEBUG
	void OnGUI () {
		if (!designerScene) return;
		showOptions = GUILayout.Toggle (showOptions, "show options", new GUILayoutOption[0]);
		if (!showOptions) return;
		texPath = GUILayout.TextField (texPath, new GUILayoutOption[0]);
		if (GUILayout.Button ("Load textures")) {
			if (Directory.Exists (texPath)) {
				Create (textureLoader.GetTextureDirectories (texPath));
			}
		}
	}
	#endif
}
