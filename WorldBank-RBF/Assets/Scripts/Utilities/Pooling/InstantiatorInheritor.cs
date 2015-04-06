using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class InstantiatorInheritor : Instantiator {

	public int count = 3;
	/*void Awake () {
		Instantiate ();
	}*/

	protected override void Instantiate () {
		for (int i = 0; i < count; i ++) {
			ObjectPool.Instantiate<QuadImage> ();
		}
	}

	protected override void Destroy () {
		ObjectPool.DestroyAll<QuadImage> ();
	}
}
