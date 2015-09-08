using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TacticsContainer : MB {

	readonly List<Tactic> tactics = new List<Tactic> ();

	void Start () {
		PlayerData.TacticGroup.onUnlock += OnUnlock;
		Events.instance.AddListener<DropTacticEvent> (OnDropTacticEvent);
	}

	public void AddTactic (Tactic tactic, TacticItem item) {
		tactic.Init (item, this);
		tactic.Parent = Transform;
		UpdateIndices ();
		tactic.Transform.Reset ();
		tactics.Add (tactic);
	}

	void OnUnlock<T> (T item) where T : TacticItem {
		Tactic t = ObjectPool.Instantiate<Tactic> ();
		AddTactic (t, item);
		/*t.Init (item, this);
		t.Parent = Transform;
		UpdateIndices ();
		t.Transform.Reset ();
		tactics.Add (t);*/
	}

	void UpdateIndices () {
		int index = 0;
		foreach (Transform child in Transform) {
			Tactic tactic = child.GetScript<Tactic> ();
			if (tactic != null) {
				tactic.Index = index;
				index ++;
			}
		}	
	}

	void OnDropTacticEvent (DropTacticEvent e) {
		UpdateIndices ();
	}
}
