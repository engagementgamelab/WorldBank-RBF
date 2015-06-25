using UnityEngine;
using System.Collections;

public class InteractionsManager : MonoBehaviour {

	static InteractionsManager instance = null;
	static public InteractionsManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (InteractionsManager)) as InteractionsManager;
			}
			return instance;
		}
	}

	public bool HasInteractions {
		get { 
			bool has = !interactions.Empty;
			if (!has) interactionsCounter.Blink ();
			return has;
		}
	}

	public InteractionsCounter interactionsCounter;

	Inventory inventory = new Inventory ();
	InteractionGroup interactions = new InteractionGroup ();

	void Awake () {
		inventory.Add (interactions);
		interactions.onUpdate += UpdateCount;
	}

	public void OnTravelToCity () {
		interactions.Set (0);
	}

	public void OnVisitCity (string citySymbol) {
		interactions.Set (DataManager.GetCityInfo (citySymbol).npc_interactions);
	}

	public void OnStayExtraDay (string citySymbol) {
		interactions.Set (GetExtraDayInteractionCount (citySymbol));
	}

	public void RemoveInteraction () {
		if (!HasInteractions)
			throw new System.Exception ("out of interactions");
		interactions.Remove ();
	}

	void UpdateCount () {
		interactionsCounter.Count = interactions.Count;
	}

	int GetExtraDayInteractionCount (string citySymbol) {
		return DataManager.GetCityNPCCount (citySymbol) - DataManager.GetCityInfo (citySymbol).npc_interactions;
	}

	#if DEBUG
	void OnGUI () {
		if (GUILayout.Button ("0 interactions")) {
			interactions.Set (0);
			UpdateCount ();
		}
	}
	#endif
}
