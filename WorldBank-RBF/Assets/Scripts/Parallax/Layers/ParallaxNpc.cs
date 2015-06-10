using UnityEngine;
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

	public override void Reset () {
		base.Reset ();
		symbol = "";
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
