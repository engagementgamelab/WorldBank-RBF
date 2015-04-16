using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LayerSettingsJson {

	int index;
	float local_separation;
	List<string> background_textures;

	public void SetIndex (int index) {
		this.index = index;
	}

	public void SetLocalSeparation (float local_separation) {
		this.local_separation = local_separation;
	}

	public void SetBackgroundTextures (List<Texture2D> background_textures) {
		this.background_textures = background_textures.ConvertAll (x => AssetDatabase.GetAssetPath (x));
	}

	public int GetIndex () {
		return index;
	}

	public float GetLocalSeparation () {
		return local_separation;
	}

	public List<string> GetBackgroundTextures () {
		return background_textures;
	}
}