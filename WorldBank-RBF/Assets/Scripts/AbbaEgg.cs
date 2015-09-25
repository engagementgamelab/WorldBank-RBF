using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbbaEgg : MonoBehaviour {

	public Image abbaImage;

	bool initial = true;
	bool unlocked = false;
	string spelled = "";

	void Update () {
		
		if (unlocked) return;

		if (Input.GetKeyDown (KeyCode.A)) {
			spelled += "a";
		} else if (Input.GetKeyDown (KeyCode.B)) {
			spelled += "b";
		} else if (Input.anyKeyDown) {
			spelled = "";
		}
		if (spelled == "abba") {
			unlocked = true;
			abbaImage.gameObject.SetActive (true);
		}
	}

	public void OnPressAbba () {
		if (initial) {
			abbaImage.rectTransform.sizeDelta = new Vector2 (-650, -500);
			initial = false;
		}
		// abbaImage.SetActive (false);
	}
}
