using UnityEngine;
using System.Collections;

public class DayGroup : ItemGroup<DayItem> {
	
	public override string ID { get { return "days"; } }
	
	public DayGroup () {}
	public DayGroup (int startCount) {
		Add (startCount);
	}
}
