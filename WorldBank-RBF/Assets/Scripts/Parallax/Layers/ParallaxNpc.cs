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

	[SerializeField] public ParallaxImage parallaxImage2 = null;


	// This second texture is for NPCs who are too big to fit on a single tile
	// A better way to handle this would be to have a group of ParallaxImages
	// (so that NPC size can be arbitrary)

	#if UNITY_EDITOR
	public string Texture2Path {
		get { return AssetDatabase.GetAssetPath (_Texture2); }
		set { 
			_Texture2 = AssetDatabase.LoadAssetAtPath (value, typeof (Texture2D)) as Texture2D;
			texture2 = _Texture2;
		}
	}
	#endif

	[ExposeInWindow] public Texture2D texture2 = null;	
	public Texture2D _Texture2 {
		get {
			return texture2;
		}
		set {
			if (value == null) {
				if (parallaxImage2 != null) {
					EditorObjectPool.Destroy<ParallaxImage> (parallaxImage2.Transform);
					parallaxImage2 = null;
				}
			} else {
				if (parallaxImage2 == null) {
					parallaxImage2 = EditorObjectPool.Create<ParallaxImage> ();
					parallaxImage2.Parent = Transform;
					parallaxImage2.Transform.Reset ();
					parallaxImage2.XOffset = -1;
				}
				parallaxImage2._Material = MaterialsManager.CreateMaterialFromTexture (value, value.format.HasAlpha ());
				parallaxImage2.gameObject.SetActive (true);
			}
			texture2 = value;
		}
	}

	protected override void Awake () {
		base.Awake ();
		_Texture2 = texture2;
	}

	public override void Reset () {
		base.Reset ();
		symbol = "";
 	}

 	public override void Refresh () {
 		base.Refresh ();
 		_Texture2 = texture2;
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
