using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionsCounter : MB {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (2).GetComponent<Text> ();
			}
			return text;
		}
	}

	Image background = null;
	Image Background {
		get {
			if (background == null) {
				background = Transform.GetChild (0).GetComponent<Image> ();
			}
			return background;
		}
	}

	public int Count {
		set { Text.text = value.ToString (); }
	}

	void Start () {
		PlayerData.InteractionGroup.onUpdate += OnUpdateCount;
		OnUpdateCount ();
	}

	void OnUpdateCount () {
		Count = PlayerData.InteractionGroup.Count;
	}

	public void Blink () {
		StartCoroutine (CoBlink ());
	}

	IEnumerator CoBlink () {
		
		Color color1 = Color.white;
		Color color2 = Color.red;
		color1.a = 0.78f;
		color2.a = 0.78f;

		float time = 0.75f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			Background.color = Mathf.Round ((eTime / time) * 10) % 2 == 0 
				? color1 
				: color2;
			yield return null;
		}

		Background.color = color1;
	}
}
