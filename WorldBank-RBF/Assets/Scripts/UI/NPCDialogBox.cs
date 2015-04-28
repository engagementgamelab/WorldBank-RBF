using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCDialogBox : MB {

	public static readonly float width = 50f; // ew

	public CanvasRenderer canvasRenderer;
	public Text text;
	public Button backButton;
	public Transform choiceGroup;

	string content = "";
	public string Content {
		get { return content; }
		set {
			content = value;
			text.text = content;
		}
	}

	public void Open (Vector3 position) {
		float z = position.z;
		float scale = z * 0.1f;
		Position = new Vector3 (0, 0, z);
		LocalScale = new Vector3 (scale, scale, 1);
	}

	public void Close () {
		NPCFocusBehavior.Instance.FocusOut ();
		ObjectPool.Destroy<NPCDialogBox> (Transform);
	}
}
