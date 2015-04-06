using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ObjectPool : MonoBehaviour, IPoolable {

	public static Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool> ();
	List<Transform> activeInstances = new List<Transform> ();
	Stack<Transform> inactiveInstances = new Stack<Transform> ();

	[SerializeField] Transform prefab;

#if UNITY_EDITOR
	void OnEnable () {
		if (name != "" && prefab != null)
			Init (name, prefab);
	}

	void OnDisable () {
		pools.Remove (name);
	}
#endif

	public void Init (string name, Transform prefab) {
		this.name = name;
		this.prefab = prefab;
		InitializeInstance (prefab);
		ReleaseInstance (prefab);
		pools[name] = this;
	}

	public void LoadInstances () {
		// TODO: after this gets called objects stop pooling correctly
		System.Type type = System.Type.GetType (name.Substring (0, name.Length-4));
		Object[] instances = Object.FindObjectsOfTypeAll (type) as Object[];
		for (int i = 0; i < instances.Length; i ++) {
			MonoBehaviour mb = instances[i] as MonoBehaviour;
			Transform t = mb.transform;
			if (t.gameObject.activeSelf) {
				activeInstances.Add (t);
			} else {
				inactiveInstances.Push (t);
			}
		}
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
		go.AddComponent<ObjectPool> ().Init (poolName, CreatePrefab<T> (prefabName).transform);
	}

	static T CreatePrefab<T> (string createPrefab) where T : MonoBehaviour {
		GameObject go = new GameObject (createPrefab);
		return go.AddComponent<T> ();
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
#if UNITY_EDITOR && DEBUG
		if (instance.GetScript<IPoolable> () == null) {
			Debug.LogError (string.Format ("The object {0} must implement the IPoolable interface", instance));
		}
#endif	
		instance.GetScript<IPoolable> ().OnCreate ();
	}

	void ReleaseInstance (Transform instance, bool remove=true) {
#if UNITY_EDITOR && DEBUG
		if (instance.GetScript<IPoolable> () == null) {
			Debug.LogError (string.Format ("The object {0} must implement the IPoolable interface", instance));
		}
#endif
		instance.GetScript<IPoolable> ().OnDestroy ();
		instance.gameObject.SetActive (false);
		instance.transform.SetParent (transform);
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
		GetPool<T> ().ReleaseInstance (instance);
	}

	public static void DestroyAll<T> () where T : MonoBehaviour {
		GetPool<T> ().ReleaseAllInstances ();
	}

	public void OnCreate () {}
	public void OnDestroy () {}
}
