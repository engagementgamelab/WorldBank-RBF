using UnityEngine;
using System.Collections.Generic;

public class TacticsContainer : DragLocation {

	public Vector3 BottomTacticPosition {
		get { return tactics.Count > 0 ? tactics[tactics.Count-1].Position : Vector3.zero; }
	}

	public Vector3 Top {
		get { 
			return tactics.Count > 0 || Transform.childCount == 0 
				? Vector3.zero
				: Transform.GetChild (0).position;
		}
	}

	readonly List<Tactic> tactics = new List<Tactic> ();

	void OnEnable () {
		Clear ();
		PlayerData.TacticGroup.onUnlock += OnUnlock;
		foreach (TacticItem item in PlayerData.TacticGroup.Items) {
			if (item.Unlocked && item.Priority == -1)
				OnUnlock<TacticItem> (item);
		}
	}

	void OnDisable () {
		PlayerData.TacticGroup.onUnlock -= OnUnlock;
	}

	public void AddTactic (Tactic tactic, TacticItem item, int atIndex=-1) {
		tactic.Init (item);
		tactic.Parent = Transform;
		if (atIndex != -1)
			tactic.Transform.SetSiblingIndex (atIndex);
		UpdateIndices ();
		tactic.Transform.Reset ();
	}

	void OnUnlock<T> (T item) where T : TacticItem {
		Tactic t = ObjectPool.Instantiate<Tactic> ();
		AddTactic (t, item);
	}

	public void UpdateIndices () {
		tactics.Clear ();
		int index = 0;
		foreach (Transform child in Transform) {
			Tactic tactic = child.GetScript<Tactic> ();
			if (tactic != null) {
				tactics.Add (tactic);
				tactic.Index = index;
				index ++;
			}
		}	
	}

	void Clear () {
		ObjectPool.DestroyAll<Tactic> ();
	}
}
