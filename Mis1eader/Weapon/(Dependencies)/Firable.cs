namespace Mis1eader.Weapon
{
	using UnityEngine;
	[RequireComponent(typeof(Rigidbody))]
	public abstract class Firable : MonoBehaviour
	{
		public abstract void Fire ();
		protected void ValidationHandler ()
		{
			if(!rigidbody || rigidbody.gameObject != gameObject)
			{
				rigidbody = GetComponent<Rigidbody>();
				if(!rigidbody)rigidbody = gameObject.AddComponent<Rigidbody>();
			}
		}
		[HideInInspector] public new Rigidbody rigidbody = null;
	}
}