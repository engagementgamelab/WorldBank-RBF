using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashingText : MB {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = GetComponent<Text> ();
			}
			return text;
		}
	}

	bool flashing = false;

	void OnEnable () {
		flashing = true;
		StartCoroutine (CoFlash ());
	}

	void OnDisable () {
		flashing = false;
	}

	IEnumerator CoFlash () {

		float eTime = 0;
		
		while (flashing) {
			eTime += Time.deltaTime;
			if (eTime >= 0.03f) {
				eTime = 0;
				Text.color = new Color (Random.value, Random.value, Random.value);
				Text.fontSize = Random.Range (14, 22);
				Text.transform.SetLocalScaleY (Random.Range (0.8f, 2f));
				Text.transform.SetLocalScaleX (Random.Range (1.2f, 1.4f));
			}
			yield return null;
		}
	}
}
