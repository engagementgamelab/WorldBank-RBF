
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LayerSettingsJson {

	int index;
	float local_separation;
	List<LayerImageSettings> images;

	public void SetIndex (int index) {
		this.index = index;
	}

	public void SetLocalSeparation (float local_separation) {
		this.local_separation = local_separation;
	}

	public void SetImages (List<LayerImageSettings> images) {
		this.images = images;
	}

	public int GetIndex () {
		return index;
	}

	public float GetLocalSeparation () {
		return local_separation;
	}

	public List<LayerImageSettings> GetImages () {
		return images;
	}
}
#endif