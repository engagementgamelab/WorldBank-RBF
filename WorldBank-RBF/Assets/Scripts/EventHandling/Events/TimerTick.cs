using UnityEngine;
using System.Collections;

namespace GameEvents {
	public class TimerTick : GameEvent {

		public readonly int SecondsElapsed; 
		public readonly string Symbol;

		public TimerTick(int seconds, string timerSymbol="timer") {
			this.SecondsElapsed = seconds;
			this.Symbol = timerSymbol;
		}

	}
}