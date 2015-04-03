using UnityEngine;
using System.Collections;

public class Regenerator<T> : ScriptableObject {

	public UnityEngine.Object Parent = null;

	public virtual bool UpdateResult(T info) {
		return false;
	}

	public static bool Refresh<C>(UnityEngine.Object parent, ref C lastResult, T newInfo) where C : Regenerator<T> {
		if(lastResult == null || lastResult.Parent != parent) {
			lastResult = ScriptableObject.CreateInstance<C>();
			lastResult.Parent = parent;
		}

		return lastResult.UpdateResult(newInfo);
	}
}