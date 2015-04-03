using UnityEngine;
using System.Collections;

public class ObjectGeneratorInfoResult : Regenerator<ObjectGeneratorInfo> {

	[SerializeField]
	ObjectGeneratorInfo Result;

	public override bool UpdateResult (ObjectGeneratorInfo info) {
		if (!info.Equals (Result)) {
			Result = info.Clone ();
			return true;
		}
		return false;
	}
}
