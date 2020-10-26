#if UNITY_5_3_OR_NEWER
#define ATTRIBUTE_DELAYED
#endif
namespace Mis1eader.Gauge
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Gauge/Gauge Target",1),ExecuteInEditMode]
	public class GaugeTarget : MonoBehaviour
	{
		public enum UpdateMode : byte {OnAwake,EveryFrame,ViaScripting}
		[System.Serializable] public class Target
		{
			[System.Serializable] public class Override
			{
				public enum Type : byte {MinimumValue,Value,AbsoluteValue,MaximumValue}
				[Tooltip("Type of variable in the gauge to override.")]
				public Type type = Type.AbsoluteValue;
				[Tooltip("Index of additional values in the gauge to override, leave it as -1 if you don't know what it does, -1 means to override the built-in value, minimum value or maximum value.")]
				public sbyte index = -1;
				[Tooltip("Value to be sent to the gauge, if the variable field below this is not left empty, then that variable's value will be passed to the gauge, otherwise, this value will be passed.")]
				public float value = 0F;
				#if ATTRIBUTE_DELAYED
				[Delayed]
				#endif
				[Tooltip("Variable to be searched for in the source, if this is not left empty, then that variable's value will be passed to the gauge, otherwise, the value field above this will be passed instead.")]
				public string variable = string.Empty;
				#if UNITY_EDITOR
				[HideInInspector] public int nameIndex = 0;
				#endif
				public Override () {}
				public Override (float value) {this.value = value;}
				public void Update ()
				{
					if(value < float.MinValue)value = float.MinValue;
					else if(value > float.MaxValue)value = float.MaxValue;
					VariableName(ref variable);
				}
				private System.Reflection.FieldInfo field = null;
				private System.Reflection.PropertyInfo property = null;
				[HideInInspector,SerializeField] private string lastVariable = null;
				public void Handle (Component source)
				{
					if(!source || variable == string.Empty)return;
					System.Type type = source.GetType();
					if(type.BaseType == typeof(MonoBehaviour))
					{
						if(field == null || lastVariable != variable)
						{
							field = type.GetField(variable.Replace(".x",string.Empty).Replace(".y",string.Empty).Replace(".z",string.Empty).Replace(".",string.Empty),System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
							lastVariable = variable;
						}
						if(field != null)
						{
							type = field.FieldType;
							if(type == typeof(float))value = (float)field.GetValue(source);
							else if(type == typeof(int))value = (int)field.GetValue(source);
							else if(type == typeof(Vector2))
							{
								if(variable.Contains(".x"))value = ((Vector2)field.GetValue(source)).x;
								else if(variable.Contains(".y"))value = ((Vector2)field.GetValue(source)).y;
							}
							else if(type == typeof(Vector3))
							{
								if(variable.Contains(".x"))value = ((Vector3)field.GetValue(source)).x;
								else if(variable.Contains(".y"))value = ((Vector3)field.GetValue(source)).y;
								else if(variable.Contains(".z"))value = ((Vector3)field.GetValue(source)).z;
							}
						}
					}
					else if(type != typeof(Transform) && type != typeof(RectTransform))
					{
						if(property == null || lastVariable != variable)
						{
							property = type.GetProperty(variable.Replace(".x",string.Empty).Replace(".y",string.Empty).Replace(".z",string.Empty).Replace(".",string.Empty));
							lastVariable = variable;
						}
						if(property != null)
						{
							type = property.PropertyType;
							if(type == typeof(float))value = (float)property.GetValue(source,null);
							else if(type == typeof(int))value = (int)property.GetValue(source,null);
							else if(type == typeof(Vector2))
							{
								if(variable.Contains(".x"))value = ((Vector2)property.GetValue(source,null)).x;
								else if(variable.Contains(".y"))value = ((Vector2)property.GetValue(source,null)).y;
							}
							else if(type == typeof(Vector3))
							{
								if(variable.Contains(".x"))value = ((Vector3)property.GetValue(source,null)).x;
								else if(variable.Contains(".y"))value = ((Vector3)property.GetValue(source,null)).y;
								else if(variable.Contains(".z"))value = ((Vector3)property.GetValue(source,null)).z;
							}
						}
					}
				}
				private static void VariableName (ref string name)
				{
					if(name == string.Empty)return;
					bool hasDot = false;
					for(int a = 0,A = name.Length; a < A; a++)
					{
						char character = name[a];
						if(a > 0)
						{
							if(!hasDot && character == '.')hasDot = true;
							if(hasDot && character == '.' && a + 1 < A && name[a + 1] != 'x' && name[a + 1] != 'y' && name[a + 1] != 'z')
							{
								name = name.Remove(a + 1,1);
								a = a - 1;
								A = A - 1;
								continue;
							}
							if(hasDot && a + 1 < A && (name[a + 1] == 'x' || name[a + 1] == 'y' || name[a + 1] == 'z') && a + 2 < A)
							{
								name = name.Remove(a + 2,1);
								a = a - 1;
								A = A - 1;
								continue;
							}
						}
						if(!char.IsLetter(character) && character != '_' && (character != '.' || a == 0 && character == '.') && ((character < '0' || character > '9') || a == 0 && character >= '0' && character <= '9'))
						{
							name = name.Remove(a,1);
							a = a - 1;
							A = A - 1;
							continue;
						}
					}
				}
				public void SetType (Type value) {type = value;}
				public void SetType (int value) {type = (Type)value;}
				public void SetIndex (sbyte value) {index = value;}
				public void SetValue (float value) {this.value = value;}
				public void SetVariable (string value) {variable = value;}
			}
			#if UNITY_EDITOR
			public string name = string.Empty;
			#endif
			public GaugeTargetProfile profile = null;
			[Tooltip("The gauge to send data to.")]
			public GaugeSystem gauge = null;
			[Tooltip("The component to get data from.")]
			public Component source = null;
			[Tooltip("Override the minor ticks of the gauge.")]
			public bool overrideMinorTicks = false;
			public byte minorTicks = 2;
			[Tooltip("Override the major ticks of the gauge.")]
			public bool overrideMajorTicks = false;
			public List<string> majorTicks = new List<string>();
			public List<Override> overrides = new List<Override>();
			[HideInInspector] public bool isUpdating = false;
			public void Update (bool run)
			{
				if(minorTicks > 50)minorTicks = 50;
				if(profile)
				{
					#if UNITY_EDITOR
					rangeGeneration = false;
					name = profile.name;
					#endif
					if(overrideMinorTicks && profile.overrideMinorTicks)minorTicks = profile.minorTicks;
					if(overrideMajorTicks && profile.overrideMajorTicks)SetMajorTicksUnlinked(profile.majorTicks);
					if(profile.overrideOverrides)SetOverridesUnlinked(profile.overrides);
				}
				if(run)isUpdating = false;
				for(int a = 0,A = overrides.Count; a < A; a++)
				{
					if(overrides[a].index < -1)overrides[a].index = -1;
					else if(gauge) {if(overrides[a].index >= gauge.additionalValues.Count)overrides[a].index = (sbyte)(gauge.additionalValues.Count - 1);}
					else if(overrides[a].index > -1)overrides[a].index = -1;
					overrides[a].Update();
					if(run && gauge)
					{
						overrides[a].Handle(source);
						if(overrideMinorTicks)gauge.minorTicks = minorTicks;
						if(overrideMajorTicks)gauge.SetMajorTicksUnlinked(majorTicks);
						if(overrides[a].index == -1)
						{
							if(overrides[a].type == Override.Type.MinimumValue)gauge.minimumValue = overrides[a].value;
							else if(overrides[a].type == Override.Type.Value)gauge.value = overrides[a].value;
							else if(overrides[a].type == Override.Type.AbsoluteValue)gauge.value = overrides[a].value < 0F ? -overrides[a].value : overrides[a].value;
							else gauge.maximumValue = overrides[a].value;
						}
						else
						{
							if(overrides[a].type == Override.Type.MinimumValue)gauge.additionalValues[overrides[a].index].minimumValue = overrides[a].value;
							else if(overrides[a].type == Override.Type.Value)gauge.additionalValues[overrides[a].index].value = overrides[a].value;
							else if(overrides[a].type == Override.Type.AbsoluteValue)gauge.additionalValues[overrides[a].index].value = overrides[a].value < 0F ? -overrides[a].value : overrides[a].value;
							else gauge.additionalValues[overrides[a].index].maximumValue = overrides[a].value;
						}
					}
				}
				#if UNITY_EDITOR
				if(!overrideMajorTicks)rangeGeneration = false;
				if(from < float.MinValue)from = float.MinValue;
				else if(from > float.MaxValue)from = float.MaxValue;
				if(to < float.MinValue)to = float.MinValue;
				else if(to > float.MaxValue)to = float.MaxValue;
				if(count < 2)count = 2;
				#endif
			}
			public void SetProfile (GaugeTargetProfile value) {profile = value;}
			public void SetGauge (GaugeSystem value) {gauge = value;}
			public void SetSource (Component value) {source = value;}
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
			public void SetOverrides (List<Override> value) {overrides = value;}
			public void SetOverridesUnlinked (List<Override> value) {int A = value.Count;if(overrides.Count != A)overrides = new List<Override>(new Override[A]);for(int a = 0; a < A; a++)overrides[a] = value[a];}
			public void SetOverrides (Override[] value) {overrides = new List<Override>(value);}
			#if UNITY_EDITOR
			[HideInInspector] public bool rangeGeneration = false;
			[HideInInspector] public GaugeSystem.Integerize integerizeRange = GaugeSystem.Integerize.Cast;
			[HideInInspector] public float from = 0F;
			[HideInInspector] public float to = 100F;
			[HideInInspector] public ushort count = 5;
			[HideInInspector] public Vector2 majorTicksScrollView = Vector2.zero;
			#endif
		}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		public List<Target> targets = new List<Target>();
		private void Awake ()
		{
			if(
			#if UNITY_EDITOR
			!Application.isPlaying ||
			#endif
			updateMode != UpdateMode.OnAwake)return;
			for(int a = 0,A = targets.Count; a < A; a++)
				targets[a].Update(true);
		}
		private void Update ()
		{
			for(int a = 0,A = targets.Count; a < A; a++)
				targets[a].Update(
				#if UNITY_EDITOR
				Application.isPlaying &&
				#endif
				(updateMode == UpdateMode.EveryFrame || (updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && targets[a].isUpdating));
		}
		public void SetUpdateMode (UpdateMode value) {updateMode = value;}
		public void SetUpdateMode (int value) {updateMode = (UpdateMode)value;}
		public void SetTargets (List<Target> value) {targets = value;}
		public void SetTargetsUnlinked (List<Target> value) {int A = value.Count;if(targets.Count != A)targets = new List<Target>(new Target[A]);for(int a = 0; a < A; a++)targets[a] = value[a];}
		public void SetTargets (Target[] value) {targets = new List<Target>(value);}
		public void UpdateTargetAtIndexImmediately (int index) {if(index >= 0 && index < targets.Count)targets[index].Update(true);}
		public void UpdateAllTargetsImmediately () {for(int a = 0,A = targets.Count; a < A; a++)targets[a].Update(true);}
		public void UpdateTargetAtIndexPending (int index) {if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && index >= 0 && index < targets.Count)targets[index].isUpdating = true;}
		public void UpdateAllTargetsPending () {if(updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting)for(int a = 0,A = targets.Count; a < A; a++)targets[a].isUpdating = true;}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public string targetsName = "Untitled";
		#endif
	}
}