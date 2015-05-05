using UnityEngine;
using System.Collections;

public class CityGroup : ItemGroup<CityItem> {
	public override string Name { get { return "Cities"; } }
}
