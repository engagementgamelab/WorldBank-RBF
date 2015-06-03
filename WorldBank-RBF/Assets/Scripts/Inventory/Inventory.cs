using UnityEngine;
using System;
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

	// Moves 'item' from this inventory to another inventory
	public void Transfer<T, U> (Inventory inventory, InventoryItem item) where T : ItemGroup where U : ItemGroup {
		Get<T> ().Remove (item);
		inventory.Get<U> ().Add (item);
	}

	public T Get<T> () where T : ItemGroup {
		try {
			return (T)groups[typeof (T)];
		} catch {
			throw new Exception ("The ItemGroup '" + typeof (T) + "' does not exist in the inventory");
		}
	}
}
