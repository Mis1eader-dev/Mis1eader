namespace Mis1eader.Weapon
{
	using UnityEngine;
	using System.Collections.Generic;
	public class InputManagerOld : WeaponInput
	{
		public string fireAxis = "Fire1";
		internal override void Handle ()
		{
			fireInput = Input.GetButton(fireAxis);
		}
	}
}