using UnityEngine;
using System.Collections;

public class ClickEvent : GameEvent {
	
	public readonly ClickSettings clickSettings;

	public ClickEvent (ClickSettings clickSettings) {
		this.clickSettings = clickSettings;
	}
}
