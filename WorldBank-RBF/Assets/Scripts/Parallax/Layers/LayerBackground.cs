using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LayerBackground : LayerTexture {

	int tileCount = 1;
	List<QuadImage> images = new List<QuadImage> ();

	/*protected override void Awake () {
		base.Awake ();
		CreateBackgrounds ();
		DestroyBackgrounds ();
		CreateBackgrounds ();
	}*/

	void CreateBackgrounds () {
		if (texture == null)
			return;
		//for (int i = 0; i < tileCount; i ++) {
			/*QuadImage image = CreateImage (i);
			image.gameObject.layer = Layer;
			images.Add (image);*/
			//testImage = ObjectPool.Instantiate ("QuadImage", new Vector3 (0, 0, 0));

		//}
	}

	void DestroyBackgrounds () {
		if (images.Count == 0) return;
		foreach	(QuadImage image in images) {
			//ObjectManager.Instance.Destroy<QuadImage> (image.Transform);
		}
		images.Clear ();
	}

	void OnEnable () {
		//CreateBackgrounds ();
		//DestroyBackgrounds ();
		if (name == "LayerBackground4") {
			CreateBackgrounds ();
		}
	}

	void OnDisable () {
		/*if (testImage != null)
			ObjectPool.Destroy ("QuadImage", testImage);*/
		/*if (name == "LayerBackground4")
			DestroyBackgrounds ();*/
	}
}