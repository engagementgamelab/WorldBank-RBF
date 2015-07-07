using UnityEngine;
using System.Collections;

public class InteractionGroup : ItemGroup<InteractionItem> {
	
	public override string Name { get { return "Interactions"; } }

	public void SetExtraInteractions (string citySymbol) {
		Set (DataManager.GetCityNPCCount (citySymbol) - DataManager.GetCityInfo (citySymbol).npc_interactions);
	}
}
