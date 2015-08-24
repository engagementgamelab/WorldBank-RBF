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

	float rPercent = -1;
	float RPercent {
		get {
			if (rPercent == -1) {
				rPercent = 0.45f + 0.13f * Aspect;
			}
			return rPercent;
		}
	}

	float lPercent = -1;
	float LPercent {
		get {
			if (lPercent == -1) {
				lPercent = 0.55f - 0.13f * Aspect;
			}
			return lPercent;
		}
	}

	public Button BackButton {
		get { return activeBox.backButton; }
	}

	public UIDialogBox screenSpaceBox;
	public UIDialogBox worldSpaceBox;
	public UIDialogBox activeBox = null;

	public void Open (Transform parent=null, bool worldSpace=false, bool left=false) {
		activeBox = worldSpace ? worldSpaceBox : screenSpaceBox;
		activeBox.gameObject.SetActive (true);
		if (worldSpace) SetPosition (left);
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

	public void AppendButtons<T>(List<T> btnChoices, Transform groupOverride=null) where T: MonoBehaviour {
		
		AddButtons<T>(btnChoices, false, null, true);
	}

	public void AddButtons<T>(List<T> btnChoices, bool vertical=false, Transform groupOverride=null, bool append=false) where T: MonoBehaviour {

		Transform group;
		if (groupOverride != null) {
			group = groupOverride;
		} else {
			group = vertical ? activeBox.verticalGroup : activeBox.horizontalGroup;
		}

		if(!append)
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

	public void AddButton<T> (T btnChoice, Transform group) where T : MonoBehaviour {
		btnChoice.transform.SetParent (group);
		btnChoice.transform.localScale = Vector3.one;
		btnChoice.transform.localPosition = Vector3.zero;
		btnChoice.transform.localEulerAngles = Vector3.zero;
	}

	public void RemoveButton<T> (T button) where T : MonoBehaviour {
		ObjectPool.Destroy<T> (button.transform);
	}

	void SetPosition (bool left) {
		Transform.SetPositionX (
			ScreenPositionHandler.ViewportToWorld (
				new Vector3 (left ? LPercent : RPercent, 0f, 10f)).x
		);
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