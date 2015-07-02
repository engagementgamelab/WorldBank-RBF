using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanTacticGroup : ModelGroup<PlanTacticItem> {
	
	public override string Name { get { return "Plan Tactics"; } }

	public PlanTacticGroup () : base ("") {
		
	}

	public string[] GetUniqueTacticSymbols () {
		List<string> symbols = new List<string> ();
		foreach (InventoryItem item in Items) {
			PlanTacticItem tactic = (PlanTacticItem)item;
			string symbol = tactic.Symbol;
			if (!symbols.Contains (symbol))
				symbols.Add (symbol);
		}
		return symbols.ToArray ();
	}
}
