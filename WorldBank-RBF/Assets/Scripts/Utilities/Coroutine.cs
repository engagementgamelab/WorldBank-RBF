using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Coroutine : MonoBehaviour {

	static Coroutine instance = null;
	static public Coroutine Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (Coroutine)) as Coroutine;
				if (instance == null) {
					GameObject go = new GameObject ("Coroutine");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<Coroutine> ();
				}
			}
			return instance;
		}
	}

	List<System.Action<float>> coroutines = new List<System.Action<float>> ();

	public void StartCoroutine (float time, System.Action<float> action, System.Action endAction=null) {
		coroutines.Add (action);
		StartCoroutine (CoCoroutine (time, action, endAction));
	}

	public void StopCoroutine (System.Action<float> action) {
		coroutines.Remove (action);
	}

	IEnumerator CoCoroutine (float time, System.Action<float> action, System.Action endAction=null) {
		
		float eTime = 0f;
	
		while (eTime < time && coroutines.Contains (action)) {
			eTime += Time.deltaTime;
			action (eTime / time);			
			yield return null;
		}

		if (endAction != null) endAction ();
	}

	public static void LoadTexture (string texturePath, AnimatedQuadTexture quadTex) {
		Coroutine.Instance.StartCoroutine (CoLoadTexture (texturePath, quadTex));
	}

	static IEnumerator CoLoadTexture (string texturePath, AnimatedQuadTexture quadTex) {
    	WWW www = new WWW (texturePath);
    	while (!www.isDone) yield return null;
    	quadTex._Material = MaterialsManager.CreateMaterialFromTexture (www.texture);
    }
}
