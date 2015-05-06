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
			json.x_pos = XPos;
			json.facing_left = FacingLeft;
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

	public InputLayer[] IgnoreLayers { 
		get { return new InputLayer[] { InputLayer.UI }; } 
	}

	[SerializeField, HideInInspector] float xIndex;
	[SerializeField, HideInInspector] float xPos;
	public float XPos {
		get { return xPos; }
		set { 
			xPos = value;
			Transform.SetLocalPositionX (xIndex + xPos);
		}
	}

	[SerializeField, HideInInspector] string npcSymbol = "";
	public string NPCSymbol {
		get { return npcSymbol; }
		set { 
			npcSymbol = value;
			if (npcSymbol == "" && behavior != null) {
				ObjectPool.Destroy<NPCBehavior> (behavior.Transform);
				behavior = null;
			}
			if (npcSymbol != "" && behavior == null) {
				behavior = ObjectPool.Instantiate<NPCBehavior> ();
				behavior.Transform.SetParent (Transform);
				behavior.Transform.Reset ();
			}
			if (behavior != null) behavior.npcSymbol = npcSymbol;
		}
	}
	
	[SerializeField, HideInInspector] NPCBehavior behavior = null;
	public NPCBehavior Behavior { get { return behavior; } }

	public bool FacingLeft {
		get {
			if (behavior != null) {
				return behavior.FacingLeft;
			}
			return false;
		}
		set {
			if (behavior != null) {
				behavior.FacingLeft = value;
			}
		}
	}

	public bool IsSprite { get { return npcSymbol != ""; } }

	float scale = 1;
	public float Scale {
		get { return scale; }
		set { 
			scale = value; 
			LocalScale = new Vector3 (scale, scale, 1);
			LocalPosition = new Vector3 (xIndex + XPos + XOffset * (scale-1), (scale-1)*0.33f, 0);
		}
	}

	const float MAX_SCALE = 2f;
	const float MIN_SCALE = 1f;

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

	public void SetParent (Transform parent, float xIndex=0) {
		this.xIndex = xIndex;
		Transform.parent = parent;
		Transform.Reset ();
	}

	protected override void OnSetTexture () {
		if (Material == null) return;
		Transform.SetLocalPosition (new Vector3 (xIndex + xPos, 0, 0));
	}
	
	public void OnClick (ClickSettings clickSettings) {
		if (!IsSprite) return;
		if (behavior != null) behavior.OnClick ();
	}

	public void Expand (float duration) {
		StartCoroutine (CoExpand (duration));
	}

	public void Shrink (float duration) {
		StartCoroutine (CoShrink (duration));
	}

	public void ScaleToPercentage (float p, float duration) {
		ScaleTo (Mathf.Lerp (MIN_SCALE, MAX_SCALE, p), duration);
	}

	public void ScaleTo (float to, float duration) {
		StartCoroutine (CoScaleTo (to, duration));
	}

	IEnumerator CoExpand (float duration) {
		
		float eTime = 0f;
		float from = Scale;
		
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			Scale = Mathf.Lerp (from, MAX_SCALE, progress);
			yield return null;
		}
	}

	IEnumerator CoShrink (float duration) {
		
		float eTime = 0f;
		float from = Scale;
		
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			Scale = Mathf.Lerp (from, MIN_SCALE, progress);
			yield return null;
		}
	}

	IEnumerator CoScaleTo (float to, float duration) {
		
		float eTime = 0f;
		float from = Scale;
	
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			Scale = Mathf.Lerp (from, to, progress);
			yield return null;
		}
	}
}
