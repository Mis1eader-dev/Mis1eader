namespace Coordination
{
	using UnityEngine;
	using System.Collections.Generic;
	public enum Direction : byte {North,NorthEast,East,SouthEast,South,SouthWest,West,NorthWest}
	[AddComponentMenu("Mis1eader/Coordination/Direction Instance",0)]
	public class DirectionInstance : MonoBehaviour
	{
		public string Direction {get {return direction == Coordination.Direction.NorthEast || direction == Coordination.Direction.SouthEast || direction == Coordination.Direction.SouthWest || direction == Coordination.Direction.NorthWest ? direction.ToString().Insert(5," ") : direction.ToString();}}
		public Coordination.Direction direction = Coordination.Direction.North;
		public float angle = 0F;
		private void Update ()
		{
			angle = transform.rotation.eulerAngles.y;
			direction = GetDirection(angle);
		}
		public static Coordination.Direction GetDirection (float angle)
		{
			return angle < 22.5F ? Coordination.Direction.North :
				   (angle < 67.5F ? Coordination.Direction.NorthEast :
				   (angle < 112.5F ? Coordination.Direction.East :
				   (angle < 157.5F ? Coordination.Direction.SouthEast :
				   (angle < 202.5F ? Coordination.Direction.South :
				   (angle < 247.5F ? Coordination.Direction.SouthWest :
				   (angle < 292.5F ? Coordination.Direction.West :
				   (angle < 337.5F ? Coordination.Direction.NorthWest : Coordination.Direction.North)))))));
		}
	}
}