using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory {

	List<ItemGroup> groups = new List<ItemGroup> ();
	public List<ItemGroup> Groups {
		get { return groups; }
	}

	public void Add (ItemGroup group) {
		group.Initialize (this);
		groups.Add (group);
	}

	/*public void AddItem<T> (InventoryItem item) where T : ItemGroup {
		Get<T> ().Add (item);
	}*/

	public T Get<T> () where T : ItemGroup {
		foreach (ItemGroup group in groups) {
			T t = group as T;
			if (t != null) return t;
		}
		return null;
	}
}
