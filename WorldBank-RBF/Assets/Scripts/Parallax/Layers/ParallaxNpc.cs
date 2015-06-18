using UnityEngine;
// Run only if inside editor
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxNpc))]
public class ParallaxNpc : ParallaxElement, IClickable, IDraggable {

	public InputLayer[] IgnoreLayers { 
		get { return new InputLayer[] { InputLayer.UI }; }
	}

	public bool MoveOnDrag { get { return false; } }
	bool dragging = false;
	
	[ExposeInWindow] public bool facingLeft = false;
	public bool FacingLeft {
		get { return facingLeft; }
	}

	[ExposeInWindow] public string symbol;

	[SerializeField, HideInInspector] public ParallaxImage parallaxImage2;

	#if UNITY_EDITOR
	public string Texture2Path {
		get { return AssetDatabase.GetAssetPath (_Texture); }
		set { 
			_Texture = AssetDatabase.LoadAssetAtPath (value, typeof (Texture2D)) as Texture2D;
			texture = _Texture;
		}
	}
	#endif

	[ExposeInWindow] public Texture2D texture2 = null;
	Texture2D cachedTexture2 = null;
	public Texture2D _Texture2 {
		get { 
			if (cachedTexture2 == null && parallaxImage2 != null) {
				cachedTexture2 = parallaxImage2._Material.mainTexture as Texture2D;
			}
			return cachedTexture2; 
		}
		set { 
			Debug.Log (value);
			Debug.Log (cachedTexture2);
			if (cachedTexture2 == value) return;
			cachedTexture2 = value;
			if (cachedTexture2 != null) {
				Debug.Log (parallaxImage2);
				if (parallaxImage2 == null) {
					parallaxImage2 = EditorObjectPool.Create<ParallaxImage> ();
					parallaxImage2.Parent = Transform;
					parallaxImage2.Transform.Reset ();
					parallaxImage2.Transform.SetLocalPositionX (LocalPosition.x - 1);
	 			}
				parallaxImage2._Material = MaterialsManager.CreateMaterialFromTexture (cachedTexture2, cachedTexture2.format.HasAlpha ());
				gameObject.SetActive (!MaterialsManager.TextureIsBlank (_Texture2));
			} else {
				parallaxImage2._Material = MaterialsManager.Blank;
			}
		}
	}

	public override void Reset () {
		base.Reset ();
		symbol = "";
 	}

 	public override void Refresh () {
 		base.Refresh ();
 		Debug.Log (texture2);
 		Debug.Log (_Texture2);
 		if (texture2 != null) _Texture2 = texture2;
 	}

	public void OnDragEnter (DragSettings dragSettings) {
		dragging = true;
	}

	public void OnDrag (DragSettings dragSettings) {}

	public void OnDragExit (DragSettings dragSettings) {
		dragging = false;
	}

	public void OnClick (ClickSettings clickSettings) {
		if (Scale > 1f) {
			SendClickMessage ();
		} else {
			// Don't focus if the player ends up doing a drag
			StartCoroutine (CoCheckForDrag ());
		}
	}

	void SendClickMessage () {
		NPCFocusBehavior.Instance.PreviewFocus (this);
	}

	IEnumerator CoCheckForDrag () {
		
		float time = 0.3f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			if (dragging) {
				dragging = false;
				yield break;
			}
			yield return null;
		}

		dragging = false;
		SendClickMessage ();
	}
}
