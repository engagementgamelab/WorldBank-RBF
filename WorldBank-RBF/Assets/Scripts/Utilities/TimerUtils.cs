/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 TimerUtils.cs
 Namespace for timer utility classes.

 Created by Johnny Richardson on 5/28/15.
==============
*/
using UnityEngine;
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;

namespace TimerUtils {
	
    /// <summary>
    /// Create a cooldown
    /// </summary>
	public class Cooldown {

	    Timer aTimer;

	    GameEvent instanceCallback;
		static System.Random random = new System.Random();

		int currentCooldown = 0;
		int elapsedSeconds = 0;

		string currentSymbol;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="Cooldown"/> class, given a double.
	    /// </summary>
		public Cooldown(string strSymbol="timer") {

			currentSymbol = strSymbol;

		}

		public int Init(int[] cooldowns, GameEvent callback, string timerSymbol=null) {

			elapsedSeconds = 0;

			if(timerSymbol != null)
				currentSymbol = timerSymbol;

			if(cooldowns.Length == 1)
			    currentCooldown = cooldowns[0];
	        else
				currentCooldown = cooldowns[random.Next(0, cooldowns.Length)];

	        instanceCallback = callback;
	        
	   //      if(aTimer != null)
				// aTimer.Stop();

			aTimer = new Timer(1000);

			aTimer.AutoReset = true;
	        aTimer.Elapsed += OnTimedEvent;
	        aTimer.Enabled = true;

	        aTimer.Start();

			Debug.Log("Timer Started - " + timerSymbol + " / " + currentCooldown + "s");

			// Get determined cooldown
			return currentCooldown;
		}

		public void Stop() {

			if(aTimer == null) return;

			Debug.Log("Timer Stopped");

			aTimer.Enabled = false;
			aTimer.Stop();
			aTimer.Dispose();

		}

		public void Pause() {

			if(aTimer == null) return;

			Debug.Log("Timer Paused");

			aTimer.Enabled = false;

		}

		public void Resume() {

			if(aTimer == null) return;

			Debug.Log("Timer Resumed");

			aTimer.Enabled = true;

		}

		public void Restart() {

			if(aTimer == null) return;

			Debug.Log("Timer Restarted");

			aTimer.Stop();
			aTimer.Start();

		}

		void OnTimedEvent(object sender, ElapsedEventArgs eventArgs)
		{

			elapsedSeconds++;

			Events.instance.Raise(new GameEvents.TimerTick(elapsedSeconds, currentSymbol));

			if(elapsedSeconds == currentCooldown) {

	            try {

					Debug.Log(currentSymbol + " Timer done");
	            	Events.instance.Raise(instanceCallback);
	            }
	            catch(Exception e) {
	                throw new Exception("No callback registered for cooldown!");
	            }
				
				Stop();
	            
	        }

		}


	}

}