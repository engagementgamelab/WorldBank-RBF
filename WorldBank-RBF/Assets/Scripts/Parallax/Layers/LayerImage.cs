using UnityEngine;
using System.Collections;
using JsonFx.Json;

public class LayerImage : QuadImage, IClickable {

	#if UNITY_EDITOR
	public LayerImageSettings Json {
		get {
			LayerImageSettings json = new LayerImageSettings ();
			json.SetIndex (Index);
			json.SetTexture (Texture);
			json.SetColliderEnabled (ColliderEnabled);
			json.SetColliderWidth (ColliderWidth);
			json.SetColliderCenter (ColliderCenter);
			return json;
		}
	}
	#endif

	public InputLayer[] IgnoreLayers { get { return null; } }

	[SerializeField, HideInInspector] float xPosition;

	bool expanded = false;
	bool scaling = false;

	float scale = 1;
	public float Scale {
		get { return scale; }
		set { 
			scale = value; 
			LocalScale = new Vector3 (scale, scale, 1);
			Transform.SetLocalPositionY ((scale-1)*0.25f);
			LocalPosition = new Vector3 (xPosition + XOffset * (scale-1), (scale-1)*0.1f, 0);
		}
	}

	float XOffset {
		get { return -BoxCollider.center.x; }
	}

	public int Layer {
		get { return gameObject.layer; }
		set {
			gameObject.layer = value;
			Material.renderQueue = 1000 - value;
		}
	}

	public void SetParent (Transform parent, float xPosition=0) {
		this.xPosition = xPosition;
		Transform.parent = parent;
		Transform.Reset ();
	}

	protected override void OnSetTexture () {
		if (Material == null) return;
		Transform.SetLocalPosition (new Vector3 (xPosition, 0, 0));
	}

	public void OnClick (ClickSettings clickSettings) {
		if (scaling) return;
		if (expanded) {
			StartCoroutine (CoShrink ());
		} else {
			StartCoroutine (CoExpand ());
		}
	}

	IEnumerator CoExpand () {
		
		float time = 0.5f;
		float eTime = 0f;
		
		scaling = true;
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Scale = Mathf.Lerp (1, 2, progress);
			yield return null;
		}
		scaling = false;
		expanded = true;
	}

	IEnumerator CoShrink () {
		
		float time = 0.5f;
		float eTime = 0f;
		
		scaling = true;
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Scale = Mathf.Lerp (2, 1, progress);
			yield return null;
		}
		scaling = false;
		expanded = false;
	}
}
