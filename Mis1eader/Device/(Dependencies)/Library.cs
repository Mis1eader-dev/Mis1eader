namespace Mis1eader.Device
{
	using UnityEngine;
	using System.Collections.Generic;
	public static class Library
	{
		public enum Size : byte {Byte,Kilobyte,Megabyte,Gigabyte,Terabyte}
		public static string Name (Size sizeUnit) {return sizeUnit == Size.Byte ? "B" : (sizeUnit == Size.Kilobyte ? "KB" : (sizeUnit == Size.Megabyte ? "MB" : (sizeUnit == Size.Gigabyte ? "GB" : "TB")));}
	}
}