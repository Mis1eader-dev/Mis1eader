namespace Mis1eader.Device.Computer
{
	using UnityEngine;
	using System.Collections.Generic;
	using Components;
	public class Computer : MonoBehaviour
	{
		public string username = string.Empty;
		public string password = string.Empty;
		public List<CPU> cpus = new List<CPU>();
		public List<GPU> gpus = new List<GPU>();
		public List<RAM> rams = new List<RAM>();
		public List<Storage> storages = new List<Storage>();
	}
}