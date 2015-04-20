using UnityEngine;
using System.Collections;
using JsonFx.Json;

public class LayerImage : QuadImage {

	[SerializeField, HideInInspector] float xPosition;

	#if UNITY_EDITOR
	public LayerImageSettings Json {
		get {
			LayerImageSettings json = new LayerImageSettings ();
			json.SetTexture (Texture);
			json.SetColliderWidth (ColliderWidth);
			json.SetColliderCenter (ColliderCenter);
			return json;
		}
	}
	#endif

	public void SetParent (Transform parent, float xPosition=0) {
		this.xPosition = xPosition;
		Transform.parent = parent;
		Transform.Reset ();
	}

	protected override void OnSetTexture () {
		if (Material == null) return;
		Transform.SetLocalPosition (new Vector3 (xPosition, 0, 0));
	}
}
