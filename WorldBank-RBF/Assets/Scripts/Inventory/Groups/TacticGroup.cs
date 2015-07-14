using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains all the plan tactics in the game.
/// </summary>
public class TacticGroup : ModelGroup<TacticItem> {
	public override string ID { get { return "tactics"; } }
}
