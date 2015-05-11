using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenericDialogBox : MB {

	public CanvasRenderer canvasRenderer;
	public Text text;

	public Transform verticalGroup;
	public Transform choiceGroup;
	
	bool open = false;
	
	string content = "";
	public string Content {
		get { return content; }
		set {
			content = value;
			text.text = content;
		}
	}

	public void Open () {
				
		Vector3 position = Vector3.one;
		float aspect = 1f / MainCamera.Instance.Aspect;
		float z = position.z;
		float scale = z * 0.09f;
		float canvasScale = 1f;

		// TODO: write an actual formula to replace these hardcoded values
		if (aspect <= 0.76f) {
			scale = z * 0.75f;
		} else if (aspect <= 0.81f) {
			scale = z * 0.65f;
		} else if (aspect <= 1f) {
			scale = z * 0.5f;
		}

		Position = new Vector3 (0, 0, z);
		verticalGroup.localScale = new Vector3 (scale * 0.1f, scale * 0.1f, 1);
		Transform.localScale = new Vector3 (canvasScale, canvasScale, 1);
		open = true;

		StartCoroutine (CoRotate());
	}

	public void Close () {
		// NPCFocusBehavior.Instance.FocusOut ();
		// npc.OnClick ();
		ObjectPool.Destroy<GenericDialogBox> (Transform);
		// callback?
	}

	float GetXPosition (bool facingLeft) {
		// TODO: write an actual formula to replace these hardcoded values
		float xOffset = 11.5f;
		return MainCamera.Instance.Positioner.Position.x + (facingLeft ? -xOffset : xOffset);
	}

	IEnumerator CoRotate () {
		
		float startTime = Time.time;
		float startRotation = 2.5f;
		float endRotation = 5f;

		while (open) {
			float eTime = startTime - Time.time;
			Transform.rotation = Quaternion.Euler (0, startRotation + Mathf.PingPong (eTime, endRotation), 0);
			yield return null;
		}
	}
}