/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ScenarioQueue.cs
 Phase two scenario cards queue.

 Created by Johnny Richardson on 7/27/15.
==============
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenarioQueue : MonoBehaviour {

	static List<Models.ScenarioCard> problemCards = new List<Models.ScenarioCard>();

	public static Models.ScenarioCard[] Problems {
		get {
			return problemCards.ToArray();
		}
	}

	public static void AddProblemCard(Models.ScenarioCard card) {

		problemCards.Add(card);

	}

	public static void RemoveProblemCard(Models.ScenarioCard card) {

		if(problemCards.Contains(card))
			problemCards.Remove(card);

	}

	public static void Clear() {

		problemCards.Clear();

	}

//////
	static List<TacticCardDialog> tacticCards = new List<TacticCardDialog>();

	public static TacticCardDialog[] Tactics {
		get {
			return tacticCards.ToArray();
		}
	}

	public static void AddTacticCard(TacticCardDialog card) {

		tacticCards.Add(card);

	}

	public static void RemoveTacticCard(int cardIndex) {

		tacticCards.RemoveAt(cardIndex);

	}
	
}