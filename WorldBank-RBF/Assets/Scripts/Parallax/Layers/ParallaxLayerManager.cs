using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

	public ParallaxLayer NearestLayer {
		get {
			if (layers == null || layers.Count == 0) {
				return null;
			}
			float nearestDistance = Mathf.Infinity;
			ParallaxLayer nearestLayer = null;
			foreach (ParallaxLayer layer in layers) {
				if (layer.Position.z < nearestDistance) {
					nearestLayer = layer;
					nearestDistance = layer.Position.z;
				}
			}
			return nearestLayer;
		}
	}

	public float FurthestNPCDistance {
		get {
			float furthestDistance = 0;
			foreach (ParallaxLayer layer in Layers) {
				foreach (ParallaxNpc npc in layer.npcs) {
					furthestDistance = Mathf.Max (furthestDistance, npc.Position.x);
				}
			}
			return furthestDistance;
		}
	}

	public delegate void OnLoad ();
	public OnLoad onLoad;

	#if DEBUG
	public bool designerScene = false;
	string texPath = "Command+V to paste file path";
	bool showOptions = true;
	TextureLoader textureLoader = new TextureLoader ("", true);
	#endif

	void Awake () {
		Events.instance.AddListener<ArriveInCityEvent> (OnArriveInCityEvent);
		PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
	}

	public void Create (Dictionary<int, List<string>> texturePaths) {
		Clear ();
		LayerCount = texturePaths.Count;
		foreach (var path in texturePaths) {
			layers[path.Key].CreateImages (path.Value);
		}
	}

	void Load (string path, bool useResources=true) {
		
		Clear ();
		ModelSerializer.Load (this, path, useResources);

		// TODO: ModelSerializer *should be* setting the layers list, but isn't for some reason
		// Also- cities would load faster if existing layers were updated rather than destroyed & instantiated
		ParallaxLayer[] pLayers = GameObject.FindObjectsOfType (typeof (ParallaxLayer)) as ParallaxLayer[];
		layers = pLayers.ToList ();
		if (onLoad != null) onLoad ();
    }

    void LoadFromSymbol (string symbol) {
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

    void OnArriveInCityEvent (ArriveInCityEvent e) {
    	LoadFromSymbol (e.City);
    }

    void OnUpdateCurrentCity (string city) {
    	Clear ();
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
