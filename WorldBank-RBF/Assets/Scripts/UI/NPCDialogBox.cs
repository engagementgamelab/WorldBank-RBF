using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCDialogBox : MB {

	public static readonly float width = 50f; // ew

	public CanvasRenderer canvasRenderer;
	public Text text;
	public Button backButton;
	public Transform verticalGroup;
	public Transform choiceGroup;

	string content = "";
	public string Content {
		get { return content; }
		set {
			content = value;
			text.text = content;
		}
	}

	public void Open (NPCBehavior npc) {
		
		Vector3 position = npc.Position;
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

		Position = new Vector3 (GetXPosition (npc.FacingLeft), 0, z);
		verticalGroup.localScale = new Vector3 (scale * 0.1f, scale * 0.1f, 1);
		Transform.localScale = new Vector3 (canvasScale, canvasScale, 1);
	}

	public void Close () {
		NPCFocusBehavior.Instance.FocusOut ();
		ObjectPool.Destroy<NPCDialogBox> (Transform);
	}

	float GetXPosition (bool facingLeft) {
		// TODO: write an actual formula to replace these hardcoded values
		float xOffset = 11.5f;
		return MainCamera.Instance.Positioner.Position.x + (facingLeft ? -xOffset : xOffset);
	}
}
