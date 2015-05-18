using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxZoomTrigger))]
public class ParallaxZoomTrigger : ParallaxElement {
	
	[WindowExposed]
	public float zoomTarget;
}
