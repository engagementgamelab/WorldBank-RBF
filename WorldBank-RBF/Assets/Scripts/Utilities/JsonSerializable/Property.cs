using UnityEngine;
using System.Collections;


[System.Serializable]
public class Property {
	public virtual object val { get; set; }
}

[System.Serializable]
public class Float : Property {

	public float _val = 0f;
	public override object val {
		get { return _val; }
		set { _val = System.Convert.ToSingle (value); }
	}

	// [HideInInspector] public float min, max;
}

[System.Serializable]
public class Int : Property {

	public int _val = 0;
	public override object val {
		get { return _val; }
		set { _val = (int)value; }
	}
}
