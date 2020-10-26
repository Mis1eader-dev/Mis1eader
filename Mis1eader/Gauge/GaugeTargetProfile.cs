namespace Mis1eader.Gauge
{
	using UnityEngine;
	using System.Collections.Generic;
	public class GaugeTargetProfile : ScriptableObject
	{
		#if UNITY_EDITOR
		public new string name = string.Empty;
		#endif
		[Tooltip("Override the minor ticks of the target.")]
		public bool overrideMinorTicks = false;
		public byte minorTicks = 2;
		[Tooltip("Override the major ticks of the target.")]
		public bool overrideMajorTicks = false;
		public List<string> majorTicks = new List<string>();
		[Tooltip("Override the overrides of the target.")]
		public bool overrideOverrides = false;
		public List<GaugeTarget.Target.Override> overrides = new List<GaugeTarget.Target.Override>();
		private void OnValidate ()
		{
			if(minorTicks > 50)minorTicks = 50;
			for(int a = 0,A = overrides.Count; a < A; a++)
			{
				if(overrides[a].index < -1)overrides[a].index = -1;
				overrides[a].Update();
			}
			#if UNITY_EDITOR
			if(from < float.MinValue)from = float.MinValue;
			else if(from > float.MaxValue)from = float.MaxValue;
			if(to < float.MinValue)to = float.MinValue;
			else if(to > float.MaxValue)to = float.MaxValue;
			if(count < 2)count = 2;
			#endif
		}
		public void SetOverrideMinorTicks (bool value) {overrideMinorTicks = value;}
		public void SetMinorTicks (byte value) {minorTicks = value;}
		public void SetOverrideMajorTicks (bool value) {overrideMajorTicks = value;}
		public void SetMajorTicks (List<string> value) {majorTicks = value;}
		public void SetMajorTicksUnlinked (List<string> value) {int A = value.Count;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a];}
		public void SetMajorTicks (string[] value) {majorTicks = new List<string>(value);}
		public void SetMajorTicks (List<float> value) {int A = value.Count;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void SetMajorTicks (float[] value) {int A = value.Length;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void SetMajorTicks (List<int> value) {int A = value.Count;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void SetMajorTicks (int[] value) {int A = value.Length;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void GenerateEmptyRange (ushort count) {majorTicks = new List<string>(new string[count]);}
		public void GenerateRangeAsFloat (float from,float to,ushort count) {GenerateRange(from,to,count,GaugeSystem.Integerize.DontIntegerize);}
		public void GenerateRangeAsInteger (float from,float to,ushort count) {GenerateRange(from,to,count,GaugeSystem.Integerize.Cast);}
		public void GenerateRange (float from,float to,ushort count,GaugeSystem.Integerize integerize)
		{
			float range = to - from;
			if(majorTicks.Count != count)
				majorTicks = new List<string>(new string[count]);
			for(int a = 0; a < count; a++)
				majorTicks[a] = (integerize == GaugeSystem.Integerize.DontIntegerize ? Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to) :
								(integerize == GaugeSystem.Integerize.Cast ? (int)Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to) :
								(integerize == GaugeSystem.Integerize.Floor ? Mathf.Floor(Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to)) :
								(integerize == GaugeSystem.Integerize.Round ? Mathf.Round(Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to)) : Mathf.Ceil(Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to)))))).ToString();
		}
		public void SetOverrideOverrides (bool value) {overrideOverrides = value;}
		public void SetOverrides (List<GaugeTarget.Target.Override> value) {overrides = value;}
		public void SetOverridesUnlinked (List<GaugeTarget.Target.Override> value) {int A = value.Count;if(overrides.Count != A)overrides = new List<GaugeTarget.Target.Override>(new GaugeTarget.Target.Override[A]);for(int a = 0; a < A; a++)overrides[a] = value[a];}
		public void SetOverrides (GaugeTarget.Target.Override[] value) {overrides = new List<GaugeTarget.Target.Override>(value);}
		#if UNITY_EDITOR
		[HideInInspector] public bool rangeGeneration = false;
		[HideInInspector] public GaugeSystem.Integerize integerizeRange = GaugeSystem.Integerize.Cast;
		[HideInInspector] public float from = 0F;
		[HideInInspector] public float to = 100F;
		[HideInInspector] public ushort count = 5;
		[HideInInspector] public Vector2 majorTicksScrollView = Vector2.zero;
		#endif
	}
}