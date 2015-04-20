
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhaseOneCity {

	string city_name;
	List<LayerSettingsJson> layers;
	int layer_count;

	public PhaseOneCity () {}

	public void SetCityName (string city_name) {
		this.city_name = city_name;
	}

	public void SetLayers (List<LayerSettingsJson> layers) {
		this.layers = layers;
	}

	public void SetLayerCount (int layer_count) {
		this.layer_count = layer_count;
	}

	public string GetCityName () {
		return city_name;
	}

	public List<LayerSettingsJson> GetLayers () {
		return layers;
	}

	public int GetLayerCount () {
		return layer_count;
	}
}
#endif