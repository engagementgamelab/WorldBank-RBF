using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ObjectPool : MonoBehaviour {

	public static Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool> ();
	public static bool Loaded { get { return LoadChecker.Instance.Loaded; } }
	
	List<Transform> activeInstances = new List<Transform> ();
	Stack<Transform> inactiveInstances = new Stack<Transform> ();
	[SerializeField] Transform prefab;

	public static bool StartupLoad () {
		if (LoadChecker.Instance.Loaded)
			return true;

		ObjectPool[] pools = Object.FindObjectsOfType (typeof (ObjectPool)) as ObjectPool[];
		if (pools.Length == 0)
			return false;

		for (int i = 0; i < pools.Length; i ++) {
			pools[i].LoadInstances ();
		}
		LoadChecker.Instance.Loaded = true;
		return true;
	}

	public void LoadInstances () {
		if (activeInstances.Count > 0 || inactiveInstances.Count > 0) {
			return;
		}
		System.Type type = System.Type.GetType (name.Substring (0, name.Length-4));
		MonoBehaviour[] instances = Resources.FindObjectsOfTypeAll (type) as MonoBehaviour[];
		for (int i = 0; i < instances.Length; i ++) {
			Transform t = instances[i].transform;
			if (!t.name.Contains ("(Clone)")) {
				continue;
			}
			if (t.gameObject.activeSelf) {
				activeInstances.Add (t);
			} else {
				inactiveInstances.Push (t);
			}
		}
		pools[name] = this;
	}

	public void Init (string name, Transform prefab) {
		this.name = name;
		this.prefab = prefab;
		InitializeInstance (prefab);
		ReleaseInstance (prefab);
		pools[name] = this;
	}

	static ObjectPool GetPool<T> () where T : MonoBehaviour {
		string poolName = GetPoolName<T> ();
		if (!pools.ContainsKey (poolName)) {
			CreatePool<T> ();
		}
		return pools[poolName];
	}

	static void CreatePool<T> () where T : MonoBehaviour {
		string prefabName = GetPrefabName<T> ();
		string poolName = GetPoolName<T> ();
		GameObject go = new GameObject (poolName);
		DontDestroyOnLoad (go);
		go.AddComponent<ObjectPool> ().Init (poolName, CreatePrefab (prefabName).transform);
	}

	static Transform CreatePrefab (string prefabName) {
		GameObject go = Instantiate (Resources.Load ("Prefabs/" + prefabName)) as GameObject;
		return go.transform;
	}

	static string GetPrefabName<T> () where T : MonoBehaviour {
		return typeof (T).Name;
	}

	static string GetPoolName<T> () where T : MonoBehaviour {
		return typeof (T).Name + "Pool";
	}

	Transform CreateInstance (Vector3 position) {
		Transform t = null;
		if (inactiveInstances.Count > 0) {
			t = inactiveInstances.Pop ();
		} else {
			t = Instantiate (prefab) as Transform;
		}
		t.position = position;
		InitializeInstance (t);
		return t;
	}

	void InitializeInstance (Transform instance) {
		activeInstances.Add (instance);
		instance.gameObject.SetActive (true);
	}

	void ReleaseInstance (Transform instance, bool remove=true) {
		instance.gameObject.SetActive (false);
		if (remove) activeInstances.Remove (instance);
		inactiveInstances.Push (instance);
	}

	void ReleaseAllInstances () {
		if (activeInstances.Count == 0)
			return;
		activeInstances.RemoveAll (item => item == null);
		for (int i = 0; i < activeInstances.Count; i ++) {
			ReleaseInstance (activeInstances[i], false);
		}
		activeInstances.Clear ();
	}

	public static T Instantiate<T> (Vector3 position = new Vector3 ()) where T : MonoBehaviour {
		return GetPool<T> ().CreateInstance (position).GetScript<T> () as T;
	}

	public static void Destroy<T> (Transform instance) where T : MonoBehaviour {
		#if UNITY_EDITOR
		StartupLoad ();
		#endif
		if (instance == null) return;
		GetPool<T> ().ReleaseInstance (instance);
	}

	public static void DestroyAll<T> () where T : MonoBehaviour {
		#if UNITY_EDITOR
		StartupLoad ();
		#endif
		GetPool<T> ().ReleaseAllInstances ();
	}

	void OnDestroy () {
		if (EditorState.InEditMode) {
			pools.Remove (name);
		}
	}
}