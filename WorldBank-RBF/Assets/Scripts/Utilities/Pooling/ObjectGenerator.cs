using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ObjectGenerator : MonoBehaviour {

	public ObjectGeneratorInfo Info = new ObjectGeneratorInfo ();
	List<QuadImage> images = new List<QuadImage> ();
	Stack<QuadImage> cachedImages = new Stack<QuadImage> ();

	[SerializeField, HideInInspector]
	ObjectGeneratorInfoResult LastResult = null;

	#if UNITY_EDITOR
	void Update () {
		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
			this.enabled = false;
		} else {
			if (Refresh (false)) {
				DestroyObjects ();
				CreateObjects ();
			}
		}
	}
	#else
	void Awake () {
		//DestroyObjects ();
		CreateObjects ();
	}
	#endif

	public bool Refresh (bool forceRefresh) {
		if (ObjectGeneratorInfoResult.Refresh (this, ref LastResult, Info) || forceRefresh) {
			return true;
		}
		return false;
	}

	void CreateObjects () {
		for (int i = 0; i < Info.imageCount; i ++) {
			if (cachedImages.Count > 0) {
				QuadImage image = cachedImages.Pop ();
				image.gameObject.SetActive (true);
			} else {
				images.Add (GameObject.Instantiate (Info.quadImage) as QuadImage);
			}
		}
	}

	void DestroyObjects () {
		if (images.Count == 0)
			return;
		for (int i = 0; i < images.Count; i ++) {
			images[i].gameObject.SetActive (false);
			cachedImages.Push (images[i]);
		}
	}
}
