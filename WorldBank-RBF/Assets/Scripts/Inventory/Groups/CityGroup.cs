using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGroup : ItemGroup<CityItem> {
	
	public override string Name { get { return "Cities"; } }

	public void AddUnique (string cityName) {
		if (!HasCity (cityName)) {
			Add (new CityItem (cityName));
		}
	}

	public bool HasCity (string cityName) {
		return Items.ConvertAll (x => x.Name).Contains (cityName);
	}
}
