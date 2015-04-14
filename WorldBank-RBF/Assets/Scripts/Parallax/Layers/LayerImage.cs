using UnityEngine;
using System.Collections;

public class LayerImage : QuadImage {

	float xPosition;

	public void SetParent (Transform parent, float xPosition=0) {
		this.xPosition = xPosition;
		Transform.parent = parent;
		Transform.Reset ();
	}

	protected override void OnSetTexture () {
		if (Material == null) return;
		Transform.SetLocalPosition (new Vector3 (xPosition, 0, 0));
		/*float scale = (float)Material.mainTexture.height / 512f;
		float bottom = -0.5f + scale / 2;
		Transform.SetLocalScale (new Vector3 (scale, scale, 1));
		Transform.SetLocalPosition (new Vector3 (xPosition, bottom, 0));*/
	}
}
