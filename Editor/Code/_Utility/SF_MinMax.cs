using UnityEngine;
using System.Collections;

[System.Serializable]
public class SF_MinMax {

	public int min = 0;
	public int max = 0;

	public SF_MinMax() {
	}
	public SF_MinMax(int min, int max) {
		this.min = min;
		this.max = max;
	}
	public override string ToString() {
		if( min == max )
			return min.ToString();
		return min + "-" + max;
	}
	public void Reset() {
		min = 0;
		max = 0;
	}

	public bool Empty() {
		return ( min == 0 && max == 0 );
	}

	public static SF_MinMax operator +( SF_MinMax a, SF_MinMax b ) {
		return new SF_MinMax(a.min+b.min, a.max+b.max);
	}

}
