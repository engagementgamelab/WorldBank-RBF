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

	Transform parent;
	public Transform Parent {
		get { return Transform.parent; }
		set { Transform.SetParent (value); }
	}

	public Vector3 Position {
		get { return Transform.position; }
		set { Transform.position = value; }
	}

	public Vector3 LocalPosition {
		get { return Transform.localPosition; }
		set { Transform.localPosition = value; }
	}

	public Vector3 LocalScale {
		get { return Transform.localScale; }
		set { Transform.localScale = value; }
	}

	// Is this the best place for this function?
	protected T CreateChildIfNoneExists<T> () where T : MB, IEditorPoolable {
		T t = Transform.GetChildOfType<T> ();
		if (t == null) {
			t = EditorObjectPool.Create<T> ();
			t.Transform.SetParent (Transform);
		}
		return t;
	}
}
