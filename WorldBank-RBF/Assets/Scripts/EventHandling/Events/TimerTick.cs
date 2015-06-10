using UnityEngine;
using System.Collections;

namespace GameEvents {
	public class TimerTick : GameEvent {

	public readonly int secondsElapsed;

		public TimerTick(int seconds) {
			this.secondsElapsed = seconds;
		}

	}
}