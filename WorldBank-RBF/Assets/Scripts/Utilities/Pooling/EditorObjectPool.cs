using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 *	A wrapper for ObjectPool that remembers objects between play modes
 *	and keeps objects in the order that they were created
 */

public class EditorObjectPool : MonoBehaviour {

	public static List<T> Create<T> (int amount) where T : MonoBehaviour, IEditorPoolable {
		
		List<T> objects = GetObjectsOfTypeInOrder<T> ();
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
		ObjectPool.Destroy<T> (LastInList<T> ().transform);
	}

	public static void Destroy<T> (Transform transform) where T : MonoBehaviour, IEditorPoolable {
		Debug.Log ("destroy " + transform.GetScript<T> ().Index);
		ObjectPool.Destroy<T> (transform);
	}

	public static void Clear () {
		ObjectPool.Clear ();
	}

	public static void CleanUp () {
		ObjectPool.CleanUp ();
	}

	public static List<T> GetObjectsOfTypeInOrder<T> () where T : MonoBehaviour, IEditorPoolable {
		List<T> objects = ObjectPool.GetInstances<T> ().ConvertAll (x => x.GetScript<T> ());
		Debug.Log (typeof (T) + "" + objects.Count);
		List<T> orderedObjects = new List<T> ();
		string db = "";
		for (int i = 0; i < objects.Count; i ++) {
			T obj = LowestIndexInList<T> (objects, i);
			obj.Index = i;
			orderedObjects.Add (obj);
			db += " " + obj.Index;
		}
		Debug.Log (orderedObjects.Count + db);
		return orderedObjects;
	}

	static T LowestIndexInList<T> (List<T> objects, int minIndex) where T : MonoBehaviour, IEditorPoolable {
		int lowestIndex = 10000000;
		T lowestIndexObject = null;
		for (int i = 0; i < objects.Count; i ++) {
			T obj = objects[i];
			int index = obj.Index;
			if (index < lowestIndex && index >= minIndex) {
				lowestIndexObject = objects[i];
				lowestIndex = index;
			}
		}
		return lowestIndexObject;
	}

	static List<Transform> GetObjectsOfType<T> () where T : MonoBehaviour, IEditorPoolable {
		return ObjectPool.GetInstances<T> ();
	}

	static T LastInList<T> () where T : MonoBehaviour, IEditorPoolable {
		List<T> objects = GetObjectsOfTypeInOrder<T> ();
		int highestIndex = 0;
		T highestObject = objects[0];
		for (int i = 0; i < objects.Count; i ++) {
			T obj = objects[i];
			int index = obj.Index;
			if (index > highestIndex) {
				highestIndex = index;
				highestObject = obj;
			}
		}
		return highestObject;
	}
}
