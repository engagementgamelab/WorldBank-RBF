using UnityEngine;
using System.Collections;

public class ModelItem : InventoryItem {

	public delegate void OnUpdateUnlocked ();

	public override string Name { get { return "Model Item"; } }

	string symbol;
	public string Symbol { get { return symbol; } }

	bool unlocked;
	public bool Unlocked { 
		get { return unlocked; }
		set { 
			unlocked = value; 
			if (onUpdateUnlocked != null) 
				onUpdateUnlocked ();
		}
	}

	protected Models.Unlockable model;
	public Models.Unlockable Model { 
		get { return model; }
		set { 
			model = value;
			symbol = model.symbol;
			unlocked = model.unlocked;
		}
	}

	public OnUpdateUnlocked onUpdateUnlocked;
}
