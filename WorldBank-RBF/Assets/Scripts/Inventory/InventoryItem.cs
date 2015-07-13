using UnityEngine;
using System.Collections;

/// <summary>
/// An item that can be stored in an Inventory.
/// </summary>
public abstract class InventoryItem {

	/// <summary>
	/// The name of the item.
	/// </summary>
	public abstract string Name { get; }
 	
 	/// <summary>
 	/// The ItemGroup that this InventoryItem is in.
 	/// </summary>
	public ItemGroup Group { get; private set; }

	/// <summary>
	/// The Inventory that this InventoryItem is in.
	/// </summary>
	public Inventory Inventory { get; private set; }

	/// <summary>
	/// Initializes the InventoryItem by setting its Inventory and ItemGroup.
	/// There is generally no need to explicitly call this because ItemGroup 
	/// calls it whenever an InventoryItem is added.
	/// </summary>
	/// <param name="inventory">The Inventory that contains this InventoryItem.</param>
	/// <param name="group">The ItemGroup that contains this InventoryItem.</param>
	public void Initialize (Inventory inventory, ItemGroup group) {
		this.Inventory = inventory;
		this.Group = group;
		OnInit ();
	}

	/// <summary>
	/// This method can be overriden to run additional code on initialization.
	/// This is run after the constructor and after Inventory and Group have been set
	/// so it is safe to reference them.
	/// </summary>
	public virtual void OnInit () {}
}
