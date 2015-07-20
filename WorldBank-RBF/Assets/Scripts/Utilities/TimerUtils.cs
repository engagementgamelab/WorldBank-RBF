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

	    private Timer aTimer;

	    private GameEvent instanceCallback;
		private static System.Random random = new System.Random();

		private int currentCooldown = 0;
		private int elapsedSeconds = 0;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="Cooldown"/> class, given a double.
	    /// </summary>
		public Cooldown() {	}

		public int Init(int[] cooldowns, GameEvent callback) {

			elapsedSeconds = 0;

			if(cooldowns.Length == 1)
			    currentCooldown = cooldowns[0];
	        else
				currentCooldown = cooldowns[random.Next(0, cooldowns.Length)];

	        instanceCallback = callback;
	        
			aTimer = new Timer(1000);

			aTimer.AutoReset = true;
	        aTimer.Elapsed += OnTimedEvent;
	        aTimer.Enabled = true;

			Debug.Log("Timer Started with cooldown of " + currentCooldown + "s");

			// Get determined cooldown
			return currentCooldown;
		}

		public void Stop() {

			Debug.Log("Timer Stopped");

			aTimer.Stop();

		}

		public void Pause() {

			Debug.Log("Timer Paused");

			// aTimer.Enabled = false;

		}

		public void Resume() {

			Debug.Log("Timer Resumed");

			aTimer.Enabled = true;

		}

		public void Restart() {

			Debug.Log("Timer Restarted");

			aTimer.Stop();
			aTimer.Start();

		}

		private void OnTimedEvent(object sender, ElapsedEventArgs eventArgs)
		{

			elapsedSeconds += 1;

			Debug.Log("Timer Tick: " + elapsedSeconds + "s");

			Events.instance.Raise(new GameEvents.TimerTick(elapsedSeconds));

			if(elapsedSeconds == currentCooldown) {

	            try {
	            	Events.instance.Raise(instanceCallback);
	            }
	            catch(Exception e) {
	                throw new Exception("No callback registered for cooldown!");
	            }
				
				aTimer.Stop();
	            
	        }

		}


	}

}