namespace Mis1eader.Device.Computer.Components
{
	using UnityEngine;
	using System.Collections.Generic;
	public class Storage : MonoBehaviour
	{
		public enum Type : byte {SolidStateDrive,HardDiskDrive}
		public new string name = string.Empty;
		public Type type = Type.SolidStateDrive;
		public Library.Size sizeUnit = Library.Size.Gigabyte;
		public ushort size = 120;
	}
}