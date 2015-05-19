using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxNpc))]
public class ParallaxNpc : ParallaxElement {

	[ExposeInWindow]
	public string symbol;

	public override void Reset () {
		base.Reset ();
		symbol = "";
	}
}
