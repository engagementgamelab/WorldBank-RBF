#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LayerSettingsJson {
	public int index { get; set; }
	public float local_separation { get; set; }
	public List<LayerImageSettings> images { get; set; }
}
#endif