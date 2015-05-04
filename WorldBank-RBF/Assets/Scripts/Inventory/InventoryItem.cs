using UnityEngine;
using System.Collections;

public abstract class InventoryItem {

	public abstract string Name { get; }

	ItemGroup group;
	public ItemGroup Group {
		get { return group; }
	}

	Inventory inventory;
	public Inventory Inventory {
		get { return inventory; }
	}

	public void Initialize (Inventory inventory, ItemGroup group) {
		this.inventory = inventory;
		this.group = group;
	}
}
