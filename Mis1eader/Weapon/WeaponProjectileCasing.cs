namespace Mis1eader.Weapon
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Weapon/Weapon Projectile Casing",2),ExecuteInEditMode]
	public class WeaponProjectileCasing : Firable
	{
		public WeaponProjectile projectile = null;
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			//ExecutionHandler();
		}
		private new void ValidationHandler ()
		{
			base.ValidationHandler();
		}
		public override void Fire ()
		{
			if(projectile)projectile.Fire();
		}
	}
}