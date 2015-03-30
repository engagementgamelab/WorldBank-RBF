using UnityEngine;
using System.Collections;

public class LayerBackground : MB {

	public Texture2D texture;

	Transform depthLayer = null;
	Transform DepthLayer {
		get {
			if (depthLayer == null) {
				depthLayer = Transform.parent;
			}
			return depthLayer;
		}
	}

	int tileCount = 3;
	Material background;

	void Awake () {
		CreateBackgrounds ();
	}

	void CreateBackgrounds () {
		if (texture == null)
			return;
		background = MaterialsManager.CreateMaterialFromTexture (texture);
		for (int i = 0; i < tileCount; i ++) {
			CreateBackground (i);
		}
	}

	void CreateBackground (float xPosition) {
		QuadImage quadImage = ObjectManager.Instance.Create<QuadImage> ();
		quadImage.Init (background);
		quadImage.Transform.SetParent (Transform);
		quadImage.Transform.SetLocalPosition (new Vector3 (xPosition, 0, 0));
	}
}
