/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 TacticsEvent.cs
 Phase two tactics events.

 Created by Johnny Richardson on 8/10/15.
==============
*/

public class TacticsEvent : ScenarioEvent {

	public static readonly string TACTIC_OPEN = "tactic_open";
	public static readonly string TACTIC_INVESTIGATE = "investigate";
	public static readonly string TACTIC_INVESTIGATE_FURTHER = "investigate_further";
	public static readonly string TACTIC_RESULTS = "tactic_results";
	public static readonly string TACTIC_CLOSED = "tactic_closed";

	public TacticsEvent (string strType, string strSymbol=null, int[] intCooldowns=null) : base(strType, strSymbol, intCooldowns) {}

}