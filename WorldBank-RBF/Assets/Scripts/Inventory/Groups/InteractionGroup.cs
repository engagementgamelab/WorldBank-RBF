using UnityEngine;
using System.Collections;

/// <summary>
/// Contains interactions.
/// </summary>
public class InteractionGroup : ItemGroup<InteractionItem> {
	
	public override string ID { get { return "interactions"; } }

	/// <summary>
	/// Each city has a number of interactions that is less than the total number of possible interactions.
	/// This method sets the interaction count = possible interactions - starting interactions so that
	/// the player can use all possible interactions.
	/// </summary>
	public void SetExtraInteractions (string citySymbol) {
		Set (DataManager.GetCityNPCCount (citySymbol) - DataManager.GetCityInfo (citySymbol).npc_interactions);
	}
}
