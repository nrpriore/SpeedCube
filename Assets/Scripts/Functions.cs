﻿using UnityEngine;					// For UI classes
using System.Collections.Generic;	// For access to Lists and dictionaries
using System.Linq;					// For access to ValueCollection definitions (ex: Values.All)

// Holds generic static functions
public static class Functions {

	// Used to covert a hex string into a Unity Color
	public static Color HexToColor(string hex) {
		hex = hex.Replace ("0x", ""); 	// In case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", ""); 	// In case the string is formatted #FFFFFF
		byte a = 255;					// Assume fully visible unless specified in hex
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);

		// Only use alpha if the string has enough characters
		if(hex.Length == 8){
			a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
		}
		
		return new Color32(r,g,b,a);
	}

	// Checks if to integer lists are equal, ignoring order
	public static bool ListEquals(List<int> list1, List<int> list2) {
		if(list1 == null || list2 == null) {
			return false;
		}
		if(list1.Count != list2.Count || list2.Count == 0) {
			return false;
		}
		var cnt = new Dictionary<int, int>();
		foreach (int s in list1) {
			if (cnt.ContainsKey(s)) {
				cnt[s]++;
			}
			else {
				cnt.Add(s, 1);
			}
		}
		foreach (int s in list2) {
			if (cnt.ContainsKey(s)) {
				cnt[s]--;
			}
			else {
				return false;
			}
		}
		return cnt.Values.All(c => c == 0);
	}
	
}