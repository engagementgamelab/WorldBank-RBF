using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains all the plan tactics in the game.
/// </summary>
public class TacticGroup : ModelGroup<TacticItem> {
	
	public override string ID { get { return "tactics"; } }

	/// <summary>
	/// Gets a list of TacticItems.
	/// </summary>
	public List<TacticItem> Tactics {
		get { return Items.ConvertAll (x => (TacticItem)x); }
	}

}