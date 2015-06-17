using UnityEngine;
using System.Collections;

public class PlayerLoginEvent : GameEvent {

	public readonly bool success;
	public readonly string error;

	public PlayerLoginEvent(bool hasSuccess, string strError=null) {
		
		this.success = hasSuccess;
		this.error = strError;

	}

}
