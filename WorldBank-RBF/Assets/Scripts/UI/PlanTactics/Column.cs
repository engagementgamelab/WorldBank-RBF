using UnityEngine;
using System.Collections;

public class Column : MB {

	public RectTransform content;
	
	Inventory inventory;
	protected Inventory Inventory {
		get {
			if (inventory == null) {
				inventory = new Inventory ();
			}
			return inventory;
		}
	}
	
	PlanTacticGroup tacticGroup;
	protected PlanTacticGroup TacticGroup {
		get {
			if (tacticGroup == null) {
				tacticGroup = new PlanTacticGroup ();
			}
			return tacticGroup;
		}
	}
}
