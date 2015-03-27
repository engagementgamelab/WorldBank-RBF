using UnityEngine;
using System.Collections;

public class MB : MonoBehaviour {

	Transform myTransform;
	public Transform Transform {
		get {
			if (myTransform == null) {
				myTransform = transform;
			}
			return myTransform;
		}
	}
}
