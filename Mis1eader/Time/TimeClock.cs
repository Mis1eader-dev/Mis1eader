namespace AdvancedAssets
{
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	[AddComponentMenu("Advanced Assets/Time/Time Clock",24)]
	public class TimeClock : MonoBehaviour
	{
		public enum UpdateMode {OnAwake,EveryFrame,ViaScripting}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		public TimeSystem source = null;
		public bool smoothHourNeedle = true;
		public bool smoothMinuteNeedle = true;
		public bool smoothSecondNeedle = true;
		public Transform hourNeedle = null;
		public Transform minuteNeedle = null;
		public Transform secondNeedle = null;
		[HideInInspector] public bool isUpdating = false;
		private void Awake ()
		{
			if(updateMode != UpdateMode.OnAwake)return;
			isUpdating = true;
			Update();
		}
		private void Update ()
		{
			if(!source || (updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && !isUpdating)return;
			if(isUpdating)isUpdating = false;
			if(smoothSecondNeedle || smoothMinuteNeedle || smoothHourNeedle)
			{
				float result = source.time.delta + source.time.second;
				if(source.time.useMinute)
				{
					if(source.time.useHour)result = result + source.time.minute * 60;
					else result = result + (source.time.minute % 86400) * 60;
				}
				if(source.time.useHour)result = result + (source.time.hour % 24) * 3600;
				if(secondNeedle && smoothSecondNeedle && secondNeedle.localRotation != Quaternion.Euler(0,0,-6 * (result % 60)))
					secondNeedle.localRotation = Quaternion.Euler(0,0,-6 * (result % 60));
				if(minuteNeedle && smoothMinuteNeedle && minuteNeedle.localRotation != Quaternion.Euler(0,0,-6 * (result % 3600) / 60))
					minuteNeedle.localRotation = Quaternion.Euler(0,0,-6 * (result % 3600) / 60);
				if(hourNeedle && smoothHourNeedle && hourNeedle.localRotation != Quaternion.Euler(0,0,-30 * (result % 86400) / 3600))
					hourNeedle.localRotation = Quaternion.Euler(0,0,-30 * (result % 86400) / 3600);
			}
			if(!smoothSecondNeedle || !smoothMinuteNeedle || !smoothHourNeedle)
			{
				if(secondNeedle && !smoothSecondNeedle && secondNeedle.localRotation != Quaternion.Euler(0,0,-6 * source.time.second))
					secondNeedle.localRotation = Quaternion.Euler(0,0,-6 * source.time.second);
				if(minuteNeedle && !smoothMinuteNeedle && minuteNeedle.localRotation != Quaternion.Euler(0,0,-6 * source.time.minute))
					minuteNeedle.localRotation = Quaternion.Euler(0,0,-6 * source.time.minute);
				if(hourNeedle && !smoothHourNeedle && hourNeedle.localRotation != Quaternion.Euler(0,0,-30 * source.time.hour))
					hourNeedle.localRotation = Quaternion.Euler(0,0,-30 * source.time.hour);
			}
		}
		public void SetUpdateMode (UpdateMode value) {if(updateMode != value)updateMode = value;}
		public void SetUpdateMode (int value)
		{
			UpdateMode convertedValue = (UpdateMode)value;
			if(updateMode != convertedValue)updateMode = convertedValue;
		}
		public void SetSource (TimeSystem value) {if(source != value)source = value;}
		public void SmoothHourNeedle (bool value) {if(smoothHourNeedle != value)smoothHourNeedle = value;}
		public void SmoothMinuteNeedle (bool value) {if(smoothMinuteNeedle != value)smoothMinuteNeedle = value;}
		public void SmoothSecondNeedle (bool value) {if(smoothSecondNeedle != value)smoothSecondNeedle = value;}
		public void SetHourNeedle (Transform value) {if(hourNeedle != value)hourNeedle = value;}
		public void SetHourNeedle (GameObject value) {if(hourNeedle != value.transform)hourNeedle = value.transform;}
		public void SetMinuteNeedle (Transform value) {if(minuteNeedle != value)minuteNeedle = value;}
		public void SetMinuteNeedle (GameObject value) {if(minuteNeedle != value.transform)minuteNeedle = value.transform;}
		public void SetSecondNeedle (Transform value) {if(secondNeedle != value)secondNeedle = value;}
		public void SetSecondNeedle (GameObject value) {if(secondNeedle != value.transform)secondNeedle = value.transform;}
		public void UpdateClockImmediately ()
		{
			if(!isUpdating)isUpdating = true;
			Update();
		}
		public void UpdateClockPending ()
		{
			if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && !isUpdating)
				isUpdating = true;
		}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeClock)),CanEditMultipleObjects]
	internal class TimeClockEditor : Editor
	{
		private TimeClock[] timeClocks
		{
			get
			{
				TimeClock[] timeClocks = new TimeClock[targets.Length];
				for(int timeClocksIndex = 0; timeClocksIndex < targets.Length; timeClocksIndex++)
					timeClocks[timeClocksIndex] = (TimeClock)targets[timeClocksIndex];
				return timeClocks;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			StatsSection();
			MainSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int timeClocksIndex = 0; timeClocksIndex < timeClocks.Length; timeClocksIndex++)
					EditorUtility.SetDirty(timeClocks[timeClocksIndex]);
			}
		}
		private void StatsSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUIStyle style = new GUIStyle() {fontSize = 11};
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("STATS",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Label("Elapsed Time:",style);
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					GUI.enabled = timeClocks[0].source;
					if(timeClocks[0].source)GUILayout.Label(timeClocks[0].source.time.year.ToString("0000") + " : " + timeClocks[0].source.time.month.ToString("00") + " : " + timeClocks[0].source.time.day.ToString("00"),style);
					else GUILayout.Label("0000 : 00 : 00",style);
					GUILayout.FlexibleSpace();
					if(!timeClocks[0].source || timeClocks[0].source && timeClocks[0].source.time.timeType == Time.TimeType.TwentyFourHour)GUILayout.Label(" | ",style);
					if(timeClocks[0].source && timeClocks[0].source.time.timeType == Time.TimeType.TwelveHour)
					{
						if(timeClocks[0].source.time.isAm)GUILayout.Label("| AM |",style);
						if(timeClocks[0].source.time.isPm)GUILayout.Label("| PM |",style);
					}
					GUILayout.FlexibleSpace();
					if(timeClocks[0].source)GUILayout.Label(timeClocks[0].source.time.hour.ToString("00") + " : " + timeClocks[0].source.time.minute.ToString("00") + " : " + timeClocks[0].source.time.second.ToString("00") + " : " + timeClocks[0].source.time.delta.ToString("0.00"),style);
					else GUILayout.Label("00 : 00 : 00 : 0.00",style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Label("Fixed Time:",style);
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					GUI.enabled = timeClocks[0].source;
					if(timeClocks[0].source)GUILayout.Label(timeClocks[0].source.time.year.ToString("0000") + " : " + (timeClocks[0].source.time.month + 1).ToString("00") + " : " + (timeClocks[0].source.time.day + 1).ToString("00"),style);
					else GUILayout.Label("0000 : 01 : 01",style);
					GUILayout.FlexibleSpace();
					if(!timeClocks[0].source || timeClocks[0].source && timeClocks[0].source.time.timeType == Time.TimeType.TwentyFourHour)GUILayout.Label(" | ",style);
					if(timeClocks[0].source && timeClocks[0].source.time.timeType == Time.TimeType.TwelveHour)
					{
						if(timeClocks[0].source.time.isAm)GUILayout.Label("| AM |",style);
						if(timeClocks[0].source.time.isPm)GUILayout.Label("| PM |",style);
					}
					GUILayout.FlexibleSpace();
					if(timeClocks[0].source)GUILayout.Label(timeClocks[0].source.time.hour.ToString("00") + " : " + timeClocks[0].source.time.minute.ToString("00") + " : " + timeClocks[0].source.time.second.ToString("00") + " : " + timeClocks[0].source.time.delta.ToString("0.00"),style);
					else GUILayout.Label("00 : 00 : 00 : 0.00",style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					string[] seasonNames = new string[] {"Spring","Summer","Autumn","Winter"};
					string[] monthNames = new string[] {"January","February","March","April","May","June","July","August","September","October","November","December"};
					string[] weekNames = new string[] {"Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"};
					GUILayout.FlexibleSpace();
					GUI.enabled = timeClocks[0].source && timeClocks[0].source.time.useSeason;
					GUILayout.Label(seasonNames[timeClocks[0].source ? (timeClocks[0].source.time.season - 1) : 3],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(" | ",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = timeClocks[0].source && timeClocks[0].source.time.useMonth;
					GUILayout.Label(monthNames[timeClocks[0].source ? (timeClocks[0].source.time.month % 12) : 0],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(" | ",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = timeClocks[0].source && timeClocks[0].source.time.useWeek;
					GUILayout.Label(weekNames[timeClocks[0].source ? (timeClocks[0].source.time.week - 1) : 6],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("MAIN",EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("updateMode"),true);
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("source"),true);
				EditorGUIUtility.labelWidth = 90;
				EditorGUILayout.BeginHorizontal();
				{
					GUI.backgroundColor = timeClocks[0].smoothHourNeedle ? Color.green : Color.red;
					if(GUILayout.Button("Smooth",GUILayout.Width(54)))
					{
						Undo.RecordObjects(targets,"Inspector");
						timeClocks[0].smoothHourNeedle = !timeClocks[0].smoothHourNeedle;
						for(int timeClocksIndex = 0; timeClocksIndex < timeClocks.Length; timeClocksIndex++)if(timeClocks[timeClocksIndex].smoothHourNeedle != timeClocks[0].smoothHourNeedle)
							timeClocks[timeClocksIndex].smoothHourNeedle = timeClocks[0].smoothHourNeedle;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("hourNeedle"),true);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					GUI.backgroundColor = timeClocks[0].smoothMinuteNeedle ? Color.green : Color.red;
					if(GUILayout.Button("Smooth",GUILayout.Width(54)))
					{
						Undo.RecordObjects(targets,"Inspector");
						timeClocks[0].smoothMinuteNeedle = !timeClocks[0].smoothMinuteNeedle;
						for(int timeClocksIndex = 0; timeClocksIndex < timeClocks.Length; timeClocksIndex++)if(timeClocks[timeClocksIndex].smoothMinuteNeedle != timeClocks[0].smoothMinuteNeedle)
							timeClocks[timeClocksIndex].smoothMinuteNeedle = timeClocks[0].smoothMinuteNeedle;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("minuteNeedle"),true);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					GUI.backgroundColor = timeClocks[0].smoothSecondNeedle ? Color.green : Color.red;
					if(GUILayout.Button("Smooth",GUILayout.Width(54)))
					{
						Undo.RecordObjects(targets,"Inspector");
						timeClocks[0].smoothSecondNeedle = !timeClocks[0].smoothSecondNeedle;
						for(int timeClocksIndex = 0; timeClocksIndex < timeClocks.Length; timeClocksIndex++)if(timeClocks[timeClocksIndex].smoothSecondNeedle != timeClocks[0].smoothSecondNeedle)
							timeClocks[timeClocksIndex].smoothSecondNeedle = timeClocks[0].smoothSecondNeedle;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("secondNeedle"),true);
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
	}
	#endif
}