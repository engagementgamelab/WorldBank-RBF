using UnityEngine;
using System.Collections;

public class ReleaseEvent : GameEvent {

	public readonly ReleaseSettings releaseSettings;

	public ReleaseEvent (ReleaseSettings releaseSettings) {
		this.releaseSettings = releaseSettings;
	}
}
