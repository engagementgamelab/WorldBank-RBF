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

	public static object Create (string typeName, Transform parent=null) {
		IEditorPoolable obj = ObjectPool.Instantiate (typeName) as IEditorPoolable;
		MonoBehaviour mb = (MonoBehaviour)obj;
		if (parent != null) {
			mb.transform.SetParent (parent);
		}
		obj.Index = ObjectPool.GetInstances (typeName).Count-1;
		obj.Init ();
		return obj as object;
	}

	public static T Create<T> () where T : MonoBehaviour, IEditorPoolable {
		T obj = ObjectPool.Instantiate<T> () as T;
		obj.Index = ObjectPool.GetInstances<T> ().Count-1;
		obj.Init ();
		return obj as T;
	}

	public static void Destroy<T> () where T : MonoBehaviour, IEditorPoolable {
		if (GetObjectsOfType<T> ().Count == 0) return;
		ObjectPool.Destroy<T> (LastInList<T> ().transform);
	}

	public static void Destroy<T> (Transform transform) where T : MonoBehaviour, IEditorPoolable {
		ObjectPool.Destroy<T> (transform);
	}

	public static void Destroy<T> (List<T> objects) where T : MonoBehaviour, IEditorPoolable {
		foreach (T obj in objects) {
			ObjectPool.Destroy<T> (obj.transform);
		}
	}

	public static void Clear () {
		ObjectPool.Clear ();
	}

	public static void CleanUp () {
		ObjectPool.CleanUp ();
	}

	public static T GetObjectAtIndex<T> (int index) where T : MonoBehaviour, IEditorPoolable {
		List<T> objects = GetObjectsOfTypeInOrder<T> ();
		if (objects.Count-1 < index) return null;
		return objects[index];
	}

	public static List<T> GetObjectsOfTypeInOrder<T> () where T : MonoBehaviour, IEditorPoolable {
		List<Transform> transforms = ObjectPool.GetInstances<T> ();
		if (transforms == null)
			return new List<T> ();
		List<T> objects = transforms.ConvertAll (x => x.GetScript<T> ());
		List<T> orderedObjects = new List<T> ();
		string db = "";
		for (int i = 0; i < objects.Count; i ++) {
			T obj = LowestIndexInList<T> (objects, i);
			obj.Index = i;
			orderedObjects.Add (obj);
			db += " " + obj.Index;
		}
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
