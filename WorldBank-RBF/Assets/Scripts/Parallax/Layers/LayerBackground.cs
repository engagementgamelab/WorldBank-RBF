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

	public virtual Texture2D Texture {
		get { return texture; }
		set {
			texture = value;
			for (int i = 0; i < images.Count; i ++) {
				images[i].Material = Material;
			}
		}
	}

	List<QuadImage> images = new List<QuadImage> ();

	public void Init () {
		DestroyBackgrounds ();
		QuadImage image = CreateImage (0);
		images.Add (image);
		//DestroyBackgrounds ();
		//CreateBackgrounds ();
	}

	void CreateBackgrounds () {
		if (texture == null)
			return;
		for (int i = 0; i < tileCount+1; i ++) {
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