using UnityEngine;
using System.Collections;

public enum InputLayer {
	UI = 5,
	DepthLayer1 = 8,
	DepthLayer2 = 9,
	DepthLayer3 = 10,
	DepthLayer4 = 11,
	DepthLayer5 = 12,
	DepthLayer6 = 13,
	CityHighlight = 14
}

public class LayerController {
	
	static int[] layers = new int[] { 5, 8, 9, 10, 11, 12, 13, 14 };
	public static int[] Layers { get { return layers; } }

	static int[] depthLayers = new int[] { 8, 9, 10, 11, 12, 13 };
	public static int[] DepthLayers { get { return depthLayers; } }
}
