using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

	static ObjectManager instanceInternal = null;
	static public ObjectManager Instance {
		get {
			if (instanceInternal == null) {
				instanceInternal = Object.FindObjectOfType (typeof (ObjectManager)) as ObjectManager;
				if (instanceInternal == null) {
					GameObject go = new GameObject ("ObjectManager");
					DontDestroyOnLoad (go);
					instanceInternal = go.AddComponent<ObjectManager>();
				}
			}
			return instanceInternal;
		}
	}

	public T Create<T> () where T : class {
		return Create<T> (Vector3.zero);
	}

	public T Create<T> (Vector3 position) where T : class {
		string name = GetName<T> ();
		if (ObjectPool.GetPool (name) == null) {
			CreatePool<T> ();
		}
		return ObjectPool.Instantiate (name, position).GetScript<T> ();
	}

	public void Destroy<T> (Transform t) where T : class {
		ObjectPool.Destroy (GetName<T> (), t);
	}

	void CreatePool<T> () where T : class {
		string prefabName = typeof (T).Name;
		GameObject go = new GameObject (prefabName);
		DontDestroyOnLoad (go);
		go.AddComponent<ObjectPool> ().Init (prefabName, ObjectBank.Instance.GetObject (prefabName).transform);
	}

	string GetName<T> () where T : class {
		string name = typeof (T).Name;
		return name.Substring (0, name.Length);
	}
}