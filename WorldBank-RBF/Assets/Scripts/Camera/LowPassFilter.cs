using System.Collections;
using System.Collections.Generic;

public class LowPassFilter {

	Queue<float> values = new Queue<float> ();
	
	int capacity = 10;
	public int Capacity {
		get { return capacity; }
		set { capacity = value; }
	}

	public float Average {
		get { 
			float total = 0f;
			foreach (float value in values) {
				total += value;
			}
			return total / (float)capacity;
		}
	}

	public LowPassFilter (int capacity=10) {
		this.capacity = capacity;
	}

	public void Add (float value) {
		values.Enqueue (value);
		if (values.Count > Capacity)
			values.Dequeue ();
	}

	public void Reset () {
		values.Clear ();
	}
}
