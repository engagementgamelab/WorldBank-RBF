using UnityEngine;
using System.Collections;

public enum InputLayer {
	UI = 5,
	ParallaxLighting1 = 8,
	ParallaxLighting2 = 9,
	CityHighlight = 14
}

public class LayerController {
	
	static int[] layers = new int[] { 5, 8, 9, 14 };
	public static int[] Layers { get { return layers; } }
}
