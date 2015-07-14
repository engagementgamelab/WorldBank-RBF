
/// <summary>
/// This is a useful way of compairing routes by using the two cities on the route.
/// </summary>
public struct Terminals {
		
	public string city1;
	public string city2;

	public Terminals (string city1, string city2) {
		this.city1 = city1;
		this.city2 = city2;
	}

	public bool ContainsCity (string city) {
		return city1 == city || city2 == city;
	}

	public static bool operator ==(Terminals a, Terminals b) {
		if (a.Equals (b)) {
			return true;
		} else {
			return a.city1 == b.city2 && a.city2 == b.city1;			
		}
	}

	public static bool operator !=(Terminals a, Terminals b) {
		if (a.Equals (b)) {
			return false;
		} else {
			return a.city1 != b.city2 || a.city2 != b.city1;
		}
	}
}