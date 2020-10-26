namespace Mis1eader.Vehicle
{
	using UnityEngine;
	using System.Collections.Generic;
	public abstract class TransportationInput : MonoBehaviour
	{
		[HideInInspector] internal Vector2 movementInput = Vector2.zero;
		[HideInInspector] internal float brakeInput = 0F;
		[HideInInspector] internal sbyte gearInput = 0;
		internal abstract void Handle ();
	}
}