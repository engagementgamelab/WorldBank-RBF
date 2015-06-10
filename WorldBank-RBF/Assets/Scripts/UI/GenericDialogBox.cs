using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GenericDialogBox : MB {

	string content = "";
	public string Content {
		get { return content; }
		set {
			content = value;
			activeBox.text.text = content;
		}
	}

	public string Header {
		get { return activeBox.header.text; }
		set { activeBox.header.text = value; }
	}

	public Transform HorizontalGroup {
		get { return activeBox.horizontalGroup; }
		set { activeBox.horizontalGroup = value; }
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
				xPercent = 0.45f + 0.13f * Aspect;
			}
			return xPercent;
		}
	}

	float offset = -1;
	float Offset {
		get {
			return ScreenPositionHandler.ViewportToWorld (new Vector3 (XPercent, 0f, 10f)).x;
		}
	}

	public Button BackButton {
		get { return activeBox.backButton; }
	}

	public UIDialogBox screenSpaceBox;
	public UIDialogBox worldSpaceBox;
	public UIDialogBox activeBox = null;

	/*void OnGUI () {
		if (GUILayout.Button ("Open world")) {
			Open (null, true);
		}

		if (GUILayout.Button ("open screen")) {
			Open (null, false);
		}

		if (GUILayout.Button ("close")) {
			Close ();
		}
	}*/

	public void Open (Transform parent=null, bool worldSpace=false, bool left=false) {
		activeBox = worldSpace ? worldSpaceBox : screenSpaceBox;
		activeBox.gameObject.SetActive (true);
		if (worldSpace) {
			SetPosition (left);
		}
	}

	public void Close () {
		ObjectPool.Destroy<GenericDialogBox> (Transform);
	}

	public void Disable() {
		activeBox.gameObject.SetActive (false);
	}

	public void Enable() {
		activeBox.gameObject.SetActive (true);
	}

	public virtual void RemoveButtons<T>(Transform group) where T : MonoBehaviour {

		T[] remove = group.GetComponentsInChildren<T>();

		foreach (T child in remove)
			ObjectPool.Destroy<T> (child.transform);

	}

	public void AddButtons<T>(List<T> btnChoices, bool vertical=false, Transform groupOverride=null) where T: MonoBehaviour {

		Transform group = vertical ? activeBox.verticalGroup : activeBox.horizontalGroup;

		if(groupOverride != null)
			group = groupOverride;

		RemoveButtons<T>(group);

		if(btnChoices != null) {
			foreach(T btnChoice in btnChoices) {
				btnChoice.transform.SetParent(group);
				btnChoice.transform.localScale = Vector3.one;
				btnChoice.transform.localPosition = Vector3.zero;
				btnChoice.transform.localEulerAngles = Vector3.zero;
			}
		}
	}

	void SetPosition (bool left) {
		// Transform.SetPositionX (left ? -Offset : Offset);
		Transform.SetPositionX (Offset);
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

		yield return null;

/*		while (open) {
			float eTime = startTime - Time.time;
			Transform.rotation = Quaternion.Euler (0, startRotation + Mathf.PingPong (eTime, endRotation), 0);
			yield return null;
		}*/
	}
}