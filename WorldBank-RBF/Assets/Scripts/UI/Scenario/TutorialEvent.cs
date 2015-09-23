/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 TutorialEvent.cs
 Tutorial events (tooltip buttons).

 Created by Johnny Richardson on 9/23/15.
==============
*/
public class TutorialEvent : GameEvent {

	public readonly string eventType;

	public TutorialEvent (string strType) {
		this.eventType = strType;
	}

}