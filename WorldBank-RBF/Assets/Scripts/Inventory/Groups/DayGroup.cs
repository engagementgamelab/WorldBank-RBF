using UnityEngine;
using System.Collections;

/// <summary>
/// Contains days.
/// </summary>
public class DayGroup : ItemGroup<DayItem> {
	
	public override string ID { get { return "days"; } }
	
	public DayGroup () {}

	/// <summary>
	/// Sets the initial number of days.
	/// </summary>
	public DayGroup (int startCount) {
		Add (startCount);
	}
}
