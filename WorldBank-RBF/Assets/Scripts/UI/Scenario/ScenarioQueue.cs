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

//////
	static List<Models.TacticCard> tacticCards = new List<Models.TacticCard>();

	public static Models.TacticCard[] Tactics {
		get {
			return tacticCards.ToArray();
		}
	}

	public static void AddTacticCard(Models.TacticCard card) {

		tacticCards.Add(card);

	}

	public static void RemoveTacticCard(Models.TacticCard card) {

		if(tacticCards.Contains(card))
			tacticCards.Remove(card);

	}
}