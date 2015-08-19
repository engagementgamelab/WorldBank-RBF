using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdvisorMessage : PortraitTextBox {

	public Text responseText;

	bool initial = false;

	public string Content {
		get { return responseText.text; }
		set { responseText.text = value; }
	}
}
