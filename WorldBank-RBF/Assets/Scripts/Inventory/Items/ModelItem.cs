using UnityEngine;
using System.Collections;

public class ModelItem : InventoryItem {

	public override string Name { get { return "Model Item"; } }

	string symbol;
	public string Symbol { get { return symbol; } }

	protected Models.Unlockable model;
	public Models.Unlockable Model { 
		get { return model; }
		set { 
			model = value;
			symbol = model.symbol;
			OnSetModel ();
		}
	}

	public virtual void OnSetModel () {}
}
