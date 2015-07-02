using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelGroup<T> : ItemGroup<T> where T : ModelItem, new () {

	protected ItemGroup<T> unlockedItems = new ItemGroup<T> ();
	
	public List<T> UnlockedItems {
		get { return unlockedItems.Items.ConvertAll (x => (T)x); }
	}

	public ModelGroup (string prefix) {
		Models.Unlockable[] unlockables = DataManager.GetUnlockablesWithPrefix (prefix);
		foreach (Models.Unlockable unlockable in unlockables) {
			// T t = new ModelItem (unlockable) as T;
			T t = new T () { Model = unlockable };
			Add (t);
			if (unlockable.unlocked) {
				unlockedItems.Add (t);
			}
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
		Transfer (unlockedItems, Find (symbol));
	}

	public void Unlock (string symbol) {
		unlockedItems.Transfer (this, Find (symbol));
	}
}
