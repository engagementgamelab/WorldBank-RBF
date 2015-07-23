using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleNotebookButton : MB {

	Image icon = null;
	Image Icon {
		get {
			if (icon == null) {
				icon = Transform.GetChild (0).GetComponent<Image> ();
			}
			return icon;
		}
	}

	Image background = null;
	Image Background {
		get {
			if (background == null) {
				background = GetComponent<Image> ();
			}
			return background;
		}
	}

	void Awake () {
		Events.instance.AddListener<UnlockItemEvent> (OnUnlockItemEvent);
	}

	public void Blink () {
		StartCoroutine (CoBlink ());
	}

	IEnumerator CoBlink () {
		
		Color color1 = Color.black;
		Color color2 = Color.red;
		color1.a = 0.78f;
		color2.a = 0.78f;

		float time = 0.75f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			Color c = Mathf.Round ((eTime / time) * 10) % 2 == 0 
				? color1 
				: color2;
			Background.color = c;
			Icon.color = c;
			yield return null;
		}

		Background.color = color1;
		Icon.color = color1;
	}

	void OnUnlockItemEvent (UnlockItemEvent e) {
		Blink ();
	}
}