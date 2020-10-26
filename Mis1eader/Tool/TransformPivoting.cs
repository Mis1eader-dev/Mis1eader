namespace Mis1eader.Tool
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Tool/Transform Pivoting",0),ExecuteInEditMode]
	public class TransformPivoting : MonoBehaviour
	{
		public bool execute = false;
		public Vector3 point = Vector3.zero;
		public bool parenting = true;
		public Renderer[] renderers = new Renderer[0];
		public bool withinThis = true;
		private void Update ()
		{
			if(withinThis)
			{
				renderers = GetComponentsInChildren<Renderer>();
				withinThis = false;
			}
			if(execute)
			{
				Bounds bounds = new Bounds();
				Vector3 min = Vector3.one * float.MaxValue;
				Vector3 max = Vector3.one * float.MinValue;
				for(int a = 0,A = renderers.Length; a < A; a++)
				{
					min = Vector3.Min(min,renderers[a].bounds.min);
					max = Vector3.Max(max,renderers[a].bounds.max);
					if(parenting)renderers[a].transform.parent = transform.parent;
				}
				bounds.min = min;
				bounds.max = max;
				transform.position = bounds.center + new Vector3(point.x * bounds.extents.x,point.y * bounds.extents.y,point.z * bounds.extents.z);
				for(int a = 0,A = renderers.Length; a < A; a++)
					if(parenting)renderers[a].transform.parent = transform;
				execute = false;
			}
		}
	}
}