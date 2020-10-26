namespace Mis1eader.Gauge
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Gauge/Gauge Mask",3),ExecuteInEditMode]
	public class GaugeMask : MonoBehaviour
	{
		[System.Serializable] public class Override
		{
			public enum Type : byte {Offset,Scale,Color}
			public enum Effect : byte {Override,Increment}
			[System.Serializable] public class Range
			{
				public enum Type : byte {Percentage,Index}
				public Type type = Type.Percentage;
				public int from = 75;
				public int to = 100;
				public void Update ()
				{
					if(type == Type.Percentage)
					{
						if(from < 0)from = 0;
						else if(from > 100)from = 100;
						if(to < 0)to = 0;
						else if(to > 100)to = 100;
					}
					else
					{
						if(from < 0)from = 0;
						if(to < 0)to = 0;
					}
				}
			}
			public Type type = Type.Color;
			public Vector2 value = Vector2.zero;
			public float factor = 1F;
			public Color color = Color.white;
			public Effect effect = Effect.Increment;
			public List<Range> ranges = new List<Range>();
			public void Update ()
			{
				for(int a = 0,A = ranges.Count; a < A; a++)ranges[a].Update();
			}
		}
		public GaugeSystem gauge = null;
		public List<Override> overrides = new List<Override>();
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			//ExecutionHandler();
		}
		private void LateUpdate ()
		{
			
		}
		private void ValidationHandler ()
		{
			if(gauge)
			{
				if(!gauge.mask)
				{
					#if UNITY_EDITOR
					UnityEditor.Undo.RecordObject(gauge,"Inspector");
					#endif
					gauge.mask = this;
				}
				else if(gauge.mask != this)
				{
					#if UNITY_EDITOR
					UnityEditor.Undo.RecordObjects(new Object[] {gauge.mask,gauge},"Inspector");
					Debug.LogWarning("The gauge of " + gauge.mask.name + " is unlinked",gauge.mask);
					#endif
					gauge.mask.gauge = null;
					gauge.mask = this;
				}
			}
			for(int a = 0,A = overrides.Count; a < A; a++)overrides[a].Update();
		}
		private void ExecutionHandler ()
		{
			
		}
		//Use UnityEngine.MaterialPropertyBlock.AddColor(int nameID, Color value) for multiple material color changes.
		//You can use: Renderer.SetPropertyBlock(MaterialPropertyBlock property).
		public Information MaskHandler (Vector2 offset,Vector2 scale,Color color,int index,int count)
		{
			Information information = new Information();
			for(int a = 0,A = overrides.Count; a < A; a++)
			{
				Override @override = overrides[a];
				if(@override.type == Override.Type.Color)
				{
					for(int b = 0,B = @override.ranges.Count; b < B; b++)
					{
						Override.Range range = @override.ranges[b];
						if(range.type == Override.Range.Type.Percentage)
						{
							int from = (int)(range.from * 0.01F * count);
							int to = (int)(range.to * 0.01F * count);
							if(index >= Mathf.Min(from,to) && index <= Mathf.Max(to,from))
							{
								if(@override.effect == Override.Effect.Override)information.color = @override.color;
								else information.color = Color.Lerp(color,@override.color,RangeConversion(index,from,to,0F,1F));
							}
						}
						else if(range.from < count && range.to < count && index >= Mathf.Min(range.from,range.to) && index <= Mathf.Max(range.to,range.from))
						{
							if(@override.effect == Override.Effect.Override)information.color = @override.color;
							else information.color = Color.Lerp(color,@override.color,RangeConversion(index,range.from,range.to,0F,1F));
						}
					}
				}
				else
				{
					if(@override.type == Override.Type.Offset)
					{
						
					}
					else if(@override.type == Override.Type.Scale)
					{
						
					}
				}
			}
			return information;
		}
		public class Information
		{
			public Vector2 offset = Vector2.zero;
			public Vector2 scale = Vector2.zero;
			public Color color = Color.clear;
		}
		public Vector2 OffsetHandler ()
		{
			for(int a = 0,A = overrides.Count; a < A; a++)
			{
				
			}
			return Vector2.zero;
		}
		public Color ColorHandler ()
		{
			for(int a = 0,A = overrides.Count; a < A; a++)
			{
				
			}
			return Color.clear;
		}
		private static float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimumValue != maximumValue ? minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum) : minimum;}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
	}
}