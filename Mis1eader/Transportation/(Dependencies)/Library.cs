namespace Mis1eader.Vehicle
{
	using UnityEngine;
	using System.Collections.Generic;
	public static class Library
	{
		public enum Speed : byte {KilometersPerHour,MilesPerHour}
		public static float Convert (float speed,Speed to) {return speed * (to == Speed.KilometersPerHour ? 3.6f : 2.2371372f);}
	}
}