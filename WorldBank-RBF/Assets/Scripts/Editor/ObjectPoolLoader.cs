using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoad]
public class ObjectPoolLoader {
	
	static ObjectPoolLoader () {
		Object[] pools = Object.FindObjectsOfType (typeof (ObjectPool));
		for (int i = 0; i < pools.Length; i ++) {
			ObjectPool pool = pools[i] as ObjectPool;
			pool.LoadInstances ();
		}
	}
}