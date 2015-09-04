using UnityEngine;
using UnityEngine.EventSystems;

public class TacticsContainer : MB {

	void Awake () {
		PlayerData.TacticGroup.onUnlock += OnUnlock;
	}

	void OnUnlock<T> (T item) where T : TacticItem {
		Tactic t = ObjectPool.Instantiate<Tactic> ();
		t.Init (item);
		t.Parent = Transform;
		t.Transform.Reset ();
	}
}
