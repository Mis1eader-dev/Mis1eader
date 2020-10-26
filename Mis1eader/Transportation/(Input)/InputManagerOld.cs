namespace Mis1eader.Vehicle
{
	using UnityEngine;
	using System.Collections.Generic;
	public class InputManagerOld : TransportationInput
	{
		public string accelerationAxis = "Vertical";
		public string steerAxis = "Horizontal";
		public string brakeAxis = "Jump";
		public string gearAxis = "Gear";
		internal override void Handle ()
		{
			try {movementInput.x = Input.GetAxis(steerAxis);} catch {}
			try {movementInput.y = Input.GetAxis(accelerationAxis);} catch {}
			try {brakeInput = Input.GetAxis(brakeAxis);} catch {}
			try
			{
				if(Input.GetButtonDown(gearAxis))
				{
					if(Input.GetAxisRaw(gearAxis) < 0F)gearInput = -1;
					else gearInput = 1;
				}
				else gearInput = 0;
			} catch {}
		}
	}
}