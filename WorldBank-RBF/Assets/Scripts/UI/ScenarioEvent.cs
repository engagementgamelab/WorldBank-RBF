/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ScenarioEvent.cs
 Phase two scenario events.

 Created by Johnny Richardson on 6/1/15.
==============
*/
public class ScenarioEvent : GameEvent {

	public readonly string eventType;

	public static readonly string NEXT = "next";
	public static readonly string TACTIC_OPEN = "tactic_open";
	public static readonly string TACTIC_INVESTIGATE = "investigate";
	public static readonly string TACTIC_INVESTIGATE_FURTHER = "investigate_further";
	public static readonly string TACTIC_RESULTS = "tactic_results";
	public static readonly string TACTIC_CLOSED = "tactic_closed";

	public ScenarioEvent (string strType) {
		this.eventType = strType;
	}

}