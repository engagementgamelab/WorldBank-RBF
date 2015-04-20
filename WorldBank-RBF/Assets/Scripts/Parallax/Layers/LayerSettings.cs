using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using JsonFx.Json;

public delegate void OnUpdateSettings ();
public delegate void OnAddImage ();

public class LayerSettings : MonoBehaviour {

	[SerializeField, HideInInspector] int index;
	public int Index { get { return index; } }

	[SerializeField, HideInInspector] bool selected;
	public bool Selected { 
		get { return selected; }
		set { selected = value; }
	}

	[SerializeField, HideInInspector] float localSeparation = 0;
	public float LocalSeparation { 
		get { return localSeparation; }
		set { 
			localSeparation = value; 
			SendUpdate ();
		}
	}

	[SerializeField, HideInInspector] List<LayerImage> images = new List<LayerImage> ();
	public List<LayerImage> Images {
		get { return images; }
		set {
			images = value;
			SendUpdate ();
		}
	}

	public List<LayerImageSettings> ImageSettings { get; set; }

	#if UNITY_EDITOR
	public LayerSettingsJson Json {
		get { 
			LayerSettingsJson json = new LayerSettingsJson ();
			json.SetIndex (Index);
			json.SetLocalSeparation (LocalSeparation);
			json.SetImages (Images.ConvertAll (x => x.Json));
			return json;
		}
	}
	#endif

	public OnUpdateSettings onUpdateSettings;
	public OnAddImage onAddImage;

	void Start () {
		// this isn't working and I'm not sure why?
		hideFlags = HideFlags.HideInHierarchy;
	}

	public void Init (int index, float localSeparation=0, List<LayerImageSettings> imageSettings=null/*, List<Texture2D> backgroundTextures=null*/) {
		this.index = index;
		this.localSeparation = localSeparation;
		if (imageSettings != null) {
			ImageSettings = imageSettings;
		}
	}

	public void AddImage () {
		if (onAddImage != null) {
			onAddImage ();
		}
	}

	void SendUpdate () {
		if (onUpdateSettings != null) {
			onUpdateSettings ();
		}
	}
}