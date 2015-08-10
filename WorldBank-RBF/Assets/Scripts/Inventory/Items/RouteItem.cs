using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// A route based on a data model.
/// </summary>
public class RouteItem : ModelItem {

	public override string Name { get { return "Route"; } }

	/// <summary>
	/// Gets the Terminals (the two cities that this route connects).
	/// </summary>
	public Terminals Terminals { get; private set; }

	/// <summary>
	/// Gets the cost to travel along the route.
	/// </summary>
	public int Cost { get; private set; }

	List<Vector3> positions;
	public List<Vector3> Positions {
		get {
			if (positions == null) {
				positions = new RouteLines ().GetPositions (Terminals);
			}
			return positions;
		}
	}

	RouteGroup routeGroup;
	Models.Route routeModel;

	/// <summary>
	/// On initialization, sets the terminals and cost based on the given data model.
	/// </summary>
	public override void OnInit () {

		routeGroup = Group as RouteGroup;
		
		if (routeGroup != null) {
			routeModel = Array.Find (routeGroup.RouteModels, x => x.symbol == Symbol.Substring (17));
			Terminals = new Terminals (routeModel.city1, routeModel.city2);
			Cost = routeModel.cost;
		}
	}
}

public class RouteLines {

	Dictionary<Terminals, List<Vector3>> routeLines;
	public Dictionary<Terminals, List<Vector3>> Positions {
		get {
			if (routeLines == null) {
				routeLines = new Dictionary<Terminals, List<Vector3>> ();
				routeLines.Add (new Terminals ("capitol", "crup"), new List<Vector3> () {
					new Vector3 (286.8047f, 349.9531f, 0),
					new Vector3 (291.9336f, 386.2773f, 0),
					new Vector3 (297.1055f, 401.2305f, 0),
					new Vector3 (305.4023f, 407.4453f, 0),
					new Vector3 (320.3516f, 419.6094f, 0),
					new Vector3 (325.4766f, 427.4766f, 0),
					new Vector3 (323.4766f, 434.3008f, 0),
					new Vector3 (319.0039f, 439.082f, 0),
					new Vector3 (309.1367f, 449.9492f, 0),
					new Vector3 (329.125f, 476.5f, 0),
					new Vector3 (344.5078f, 491.4453f, 0),
					new Vector3 (354.8477f, 497.3125f, 0),
					new Vector3 (365.2344f, 499.3984f, 0),
					new Vector3 (386.4844f, 497.3555f, 0),
					new Vector3 (408.4766f, 511.1328f, 0)
				});
				routeLines.Add (new Terminals ("kibari", "crup"), new List<Vector3> () {
					new Vector3 (557.5898f, 574.875f, 0),
					new Vector3 (537.6445f, 540.543f, 0),
					new Vector3 (523.5625f, 514.4258f, 0),
					new Vector3 (503.4883f, 482.6172f, 0),
					new Vector3 (473.375f, 442.6875f, 0),
					new Vector3 (475.1133f, 435.168f, 0),
					new Vector3 (483.7188f, 427.9102f, 0)
				});
				routeLines.Add (new Terminals ("kibari", "zima"), new List<Vector3> () {
					new Vector3 (577.7734f, 415.5742f, 0),
					new Vector3 (643.9531f, 428.6094f, 0),
					new Vector3 (642.9102f, 418.3125f, 0),
					new Vector3 (640.1289f, 405.1445f, 0),
					new Vector3 (655.2969f, 394.1914f, 0),
					new Vector3 (694.2813f, 397.3203f, 0),
					new Vector3 (702.5352f, 394.7109f, 0),
					new Vector3 (731.0039f, 389.582f, 0)
				});
				routeLines.Add (new Terminals ("mile", "zima"), new List<Vector3> () {
					new Vector3 (739.0352f, 370.1211f, 0),
					new Vector3 (728.0391f, 346.6992f, 0),
					new Vector3 (719.1328f, 334.0078f, 0),
					new Vector3 (707.7031f, 326.8398f, 0),
					new Vector3 (689.7148f, 318.9297f, 0),
					new Vector3 (653.6055f, 305.4141f, 0),
					new Vector3 (640.7852f, 299.5508f, 0),
					new Vector3 (627.6641f, 292.25f, 0),
					new Vector3 (617.3203f, 281.9922f, 0),
					new Vector3 (612.4102f, 269.8711f, 0),
					new Vector3 (611.3672f, 257.7891f, 0),
					new Vector3 (611.3672f, 245.793f, 0),
					new Vector3 (606.2422f, 234.8438f, 0),
					new Vector3 (592.9414f, 223.543f, 0),
					new Vector3 (566.4766f, 204.332f, 0),
					new Vector3 (549.4414f, 188.8633f, 0),
					new Vector3 (537.0156f, 174.7813f, 0),
					new Vector3 (527.6719f, 160.3125f, 0),
					new Vector3 (522.457f, 145.4063f, 0)
				});
				routeLines.Add (new Terminals ("mile", "malcom"), new List<Vector3> () {
					new Vector3 (462.1836f, 55.03516f, 0),
					new Vector3 (451.8828f, 45.86328f, 0),
					new Vector3 (436.6758f, 36.95703f, 0),
					new Vector3 (421.2031f, 32.87109f, 0),
					new Vector3 (402.0859f, 32.87109f, 0),
					new Vector3 (386.6172f, 38f, 0),
					new Vector3 (366.582f, 46.25391f, 0),
					new Vector3 (337.0391f, 65.11719f, 0),
					new Vector3 (324.3906f, 74.67969f, 0),
					new Vector3 (315.3516f, 79.15234f, 0),
					new Vector3 (307.5273f, 81.15234f, 0),
					new Vector3 (298.5352f, 81.15234f, 0),
					new Vector3 (287.5391f, 79.06641f, 0),
					new Vector3 (275.457f, 76.33203f, 0),
					new Vector3 (262.0313f, 75.33203f, 0),
					new Vector3 (247.6445f, 74.33203f, 0),
					new Vector3 (231.6563f, 76.5f, 0),
					new Vector3 (219.3984f, 81.58594f, 0),
					new Vector3 (206.9297f, 90.23438f, 0),
					new Vector3 (197.3242f, 100.2305f, 0),
					new Vector3 (188.4141f, 109.1797f, 0),
					new Vector3 (176.7227f, 119.4375f, 0),
					new Vector3 (167.207f, 128.5664f, 0),
					new Vector3 (160.2969f, 135.3047f, 0),
					new Vector3 (160.6445f, 142.8242f, 0),
					new Vector3 (162.9922f, 148.9961f, 0),
					new Vector3 (169.1211f, 156.5547f, 0)
				});
				routeLines.Add (new Terminals ("valeria", "malcom"), new List<Vector3> () {
					new Vector3 (169.8047f, 184.8086f, 0),
					new Vector3 (193.625f, 221.9648f, 0),
					new Vector3 (212.4844f, 245.3867f, 0),
					new Vector3 (217.9609f, 260.8125f, 0),
					new Vector3 (218.0039f, 273.1992f, 0),
					new Vector3 (212.918f, 285.543f, 0),
					new Vector3 (204.7031f, 297.5352f, 0),
					new Vector3 (194.6172f, 307.793f, 0),
					new Vector3 (177.4102f, 320.2227f, 0),
					new Vector3 (71.51563f, 379.3203f, 0),
					new Vector3 (57.47656f, 390.5742f, 0),
					new Vector3 (46.39453f, 403.1328f, 0),
					new Vector3 (38.875f, 417.5625f, 0),
					new Vector3 (32.79297f, 434.6836f, 0),
					new Vector3 (31.05469f, 454.1992f, 0),
					new Vector3 (33.48828f, 469.4922f, 0),
					new Vector3 (38.61328f, 484.9609f, 0),
					new Vector3 (45.4375f, 496f, 0),
					new Vector3 (55.69531f, 505.2578f, 0),
					new Vector3 (66.60547f, 512.0391f, 0),
					new Vector3 (82.72656f, 521.6016f, 0)
				});
				routeLines.Add (new Terminals ("kibari", "malcom"), new List<Vector3> () {
					new Vector3 (229.6367f, 190.6758f, 0),
					new Vector3 (225.8125f, 209.1484f, 0),
					new Vector3 (230.9844f, 219.3633f, 0),
					new Vector3 (241.0195f, 230.0078f, 0),
					new Vector3 (287.8203f, 248.0859f, 0),
					new Vector3 (312.6758f, 256.2969f, 0),
					new Vector3 (329.3164f, 267.6836f, 0),
					new Vector3 (359.6055f, 290.0195f, 0),
					new Vector3 (375.2969f, 302.2773f, 0),
					new Vector3 (392.8086f, 319.0547f, 0),
					new Vector3 (412.8008f, 339.3906f, 0),
					new Vector3 (434.875f, 363.4648f, 0),
					new Vector3 (440.0039f, 374.1133f, 0),
					new Vector3 (447.9531f, 385.6758f, 0),
					new Vector3 (454.8633f, 391.457f, 0),
					new Vector3 (471.0742f, 402.1875f, 0)
				});
				routeLines.Add (new Terminals ("kibari", "mile"), new List<Vector3> () {
					new Vector3 (472.1328f, 402.3594f, 0),
					new Vector3 (458.3555f, 384.5391f, 0),
					new Vector3 (447.707f, 366.332f, 0),
					new Vector3 (447.707f, 352.293f, 0),
					new Vector3 (450.4453f, 337.1289f, 0),
					new Vector3 (454.8359f, 301.4961f, 0),
					new Vector3 (457.9648f, 286.0273f, 0),
					new Vector3 (454.9219f, 271.8164f, 0),
					new Vector3 (451.1836f, 251.5234f, 0),
					new Vector3 (443.0117f, 234.0078f, 0),
					new Vector3 (434.0195f, 213.8906f, 0),
					new Vector3 (441.8867f, 197.418f, 0),
					new Vector3 (437.4102f, 184.6875f, 0),
					new Vector3 (440.1484f, 164.6953f, 0),
					new Vector3 (420.418f, 143.5352f, 0),
					new Vector3 (415.2461f, 130.7188f, 0),
					new Vector3 (411.1211f, 113.9844f, 0),
					new Vector3 (425.1992f, 104.9063f, 0),
					new Vector3 (430.9375f, 99.47266f, 0),
					new Vector3 (442.3672f, 82.26172f, 0)
				});
				routeLines.Add (new Terminals ("valeria", "zima"), new List<Vector3> () {
					new Vector3 (92.57422f, 525.8203f, 0f),
					new Vector3 (743.375f, 377.2188f, 0f)
				});
				routeLines.Add (new Terminals ("capitol", "malcom"), new List<Vector3> () {
					new Vector3 (287.9102f, 329.9727f, 0f),
					new Vector3 (297.082f, 333.0156f, 0f),
					new Vector3 (310.4688f, 334.0156f, 0f),
					new Vector3 (320.418f, 331.9297f, 0f),
					new Vector3 (332.1523f, 326.8438f, 0f),
					new Vector3 (341.8828f, 318.3242f, 0f),
					new Vector3 (343.9258f, 309.6758f, 0f),
					new Vector3 (343.5781f, 297.7695f, 0f),
					new Vector3 (339.5352f, 292.7266f, 0f),
					new Vector3 (332.6289f, 285.9453f, 0f),
					new Vector3 (324.6758f, 279.3398f, 0f),
					new Vector3 (313.6797f, 274.168f, 0f),
					new Vector3 (296.125f, 267.2617f, 0f),
					new Vector3 (267.793f, 257.9609f, 0f),
					new Vector3 (248.0273f, 249.0078f, 0f),
					new Vector3 (228.6055f, 236.8438f, 0f),
					new Vector3 (214.7422f, 223.0664f, 0f),
					new Vector3 (207.5273f, 212.8086f, 0f),
					new Vector3 (205.1367f, 202.7305f, 0f),
					new Vector3 (205.832f, 195.5156f, 0f),
					new Vector3 (206.1367f, 191.5586f, 0f),
					new Vector3 (211.0039f, 181.3008f, 0f)
				});
			}
			return routeLines;
		}
	}

	public List<Vector3> GetPositions (Terminals terminals) {
		try {
			return Positions.SingleOrDefault (x => x.Key == terminals).Value;
		} catch {
			throw new System.Exception ("No route between " + terminals.city1 + " and " + terminals.city2 + " exists.");
		}
	}
}