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
	public readonly string eventSymbol;
	public readonly float[] cooldown;

	public static readonly string FEEDBACK = "feedback";
	public static readonly string NEXT = "next";
	public static readonly string NEXT_YEAR = "next_year";
	public static readonly string PROBLEM_QUEUE = "problem_queue";
	public static readonly string MONTH_END = "month_end";
	public static readonly string DECISION_SELECTED = "decision_selected";
	public static readonly string AFFECT_USED = "affect_used";

	public ScenarioEvent (string strType, string strSymbol=null, float[] floatCooldowns=null) {
		this.eventType = strType;
		this.eventSymbol = strSymbol;
		this.cooldown = floatCooldowns;
	}

}