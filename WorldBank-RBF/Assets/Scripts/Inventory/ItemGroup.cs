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
	public abstract void Add ();
	public abstract void Add (InventoryItem item);
	public abstract void Remove ();
	public abstract void Remove (InventoryItem item);
}

public class ItemGroup<T> : ItemGroup where T : InventoryItem {
	
	public override string Name { get { return ""; } }

	public override void Initialize (Inventory inventory) {
		this.inventory = inventory;
	}

	public void Add2 () {
		Add (new DayItem ());
	}

	public override void Add () {
		Add (new InventoryItem () as T);
	}

	public override void Add (InventoryItem item) {
		Add (new List<InventoryItem> () { item });
	}

	public void Add (List<InventoryItem> newItems) {
		while (newItems.Count > 0) {
			InventoryItem newItem = newItems[0];
			if (newItem != null) {
				newItem.Initialize (Inventory, this);
				items.Add (newItem as T);
			}
			newItems.RemoveAt (0);
		}
	}

	public override void Remove () {
		items.RemoveAt (0);
	}

	public override void Remove (InventoryItem item) {
		items.Remove (item);
	}
}