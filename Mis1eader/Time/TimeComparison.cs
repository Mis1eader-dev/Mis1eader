namespace AdvancedAssets
{
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using System.Collections.Generic;
	[AddComponentMenu("Advanced Assets/Time/Time Comparison",22),ExecuteInEditMode]
	public class TimeComparison : MonoBehaviour
	{
		public enum UpdateMode {OnAwake,EveryFrame,ViaScripting}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		#if UNITY_EDITOR
		[HideInInspector] public string rangeComparisonsName = "Untitled";
		[HideInInspector] public string operatorComparisonsName = "Untitled";
		#endif
		public List<RangeComparison> rangeComparisons = new List<RangeComparison>();
		public List<OperatorComparison> operatorComparisons = new List<OperatorComparison>();
		#if UNITY_EDITOR
		[HideInInspector] public bool rangeComparisonsIsExpanded = true;
		[HideInInspector] public bool operatorComparisonsIsExpanded = true;
		#endif
		[System.Serializable] public class RangeComparison
		{
			public string name = string.Empty;
			public float minimumOutput = -1;
			public float output = 0;
			public float maximumOutput = 1;
			public bool absolute = false;
			public bool clamp = true;
			public Time minimumTime = new Time();
			public Time time = new Time();
			public Time maximumTime = new Time();
			[HideInInspector] public bool isUpdating = false;
			#if UNITY_EDITOR
			[HideInInspector] public bool isExpanded = false;
			#endif
			[System.Serializable] public class Time : TimeSystem.Time
			{
				public TimeSystem source = null;
				#if UNITY_EDITOR
				[HideInInspector] public bool isExpanded = false;
				#endif
				public new void Update ()
				{
					if(source)
					{
						if(timeType != source.time.timeType)
							timeType = source.time.timeType;
						if(isAm != source.time.isAm)
							isAm = source.time.isAm;
						if(isPm != source.time.isPm)
							isPm = source.time.isPm;
						if(useMinute != source.time.useMinute)
							useMinute = source.time.useMinute;
						if(useHour != source.time.useHour)
							useHour = source.time.useHour;
						if(useDay != source.time.useDay)
							useDay = source.time.useDay;
						if(useMonth != source.time.useMonth)
							useMonth = source.time.useMonth;
						if(useYear != source.time.useYear)
							useYear = source.time.useYear;
						if(delta != source.time.delta)
							delta = source.time.delta;
						if(second != source.time.second)
							second = source.time.second;
						if(minute != source.time.minute)
							minute = source.time.minute;
						if(hour != source.time.hour)
							hour = source.time.hour;
						if(day != source.time.day)
							day = source.time.day;
						if(month != source.time.month)
							month = source.time.month;
						if(year != source.time.year)
							year = source.time.year;
						if(wasAm != source.time.wasAm)
							wasAm = source.time.wasAm;
						if(wasPm != source.time.wasPm)
							wasPm = source.time.wasPm;
						if(isTwelveHourSwitch != (timeType == Time.TimeType.TwelveHour))
							isTwelveHourSwitch = timeType == Time.TimeType.TwelveHour;
						if(isTwentyFourHourSwitch != (timeType == Time.TimeType.TwentyFourHour))
							isTwentyFourHourSwitch = timeType == Time.TimeType.TwentyFourHour;
					}
					else base.Update();
				}
				public void SetSource (TimeSystem value) {if(source != value)source = value;}
			}
			public void Update (bool run)
			{
				minimumTime.Update();
				time.Update();
				maximumTime.Update();
				if(!run)return;
				ComparisonHandler();
			}
			private void ComparisonHandler ()
			{
				double minimum = TotalTime(minimumTime.useMinute,minimumTime.useHour,minimumTime.useDay,minimumTime.useMonth,minimumTime.useYear,minimumTime.delta,minimumTime.second,minimumTime.minute,minimumTime.hour,minimumTime.day,minimumTime.month,minimumTime.year,minimumTime.timeType == Time.TimeType.TwelveHour,minimumTime.isAm,minimumTime.isPm);
				double time = TotalTime(this.time.useMinute,this.time.useHour,this.time.useDay,this.time.useMonth,this.time.useYear,this.time.delta,this.time.second,this.time.minute,this.time.hour,this.time.day,this.time.month,this.time.year,this.time.timeType == Time.TimeType.TwelveHour,this.time.isAm,this.time.isPm);
				double maximum = TotalTime(maximumTime.useMinute,maximumTime.useHour,maximumTime.useDay,maximumTime.useMonth,maximumTime.useYear,maximumTime.delta,maximumTime.second,maximumTime.minute,maximumTime.hour,maximumTime.day,maximumTime.month,maximumTime.year,maximumTime.timeType == Time.TimeType.TwelveHour,maximumTime.isAm,maximumTime.isPm);
				output = RangeConversion(time,minimum,maximum,minimumOutput,maximumOutput);
				if(clamp || absolute)output = clamp ? Mathf.Clamp(absolute ? Mathf.Abs(output) : output,minimumOutput,maximumOutput) : (absolute ? Mathf.Abs(output) : output);
			}
			private double TotalTime (bool useMinute,bool useHour,bool useDay,bool useMonth,bool useYear,float delta,int second,int minute,int hour,int day,int month,uint year,bool isTwelveHour,bool isAm,bool isPm)
			{
				double result = (delta + second) * 0.01f;
				byte[] days = new byte[] {31,28,31,30,31,30,31,31,30,31,30,31};
				if(useMinute)result = result + minute * 0.6f;
				if(useHour)
				{
					if(isTwelveHour)
					{
						if(isAm && hour == 12)hour = hour - 12;
						if(isPm && hour >= 1 && hour < 12)hour = hour + 12;
					}
					result = result + hour * 36;
				}
				if(useDay)result = result + day * 864;
				if(useMonth)for(int a = 0; a < month % 12; a++)
					result = result + 864 * days[a];
				if(useYear)result = result + year * 315360 + (year / 4) * 864 + (year / 400) * 864  - (year / 100) * 864 + (year % 4 == 0 && month >= 2 ? 864 : 0) + (year % 4 > 0 ? 864 : 0);
				return result;
			}
			private static float RangeConversion (double value,double minimumValue,double maximumValue,float minimum,float maximum) {return minimumValue != maximumValue ? minimum + (float)((value - minimumValue) / (maximumValue - minimumValue)) * (maximum - minimum) : minimum;}
			public float GetOutput () {return output;}
			public void SetName (string value) {if(name != value)name = value;}
			public void SetMinimumOutput (float value) {if(minimumOutput != value)minimumOutput = value;}
			public void SetMaximumOutput (float value) {if(maximumOutput != value)maximumOutput = value;}
			public void Absolute (bool value) {if(absolute != value)absolute = value;}
			public void Clamp (bool value) {if(clamp != value)clamp = value;}
			public void SetMinimumTime (Time value) {if(minimumTime != value)minimumTime = value;}
			public void SetTime (Time value) {if(time != value)time = value;}
			public void SetMaximumTime (Time value) {if(maximumTime != value)maximumTime = value;}
		}
		[System.Serializable] public class OperatorComparison
		{
			public string name = string.Empty;
			public sbyte sign = 0;
			public AdvancedAssets.Time output = new AdvancedAssets.Time();
			public List<Time> times = new List<Time>();
			[HideInInspector] public bool isUpdating = false;
			#if UNITY_EDITOR
			[HideInInspector] public bool isExpanded = false;
			[HideInInspector] public bool timesIsExpanded = true;
			#endif
			[System.Serializable] public class Time : RangeComparison.Time
			{
				public enum Operator {Addition,Subtraction}
				public Operator _operator = Operator.Addition;
				public void SetOperator (Operator value) {if(_operator != value)_operator = value;}
				public void SetOperator (int value)
				{
					Operator convertedValue = (Operator)value;
					if(_operator != convertedValue)_operator = convertedValue;
				}
			}
			public void Update (bool run)
			{
				for(int a = 0,A = times.Count; a < A; a++)times[a].Update();
				if(run)ComparisonHandler();
			}
			private void ComparisonHandler ()
			{
				sbyte sign = 0;
				output.isAm = true;
				output.isPm = false;
				output.delta = 0;
				output.second = 0;
				output.minute = 0;
				output.hour = 0;
				output.day = 0;
				output.month = 0;
				output.year = 0;
				output.wasAm = true;
				output.wasPm = false;
				output.onTwelveHourSwitch = false;
				output.onTwentyFourHourSwitch = false;
				output.isTwelveHourSwitch = false;
				output.isTwentyFourHourSwitch = false;
				output.isLeapYear = false;
				for(int a = 0,A = times.Count; a < A; a++)
				{
					if(times[a]._operator == Time.Operator.Addition)
					{
						output.delta = output.delta + times[a].delta;
						output.second = output.second + times[a].second;
						output.minute = output.minute + times[a].minute;
						if(times[a].timeType == Time.TimeType.TwelveHour && times[a].isPm && times[a].hour >= 1 && times[a].hour < 12)
							output.hour = output.hour + times[a].hour + 12;
						if(times[a].timeType == Time.TimeType.TwentyFourHour || times[a].timeType == Time.TimeType.TwelveHour && (times[a].isAm && times[a].hour < 12 || times[a].isPm && times[a].hour == 12))
							output.hour = output.hour + times[a].hour;
						output.day = output.day + times[a].day;
						output.month = output.month + times[a].month;
						if(sign == -1)
						{
							if(output.year < times[a].year)
							{
								output.year = times[a].year - output.year;
								sign = 1;
							}
							if(output.year == times[a].year)output.year = 0;
							if(output.year > times[a].year)output.year = output.year - times[a].year;
							continue;
						}
						if(sign == 0)
						{
							output.year = times[a].year;
							sign = 1;
							continue;
						}
						if(sign == 1)output.year = output.year + times[a].year;
					}
					if(times[a]._operator == Time.Operator.Subtraction)
					{
						output.delta = output.delta - times[a].delta;
						output.second = output.second - times[a].second;
						output.minute = output.minute - times[a].minute;
						if(times[a].timeType == Time.TimeType.TwelveHour && times[a].isPm && times[a].hour >= 1 && times[a].hour < 12)
							output.hour = output.hour - times[a].hour + 12;
						if(times[a].timeType == Time.TimeType.TwentyFourHour || times[a].timeType == Time.TimeType.TwelveHour && (times[a].isAm && times[a].hour < 12 || times[a].isPm && times[a].hour == 12))
							output.hour = output.hour - times[a].hour;
						output.day = output.day - times[a].day;
						output.month = output.month - times[a].month;
						if(sign == -1)output.year = output.year + times[a].year;
						if(sign == 0)
						{
							output.year = times[a].year;
							sign = -1;
						}
						if(sign == 1)
						{
							if(output.year < times[a].year)
							{
								output.year = times[a].year - output.year;
								sign = -1;
							}
							if(output.year == times[a].year)output.year = 0;
							if(output.year > times[a].year)output.year = output.year - times[a].year;
						}
					}
				}
				double result = TotalTime(output.useMinute,output.useHour,output.useDay,output.useMonth,output.useYear,output.delta,output.second,output.minute,output.hour,output.day,output.month,output.year,sign,output.timeType == Time.TimeType.TwelveHour,output.isAm,output.isPm);
				if(result < 0)this.sign = -1;
				if(result == 0)this.sign = 0;
				if(result > 0)this.sign = 1;
				output.delta = Mathf.Abs(output.delta);
				output.second = Mathf.Abs(output.second);
				output.minute = Mathf.Abs(output.minute);
				output.hour = Mathf.Abs(output.hour);
				output.day = Mathf.Abs(output.day);
				output.month = Mathf.Abs(output.month);
				output.Update();
			}
			private double TotalTime (bool useMinute,bool useHour,bool useDay,bool useMonth,bool useYear,float delta,int second,int minute,int hour,int day,int month,uint year,sbyte sign,bool isTwelveHour,bool isAm,bool isPm)
			{
				double result = (delta + second) * 0.01f;
				byte[] days = new byte[] {31,28,31,30,31,30,31,31,30,31,30,31};
				if(useMinute)result = result + minute * 0.6f;
				if(useHour)
				{
					if(isTwelveHour)
					{
						if(isAm && hour == 12)hour = hour - 12;
						if(isPm && hour >= 1 && hour < 12)hour = hour + 12;
					}
					result = result + hour * 36;
				}
				if(useDay)result = result + day * 864;
				if(useMonth)for(int a = 0; a < month % 12; a++)
					result = result + 864 * days[a];
				if(useYear)
				{
					if(sign == -1)result = result - year * 315360 - (year / 4) * 864 - (year / 400) * 864  + (year / 100) * 864 - (year % 4 == 0 && month >= 2 ? 864 : 0) - (year % 4 > 0 ? 864 : 0);
					if(sign == 1)result = result + year * 315360 + (year / 4) * 864 + (year / 400) * 864  - (year / 100) * 864 + (year % 4 == 0 && month >= 2 ? 864 : 0) + (year % 4 > 0 ? 864 : 0);
				}
				return result;
			}
			public sbyte GetSign () {return sign;}
			public sbyte GetOutputSign () {return sign;}
			public AdvancedAssets.Time GetOutput () {return output;}
			public bool GetOutputAm () {return output.isAm;}
			public bool GetOutputPm () {return output.isPm;}
			public uint GetOutputYear () {return output.year;}
			public int GetOutputMonth () {return output.month;}
			public int GetOutputDay () {return output.day;}
			public int GetOutputHour () {return output.hour;}
			public int GetOutputMinute () {return output.minute;}
			public int GetOutputSecond () {return output.second;}
			public float GetOutputDelta () {return output.delta;}
			public void SetName (string value) {if(name != value)name = value;}
			public void SetOutputTimeType (Time.TimeType value) {if(output.timeType != value)output.timeType = value;}
			public void SetOutputTimeType (int value)
			{
				Time.TimeType convertedValue = (Time.TimeType)value;
				if(output.timeType != convertedValue)output.timeType = convertedValue;
			}
			public void SetTimes (List<Time> value)
			{
				int A = value.Count;
				if(times.Count != A)times = new List<Time>(new Time[A]);
				for(int a = 0; a < A; a++)if(times[a] != value[a])times[a] = value[a];
			}
			public void SetTimes (Time[] value)
			{
				List<Time> convertedValue = new List<Time>(value);
				if(times != convertedValue)times = convertedValue;
			}
		}
		private void Awake ()
		{
			if(
			#if UNITY_EDITOR
			!Application.isPlaying ||
			#endif
			updateMode != UpdateMode.OnAwake)return;
			for(int a = 0,A = rangeComparisons.Count; a < A; a++)
				rangeComparisons[a].Update(true);
			for(int a = 0,A = operatorComparisons.Count; a < A; a++)
				operatorComparisons[a].Update(true);
		}
		private void Update ()
		{
			for(int a = 0,A = rangeComparisons.Count; a < A; a++)
				rangeComparisons[a].Update(
				#if UNITY_EDITOR
				Application.isPlaying &&
				#endif
				(updateMode == UpdateMode.EveryFrame || updateMode == UpdateMode.ViaScripting && rangeComparisons[a].isUpdating));
			for(int a = 0,A = operatorComparisons.Count; a < A; a++)
				operatorComparisons[a].Update(
				#if UNITY_EDITOR
				Application.isPlaying &&
				#endif
				(updateMode == UpdateMode.EveryFrame || updateMode == UpdateMode.ViaScripting && operatorComparisons[a].isUpdating));
		}
		public float GetRangeComparisonOutput (int index)
		{
			if(index >= 0 && index < rangeComparisons.Count)return rangeComparisons[index].output;
			return 0;
		}
		public void SetUpdateMode (UpdateMode value) {if(updateMode != value)updateMode = value;}
		public void SetUpdateMode (int value)
		{
			UpdateMode convertedValue = (UpdateMode)value;
			if(updateMode != convertedValue)updateMode = convertedValue;
		}
		public void SetRangeComparisons (List<RangeComparison> value)
		{
			int A = value.Count;
			if(rangeComparisons.Count != A)rangeComparisons = new List<RangeComparison>(new RangeComparison[A]);
			for(int a = 0; a < A; a++)if(rangeComparisons[a] != value[a])rangeComparisons[a] = value[a];
		}
		public void SetRangeComparisons (RangeComparison[] value)
		{
			List<RangeComparison> convertedValue = new List<RangeComparison>(value);
			if(rangeComparisons != convertedValue)rangeComparisons = convertedValue;
		}
		public void SetOperatorComparisons (List<OperatorComparison> value)
		{
			int A = value.Count;
			if(operatorComparisons.Count != A)operatorComparisons = new List<OperatorComparison>(new OperatorComparison[A]);
			for(int a = 0; a < A; a++)if(operatorComparisons[a] != value[a])operatorComparisons[a] = value[a];
		}
		public void SetOperatorComparisons (OperatorComparison[] value)
		{
			List<OperatorComparison> convertedValue = new List<OperatorComparison>(value);
			if(operatorComparisons != convertedValue)operatorComparisons = convertedValue;
		}
		public void UpdateRangeComparisonAtIndexImmediately (int index)
		{
			if(index >= 0 && index < rangeComparisons.Count)
				rangeComparisons[index].Update(true);
		}
		public void UpdateAllRangeComparisonsImmediately ()
		{
			for(int a = 0,A = rangeComparisons.Count; a < A; a++)
				rangeComparisons[a].Update(true);
		}
		public void UpdateOperatorComparisonAtIndexImmediately (int index)
		{
			if(index >= 0 && index < operatorComparisons.Count)
				operatorComparisons[index].Update(true);
		}
		public void UpdateAllOperatorComparisonsImmediately ()
		{
			for(int a = 0,A = operatorComparisons.Count; a < A; a++)
				operatorComparisons[a].Update(true);
		}
		public void UpdateRangeComparisonAtIndexPending (int index)
		{
			if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && index >= 0 && index < rangeComparisons.Count && !rangeComparisons[index].isUpdating)
				rangeComparisons[index].isUpdating = true;
		}
		public void UpdateAllRangeComparisonsPending ()
		{
			if(updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting)for(int a = 0,A = rangeComparisons.Count; a < A; a++)if(!rangeComparisons[a].isUpdating)
				rangeComparisons[a].isUpdating = true;
		}
		public void UpdateOperatorComparisonAtIndexPending (int index)
		{
			if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && index >= 0 && index < operatorComparisons.Count && !operatorComparisons[index].isUpdating)
				operatorComparisons[index].isUpdating = true;
		}
		public void UpdateAllOperatorComparisonsPending ()
		{
			if(updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting)for(int a = 0,A = operatorComparisons.Count; a < A; a++)if(!operatorComparisons[a].isUpdating)
				operatorComparisons[a].isUpdating = true;
		}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeComparison)),CanEditMultipleObjects]
	internal class TimeComparisonEditor : Editor
	{
		private TimeComparison timeComparison {get {return (TimeComparison)target;}}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			MainSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(timeComparison);
			}
		}
		private void MainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("MAIN",EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("updateMode"),true);
				if(!serializedObject.isEditingMultipleObjects)
				{
					MainSectionRangeComparisonsContainer();
					MainSectionOperatorComparisonsContainer();
				}
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Range Comparisons",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Operator Comparisons",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionRangeComparisonsContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Range Comparisons","Box",GUILayout.ExpandWidth(true)))
					{
						timeComparison.rangeComparisonsIsExpanded = !timeComparison.rangeComparisonsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = timeComparison.rangeComparisons.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						timeComparison.rangeComparisons.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(timeComparison.rangeComparisonsIsExpanded)
				{
					for(int a = 0; a < timeComparison.rangeComparisons.Count; a++)
					{
						TimeComparison.RangeComparison currentRangeComparison = timeComparison.rangeComparisons[a];
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button(currentRangeComparison.name,"Box",GUILayout.ExpandWidth(true)))
								{
									currentRangeComparison.isExpanded = !currentRangeComparison.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeComparison.RangeComparison previous = timeComparison.rangeComparisons[a - 1];
									timeComparison.rangeComparisons[a - 1] = currentRangeComparison;
									timeComparison.rangeComparisons[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != timeComparison.rangeComparisons.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeComparison.RangeComparison next = timeComparison.rangeComparisons[a + 1];
									timeComparison.rangeComparisons[a + 1] = currentRangeComparison;
									timeComparison.rangeComparisons[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									timeComparison.rangeComparisons.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentRangeComparison.isExpanded)
							{
								SerializedProperty currentRangeComparisonProperty = serializedObject.FindProperty("rangeComparisons").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 40;
										EditorGUILayout.PropertyField(currentRangeComparisonProperty.FindPropertyRelative("name"),true);
										MainSectionRangeComparisonsContainerOutputContainer(currentRangeComparison,currentRangeComparisonProperty);
										MainSectionRangeComparisonsContainerTimeContainer("Minimum Time",currentRangeComparison.minimumTime,currentRangeComparisonProperty.FindPropertyRelative("minimumTime"));
										MainSectionRangeComparisonsContainerTimeContainer("Time",currentRangeComparison.time,currentRangeComparisonProperty.FindPropertyRelative("time"));
										MainSectionRangeComparisonsContainerTimeContainer("Maximum Time",currentRangeComparison.maximumTime,currentRangeComparisonProperty.FindPropertyRelative("maximumTime"));
									}
									EditorGUILayout.EndVertical();
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUIUtility.labelWidth = 40;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("rangeComparisonsName"),new GUIContent("Name"),true);
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							timeComparison.rangeComparisons.Add(new TimeComparison.RangeComparison() {name = timeComparison.rangeComparisonsName});
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionRangeComparisonsContainerOutputContainer (TimeComparison.RangeComparison currentRangeComparison,SerializedProperty currentRangeComparisonProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("Output");
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical("Box");
				{
					EditorGUILayout.BeginHorizontal();
					{
						GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
						EditorGUILayout.BeginVertical("Box",GUILayout.Height(37));
						{
							GUILayout.FlexibleSpace();
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label(Application.isPlaying ? currentRangeComparison.output.ToString() : "[In Editor]");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical();
						{
							GUI.backgroundColor = currentRangeComparison.absolute ? Color.green : Color.red;
							if(GUILayout.Button("Absolute"))
							{
								Undo.RecordObjects(targets,"Inspector");
								currentRangeComparison.absolute = !currentRangeComparison.absolute;
								GUI.FocusControl(null);
							}
							GUI.backgroundColor = currentRangeComparison.clamp ? Color.green : Color.red;
							if(GUILayout.Button("Clamp"))
							{
								Undo.RecordObjects(targets,"Inspector");
								currentRangeComparison.clamp = !currentRangeComparison.clamp;
								GUI.FocusControl(null);
							}
							GUI.backgroundColor = Color.white;
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					{
						GUIStyle style = new GUIStyle() {fontSize = 8};
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label("Minimum Output",style);
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.PropertyField(currentRangeComparisonProperty.FindPropertyRelative("minimumOutput"),GUIContent.none,true);
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label("Maximum Output",style);
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.PropertyField(currentRangeComparisonProperty.FindPropertyRelative("maximumOutput"),GUIContent.none,true);
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionRangeComparisonsContainerTimeContainer (string name,TimeComparison.RangeComparison.Time currentTime,SerializedProperty currentTimeProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				if(GUILayout.Button(name,"Box",GUILayout.ExpandWidth(true)))
				{
					currentTime.isExpanded = !currentTime.isExpanded;
					GUI.FocusControl(null);
				}
				if(currentTime.isExpanded)
				{
					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Space(20);
						EditorGUILayout.BeginVertical();
						{
							EditorGUIUtility.labelWidth = 60;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("source"),true);
							EditorGUIUtility.labelWidth = 80;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("timeType"),true);
							if(currentTime.timeType == Time.TimeType.TwelveHour)
							{
								EditorGUILayout.BeginHorizontal();
								{
									GUI.enabled = !currentTime.source && currentTime.useHour;
									GUI.backgroundColor = currentTime.isAm ? Color.green : Color.red;
									if(GUILayout.Button("AM") && !currentTime.isAm)
									{
										Undo.RecordObject(target,"Inspector");
										currentTime.isAm = true;
										currentTime.isPm = false;
										currentTime.wasAm = !currentTime.wasAm;
										currentTime.wasPm = !currentTime.wasPm;
										GUI.FocusControl(null);
									}
									GUI.backgroundColor = currentTime.isPm ? Color.green : Color.red;
									if(GUILayout.Button("PM") && !currentTime.isPm)
									{
										Undo.RecordObject(target,"Inspector");
										currentTime.isAm = false;
										currentTime.isPm = true;
										currentTime.wasAm = !currentTime.wasAm;
										currentTime.wasPm = !currentTime.wasPm;
										GUI.FocusControl(null);
									}
									GUI.backgroundColor = Color.white;
									GUI.enabled = true;
								}
								EditorGUILayout.EndHorizontal();
							}
							EditorGUILayout.BeginVertical("Box");
							{
								EditorGUILayout.BeginHorizontal();
								{
									EditorGUIUtility.labelWidth = 1;
									GUI.enabled = !currentTime.source;
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useYear"),GUIContent.none,true);
									GUI.enabled = true;
									EditorGUIUtility.labelWidth = 0;
									GUILayout.Label("Year");
									GUILayout.FlexibleSpace();
								}
								EditorGUILayout.EndHorizontal();
								GUI.enabled = !currentTime.source && currentTime.useYear;
								EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("year"),GUIContent.none,true);
								GUI.enabled = true;
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										EditorGUIUtility.labelWidth = 1;
										GUI.enabled = !currentTime.source;
										EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useMonth"),GUIContent.none,true);
										GUI.enabled = true;
										EditorGUIUtility.labelWidth = 0;
										GUILayout.Label("Month");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									GUI.enabled = !currentTime.source && currentTime.useMonth;
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("month"),GUIContent.none,true);
									GUI.enabled = true;
								}
								EditorGUILayout.EndVertical();
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										EditorGUIUtility.labelWidth = 1;
										GUI.enabled = !currentTime.source;
										EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useDay"),GUIContent.none,true);
										GUI.enabled = true;
										EditorGUIUtility.labelWidth = 0;
										GUILayout.Label("Day");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									GUI.enabled = !currentTime.source && currentTime.useDay;
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("day"),GUIContent.none,true);
									GUI.enabled = true;
								}
								EditorGUILayout.EndVertical();
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										EditorGUIUtility.labelWidth = 1;
										GUI.enabled = !currentTime.source;
										EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useHour"),GUIContent.none,true);
										GUI.enabled = true;
										EditorGUIUtility.labelWidth = 0;
										GUILayout.Label("Hour");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									GUI.enabled = !currentTime.source && currentTime.useHour;
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("hour"),GUIContent.none,true);
									GUI.enabled = true;
								}
								EditorGUILayout.EndVertical();
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										EditorGUIUtility.labelWidth = 1;
										GUI.enabled = !currentTime.source;
										EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useMinute"),GUIContent.none,true);
										GUI.enabled = true;
										EditorGUIUtility.labelWidth = 0;
										GUILayout.Label("Minute");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									GUI.enabled = !currentTime.source && currentTime.useMinute;
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("minute"),GUIContent.none,true);
									GUI.enabled = true;
								}
								EditorGUILayout.EndVertical();
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										GUILayout.FlexibleSpace();
										GUILayout.Label("Second");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									GUI.enabled = !currentTime.source;
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("second"),GUIContent.none,true);
									GUI.enabled = true;
								}
								EditorGUILayout.EndVertical();
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										GUILayout.FlexibleSpace();
										GUILayout.Label("Delta");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									GUI.enabled = !currentTime.source;
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("delta"),GUIContent.none,true);
									GUI.enabled = true;
								}
								EditorGUILayout.EndVertical();
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionOperatorComparisonsContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Operator Comparisons","Box",GUILayout.ExpandWidth(true)))
					{
						timeComparison.operatorComparisonsIsExpanded = !timeComparison.operatorComparisonsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = timeComparison.operatorComparisons.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						timeComparison.operatorComparisons.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(timeComparison.operatorComparisonsIsExpanded)
				{
					for(int a = 0; a < timeComparison.operatorComparisons.Count; a++)
					{
						TimeComparison.OperatorComparison currentOperatorComparison = timeComparison.operatorComparisons[a];
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button(currentOperatorComparison.name,"Box",GUILayout.ExpandWidth(true)))
								{
									currentOperatorComparison.isExpanded = !currentOperatorComparison.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeComparison.OperatorComparison previous = timeComparison.operatorComparisons[a - 1];
									timeComparison.operatorComparisons[a - 1] = currentOperatorComparison;
									timeComparison.operatorComparisons[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != timeComparison.operatorComparisons.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeComparison.OperatorComparison next = timeComparison.operatorComparisons[a + 1];
									timeComparison.operatorComparisons[a + 1] = currentOperatorComparison;
									timeComparison.operatorComparisons[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									timeComparison.operatorComparisons.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentOperatorComparison.isExpanded)
							{
								SerializedProperty currentOperatorComparisonProperty = serializedObject.FindProperty("operatorComparisons").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 40;
										EditorGUILayout.PropertyField(currentOperatorComparisonProperty.FindPropertyRelative("name"),true);
										MainSectionOperatorComparisonsContainerOutputContainer(currentOperatorComparison.sign,currentOperatorComparison.output,currentOperatorComparisonProperty.FindPropertyRelative("output"));
										MainSectionOperatorComparisonsContainerTimesContainer(currentOperatorComparison,currentOperatorComparisonProperty);
									}
									EditorGUILayout.EndVertical();
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUIUtility.labelWidth = 40;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("operatorComparisonsName"),new GUIContent("Name"),true);
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							timeComparison.operatorComparisons.Add(new TimeComparison.OperatorComparison() {name = timeComparison.operatorComparisonsName});
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionOperatorComparisonsContainerOutputContainer (sbyte sign,AdvancedAssets.Time currentTime,SerializedProperty currentTimeProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("Output");
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical("Box");
				{
					EditorGUIUtility.labelWidth = 80;
					EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("timeType"),true);
					GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
					EditorGUILayout.BeginVertical("Box",GUILayout.Height(37));
					{
						GUILayout.FlexibleSpace();
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.BeginHorizontal("Box");
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label(Application.isPlaying ? "Sign: " + sign.ToString() : "[In Editor]");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							if(currentTime.timeType == Time.TimeType.TwelveHour)
							{
								EditorGUILayout.BeginHorizontal();
								{
									GUI.enabled = false;
									GUI.backgroundColor = currentTime.isAm ? Color.green : Color.red;
									if(GUILayout.Button("AM") && !currentTime.isAm)
									{
										Undo.RecordObject(target,"Inspector");
										currentTime.isAm = true;
										currentTime.isPm = false;
										currentTime.wasAm = !currentTime.wasAm;
										currentTime.wasPm = !currentTime.wasPm;
										GUI.FocusControl(null);
									}
									GUI.backgroundColor = currentTime.isPm ? Color.green : Color.red;
									if(GUILayout.Button("PM") && !currentTime.isPm)
									{
										Undo.RecordObject(target,"Inspector");
										currentTime.isAm = false;
										currentTime.isPm = true;
										currentTime.wasAm = !currentTime.wasAm;
										currentTime.wasPm = !currentTime.wasPm;
										GUI.FocusControl(null);
									}
									GUI.backgroundColor = Color.white;
									GUI.enabled = true;
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
					}
					EditorGUILayout.EndVertical();
					GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							GUI.backgroundColor = Color.white;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useYear"),GUIContent.none,true);
							GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
							EditorGUIUtility.labelWidth = 0;
							GUILayout.Label("Year");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = false;
						EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("year"),GUIContent.none,true);
						GUI.enabled = true;
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUIUtility.labelWidth = 1;
								GUI.backgroundColor = Color.white;
								EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useMonth"),GUIContent.none,true);
								GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
								EditorGUIUtility.labelWidth = 0;
								GUILayout.Label("Month");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							GUI.enabled = false;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("month"),GUIContent.none,true);
							GUI.enabled = true;
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUIUtility.labelWidth = 1;
								GUI.backgroundColor = Color.white;
								EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useDay"),GUIContent.none,true);
								GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
								EditorGUIUtility.labelWidth = 0;
								GUILayout.Label("Day");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							GUI.enabled = false;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("day"),GUIContent.none,true);
							GUI.enabled = true;
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUIUtility.labelWidth = 1;
								GUI.backgroundColor = Color.white;
								EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useHour"),GUIContent.none,true);
								GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
								EditorGUIUtility.labelWidth = 0;
								GUILayout.Label("Hour");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							GUI.enabled = false;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("hour"),GUIContent.none,true);
							GUI.enabled = true;
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUIUtility.labelWidth = 1;
								GUI.backgroundColor = Color.white;
								EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useMinute"),GUIContent.none,true);
								GUI.backgroundColor = Application.isPlaying ? Color.green : Color.yellow;
								EditorGUIUtility.labelWidth = 0;
								GUILayout.Label("Minute");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							GUI.enabled = false;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("minute"),GUIContent.none,true);
							GUI.enabled = true;
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label("Second");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							GUI.enabled = false;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("second"),GUIContent.none,true);
							GUI.enabled = true;
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label("Delta");
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
							GUI.enabled = false;
							EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("delta"),GUIContent.none,true);
							GUI.enabled = true;
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
					GUI.backgroundColor = Color.white;
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionOperatorComparisonsContainerTimesContainer (TimeComparison.OperatorComparison currentOperatorComparison,SerializedProperty currentOperatorComparisonProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Times","Box",GUILayout.ExpandWidth(true)))
					{
						currentOperatorComparison.timesIsExpanded = !currentOperatorComparison.timesIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = currentOperatorComparison.times.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						currentOperatorComparison.times.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(currentOperatorComparison.timesIsExpanded)
				{
					for(int a = 0; a < currentOperatorComparison.times.Count; a++)
					{
						TimeComparison.OperatorComparison.Time currentTime = currentOperatorComparison.times[a];
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUI.backgroundColor = currentTime._operator == TimeComparison.OperatorComparison.Time.Operator.Addition ? Color.green : Color.red;
							if(GUILayout.Button("+ : Addition"))
							{
								if(currentTime._operator != TimeComparison.OperatorComparison.Time.Operator.Addition)
								{
									Undo.RecordObject(target,"Inspector");
									currentTime._operator = TimeComparison.OperatorComparison.Time.Operator.Addition;
								}
								GUI.FocusControl(null);
							}
							GUI.backgroundColor = currentTime._operator == TimeComparison.OperatorComparison.Time.Operator.Subtraction ? Color.green : Color.red;
							if(GUILayout.Button("- : Subtraction"))
							{
								if(currentTime._operator != TimeComparison.OperatorComparison.Time.Operator.Subtraction)
								{
									Undo.RecordObject(target,"Inspector");
									currentTime._operator = TimeComparison.OperatorComparison.Time.Operator.Subtraction;
								}
								GUI.FocusControl(null);
							}
							GUI.backgroundColor = Color.white;
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button("Time " + (a + 1),"Box",GUILayout.ExpandWidth(true)))
								{
									currentTime.isExpanded = !currentTime.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeComparison.OperatorComparison.Time previous = currentOperatorComparison.times[a - 1];
									currentOperatorComparison.times[a - 1] = currentTime;
									currentOperatorComparison.times[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != currentOperatorComparison.times.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeComparison.OperatorComparison.Time next = currentOperatorComparison.times[a + 1];
									currentOperatorComparison.times[a + 1] = currentTime;
									currentOperatorComparison.times[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									currentOperatorComparison.times.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentTime.isExpanded)
							{
								SerializedProperty currentTimeProperty = currentOperatorComparisonProperty.FindPropertyRelative("times").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 60;
										EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("source"),true);
										EditorGUIUtility.labelWidth = 80;
										EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("timeType"),true);
										if(currentTime.timeType == Time.TimeType.TwelveHour)
										{
											EditorGUILayout.BeginHorizontal();
											{
												GUI.enabled = !currentTime.source && currentTime.useHour;
												GUI.backgroundColor = currentTime.isAm ? Color.green : Color.red;
												if(GUILayout.Button("AM") && !currentTime.isAm)
												{
													Undo.RecordObject(target,"Inspector");
													currentTime.isAm = true;
													currentTime.isPm = false;
													currentTime.wasAm = !currentTime.wasAm;
													currentTime.wasPm = !currentTime.wasPm;
													GUI.FocusControl(null);
												}
												GUI.backgroundColor = currentTime.isPm ? Color.green : Color.red;
												if(GUILayout.Button("PM") && !currentTime.isPm)
												{
													Undo.RecordObject(target,"Inspector");
													currentTime.isAm = false;
													currentTime.isPm = true;
													currentTime.wasAm = !currentTime.wasAm;
													currentTime.wasPm = !currentTime.wasPm;
													GUI.FocusControl(null);
												}
												GUI.backgroundColor = Color.white;
												GUI.enabled = true;
											}
											EditorGUILayout.EndHorizontal();
										}
										EditorGUILayout.BeginVertical("Box");
										{
											EditorGUILayout.BeginHorizontal();
											{
												EditorGUIUtility.labelWidth = 1;
												GUI.enabled = !currentTime.source;
												EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useYear"),GUIContent.none,true);
												GUI.enabled = true;
												EditorGUIUtility.labelWidth = 0;
												GUILayout.Label("Year");
												GUILayout.FlexibleSpace();
											}
											EditorGUILayout.EndHorizontal();
											GUI.enabled = !currentTime.source && currentTime.useYear;
											EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("year"),GUIContent.none,true);
											GUI.enabled = true;
										}
										EditorGUILayout.EndVertical();
										EditorGUILayout.BeginHorizontal();
										{
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = !currentTime.source;
													EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useMonth"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Month");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = !currentTime.source && currentTime.useMonth;
												EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("month"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = !currentTime.source;
													EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useDay"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Day");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = !currentTime.source && currentTime.useDay;
												EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("day"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
										}
										EditorGUILayout.EndHorizontal();
										EditorGUILayout.BeginHorizontal();
										{
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = !currentTime.source;
													EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useHour"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Hour");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = !currentTime.source && currentTime.useHour;
												EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("hour"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = !currentTime.source;
													EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("useMinute"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Minute");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = !currentTime.source && currentTime.useMinute;
												EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("minute"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
										}
										EditorGUILayout.EndHorizontal();
										EditorGUILayout.BeginHorizontal();
										{
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													GUILayout.FlexibleSpace();
													GUILayout.Label("Second");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = !currentTime.source;
												EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("second"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													GUILayout.FlexibleSpace();
													GUILayout.Label("Delta");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = !currentTime.source;
												EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("delta"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
										}
										EditorGUILayout.EndHorizontal();
									}
									EditorGUILayout.EndVertical();
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Add a new Time?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							currentOperatorComparison.times.Add(new TimeComparison.OperatorComparison.Time());
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
	}
	#endif
}