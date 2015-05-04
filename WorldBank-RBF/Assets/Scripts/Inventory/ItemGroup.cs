using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemGroup {

	public abstract string Name { get; }

	protected List<InventoryItem> items = new List<InventoryItem> ();
	public List<InventoryItem> Items {
		get { return items; }
	}

	protected Inventory inventory;
	public Inventory Inventory {
		get { return inventory; }
	}

	public int Count { 
		get { return items.Count; }
	}

	public bool Empty { 
		get { return items.Count == 0; }
	}

	public abstract void Initialize (Inventory inventory);
	public abstract void Add (InventoryItem item=null);
	public abstract void Add (List<InventoryItem> newItems);
	public abstract void Remove (InventoryItem item=null);
}

public class ItemGroup<T> : ItemGroup where T : InventoryItem, new () {
	
	public override string Name { get { return ""; } }

	public override void Initialize (Inventory inventory) {
		this.inventory = inventory;
	}

	public override void Add (InventoryItem item=null) {
		if (item == null) item = new T ();
		Add (new List<InventoryItem> () { item });
	}

	public override void Add (List<InventoryItem> newItems) {
		while (newItems.Count > 0) {
			InventoryItem newItem = newItems[0];
			if (newItem != null) {
				newItem.Initialize (Inventory, this);
				items.Add (newItem as T);
			}
			newItems.RemoveAt (0);
		}
	}

	public override void Remove (InventoryItem item=null) {
		if (Empty) return;
		if (item == null) 
			items.RemoveAt (0);
		else 
			items.Remove (item);
	}
}