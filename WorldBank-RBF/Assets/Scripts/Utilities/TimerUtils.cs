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

	    /// <summary>
	    /// Initializes a new instance of the <see cref="Cooldown"/> class, given a double.
	    /// </summary>
		public Cooldown() {	}

		public void Init(double cooldown, GameEvent callback) {

	        aTimer = new System.Timers.Timer(cooldown * 1000);

	        instanceCallback = callback;

	        aTimer.Elapsed += OnTimedEvent;
	        aTimer.Enabled = true;

		}

		public void Pause() {

			Debug.Log("Timer Paused");

			aTimer.Enabled = false;

		}

		public void Resume() {

			Debug.Log("Timer Resumed");

			aTimer.Enabled = true;

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
	
    /// <summary>
    /// Create a random cooldown
    /// </summary>
	public class RandomCooldown : Cooldown {

		private static System.Random random = new System.Random();

	    /// <summary>
	    /// Initializes a new instance of the <see cref="RandomCooldown"/> class, given a set of ints.
	    /// </summary>
	    /// <param name="possibleIntervals">A set of possible ints for a cooldown, in seconds.</param>
	    /// <param name="callback">The callback to run at the end of the cooldown.</param>
		public RandomCooldown(int[] possibleIntervals, Func<bool> callback=null) : base() {

			// TODO: change to TimerEvent
			Init(possibleIntervals[random.Next(0, possibleIntervals.Length)], new ScenarioEvent(ScenarioEvent.COOLDOWN));

		}

	}

}