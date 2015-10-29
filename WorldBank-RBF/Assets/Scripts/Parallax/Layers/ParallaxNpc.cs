#define TEST_STANDALONE_LOAD
using UnityEngine;
using UnityEngine.EventSystems;
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
		// get { return facingLeft; }
		get { return true; }
	}

	[ExposeInWindow] public string symbol;
	[ExposeInWindow] public string voice = "fem1";

	[SerializeField] public ParallaxImage parallaxImage2 = null;
	ParallaxImage ParallaxImage2 {
		get {
			if (parallaxImage2 == null) {
				parallaxImage2 = EditorObjectPool.Create<ParallaxImage> ();
				parallaxImage2.Parent = Transform;
				parallaxImage2.Transform.Reset ();
				parallaxImage2.XOffset = -1;
			}
			return parallaxImage2;
		}
		set { parallaxImage2 = value; }
	}

	// This second texture is for NPCs who are too big to fit on a single tile
	// A better way to handle this would be to have a group of ParallaxImages
	// (so that NPC size can be arbitrary)

	[ExposeInWindow] public Texture2D texture2 = null;
	public Texture2D _Texture2 {
		set { ParallaxImage2._Texture = value; }
	}

	string texture2Path = "";
	public string Texture2Path {
		get { 
			// return ParallaxImage2.TexturePath; }
			return texture2Path; }
		set {
			ParallaxImage2.TexturePath = value;
			texture2Path = value;
			texture2 = null;
		}
	}

	protected override void Awake () {
		base.Awake ();
		if (texture2 != null) _Texture2 = texture2;
	}

	public override void Reset () {
		base.Reset ();
		symbol = "";
 	}

 	public override void Refresh () {
 		base.Refresh ();
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

		// Disallow click if NPC any top-level UI is showing (mobile)
		foreach(Touch t in Input.touches)
    {
		  if(EventSystem.current.IsPointerOverGameObject(t.fingerId))
        return;
    }

    if(EventSystem.current.IsPointerOverGameObject())
    	return;

		if (Scale > 1f) {
			SendClickMessage ();
		} else {
			// Don't trigger if the player begins dragging the mouse
			StartCoroutine (CoCheckForDrag ());
		}
	}

	void SendClickMessage () {
		NPCFocusBehavior.Instance.PreviewFocus (this);

		// Tutorial
		DialogManager.instance.RemoveTutorialScreen();
	}

	IEnumerator CoCheckForDrag () {
		
		float time = 0.35f;
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
