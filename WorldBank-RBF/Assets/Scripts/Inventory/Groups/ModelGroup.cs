using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelGroup<T> : ItemGroup<T> where T : ModelItem, new () {

	public ModelGroup (string prefix) {
		Models.Unlockable[] unlockables = DataManager.GetUnlockablesWithPrefix (prefix);
		foreach (Models.Unlockable unlockable in unlockables) {
			T t = new T () { Model = unlockable };
			Add (t);
		}
	}

	ModelItem Find (string symbol) {
		foreach (ModelItem item in Items) {
			if (item.Symbol == symbol)
				return item;
		}
		return null;
	}

	public void Lock (string symbol) {
		Find (symbol).Unlocked = false;
		SendUpdateMessage ();
	}

	public void Unlock (string symbol) {
		Find (symbol).Unlocked = true;
		SendUpdateMessage ();
	}
}
