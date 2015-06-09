using UnityEngine;
using System.Collections;

public class PlanTacticItem : InventoryItem {
	
	public override string Name { get { return "Plan Tactic"; } }
	
	public string Title {
		get { return model.title; }
	}

	public string Symbol {
		get { return model.symbol; }
	}

	int priority = -1;
	public int Priority {
		get { return priority; }
		set { priority = value; }
	}

	Models.Unlockable model;

	// <temp>
	Models.Unlockable test1 = new Models.Unlockable ();
	Models.Unlockable test2 = new Models.Unlockable ();
	Models.Unlockable test3 = new Models.Unlockable ();
	Models.Unlockable test4 = new Models.Unlockable ();
	Models.Unlockable test5 = new Models.Unlockable ();
	Models.Unlockable test6 = new Models.Unlockable ();
	// </temp>

	public PlanTacticItem () {}
	public PlanTacticItem (Models.Unlockable model, int testTactic=-1) {
		this.model = model;

		// <temp>
		SetTestModels ();
		switch (testTactic) {
			case 1: this.model = test1; break;
			case 2: this.model = test2; break;
			case 3: this.model = test3; break;
			case 4: this.model = test4; break;
			case 5: this.model = test5; break;
			case 6: this.model = test6; break;
		}
		// </temp>
	}

	// <temp>
	void SetTestModels () {

		// 1
		test1.symbol = "unlockable_incentivise_improvement";
		test1.title = "incentivise improvement in patient/provider relations";
		test1.description = new string[] {
			"Incentivise good customer service (attitudes) through bonuses for workers",
			"More info",
			"More more info"
		};
		test1.type = "professionalism";
		test1.priority = 2;

		// 2
		test2.symbol = "unlockable_incentivise_health";
		test2.title = "Incentivising health providers to follow standard protocols (quality of care)";
		test2.description = new string[] {
			"info",
			"More info",
			"More more info"
		};
		test2.type = "professionalism";
		test2.priority = 1;

		// 3
		test3.symbol = "unlockable_use_ngo";
		test3.title = "Use an NGO to provide services";
		test3.description = new string[] {"pbc option"};
		test3.type = "professionalism";
		test3.priority = 2;

		// 4
		test4.symbol = "unlockable_outreach_campaign";
		test4.title = "Use an NGO to provide services";
		test4.description = new string[] {"pbc option"};
		test4.type = "professionalism";
		test4.priority = 2;

		// 5
		test5.symbol = "unlockable_awareness_campaign";
		test5.title = "Use an NGO to provide services";
		test5.description = new string[] {"pbc option"};
		test5.type = "professionalism";
		test5.priority = 3;
		
		// 6
		test6.symbol = "unlockable_voucher_management";
		test6.title = "Use an NGO to provide services";
		test6.description = new string[] {"pbc option"};
		test6.type = "professionalism";
		test6.priority = 2;
	}
	// </temp>
}

