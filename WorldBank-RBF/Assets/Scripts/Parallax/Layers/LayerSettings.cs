using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JsonFx.Json;

public delegate void OnUpdateSettings ();

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

	[SerializeField, HideInInspector] List<Texture2D> backgroundTextures = new List<Texture2D> ();
	public List<Texture2D> BackgroundTextures { 
		get { return backgroundTextures; } 
		set { 
			backgroundTextures = value; 
			SendUpdate ();
		}
	}

	public LayerSettingsJson Json {
		get { 
			LayerSettingsJson json = new LayerSettingsJson ();
			json.SetIndex (Index);
			json.SetLocalSeparation (LocalSeparation);
			json.SetBackgroundTextures (BackgroundTextures);
			return json;
		}
	}

	public OnUpdateSettings onUpdateSettings;

	void Start () {
		// this isn't working and I'm not sure why?
		hideFlags = HideFlags.HideInHierarchy;
	}

	public void Init (int index, float localSeparation=0, List<Texture2D> backgroundTextures=null) {
		this.index = index;
		this.localSeparation = localSeparation;
		if (backgroundTextures != null) {
			this.backgroundTextures = backgroundTextures;
		}
	}

	void SendUpdate () {
		if (onUpdateSettings != null) {
			onUpdateSettings ();
		}
	}
}