namespace Mis1eader.Vehicle
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Transportation/Wheel Controller",0),RequireComponent(typeof(WheelCollider)),ExecuteInEditMode]
	public class WheelController : MonoBehaviour
	{
		public bool motor = true;
		public bool brake = true;
		public bool steering = false;
		public Transform movable = null;
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			ExecutionHandler();
		}
		private void ValidationHandler ()
		{
			if(!wheelCollider || wheelCollider.gameObject != gameObject)
			{
				wheelCollider = GetComponent<WheelCollider>();
				if(!wheelCollider)wheelCollider = gameObject.AddComponent<WheelCollider>();
			}
			if(!movable && transform.childCount != 0)
				movable = transform.GetChild(0);
		}
		private void ExecutionHandler ()
		{
			if(movable)
			{
				Vector3 position = Vector3.zero;
				Quaternion rotation = Quaternion.identity;
				wheelCollider.GetWorldPose(out position,out rotation);
				movable.position = position;
				movable.rotation = rotation;
			}
		}
		public void Handle (float motorTorque,float brakeTorque,float steerAngle)
		{
			if(motor)wheelCollider.motorTorque = motorTorque;
			if(brake)wheelCollider.brakeTorque = brakeTorque;
			if(steering)wheelCollider.steerAngle = steerAngle;
		}
		[HideInInspector] public WheelCollider wheelCollider = null;
	}
}