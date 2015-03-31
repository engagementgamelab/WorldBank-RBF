using UnityEngine;
using System.Collections;

public class LayerBackground : LayerTexture {

	int tileCount = 3;

	protected override void Awake () {
		base.Awake ();
		CreateBackgrounds ();
	}

	void CreateBackgrounds () {
		for (int i = 0; i < tileCount; i ++) {
			CreateImage (i).gameObject.layer = Layer;
		}
	}
}
