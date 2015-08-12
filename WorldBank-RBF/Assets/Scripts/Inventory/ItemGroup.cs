using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnUpdate ();
public delegate void OnEmpty ();
public delegate void OnAdd<T> (List<T> items) where T : InventoryItem;

/// <summary>
/// An ItemGroup contains InventoryItems. This abstract class is useful for grouping ItemGroups
/// of different types together, but only ItemGroup<T> should ever inherit from it.
/// </summary>
public abstract class ItemGroup {

	/// <summary>
	/// Get the ID. This is useful when finding an ItemGroup in the Inventory.
	/// </summary>
	public abstract string ID { get; }

	protected List<InventoryItem> items = new List<InventoryItem> ();

	/// <summary>
	/// Get the InventoryItems.
	/// </summary>
	public List<InventoryItem> Items {
		get { return items; }
	}

	protected Inventory inventory;

	/// <summary>
	/// Get the Inventory that this ItemGroup is contained in.
	/// </summary>
	public Inventory Inventory {
		get { return inventory; }
	}

	/// <summary>
	/// Get the total number of items.
	/// </summary>
	public int Count { 
		get { return items.Count; }
	}

	/// <summary>
	/// Returns true if there are no items.
	/// </summary>
	public bool Empty { 
		get { return Count == 0; }
	}

	/// <summary>
	/// Called any time an item is added or removed.
	/// </summary>
	public OnUpdate onUpdate;

	/// <summary>
	/// Called when the last item is removed.
	/// </summary>
	public OnEmpty onEmpty;

	public abstract void Initialize (Inventory inventory);
	public abstract void Set (int count);
	public abstract void Add (int count);
	public abstract void Add (InventoryItem item=null);
	public abstract void Add (List<InventoryItem> newItems);
	public abstract void Remove (int count);
	public abstract InventoryItem Remove (InventoryItem item=null);
	public abstract void Clear ();
	public abstract void Transfer (ItemGroup toGroup, InventoryItem item);
	public abstract bool Contains (InventoryItem item);
	protected abstract void SendUpdateMessage ();
	protected abstract void SendEmptyMessage ();
	public abstract void Print ();
}

/// <summary>
/// Contains InventoryItems of type T. Every type of InventoryItem should have a corresponding ItemGroup
/// and all new ItemGroups should inherit from this class.
/// </summary>
public class ItemGroup<T> : ItemGroup where T : InventoryItem, new () {
	
	public override string ID { get { return ""; } }

	public List<T> MyItems {
		get { return Items.ConvertAll (x => (T)x); }
	}

	/// <summary>
	/// Called any time items are added.
	/// </summary>
	public OnAdd<T> onAdd;

	/// <summary>
	/// Initialize by setting the Inventory that contains this ItemGroup. There is generally no need
	/// to explicitly use this function because Inventory already calls it whenever an ItemGroup is added.
	/// </summary>
	/// <param name="inventory">The Inventory that contains this ItemGroup.</param>
	public override void Initialize (Inventory inventory) {
		this.inventory = inventory;
	}

	/// <summary>
	/// Set the number of items.
	/// </summary>
	public override void Set (int count) {
		if (count == Count) return;
		if (Count < count) {
			Add (count - Count);
		} else {
			Remove (Count - count);
		}
	}

	/// <summary>
	/// Add a number of items.
	/// </summary>
	/// <param name="count">The number of items to add.</param>
	public override void Add (int count) {
		for (int i = 0; i < count; i ++) {
			Add ();
		}
	}

	/// <summary>
	/// Add an InventoryItem
	/// </summary>
	/// <param name="item">The InventoryItem to add.</param>
	public override void Add (InventoryItem item=null) {
		if (item == null) item = new T ();
		Add (new List<InventoryItem> () { item });
	}

	/// <summary>
	/// Add a list of InventoryItems.
	/// </summary>
	/// <param name="newItems">A list of InventoryItems to be added.</param>
	public override void Add (List<InventoryItem> newItems) {
		
		List<InventoryItem> addedItems = new List<InventoryItem> ();
		while (newItems.Count > 0) {
			InventoryItem newItem = newItems[0];
			if (newItem != null) {
				newItem.Initialize (Inventory, this);
				addedItems.Add ((T)newItem);
			}
			newItems.RemoveAt (0);
		}
		items.AddRange (addedItems);

		SendAddMessage (addedItems.ConvertAll (x => (T)x));
		SendUpdateMessage ();
	}

	/// <summary>
	/// Removes a number of items.
	/// </summary>
	/// <param name="count">The number of items to remove</param>
	public override void Remove (int count) {
		for (int i = 0; i < count; i ++) {
			Remove ();
		}
	}

	/// <summary>
	/// Removes the given InventoryItem.
	/// </summary>
	/// <param name="item">The InventoryItem to remove.</param>
	/// <returns>The removed InventoryItem (null if the item was not in the group)</returns>
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

		SendUpdateMessage ();
		if (Empty) SendEmptyMessage ();
		
		return removedItem;
	}

	/// <summary>
	/// Removes all items.
	/// </summary>
	public override void Clear () {
		items.Clear ();
		SendUpdateMessage ();
		SendEmptyMessage ();
	}

	/// <summary>
	/// Transfers an item from this ItemGroup to another ItemGroup.
	/// </summary>
	/// <param name="toGroup">ItemGroup to send the item to.</param>
	/// <param name="item">The InventoryItem to transfer.</param>
	public override void Transfer (ItemGroup toGroup, InventoryItem item) {
		Remove (item);
		toGroup.Add (item);
	}

	/// <summary>
	/// Checks if this ItemGroup contains a specfic InventoryItem.
	/// </summary>
	/// <param name="item">The InventoryItem to search for.</param>
	/// <returns>True if the item was found.</returns>
	public override bool Contains (InventoryItem item) {
		return Items.Contains (item);
	}

	protected override void SendUpdateMessage () {
		if (onUpdate != null) onUpdate ();
	}

	protected override void SendEmptyMessage () {
		if (onEmpty != null) onEmpty ();
	}

	protected void SendAddMessage (List<T> items) {
		if (onAdd != null) onAdd (items);
	}

	/// <summary>
	/// Prints every InventoryItem to the console.
	/// </summary>
	public override void Print () {
		foreach (InventoryItem item in Items) {
			Debug.Log (item.Name);
		}
	}
}