using UnityEngine;
using System;
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

	/// <summary>
	/// Gets the speed of travel along the route.
	/// </summary>
	public float Speed { get; private set; }

	/// <summary>
	/// Gets the mode of transportation along the route.
	/// </summary>
	public string TransportationMode { get; private set; }

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
			Speed = routeModel.speed;
			TransportationMode = routeModel.transportation_mode;
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
					new Vector3 (-85.16341f, 50.01354f, 0f),
					new Vector3 (-84.73814f, 62.32241f, 0f),
					new Vector3 (-82.21521f, 73.69712f, 0f),
					new Vector3 (-78.44036f, 84.17113f, 0f),
					new Vector3 (-73.01701f, 88.58148f, 0f),
					new Vector3 (-68.60426f, 92.33005f, 0f),
					new Vector3 (-63.1809f, 96.05234f, 0f),
					new Vector3 (-58.98318f, 100.2262f, 0f),
					new Vector3 (-57.14831f, 105.6495f, 0f),
					new Vector3 (-57.14831f, 109.3981f, 0f),
					new Vector3 (-60.04395f, 113.1467f, 0f),
					new Vector3 (-64.4567f, 116.9191f, 0f),
					new Vector3 (-67.38101f, 121.5445f, 0f),
					new Vector3 (-64.90825f, 126.7815f, 0f),
					new Vector3 (-57.57119f, 136.1087f, 0f),
					new Vector3 (-51.91131f, 144.2438f, 0f),
					new Vector3 (-42.66055f, 152.2713f, 0f),
					new Vector3 (-35.53851f, 157.1069f, 0f),
					new Vector3 (-29.15949f, 158.3827f, 0f),
					new Vector3 (-20.89305f, 157.9574f, 0f),
					new Vector3 (-14.14372f, 156.469f, 0f),
					new Vector3 (-8.481453f, 157.5059f, 0f),
					new Vector3 (-3.961193f, 161.4408f, 0f),
					new Vector3 (5.609716f, 169.4421f, 0f)
				});
				routeLines.Add (new Terminals ("kibari", "crup"), new List<Vector3> () {
					new Vector3 (117.6343f, 213.9884f, 0f),
					new Vector3 (114.0983f, 209.1767f, 0f),
					new Vector3 (107.6118f, 198.9177f, 0f),
					new Vector3 (100.4348f, 185.4167f, 0f),
					new Vector3 (93.07388f, 169.892f, 0f),
					new Vector3 (85.94945f, 158.0395f, 0f),
					new Vector3 (75.58534f, 143.2363f, 0f),
					new Vector3 (66.97248f, 132.4709f, 0f),
					new Vector3 (60.27332f, 123.275f, 0f),
					new Vector3 (55.88685f, 116.2844f, 0f),
					new Vector3 (57.16265f, 110.8085f, 0f),
					new Vector3 (58.19955f, 110.1969f, 0f)
				});
				routeLines.Add (new Terminals ("kibari", "zima"), new List<Vector3> () {
					new Vector3 (245.7258f, 77.01166f, 0f),
					new Vector3 (236.7403f, 78.28747f, 0f),
					new Vector3 (231.9022f, 79.35063f, 0f),
					new Vector3 (227.1192f, 79.35063f, 0f),
					new Vector3 (222.3074f, 82.06231f, 0f),
					new Vector3 (213.2167f, 82.06231f, 0f),
					new Vector3 (202.9052f, 81.84968f, 0f),
					new Vector3 (193.2029f, 78.68645f, 0f),
					new Vector3 (188.869f, 81.15921f, 0f),
					new Vector3 (183.2617f, 85.5194f, 0f),
					new Vector3 (180.7889f, 88.65634f, 0f),
					new Vector3 (182.6501f, 96.84155f, 0f),
					new Vector3 (183.2856f, 103.567f, 0f),
					new Vector3 (179.5394f, 105.2418f, 0f),
					new Vector3 (172.974f, 104.6039f, 0f),
					new Vector3 (166.2223f, 102.0786f, 0f),
					new Vector3 (160.5624f, 98.32999f, 0f),
					new Vector3 (153.8895f, 98.94161f, 0f),
					new Vector3 (148.4399f, 98.11736f, 0f),
					new Vector3 (141.5018f, 96.84155f, 0f)
				});
				routeLines.Add (new Terminals ("mile", "zima"), new List<Vector3> () {
					new Vector3 (93.59232f, -101.1263f, 0f),
					new Vector3 (98.13885f, -92.72842f, 0f),
					new Vector3 (105.4735f, -83.74284f, 0f),
					new Vector3 (111.5873f, -75.79416f, 0f),
					new Vector3 (119.8251f, -68.43321f, 0f),
					new Vector3 (130.1653f, -59.39508f, 0f),
					new Vector3 (143.6138f, -49.93407f, 0f),
					new Vector3 (152.5444f, -42.96972f, 0f),
					new Vector3 (157.8316f, -36.88218f, 0f),
					new Vector3 (159.0023f, -28.05907f, 0f),
					new Vector3 (159.0023f, -21.14728f, 0f),
					new Vector3 (160.0655f, -13.97029f, 0f),
					new Vector3 (162.113f, -7.060886f, 0f),
					new Vector3 (167.4025f, -0.5480804f, 0f),
					new Vector3 (174.7396f, 5.885885f, 0f),
					new Vector3 (184.4395f, 10.32252f, 0f),
					new Vector3 (195.3388f, 14.54892f, 0f),
					new Vector3 (210.302f, 20.02484f, 0f),
					new Vector3 (219.8729f, 23.72084f, 0f),
					new Vector3 (228.6434f, 28.08103f, 0f),
					new Vector3 (235.5552f, 31.59068f, 0f),
					new Vector3 (240.527f, 36.77512f, 0f),
					new Vector3 (243.9029f, 40.5213f, 0f),
					new Vector3 (248.2894f, 47.85836f, 0f),
					new Vector3 (249.7253f, 52.9353f, 0f),
					new Vector3 (252.8861f, 59.23547f, 0f)
				});
				routeLines.Add (new Terminals ("mile", "malcom"), new List<Vector3> () {
					new Vector3 (-174.9814f, -100.8212f, 0f),
					new Vector3 (-178.9426f, -104.995f, 0f),
					new Vector3 (-180.0058f, -110.471f, 0f),
					new Vector3 (-178.73f, -115.7056f, 0f),
					new Vector3 (-171.768f, -122.8826f, 0f),
					new Vector3 (-161.3729f, -130.7261f, 0f),
					new Vector3 (-156.5349f, -136.202f, 0f),
					new Vector3 (-150.7125f, -143.9883f, 0f),
					new Vector3 (-143.1389f, -151.5642f, 0f),
					new Vector3 (-136.0145f, -155.5517f, 0f),
					new Vector3 (-128.1757f, -159.0876f, 0f),
					new Vector3 (-116.0006f, -161.1614f, 0f),
					new Vector3 (-105.3976f, -160.5235f, 0f),
					new Vector3 (-94.71097f, -159.8856f, 0f),
					new Vector3 (-83.17618f, -156.7487f, 0f),
					new Vector3 (-74.3244f, -154.6749f, 0f),
					new Vector3 (-67.22626f, -155.2865f, 0f),
					new Vector3 (-60.55577f, -158.7699f, 0f),
					new Vector3 (-53.21871f, -163.6079f, 0f),
					new Vector3 (-41.30883f, -171.9795f, 0f),
					new Vector3 (-28.04669f, -179.8756f, 0f),
					new Vector3 (-19.03723f, -185.2727f, 0f),
					new Vector3 (-8.216797f, -190.2182f, 0f),
					new Vector3 (-0.002920787f, -192.691f, 0f),
					new Vector3 (8.820187f, -193.4889f, 0f),
					new Vector3 (16.63508f, -193.0637f, 0f),
					new Vector3 (24.20866f, -191.3889f, 0f),
					new Vector3 (30.85526f, -188.4383f, 0f),
					new Vector3 (36.78273f, -184.8761f, 0f),
					new Vector3 (41.72825f, -180.8886f, 0f),
					new Vector3 (43.40304f, -178.1506f, 0f)
				});
				routeLines.Add (new Terminals ("valeria", "malcom"), new List<Vector3> () {
					new Vector3 (-173.7661f, -74.73956f, 0f),
					new Vector3 (-166.5079f, -63.0686f, 0f),
					new Vector3 (-157.2332f, -50.57816f, 0f),
					new Vector3 (-146.2575f, -36.09277f, 0f),
					new Vector3 (-141.7922f, -27.82634f, 0f),
					new Vector3 (-139.7447f, -19.23976f, 0f),
					new Vector3 (-140.3563f, -9.379761f, 0f),
					new Vector3 (-143.9711f, -1.591156f, 0f),
					new Vector3 (-150.2426f, 6.4077f, 0f),
					new Vector3 (-158.7503f, 14.38267f, 0f),
					new Vector3 (-166.9905f, 22.54398f, 0f),
					new Vector3 (-178.6567f, 28.60284f, 0f),
					new Vector3 (-196.2503f, 38.51779f, 0f),
					new Vector3 (-212.9671f, 46.703f, 0f),
					new Vector3 (-235.1073f, 59.14327f, 0f),
					new Vector3 (-250.0443f, 67.59367f, 0f),
					new Vector3 (-259.6152f, 75.91506f, 0f),
					new Vector3 (-267.0813f, 84.60677f, 0f),
					new Vector3 (-272.3708f, 93.37731f, 0f),
					new Vector3 (-275.4552f, 102.7093f, 0f),
					new Vector3 (-278.3795f, 113.2072f, 0f),
					new Vector3 (-279.0174f, 126.6318f, 0f),
					new Vector3 (-277.7679f, 135.7201f, 0f),
					new Vector3 (-275.0825f, 144.333f, 0f),
					new Vector3 (-270.696f, 151.2973f, 0f),
					new Vector3 (-265.8317f, 157.3825f, 0f),
					new Vector3 (-258.2844f, 164.5069f, 0f),
					new Vector3 (-251.8242f, 168.5207f, 0f),
					new Vector3 (-242.5997f, 173.1436f, 0f)
				});
				routeLines.Add (new Terminals ("kibari", "malcom"), new List<Vector3> () {
					new Vector3 (-129.8345f, -65.79222f, 0f),
					new Vector3 (-131.1103f, -59.06677f, 0f),
					new Vector3 (-127.9734f, -52.31984f, 0f),
					new Vector3 (-121.2479f, -43.89333f, 0f),
					new Vector3 (-109.2879f, -36.42248f, 0f),
					new Vector3 (-83.55919f, -28.37106f, 0f),
					new Vector3 (-68.22328f, -21.40671f, 0f),
					new Vector3 (-52.72729f, -12.13205f, 0f),
					new Vector3 (-39.78052f, -3.225322f, 0f),
					new Vector3 (-27.44775f, 5.148626f, 0f),
					new Vector3 (-16.76112f, 14.71715f, 0f),
					new Vector3 (-3.498983f, 26.86594f, 0f),
					new Vector3 (8.437185f, 38.90962f, 0f),
					new Vector3 (21.24777f, 52.78338f, 0f),
					new Vector3 (27.94693f, 63.46762f, 0f),
					new Vector3 (35.54679f, 73.16994f, 0f),
					new Vector3 (44.18594f, 79.92166f, 0f),
					new Vector3 (50.40727f, 84.65456f, 0f)
				});
				routeLines.Add (new Terminals ("kibari", "mile"), new List<Vector3> () {
					new Vector3 (50.37621f, 84.67128f, 0f),
					new Vector3 (42.8528f, 75.05019f, 0f),
					new Vector3 (37.88099f, 66.48989f, 0f),
					new Vector3 (35.35566f, 54.451f, 0f),
					new Vector3 (36.01985f, 42.96638f, 0f),
					new Vector3 (39.15679f, 32.41592f, 0f),
					new Vector3 (39.76841f, 21.41392f, 0f),
					new Vector3 (40.19368f, 12.50958f, 0f),
					new Vector3 (43.14427f, 3.18236f, 0f),
					new Vector3 (42.10738f, -2.878896f, 0f),
					new Vector3 (38.94416f, -21.4306f, 0f),
					new Vector3 (37.24309f, -30.894f, 0f),
					new Vector3 (32.37879f, -39.63826f, 0f),
					new Vector3 (26.95782f, -53.06286f, 0f),
					new Vector3 (28.01861f, -58.32614f, 0f),
					new Vector3 (30.49137f, -63.16177f, 0f),
					new Vector3 (31.10299f, -70.31248f, 0f),
					new Vector3 (29.24185f, -79.05914f, 0f),
					new Vector3 (28.63022f, -88.20238f, 0f),
					new Vector3 (30.9692f, -92.80148f, 0f),
					new Vector3 (27.83225f, -97.58694f, 0f),
					new Vector3 (22.01229f, -103.0366f, 0f),
					new Vector3 (17.20056f, -107.9821f, 0f),
					new Vector3 (14.67524f, -112.3925f, 0f),
					new Vector3 (10.97923f, -120.3411f, 0f),
					new Vector3 (8.852896f, -127.2792f, 0f),
					new Vector3 (9.916057f, -131.4793f, 0f),
					new Vector3 (13.05301f, -133.9521f, 0f),
					new Vector3 (18.98047f, -137.4856f, 0f),
					new Vector3 (22.14131f, -140.5963f, 0f),
					new Vector3 (25.06562f, -145.1691f, 0f),
					new Vector3 (28.01622f, -149.3955f, 0f),
					new Vector3 (30.75417f, -153.1441f, 0f)
				});
				routeLines.Add (new Terminals ("valeria", "zima"), new List<Vector3> () {
					new Vector3 (-305.2752f, 195.2289f, 0f),
					new Vector3 (-322.0757f, 190.1544f, 0f),
					new Vector3 (-338.0758f, 182.8173f, 0f),
					new Vector3 (-352.64f, 174.6034f, 0f),
					new Vector3 (-360.216f, 168.0405f, 0f),
					new Vector3 (-365.2141f, 161.9792f, 0f),
					new Vector3 (-369.4907f, 153.7916f, 0f),
					new Vector3 (-372.6276f, 146.8296f, 0f),
					new Vector3 (-376.0034f, 130.2442f, 0f),
					new Vector3 (-376.8277f, 113.2048f, 0f),
					new Vector3 (-373.8246f, 96.62416f, 0f),
					new Vector3 (-368.4322f, 80.91793f, 0f),
					new Vector3 (-355.2752f, 59.92213f, 0f),
					new Vector3 (-345.5466f, 49.57953f, 0f),
					new Vector3 (-329.893f, 35.81329f, 0f),
					new Vector3 (-302.3557f, 13.11403f, 0f),
					new Vector3 (-290.4243f, 0.7573802f, 0f),
					new Vector3 (-280.7483f, -10.43337f, 0f),
					new Vector3 (-272.7231f, -26.59115f, 0f),
					new Vector3 (-266.1578f, -44.98039f, 0f),
					new Vector3 (-264.1915f, -57.97972f, 0f),
					new Vector3 (-263.1284f, -70.34116f, 0f),
					new Vector3 (-263.5536f, -86.659f, 0f),
					new Vector3 (-263.9789f, -96.14629f, 0f),
					new Vector3 (-265.2284f, -109.9125f, 0f),
					new Vector3 (-262.6768f, -122.0303f, 0f),
					new Vector3 (-259.0907f, -136.757f, 0f),
					new Vector3 (-253.4547f, -149.6989f, 0f),
					new Vector3 (-247.4197f, -161.4989f, 0f),
					new Vector3 (-237.9301f, -174.6536f, 0f),
					new Vector3 (-225.0669f, -188.1307f, 0f),
					new Vector3 (-212.469f, -198.7074f, 0f),
					new Vector3 (-199.0444f, -207.2677f, 0f),
					new Vector3 (-183.7897f, -214.2297f, 0f),
					new Vector3 (-168.1886f, -221.1941f, 0f),
					new Vector3 (-151.3379f, -226.6437f, 0f),
					new Vector3 (-134.5136f, -230.8701f, 0f),
					new Vector3 (-119.3377f, -233.8446f, 0f),
					new Vector3 (-93.31757f, -240.011f, 0f),
					new Vector3 (-67.83257f, -243.3056f, 0f),
					new Vector3 (-43.08821f, -247.716f, 0f),
					new Vector3 (-23.44706f, -250.0549f, 0f),
					new Vector3 (-0.007171631f, -251.5171f, 0f),
					new Vector3 (15.59394f, -251.4908f, 0f),
					new Vector3 (41.50659f, -250.8792f, 0f),
					new Vector3 (61.2003f, -248.5665f, 0f),
					new Vector3 (76.21608f, -244.7654f, 0f),
					new Vector3 (93.43941f, -240.3789f, 0f),
					new Vector3 (109.8934f, -234.8791f, 0f),
					new Vector3 (126.3451f, -228.1537f, 0f),
					new Vector3 (140.5103f, -220.1261f, 0f),
					new Vector3 (152.7666f, -210.0798f, 0f),
					new Vector3 (163.8499f, -196.6289f, 0f),
					new Vector3 (169.2995f, -186.7689f, 0f),
					new Vector3 (176.4478f, -173.5306f, 0f),
					new Vector3 (183.9163f, -164.7076f, 0f),
					new Vector3 (191.9677f, -156.0947f, 0f),
					new Vector3 (205.7101f, -145.7569f, 0f),
					new Vector3 (222.9071f, -132.9964f, 0f),
					new Vector3 (247.2238f, -118.9101f, 0f),
					new Vector3 (265.0875f, -105.2226f, 0f),
					new Vector3 (283.1064f, -93.10491f, 0f),
					new Vector3 (306.4388f, -75.48498f, 0f),
					new Vector3 (326.24f, -60.49549f, 0f),
					new Vector3 (339.9799f, -49.14466f, 0f),
					new Vector3 (352.499f, -37.29213f, 0f),
					new Vector3 (363.6874f, -23.79108f, 0f),
					new Vector3 (370.8883f, -10.82041f, 0f),
					new Vector3 (377.2147f, 0.1290334f, 0f),
					new Vector3 (379.1021f, 8.555542f, 0f),
					new Vector3 (378.2779f, 15.33116f, 0f),
					new Vector3 (375.7526f, 23.30611f, 0f),
					new Vector3 (371.5262f, 30.43054f, 0f),
					new Vector3 (366.1578f, 38.24543f, 0f),
					new Vector3 (359.6712f, 43.90771f, 0f),
					new Vector3 (353.0796f, 48.93207f, 0f),
					new Vector3 (345.6661f, 53.15847f, 0f),
					new Vector3 (335.1658f, 58.84463f, 0f),
					new Vector3 (322.0638f, 64.74581f, 0f)
				});
				routeLines.Add (new Terminals ("capitol", "malcom"), new List<Vector3> () {
					new Vector3 (-80.44116f, 33.62878f, 0f),
					new Vector3 (-73.29046f, 33.62878f, 0f),
					new Vector3 (-66.3261f, 33.62878f, 0f),
					new Vector3 (-59.65561f, 31.76764f, 0f),
					new Vector3 (-52.31855f, 28.60441f, 0f),
					new Vector3 (-46.92147f, 24.43058f, 0f),
					new Vector3 (-43.43811f, 16.95973f, 0f),
					new Vector3 (-42.61385f, 9.462593f, 0f),
					new Vector3 (-47.0266f, 3.590078f, 0f),
					new Vector3 (-52.02469f, -1.859558f, 0f),
					new Vector3 (-60.18362f, -7.760744f, 0f),
					new Vector3 (-67.9985f, -11.80079f, 0f),
					new Vector3 (-79.7722f, -16.10841f, 0f),
					new Vector3 (-95.96104f, -21.52938f, 0f),
					new Vector3 (-108.5853f, -26.26227f, 0f),
					new Vector3 (-121.6873f, -32.56245f, 0f),
					new Vector3 (-133.7023f, -40.80021f, 0f),
					new Vector3 (-140.4254f, -47.52565f, 0f),
					new Vector3 (-143.376f, -53.58691f, 0f),
					new Vector3 (-147.1508f, -61.00042f, 0f),
					new Vector3 (-146.7519f, -67.3006f, 0f),
					new Vector3 (-144.8907f, -73.60077f, 0f)
				});
				routeLines.Add (new Terminals ("crup", "zima"), new List<Vector3> () {
					new Vector3 (143.5495f, 213.6667f, 0f),
					new Vector3 (155.513f, 210.4792f, 0f),
					new Vector3 (170f, 207.7552f, 0f),
					new Vector3 (180.8047f, 205.7266f, 0f),
					new Vector3 (193.4063f, 201.8724f, 0f),
					new Vector3 (206.0677f, 196.0781f, 0f),
					new Vector3 (224.7552f, 186.9818f, 0f),
					new Vector3 (237.6458f, 180.1432f, 0f),
					new Vector3 (241.0938f, 179.6797f, 0f),
					new Vector3 (243.6146f, 179.013f, 0f),
					new Vector3 (246.3099f, 176.3177f, 0f),
					new Vector3 (246.9766f, 173.8255f, 0f),
					new Vector3 (253.5833f, 170.6667f, 0f),
					new Vector3 (264.7943f, 164.987f, 0f),
					new Vector3 (272.0677f, 160.4662f, 0f),
					new Vector3 (280.4427f, 156.151f, 0f),
					new Vector3 (289.1354f, 148.849f, 0f),
					new Vector3 (297.362f, 140.1563f, 0f),
					new Vector3 (302.5781f, 134.0417f, 0f),
					new Vector3 (303.2448f, 130.625f, 0f),
					new Vector3 (301.8828f, 125.5833f, 0f),
					new Vector3 (299.1589f, 121.1198f, 0f),
					new Vector3 (295.3333f, 116.3385f, 0f),
					new Vector3 (293.3047f, 111.7891f, 0f)
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