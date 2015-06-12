﻿using UnityEngine;
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
	public abstract void Set (int count);
	public abstract void Add (int count);
	public abstract void Add (InventoryItem item=null);
	public abstract void Add (List<InventoryItem> newItems);
	public abstract void Remove (int count);
	public abstract InventoryItem Remove (InventoryItem item=null);
	public abstract void Clear ();
	public abstract void Transfer (ItemGroup toGroup, InventoryItem item);
	public abstract void SetItemOrder (InventoryItem item, int position);
	public abstract void MoveItemUp (InventoryItem item);
	public abstract void MoveItemDown (InventoryItem item);
	public abstract void Print ();
}

public class ItemGroup<T> : ItemGroup where T : InventoryItem, new () {
	
	public override string Name { get { return ""; } }

	public override void Initialize (Inventory inventory) {
		this.inventory = inventory;
	}

	public override void Set (int count) {
		if (count == Count) return;
		if (Count < count) {
			Add (count - Count);
		} else {
			Remove (Count - count);
		}
	}

	public override void Add (int count) {
		for (int i = 0; i < count; i ++) {
			Add ();
		}
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

	public override void Remove (int count) {
		for (int i = 0; i < count; i ++) {
			Remove ();
		}
	}

	public override InventoryItem Remove (InventoryItem item=null) {
		if (Empty) return null;
		InventoryItem removedItem = (item == null)
			? removedItem = items[0]
			: removedItem = item;
		if (item == null) {
			items.RemoveAt (0);
		} else {
			items.Remove (item);
		}
		return removedItem;
	}

	public override void Clear () {
		items.Clear ();
	}

	// Moves item from this group to another group
	public override void Transfer (ItemGroup toGroup, InventoryItem item) {
		Remove (item);
		toGroup.Add (item);
	}

	public override void SetItemOrder (InventoryItem item, int position) {
		position = Mathf.Clamp (position, 0, Count-1);
		items.Remove (item);
		items.Insert (position, item);
	}

	public override void MoveItemUp (InventoryItem item) {
		SetItemOrder (item, items.IndexOf (item)-1);
	}

	public override void MoveItemDown (InventoryItem item) {
		SetItemOrder (item, items.IndexOf (item)+1);
	}

	public override void Print () {
		foreach (InventoryItem item in Items) {
			Debug.Log (item.Name);
		}
	}
}