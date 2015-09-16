
public delegate void OnUnlock<T> (T item) where T : ModelItem;

/// <summary>
/// Group of ModelItems.
/// </summary>
public class ModelGroup<T> : ItemGroup<T> where T : ModelItem, new () {

	public OnUnlock<T> onUnlock;

	/// <summary>
	/// Populates the group with all models containing the given prefix.
	/// (for now this means finding all unlockables and adding models whose symbols
	/// begin with "unlockable_[prefix]_")
	/// </summary>
	/// <param name="prefix">The symbol prefix used to filter data models.</param>
	public ModelGroup (string prefix="") {
		Models.Unlockable[] unlockables = DataManager.GetUnlockablesWithPrefix (prefix);
		foreach (Models.Unlockable unlockable in unlockables) {
			Add (new T () { Model = unlockable });
		}
	}

	ModelItem Find (string symbol) {
		foreach (ModelItem item in Items) {
			if (item.Symbol == symbol)
				return item;
		}

		throw new System.Exception("Unable to find unlockable with symbol '" + symbol + "'!");
	}

	/// <summary>
	/// Locks the item with the given symbol.
	/// </summary>
	/// <param name="symbol">The symbol of the item to lock.</param>
	public void Lock (string symbol) {
		Find (symbol).Unlocked = false;
		SendUpdateMessage ();
	}

	/// <summary>
	/// Unlocks the item with the given symbol.
	/// </summary>
	/// <param name="symbol">The symbol of the item to unlock.</param>
	public void Unlock (string symbol) {
		T item = Find (symbol) as T;
		item.Unlocked = true;
		SendUnlockMessage (item);
		SendUpdateMessage ();
	}

	/// <summary>
	/// Unlocks the item with the given symbol and set context.
	/// </summary>
	/// <param name="symbol">The symbol of the item to unlock.</param>
	/// <param name="context">The item's context string.</param>
	/// <param name="npc">The symbol of the NPC that unlocked this item (optional).</param>
	public void UnlockWithContext (string symbol, string context, string npcSymbol="") {
		T item = Find (symbol) as T;
		item.Npc.Add (npcSymbol);
		item.Context.Add (context);
		Unlock(symbol);
	}

	/// <summary>
	/// Is the item with the given symbol unlocked?
	/// </summary>
	/// <param name="symbol">The symbol of the item.</param>
    /// <returns>Is the item unlocked?</returns>
	public bool IsUnlocked (string symbol) {
		return Find(symbol).Unlocked;
	}

	void SendUnlockMessage (T item) {
		if (onUnlock != null) onUnlock (item);
	}
}
