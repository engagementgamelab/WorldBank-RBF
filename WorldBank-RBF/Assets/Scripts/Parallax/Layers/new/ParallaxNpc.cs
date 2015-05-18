using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxNpc))]
public class ParallaxNpc : ParallaxElement {

	[WindowExposed]
	public string symbol;
}
