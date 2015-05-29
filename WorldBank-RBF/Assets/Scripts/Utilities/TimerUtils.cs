
using UnityEngine;
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;

namespace TimerUtils {
	
	public class RandomCooldown {

	    private static System.Timers.Timer aTimer;
		private static System.Random random = new System.Random();

		private Action instanceCallback;

		public RandomCooldown(int[] possibleIntervals, Action callback) {

	        aTimer = new System.Timers.Timer(possibleIntervals[random.Next(0, possibleIntervals.Length)] * 1000);

	        instanceCallback = callback;

	        aTimer.Elapsed += OnTimedEvent;
	        aTimer.Enabled = true;

		}

		public void Pause() {

			aTimer.Enabled = false;

		}

		public void Resume() {

			aTimer.Enabled = true;

		}

		private void OnTimedEvent(object sender, ElapsedEventArgs eventArgs)
		{

            try {
       			instanceCallback();
            }
            catch(Exception e) {
                throw new Exception("No callback registered for RandomCooldown()!");
            }

		}

	}

}