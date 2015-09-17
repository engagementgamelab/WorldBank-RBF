using UnityEngine;
using UnityEngine.UI;

public class IndicatorsSummary : MonoBehaviour {

	public Text births;
	public Text vaccinations;
	public Text quality;

	int Births {
		set { births.text = "Facility Births: " + value + "%"; }
	}

	int Vaccinations {
		set { births.text = "Vaccinations: " + value + "%"; }
	}

	int Quality {
		set { births.text = "Quality of Care: " + value + "%"; }
	}

	public void SetIndicators (int births, int vaccinations, int quality) {
		Births = births;
		Vaccinations = vaccinations;
		Quality = quality;
	}
}
