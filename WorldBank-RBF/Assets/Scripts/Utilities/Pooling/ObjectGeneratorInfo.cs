using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ObjectGeneratorInfo {

	public QuadImage quadImage;
	public int imageCount = 2;

	public bool Equals (ObjectGeneratorInfo info) {
		if (info == null) {
			return false;
		}
		return info.imageCount == imageCount;
	}

	public ObjectGeneratorInfo Clone () {
		return (ObjectGeneratorInfo)this.MemberwiseClone ();
	}
}
