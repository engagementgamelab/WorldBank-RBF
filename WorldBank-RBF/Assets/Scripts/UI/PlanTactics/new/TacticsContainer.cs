using UnityEngine;

public class TacticsContainer : DragLocation {

	void Start () {
		PlayerData.TacticGroup.onUnlock += OnUnlock;
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
		int index = 0;
		foreach (Transform child in Transform) {
			Tactic tactic = child.GetScript<Tactic> ();
			if (tactic != null) {
				tactic.Index = index;
				index ++;
			}
		}	
	}
}
