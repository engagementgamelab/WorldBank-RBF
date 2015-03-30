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

	public Vector3 Position {
		get { return Transform.position; }
		set { Transform.position = value; }
	}

	public Vector3 LocalScale {
		get { return Transform.localScale; }
		set { Transform.localScale = value; }
	}
}
