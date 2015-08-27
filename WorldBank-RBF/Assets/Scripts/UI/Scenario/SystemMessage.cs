using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SystemMessage : MonoBehaviour {

	public Text content;
	public string Content {
		get { return content.text; }
		set { content.text = value; }
	}
}
