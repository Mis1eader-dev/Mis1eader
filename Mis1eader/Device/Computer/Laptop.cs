namespace Mis1eader.Device.Computer
{
	using UnityEngine;
	using System.Collections.Generic;
	using Components;
	[RequireComponent(typeof(CPU),typeof(GPU),typeof(RAM)),RequireComponent(typeof(Storage))]
	public class Laptop : Computer
	{
		#if UNITY_EDITOR
		private void Reset ()
		{
			cpus = new List<CPU>(new CPU[1] {GetComponent<CPU>()});
			gpus = new List<GPU>(new GPU[1] {GetComponent<GPU>()});
			rams = new List<RAM>(new RAM[1] {GetComponent<RAM>()});
			storages = new List<Storage>(new Storage[1] {GetComponent<Storage>()});
			gpus[0].type = GPU.Type.Integrated;
		}
		#endif
	}
}