namespace Mis1eader.Weapon
{
	using UnityEngine;
	using System.Collections.Generic;
	public abstract class WeaponInput : MonoBehaviour
	{
		[HideInInspector] internal bool fireInput = false;
		internal abstract void Handle ();
	}
}