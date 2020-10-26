namespace Mis1eader.Device.Computer.Components
{
	using UnityEngine;
	using System.Collections.Generic;
	public class GPU : MonoBehaviour
	{
		public enum Type : byte {Integrated,Dedicated}
		public new string name = string.Empty;
		public Type type = Type.Dedicated;
		
	}
}