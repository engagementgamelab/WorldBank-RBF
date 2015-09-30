using UnityEngine;
using System.Collections;

public class SteveImage : MB {

	float speed = 2f;

	void OnEnable () {
		speed = Random.Range (1f, 6f);
	}

	void Update () {
		Transform.SetLocalEulerAnglesZ (Transform.localEulerAngles.z + speed);
	}
}
