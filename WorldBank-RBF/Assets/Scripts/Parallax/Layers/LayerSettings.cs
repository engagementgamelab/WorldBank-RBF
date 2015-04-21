using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using JsonFx.Json;

public class LayerSettings : MB, IEditorPoolable {

	[SerializeField, HideInInspector] int index;
	public int Index { 
		get { return index; } 
		set { index = value; }
	}

	[SerializeField, HideInInspector] bool selected;
	public bool Selected { 
		get { return selected; }
		set { selected = value; }
	}

	[SerializeField, HideInInspector] float localSeparation = 0;
	public float LocalSeparation { 
		get { return localSeparation; }
		set { localSeparation = value; }
	}

	[SerializeField, HideInInspector] List<LayerImageSettings> imageSettings;
	public List<LayerImageSettings> ImageSettings { 
		get { return imageSettings; }
		set { imageSettings = value; }
	}

	#if UNITY_EDITOR
	public LayerSettingsJson Json {
		get { 

			// Update properties from accompanying DepthLayer
			DepthLayer layer = EditorObjectPool.GetObjectAtIndex<DepthLayer> (Index);
			LocalSeparation = layer.LocalSeparation;
			ImageSettings = layer.Images.ConvertAll (x => x.Json);

			// Create a json serializable object
			LayerSettingsJson json = new LayerSettingsJson ();
			json.SetIndex (Index);
			json.SetLocalSeparation (LocalSeparation);
			json.SetImages (ImageSettings);
			return json;
		}
	}
	#endif

	void Start () {
		// this isn't working and I'm not sure why?
		hideFlags = HideFlags.HideInHierarchy;
	}

	public void Init (int index, float localSeparation=0, List<LayerImageSettings> imageSettings=null) {
		this.imageSettings = imageSettings;
		this.index = index;
		this.localSeparation = localSeparation;
	}

	public void Init () {}
}