using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 *	A wrapper for ObjectPool that remembers objects between play modes
 *	and keeps objects in the order that they were created
 */

public class EditorObjectPool : MonoBehaviour {

	public static List<Transform> Create<T> (int amount) where T : MonoBehaviour, IEditorPoolable {
		
		List<Transform> objects = GetObjectsOfType<T> ();
		int objCount = (objects == null) ? 0 : objects.Count;

		if (objCount == amount) return objects;
		if (amount > objCount) {
			int addAmount = amount - objCount;
			for (int i = 0; i < addAmount; i ++) {
				Create<T> ();
			}
		}
		if (amount < objCount) {
			int removeAmount = objCount - amount;
			for (int i = 0; i < removeAmount; i ++) {
				Destroy<T> ();
			}
		}
		return objects;
	}

	public static T Create<T> () where T : MonoBehaviour, IEditorPoolable {
		T obj = ObjectPool.Instantiate<T> () as T;
		obj.Index = ObjectPool.GetInstances<T> ().Count-1;
		obj.Init ();
		return obj;
	}

	public static void Destroy<T> () where T : MonoBehaviour, IEditorPoolable {
		if (GetObjectsOfType<T> ().Count == 0) return;
		ObjectPool.Destroy<T> (LastInList<T> ());
	}

	static List<Transform> GetObjectsOfType<T> () where T : MonoBehaviour, IEditorPoolable {
		return ObjectPool.GetInstances<T> ();
	}

	static Transform LastInList<T> () where T : MonoBehaviour, IEditorPoolable {
		List<Transform> objects = GetObjectsOfType<T> ();
		int highestIndex = 0;
		Transform highestObject = objects[0];
		for (int i = 0; i < objects.Count; i ++) {
			Transform obj = objects[i];
			int index = obj.GetScript<T> ().Index;
			if (index > highestIndex) {
				highestIndex = index;
				highestObject = obj;
			}
		}
		return highestObject;
	}
}
