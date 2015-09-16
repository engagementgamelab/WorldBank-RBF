using System.Collections.Generic;

/// <summary>
/// An InventoryItem that uses a game data model.
/// </summary>
public class ModelItem : InventoryItem {

	public delegate void OnUpdateUnlocked ();

	public override string Name { get { return "Model Item"; } }

	string symbol;

	/// <summary>
	/// The symbol of the model.
	/// </summary>
	public string Symbol { get { return symbol; } }

	bool unlocked;

	/// <summary>
	/// Gets/sets the unlocked state. The initial unlocked state is set when the model is loaded.
	/// Setting also triggers the onUpdateUnlocked callback.
	/// </summary>
	public bool Unlocked { 
		get { return unlocked; }
		set { 
			unlocked = value; 
			if (onUpdateUnlocked != null) 
				onUpdateUnlocked ();
		}
	}

	List<string> context = new List<string> ();

	/// <summary>
	/// Gets/sets the unlockable context.
	/// </summary>
	public List<string> Context {
		get { return context; }
		set { context = value; }
	}

	List<string> npc = new List<string> ();

	//// <summary>
	/// Gets/sets the NPC that unlocked this item.
	/// </summary>
	public List<string> Npc {
		get { return npc; }
		set { npc = value; }
	}

	protected Models.Unlockable model;

	/// <summary>
	/// Gets/sets the data model that this InventoryItem is using. This should only be set once
	/// when the item is created.
	/// </summary>
	public Models.Unlockable Model { 
		get { return model; }
		set { 
			model = value;
			symbol = model.symbol;
			unlocked = model.unlocked;
		}
	}

	/// <summary>
	/// Called whenever the unlocked state of the item changes.
	/// </summary>
	public OnUpdateUnlocked onUpdateUnlocked;
}
