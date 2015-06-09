using UnityEngine;
using System.Collections;

public class DialogBox : MB {

	// TODO: Move this to GenericDialogBox
	static DialogBox instance = null;
	static public DialogBox Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (DialogBox)) as DialogBox;
				if (instance == null) {
					GameObject go = new GameObject ("DialogBox");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<DialogBox>();
				}
			}
			return instance;
		}
	}

	float aspect = -1;
	float Aspect {
		get {
			if (aspect == -1) {
				aspect = (float)Screen.width / (float)Screen.height;
			}
			return aspect;
		}
	}

	float xPercent = -1;
	float XPercent {
		get {
			if (xPercent == -1) {
				xPercent = 0.525f + 0.13f * Aspect;
			}
			return xPercent;
		}
	}

	float offset = -1;
	float Offset {
		get {
			if (offset == -1) {
				offset = Camera.main.ViewportToWorldPoint (new Vector3 (XPercent, 0f, Panel.position.z)).x;
			}
			return offset;
		}
	}

	Transform panel = null;
	Transform Panel {
		get {
			if (panel == null) {
				panel = Transform.GetChild (0).transform;
			}
			return panel;
		}
	}

	void Awake () {
		Close ();
	}

	public void Open (ParallaxNpc npc) {
		Panel.gameObject.SetActive (true);
		SetPosition (npc.FacingLeft);
	}

	public void Close () {
		Panel.gameObject.SetActive (false);
	}

	void SetPosition (bool left) {
		Panel.SetPositionX (left ? -Offset : Offset);
	}
}
