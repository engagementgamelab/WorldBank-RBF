using UnityEngine;
using System.Collections;

public interface IInventoryModel {
	string symbol { get; set; }
	bool unlocked { get; set; }
}
