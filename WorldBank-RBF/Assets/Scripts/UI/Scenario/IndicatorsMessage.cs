using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IndicatorsMessage : MonoBehaviour {

	public RectTransform vaccinationsPanel;
	public RectTransform birthsPanel;
	public RectTransform qocPanel;

	public Text vaccinationsText;
	public Text birthsText;
	public Text qocText;

	public void Display(Dictionary<string, int> dictAffect) {

		vaccinationsPanel.gameObject.SetActive(dictAffect["indicator_1"] != 0);
		birthsPanel.gameObject.SetActive(dictAffect["indicator_2"] != 0);
		qocPanel.gameObject.SetActive(dictAffect["indicator_3"] != 0);

		vaccinationsText.text = dictAffect["indicator_1"].ToString();
		birthsText.text = dictAffect["indicator_2"].ToString();
		qocText.text = dictAffect["indicator_3"].ToString();
			
	}
}
