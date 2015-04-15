using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
		get { return new LayerSettingsJson (Index, LocalSeparation, BackgroundTextures); }
	}

	public OnUpdateSettings onUpdateSettings;

	void Awake () {
		hideFlags = HideFlags.HideInHierarchy;
	}

	public void Init (int index) {
		this.index = index;
	}

	void SendUpdate () {
		if (onUpdateSettings != null) {
			onUpdateSettings ();
		}
	}
}

public class LayerSettingsJson {

	int index;
	float localSeparation;
	List<Texture2D> backgroundTextures;

	public LayerSettingsJson (int index, float localSeparation, List<Texture2D> backgroundTextures) {
		this.index = index;
		this.localSeparation = localSeparation;
		this.backgroundTextures = backgroundTextures;
	}
}