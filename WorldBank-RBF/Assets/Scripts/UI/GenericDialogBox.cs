using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GenericDialogBox : MB {

	public Text text;

	public Transform choiceGroup;
	public Transform verticalChoiceGroup;
		
	string content = "";
	public string Content {
		get { return content; }
		set {
			content = value;
			text.text = content;
		}
	}

	public void Open (Transform parent=null) {
				
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

		Position = Vector3.zero;
		Transform.localScale = Vector3.one;

		if(parent != null)
			Transform.SetParent(parent);

		StartCoroutine (CoRotate());
	}

	public void Close () {
		// NPCFocusBehavior.Instance.FocusOut ();
		// npc.OnClick ();
		ObjectPool.Destroy<GenericDialogBox> (Transform);
		// callback?
	}

	public void Disable() {

		gameObject.SetActive(false);

	}

	public void Enable() {

		gameObject.SetActive(true);

	}

	public virtual void RemoveButtons(bool vertical=false) {

		Transform group = vertical ? verticalChoiceGroup : choiceGroup;

		GenericButton[] remove = group.GetComponentsInChildren<GenericButton>();

		foreach (GenericButton child in remove)
			ObjectPool.Destroy<GenericButton> (child.transform);

	}

	public void AddButtons(List<GenericButton> btnChoices, bool vertical=false) {

		RemoveButtons(vertical);

		Transform group = vertical ? verticalChoiceGroup : choiceGroup;

		if(btnChoices != null) {
			foreach(GenericButton btnChoice in btnChoices) {
				btnChoice.transform.SetParent(group);
				btnChoice.transform.localScale = Vector3.one;
				btnChoice.transform.localPosition = Vector3.zero;
				btnChoice.transform.localEulerAngles = Vector3.zero;
			}
		}

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