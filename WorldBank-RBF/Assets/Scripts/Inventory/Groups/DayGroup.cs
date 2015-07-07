using UnityEngine;
using System.Collections;

public class DayGroup : ItemGroup<DayItem> {
	
	public override string Name { get { return "Days"; } }
	
	public DayGroup () {}
	public DayGroup (int startCount) {
		Add (startCount);
	}
}
