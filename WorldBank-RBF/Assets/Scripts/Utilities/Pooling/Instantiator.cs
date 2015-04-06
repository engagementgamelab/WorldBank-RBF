using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Instantiator : MB {

	public bool refresh = false;
	bool prevRefresh = false;

	public virtual bool ListenForRefresh { 
		get { return true; }
	}

	protected virtual void OnRefresh () {
		Destroy ();
		Instantiate ();
	}

	protected virtual void Instantiate () {}
	protected virtual void Destroy () {}

#if UNITY_EDITOR
	void Update () {
		if (refresh != prevRefresh) {
			Refresher.Instance.Refresh ();
			refresh = prevRefresh;
		}
	}

	void OnEnable () {
		if (ListenForRefresh) {
			Refresher.OnRefresh += OnRefresh;
		}
		OnEditorEnable ();
	}

	void OnDisable () {
		if (ListenForRefresh) {
			Refresher.OnRefresh -= OnRefresh;
		}
		OnEditorDisable ();
	}

	protected virtual void OnEditorEnable () {}
	protected virtual void OnEditorDisable () {}
#endif
}
