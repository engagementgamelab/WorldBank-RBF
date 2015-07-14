using UnityEngine;
using System.Collections;

/// <summary>
/// Any game data model that gets used as an InventoryItem should implement this.
/// </summary>
public interface IInventoryModel {
	string symbol { get; set; }
	bool unlocked { get; set; }
}
