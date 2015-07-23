using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorColumn : MonoBehaviour {

	public Text txtVaccinations;
	public Text txtFacility;
	public Text txtQoC;

	public string Vaccinations {

		set {
			txtVaccinations.text = value;
		}

	}

	public string FacilityBirths {

		set {
			txtFacility.text = value;
		}

	}

	public string QualityOfCare {

		set {
			txtQoC.text = value;
		}

	}

}
