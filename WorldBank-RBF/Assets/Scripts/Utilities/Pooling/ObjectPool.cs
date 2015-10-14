using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[ExecuteInEditMode]
public class ObjectPool : MonoBehaviour {

	public static Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool> ();
	
	Stack<Transform> inactiveInstances = new Stack<Transform> ();
	public Stack<Transform> InactiveInstances {
		get { return inactiveInstances; }
	}

	List<Transform> activeInstances = new List<Transform> ();
	public List<Transform> ActiveInstances {
		get { return activeInstances; }
	}

	public static int ActiveInstancesCount<T> () where T : MonoBehaviour {
		return GetPool<T> ().ActiveInstances.Count;
	}

	[SerializeField] Transform prefab;

	public static bool StartupLoad () {
		#if UNITY_EDITOR
			ObjectPool[] pools = Object.FindObjectsOfType (typeof (ObjectPool)) as ObjectPool[];
			if (pools.Length == 0)
				return false;

			for (int i = 0; i < pools.Length; i ++) {
				pools[i].LoadInstances ();
			}
			return true;
		#else
			return true;
		#endif
	}

	public void LoadInstances () {
		
		pools[name] = this;
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
	}

	public void Init (string name, Transform prefab) {
		this.name = name;
		this.prefab = prefab;
		InitializeInstance (prefab);
		ReleaseInstance (prefab);
		pools[name] = this;
	}

	static ObjectPool GetPool<T> (string additionalPath=null) where T : MonoBehaviour {
		string poolName = GetPoolName<T> ();
		if (!pools.ContainsKey (poolName)) {
			CreatePool<T> (additionalPath);
		}
		return pools[poolName];
	}

	static ObjectPool GetPool (string typeName, string additionalPath=null) {
		string poolName = typeName + "Pool";
		if (!pools.ContainsKey (poolName)) {
			CreatePool (typeName, additionalPath);
		}
		return pools[poolName];
	}

	static void CreatePool<T> (string additionalPath=null) where T : MonoBehaviour {
		string prefabName = GetPrefabName<T> ();
		string poolName = GetPoolName<T> ();
		GameObject go = new GameObject (poolName);
		// DontDestroyOnLoad (go); // 9/2/15 - this was causing problems - jay
		go.AddComponent<ObjectPool> ().Init (poolName, CreatePrefab (prefabName, additionalPath).transform);
	}

	static void CreatePool (string typeName, string additionalPath=null) {
		string poolName = typeName + "Pool";
		GameObject go = new GameObject (poolName);
		// DontDestroyOnLoad (go); // 9/2/15 - this was causing problems - jay
		go.AddComponent<ObjectPool> ().Init (poolName, CreatePrefab (typeName, additionalPath).transform);	
	}

	static Transform CreatePrefab (string prefabName, string additionalPath=null) {
		GameObject go = null;

		Object resourceObj = Resources.Load ("Prefabs/" + ((additionalPath == null) ? "" : additionalPath + "/")  + prefabName);

		try {
			go = Instantiate (resourceObj) as GameObject;
		}
		catch {
			if(go == null) {
				if(additionalPath == null)
					throw new System.Exception ("The prefab '" + prefabName + "' was not found in the Resources/Prefabs folder or any of its subfolders.");
				else
					throw new System.Exception ("The prefab '" + prefabName + "' was not found in the Resources/Prefabs '" + additionalPath + "' subfolder.");
			}
		}

		#if UNITY_EDITOR
		if (go == null)
			Debug.Log (string.Format ("{0} was not found. Is it in the Resources/Prefabs directory?", prefabName));
		#endif

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
			try {
				t = Instantiate (prefab) as Transform;
			} catch {
				throw new System.Exception ("Could not find a prefab called '" + prefab + ".' This could be because the prefab is not in a Resources folder or there is more than one ObjectPool of this type in the scene.");
			}
		}
		
