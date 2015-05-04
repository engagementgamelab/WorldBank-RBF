using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory {

	Dictionary<System.Type, ItemGroup> groups = new Dictionary<System.Type, ItemGroup> ();
	public Dictionary<System.Type, ItemGroup> Groups {
		get { return groups; }
	}

	public void Add (ItemGroup group) {
		group.Initialize (this);
		groups.Add (group.GetType (), group);
	}

	public void AddItem<T> (InventoryItem item=null) where T : ItemGroup {
		Get<T> ().Add (item);
	}

	public void RemoveItem<T> (InventoryItem item=null) where T : ItemGroup {
		Get<T> ().Remove (item);
	}

	public T Get<T> () where T : ItemGroup {
		ItemGroup group;
		if (groups.TryGetValue (typeof (T), out group)) {
			return group as T;
		}
		return null;
	}
}
