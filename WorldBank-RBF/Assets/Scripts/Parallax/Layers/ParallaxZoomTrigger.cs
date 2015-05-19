using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxZoomTrigger))]
public class ParallaxZoomTrigger : ParallaxElement {
	
	[ExposeInWindow]
	public float zoomTarget;

	public override void Reset () {
		base.Reset ();
		zoomTarget = 0f;
	}
}
