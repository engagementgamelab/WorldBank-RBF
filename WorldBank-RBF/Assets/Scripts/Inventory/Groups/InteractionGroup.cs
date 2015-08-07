using UnityEngine;
using System.Collections;

/// <summary>
/// Contains interactions.
/// </summary>
public class InteractionGroup : ItemGroup<InteractionItem> {
	
	public override string ID { get { return "interactions"; } }

	/// <summary>
	/// Sets the interaction count on visiting the city for the first time
	/// </summary>
	public void SetInteractions (string citySymbol) {
		Set (DataManager.GetCityInfo (citySymbol).npc_interactions[0]);
	}

	/// <summary>
	/// Sets the interaction count on spending an extra day in the city
	/// </summary>
	public void SetExtraInteractions (string citySymbol) {
		Set (DataManager.GetCityInfo (citySymbol).npc_interactions[1]);
	}
}
