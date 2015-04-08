using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerBackground : LayerTexture {

	int tileCount = 1;
	public int TileCount {
		get { return tileCount; }
		set { 
			if (value != tileCount) {
				DestroyBackgrounds ();
				CreateBackgrounds ();
			}
			tileCount = value; 
		}
	}
	List<QuadImage> images = new List<QuadImage> ();

	/*protected override void Awake () {
		base.Awake ();
		CreateBackgrounds ();
		DestroyBackgrounds ();
		CreateBackgrounds ();
	}*/

	public void Init () {
		CreateBackgrounds ();
	}

	void CreateBackgrounds () {
		if (texture == null)
			return;
		for (int i = 0; i < tileCount; i ++) {
			QuadImage image = CreateImage (i);
			image.gameObject.layer = Layer;
			images.Add (image);
		}
	}

	void DestroyBackgrounds () {
		if (images.Count == 0) return;
		foreach	(QuadImage image in images) {
			ObjectPool.Destroy<QuadImage> (image.Transform);
		}
		images.Clear ();
	}
}