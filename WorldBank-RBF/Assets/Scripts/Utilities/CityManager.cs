using UnityEngine;
using System.Collections;

public class CityManager {

	public static void SetCity (string citySymbol) {
		DataManager.SetSceneContext (citySymbol);
		NpcManager.InitNpcs ();
	}
}
