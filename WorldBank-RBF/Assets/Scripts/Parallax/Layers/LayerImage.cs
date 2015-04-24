using UnityEngine;
using System.Collections;
using JsonFx.Json;

public class LayerImage : QuadImage, IClickable {

	#if UNITY_EDITOR
	public LayerImageSettings Json {
		get {
			LayerImageSettings json = new LayerImageSettings ();
			json.index = Index;
			json.npc_symbol = npcSymbol;
			json.SetTexture (Texture);
			json.collider_enabled = ColliderEnabled;
			json.collider_width = ColliderWidth;
			json.collider_height = ColliderHeight;
			json.collider_center_x = ColliderCenterX;
			json.collider_center_y = ColliderCenterY;
			return json;
		}
	}
	#endif

	public InputLayer[] IgnoreLayers { get { return null; } }

	[SerializeField, HideInInspector] protected float xPosition;
	[SerializeField, HideInInspector] protected string npcSymbol = "";
	public string NPCSymbol {
		get { return npcSymbol; }
		set { npcSymbol = value; }
	}

	public bool IsSprite { get { return npcSymbol != ""; } }

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

	public float XPosition {
		get { return Position.x + XOffset; }
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
		if (!IsSprite) return;
		NPCFocusBehavior.Instance.ToggleFocus (this);
	}

	public void Expand (float duration) {
		StartCoroutine (CoExpand (duration));
	}

	public void Shrink (float duration) {
		StartCoroutine (CoShrink (duration));
	}

	IEnumerator CoExpand (float duration) {
		
		float eTime = 0f;
		
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			Scale = Mathf.Lerp (1, 2, progress);
			yield return null;
		}
	}

	IEnumerator CoShrink (float duration) {
		
		float eTime = 0f;
		
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			Scale = Mathf.Lerp (2, 1, progress);
			yield return null;
		}
	}
}
