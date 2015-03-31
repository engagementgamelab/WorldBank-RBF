using UnityEngine;
using System.Collections;

public class LayerImage : LayerTexture {

	protected override void Awake () {
		base.Awake ();
		CreateImage (0, true).gameObject.layer = Layer;
	}
}
