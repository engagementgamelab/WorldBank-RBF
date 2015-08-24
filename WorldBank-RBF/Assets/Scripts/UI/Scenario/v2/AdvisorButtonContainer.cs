using UnityEngine;
using System.Collections;

public class AdvisorButtonContainer : MonoBehaviour {

	public delegate void OnEndHide ();
	public OnEndHide onEndHide;

	public void OnAnimationEnd (string id) {
		if (id == "hide" && onEndHide != null)
			onEndHide ();
	}
}