		if(t == null) {
			Debug.LogWarning("Prefab " + prefab + " to be positioned is null!");
			return null;
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
		instance.SetParent (null);
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

	public static T Instantiate<T> (string additionalPath=null, Vector3 position = new Vector3 ()) where T : MonoBehaviour {

		ObjectPool thisPool = GetPool<T> (additionalPath);
		Transform thisInstance = thisPool.CreateInstance (position);

		if(thisInstance == null)
		{
			Debug.LogWarning("Instance is null!");
			return null;
		}

		T returnType = thisInstance.GetScript<T> ();

		return returnType as T;
		
	}

	public static MonoBehaviour Instantiate (string typeName) {
		return GetPool (typeName).CreateInstance (Vector3.zero).GetScript<MonoBehaviour> ();
	}

	public static Transform InstantiateTransform<T> (Vector3 position = new Vector3 ()) where T : MonoBehaviour {
		return GetPool<T> ().CreateInstance (position);
	}

	public static void Destroy<T> (Transform instance, string additionalPath=null) where T : MonoBehaviour {
		if (instance == null) return;
		StartupLoad ();
		GetPool<T> (additionalPath).ReleaseInstance (instance);
	}

	public static void Destroy<T> (List<Transform> instances, string additionalPath=null) where T : MonoBehaviour {
		if (instances == null || instances.Count == 0) return;
		StartupLoad ();
		ObjectPool p = GetPool<T> (additionalPath);
		int count = instances.Count;
		for (int i = 0; i < count; i ++) {
			p.ReleaseInstance (instances[i]);
		}
	}

	public static void DestroyChildren<T> (Transform instance, string additionalPath=null) where T : MonoBehaviour {
		if (instance == null) return;

		T[] removeObjects = instance.GetComponentsInChildren<T>(true);

		if(removeObjects.Length == 0) return;

		foreach (T obj in removeObjects)
			Destroy<T>(obj.transform, additionalPath);
	}

	public static void DestroyAll<T> (string additionalPath=null) where T : MonoBehaviour {
		StartupLoad ();
		GetPool<T> (additionalPath).ReleaseAllInstances ();
	}

	public static List<Transform> GetInstances<T> () where T : MonoBehaviour {
		return GetPool<T> ().ActiveInstances;
	}

	public static List<Transform> GetInstances (string typeName) {
		return GetPool (typeName).ActiveInstances;
	}

	// Destroys all pooled objects and pools
	public static void Clear () {
		StartupLoad ();
		List<GameObject> poolsToDestroy = new List<GameObject> ();

		// Destroy instances in each pool first
		foreach (var keyval in pools) {
			ObjectPool pool = keyval.Value;
			List<Transform> instances = pool.ActiveInstances;
			for (int i = 0; i < pool.ActiveInstances.Count; i ++) {
				if (instances[i] != null)
					DestroyImmediate (instances[i].gameObject);
			}
			foreach (Transform t in pool.InactiveInstances) {
				if (t != null)
					DestroyImmediate (t.gameObject);
			}
			if (pool != null) poolsToDestroy.Add (pool.gameObject);
		}

		// Then destroy all the pools
		int poolCount = poolsToDestroy.Count;
		for (int i = 0; i < poolCount; i ++) {
			DestroyImmediate (poolsToDestroy[i]);
		}
		pools.Clear ();
	}

	public static void DestroyPool<T> () where T : MonoBehaviour {
		ObjectPool p = GetPool<T> ();
		List<Transform> instances = p.ActiveInstances;
		for (int i = 0; i < p.ActiveInstances.Count; i ++) {
			if (instances[i] != null)
				DestroyImmediate (instances[i].gameObject);
		}
		foreach (Transform t in p.InactiveInstances) {
			if (t != null)
				DestroyImmediate (t.gameObject);
		}
		pools.Remove (pools.First (x => x.Value == p).Key);
		DestroyImmediate (p.gameObject);
	}

	// Destroys all inactive instances and empty pools
	public static void CleanUp () {
		StartupLoad ();
		List<GameObject> poolsToDestroy = new List<GameObject> ();

		// Destroy inactive instances in each pool first
		foreach (var keyval in pools) {
			ObjectPool pool = keyval.Value;
			foreach (Transform t in pool.InactiveInstances) {
				if (t != null)
					DestroyImmediate (t.gameObject);
			}
			pool.InactiveInstances.Clear ();
			if (pool.ActiveInstances.Count == 0) {
				pools[keyval.Key] = null;
				poolsToDestroy.Add (pool.gameObject);
			}
		}

		// Then destroy any pools that are empty
		int poolCount = poolsToDestroy.Count;
		for (int i = 0; i < poolCount; i ++) {
			DestroyImmediate (poolsToDestroy[i]);
		}
	}

	#if UNITY_EDITOR
	void OnDestroy () {
		if (EditorState.InEditMode) {
			pools.Remove (name);
		}
	}
	#endif
}