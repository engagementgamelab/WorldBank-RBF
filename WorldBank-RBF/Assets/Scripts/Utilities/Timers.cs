/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 Timers.cs
 Namespace for timer utility classes.

 Created by Johnny Richardson on 5/28/15.
==============
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timers : MonoBehaviour {

	public delegate void OnEnd();
	public delegate void OnTick(GameEvents.TimerTick e);

	#region Static registers
	public static List<TimerInstance> AllTimers = new List<TimerInstance>();
	public static List<TimerInstance> TimersToAdd = new List<TimerInstance>();

	public static TimerInstance StartTimer(GameObject target, float[] possibleDurations) {
		Timers obj = target.GetComponent<Timers>();
		
		if (obj == null) {
			obj = target.AddComponent<Timers>();
		}
		
		TimerInstance newTimer = obj.AddTimer(possibleDurations);

		TimersToAdd.Add(newTimer);

		return newTimer;
	}
	#endregion

    public class TimerInstance {
    	public string Symbol { 
    		get {
    			return symbol; 
    		}
    		set {
    			symbol = value;
    		} 
    	}

        public float Seconds {
    		get {
    			return elapsedSeconds; 
    		}
    		set {
    			elapsedSeconds = value;
    		} 
    	}

        public float Duration {
    		get {
    			return durationSeconds; 
    		}
    		set {
    			durationSeconds = value;
    		} 
    	}

        public bool Active { 
    		get {
    			return isActive; 
    		}
    		set {
    			isActive = value;
    		} 
    	}

		/// <summary>
		/// Called when the timer ends
		/// </summary>
		public OnEnd onEnd;

		/// <summary>
		/// Called when the timer ticks
		/// </summary>
		public OnTick onTick;

		public void Stop() {
			isActive = false;
		}

		public void Resume() {
			isActive = true;
		}

		public void Restart() {
			Seconds = 0f;
		}

		public float GetCurrentTime() {
			return Seconds;
		}

		public string GetCurrentTimeString() {
			return Seconds.ToString("00.00");
		}

		public bool IsRunning() {
			return Active;
		}

        protected string symbol = "Timer";
        
        protected float elapsedSeconds = 0f;
        protected float durationSeconds = 0f;

        protected bool isActive = false;
    }

	System.Random random = new System.Random();

	public TimerInstance AddTimer(float[] possibleDurations) {
		float duration = 0f;

		if (possibleDurations.Length == 1)
			duration = possibleDurations[0];
		else	
			duration = possibleDurations[random.Next(0, possibleDurations.Length)];

		TimerInstance newTimer = new TimerInstance();
		newTimer.Duration = duration;
		newTimer.Active = true;

		return newTimer;
	}

	// Update is called once per frame
	void Update() {

		foreach(TimerInstance timerInst in AllTimers) {
			
			if (timerInst.IsRunning()) {
				timerInst.Seconds += Time.deltaTime;

				if(timerInst.onTick != null)
					timerInst.onTick(new GameEvents.TimerTick(timerInst.Seconds, timerInst.Symbol));

				if (timerInst.Seconds >= timerInst.Duration) {
					// Debug.Log("TIMER " + timerInst.Symbol + " END");
					timerInst.onEnd();
				}
			}

		}

	}

	// We use late update to add any new timers that have been queued, so we don't hit enumeration issues
	void LateUpdate() {

		foreach(TimerInstance timerInst in TimersToAdd)
			AllTimers.Add(timerInst);

		TimersToAdd.Clear();

	}

}