namespace Coordination
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Coordination/Compass",1)]
	public class Compass : MonoBehaviour
	{
		public enum Motion : byte {Instant,Linear,Slerp}
		public DirectionInstance synchronization = null;
		public Coordination.Direction direction = Coordination.Direction.North;
		public Motion motion = Motion.Instant;
		public Transform needle = null;
		public Vector3 axis = -Vector3.up;
		public float angle = 0F;
		public float smoothing = 1F;
		private void Update ()
		{
			if(synchronization)
			{
				angle = synchronization.angle;
				direction = synchronization.direction;
			}
			else
			{
				angle = transform.rotation.eulerAngles.y;
				direction = DirectionInstance.GetDirection(angle);
			}
			if(needle)
			{
				if(motion == Motion.Instant)needle.localRotation = Quaternion.AngleAxis(angle,axis);
				else if(motion == Motion.Linear)
				{
					
				}
				else
				{
					
				}
			}
		}
	}
}