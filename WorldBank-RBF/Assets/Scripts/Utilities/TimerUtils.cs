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

	    public System.Timers.Timer aTimer;

	    private GameEvent instanceCallback;
		private static System.Random random = new System.Random();


	    /// <summary>
	    /// Initializes a new instance of the <see cref="Cooldown"/> class, given a double.
	    /// </summary>
		public Cooldown() {	}

		public void Init(int[] cooldowns, GameEvent callback) {

			int currentCooldown = 0;

			if(cooldowns.Length == 1)
			    currentCooldown = cooldowns[0] * 1000;
	        else
				currentCooldown = cooldowns[random.Next(0, cooldowns.Length)] * 1000;

	        instanceCallback = callback;
	        
			aTimer = new System.Timers.Timer(currentCooldown);

	        aTimer.Elapsed += OnTimedEvent;
	        aTimer.Enabled = true;

			Debug.Log("Timer Started with cooldown of " + (currentCooldown / 1000) + "s");
		}

		public void Pause() {

			Debug.Log("Timer Paused");

			aTimer.Enabled = false;

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

		public virtual void OnTimedEvent(object sender, ElapsedEventArgs eventArgs)
		{
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