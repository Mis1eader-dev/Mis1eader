namespace Mis1eader.Device.Computer.Components
{
	using UnityEngine;
	using System.Collections.Generic;
	public class RAM : MonoBehaviour
	{
		public new string name = string.Empty;
		public Library.Size sizeUnit = Library.Size.Gigabyte;
		public ushort size = 4;
	}
}