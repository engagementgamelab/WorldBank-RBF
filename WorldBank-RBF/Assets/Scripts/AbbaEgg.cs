using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbbaEgg : MonoBehaviour {

	public Image abbaImage;

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
		// abbaImage.SetActive (false);
	}
}
