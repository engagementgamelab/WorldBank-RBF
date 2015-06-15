using UnityEngine;
using System.Collections;

public class PlayerFormEvent : GameEvent {

	public readonly string error;

	public PlayerFormEvent(string strError) {
		this.error = strError;
	}

}
