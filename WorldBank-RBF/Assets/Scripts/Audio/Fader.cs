using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour {

	static Fader instance = null;
	static public Fader Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (Fader)) as Fader;
				if (instance == null) {
					GameObject go = new GameObject ("Fader");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<Fader>();
				}
			}
			return instance;
		}
	}

	public void Fade (AudioItem item, float from, float to, float time, System.Action onEndFade=null) {
		StartCoroutine (CoFade (item, from, to, time, onEndFade));
	}

	IEnumerator CoFade (AudioItem item, float from, float to, float time, System.Action onEndFade=null) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = eTime / time;
			item.Attenuation = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		item.Attenuation = to;
		if (onEndFade != null) onEndFade ();
	}
}
