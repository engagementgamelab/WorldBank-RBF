using UnityEngine;
using System.Collections;

public class PreviewCamera : MonoBehaviour {

	public RenderTexture RenderTexture {
		get { return GetComponent<Camera> ().targetTexture; }
	}

	#if !UNITY_EDITOR
	void Awake () {
		gameObject.SetActive (false);
	}
	#endif
}
