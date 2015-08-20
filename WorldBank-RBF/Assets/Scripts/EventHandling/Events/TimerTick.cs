using UnityEngine;
using System.Collections;

namespace GameEvents {
	public class TimerTick : GameEvent {

		public readonly float SecondsElapsed; 
		public readonly string Symbol;

		public TimerTick(float seconds, string timerSymbol="timer") {
			this.SecondsElapsed = seconds;
			this.Symbol = timerSymbol;
		}

	}
}