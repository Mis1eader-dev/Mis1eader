namespace AdvancedAssets
{
	using UnityEngine;
	using UnityEngine.UI;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	[AddComponentMenu("Advanced Assets/Time/Time UI",25)]
	public class TimeUI : MonoBehaviour
	{
		public enum UpdateMode {OnAwake,EveryFrame,ViaScripting}
		public enum TimeUsage {Elapsed,Fixed}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		public TimeSystem source = null;
		public TimeUsage timeUsage = TimeUsage.Fixed;
		public Transform timePeriodNameUI = null;
		public Transform yearUI = null;
		public Transform seasonUI = null;
		public Transform seasonNameUI = null;
		public Transform monthUI = null;
		public Transform monthNameUI = null;
		public Transform weekUI = null;
		public Transform weekNameUI = null;
		public Transform dayUI = null;
		public Transform hourUI = null;
		public Transform minuteUI = null;
		public Transform secondUI = null;
		public Transform deltaUI = null;
		[HideInInspector] public bool isUpdating = false;
		private void Awake ()
		{
			if(!source || updateMode != UpdateMode.OnAwake)return;
			isUpdating = true;
			Update();
		}
		private void Update ()
		{
			if(!source || (updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && !isUpdating)return;
			if(isUpdating)isUpdating = false;
			if(timePeriodNameUI)
			{
				Text textObject = timePeriodNameUI.GetComponent<Text>();
				TextMesh textMeshObject = timePeriodNameUI.GetComponent<TextMesh>();
				if(textObject && textObject.text != (source.time.isAm ? "AM" : "PM"))
					textObject.text = source.time.isAm ? "AM" : "PM";
				if(textMeshObject && textMeshObject.text != (source.time.isAm ? "AM" : "PM"))
					textMeshObject.text = source.time.isAm ? "AM" : "PM";
			}
			if(source.time.useYear && yearUI)
			{
				Text textObject = yearUI.GetComponent<Text>();
				TextMesh textMeshObject = yearUI.GetComponent<TextMesh>();
				if(textObject && textObject.text != source.time.year.ToString("0000"))
					textObject.text = source.time.year.ToString("0000");
				if(textMeshObject && textMeshObject.text != source.time.year.ToString("0000"))
					textMeshObject.text = source.time.year.ToString("0000");
			}
			if(source.time.useSeason)
			{
				if(seasonUI && seasonNameUI && seasonUI == seasonNameUI)
					seasonUI = null;
				if(seasonUI)
				{
					Text textObject = seasonUI.GetComponent<Text>();
					TextMesh textMeshObject = seasonUI.GetComponent<TextMesh>();
					if(textObject && textObject.text != source.time.season.ToString())
						textObject.text = source.time.season.ToString();
					if(textMeshObject && textMeshObject.text != source.time.season.ToString())
						textMeshObject.text = source.time.season.ToString();
				}
				if(seasonNameUI)
				{
					string[] seasonNames = new string[] {"Spring","Summer","Autumn","Winter"};
					Text textObject = seasonNameUI.GetComponent<Text>();
					TextMesh textMeshObject = seasonNameUI.GetComponent<TextMesh>();
					if(textObject && textObject.text != seasonNames[source.time.season - 1])
						textObject.text = seasonNames[source.time.season - 1];
					if(textMeshObject && textMeshObject.text != seasonNames[source.time.season - 1])
						textMeshObject.text = seasonNames[source.time.season - 1];
				}
			}
			if(source.time.useMonth)
			{
				if(monthUI && monthNameUI && monthUI == monthNameUI)
					monthUI = null;
				if(monthUI)
				{
					Text textObject = monthUI.GetComponent<Text>();
					TextMesh textMeshObject = monthUI.GetComponent<TextMesh>();
					if(textObject && textObject.text != (timeUsage == TimeUsage.Elapsed ? source.time.month : (source.time.month + 1)).ToString("00"))
						textObject.text = (timeUsage == TimeUsage.Elapsed ? source.time.month : (source.time.month + 1)).ToString("00");
					if(textMeshObject && textMeshObject.text != (timeUsage == TimeUsage.Elapsed ? source.time.month : (source.time.month + 1)).ToString("00"))
						textMeshObject.text = (timeUsage == TimeUsage.Elapsed ? source.time.month : (source.time.month + 1)).ToString("00");
				}
				if(monthNameUI)
				{
					string[] monthNames = new string[] {"January","February","March","April","May","June","July","August","September","October","November","December"};
					Text textObject = monthNameUI.GetComponent<Text>();
					TextMesh textMeshObject = monthNameUI.GetComponent<TextMesh>();
					if(textObject && textObject.text != monthNames[source.time.month])
						textObject.text = monthNames[source.time.month];
					if(textMeshObject && textMeshObject.text != monthNames[source.time.month])
						textMeshObject.text = monthNames[source.time.month];
				}
			}
			if(source.time.useWeek)
			{
				if(weekUI && weekNameUI && weekUI == weekNameUI)
					weekUI = null;
				if(weekUI)
				{
					Text textObject = weekUI.GetComponent<Text>();
					TextMesh textMeshObject = weekUI.GetComponent<TextMesh>();
					if(textObject && textObject.text != source.time.week.ToString())
						textObject.text = source.time.week.ToString();
					if(textMeshObject && textMeshObject.text != source.time.week.ToString())
						textMeshObject.text = source.time.week.ToString();
				}
				if(weekNameUI)
				{
					string[] weekNames = new string[] {"Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"};
					Text textObject = weekNameUI.GetComponent<Text>();
					TextMesh textMeshObject = weekNameUI.GetComponent<TextMesh>();
					if(textObject && textObject.text != weekNames[source.time.week - 1])
						textObject.text = weekNames[source.time.week - 1];
					if(textMeshObject && textMeshObject.text != weekNames[source.time.week - 1])
						textMeshObject.text = weekNames[source.time.week - 1];
				}
			}
			if(source.time.useDay && dayUI)
			{
				Text textObject = dayUI.GetComponent<Text>();
				TextMesh textMeshObject = dayUI.GetComponent<TextMesh>();
				if(textObject && textObject.text != (timeUsage == TimeUsage.Elapsed ? source.time.day : (source.time.day + 1)).ToString("00"))
					textObject.text = (timeUsage == TimeUsage.Elapsed ? source.time.day : (source.time.day + 1)).ToString("00");
				if(textMeshObject && textMeshObject.text != (timeUsage == TimeUsage.Elapsed ? source.time.day : (source.time.day + 1)).ToString("00"))
					textMeshObject.text = (timeUsage == TimeUsage.Elapsed ? source.time.day : (source.time.day + 1)).ToString("00");
			}
			if(source.time.useHour && hourUI)
			{
				Text textObject = hourUI.GetComponent<Text>();
				TextMesh textMeshObject = hourUI.GetComponent<TextMesh>();
				if(textObject && textObject.text != source.time.hour.ToString("00"))
					textObject.text = source.time.hour.ToString("00");
				if(textMeshObject && textMeshObject.text != source.time.hour.ToString("00"))
					textMeshObject.text = source.time.hour.ToString("00");
			}
			if(source.time.useMinute && minuteUI)
			{
				Text textObject = minuteUI.GetComponent<Text>();
				TextMesh textMeshObject = minuteUI.GetComponent<TextMesh>();
				if(textObject && textObject.text != source.time.minute.ToString("00"))
					textObject.text = source.time.minute.ToString("00");
				if(textMeshObject && textMeshObject.text != source.time.minute.ToString("00"))
					textMeshObject.text = source.time.minute.ToString("00");
			}
			if(secondUI)
			{
				Text textObject = secondUI.GetComponent<Text>();
				TextMesh textMeshObject = secondUI.GetComponent<TextMesh>();
				if(textObject && textObject.text != source.time.second.ToString("00"))
					textObject.text = source.time.second.ToString("00");
				if(textMeshObject && textMeshObject.text != source.time.second.ToString("00"))
					textMeshObject.text = source.time.second.ToString("00");
			}
			if(deltaUI)
			{
				Text textObject = deltaUI.GetComponent<Text>();
				TextMesh textMeshObject = deltaUI.GetComponent<TextMesh>();
				if(textObject && textObject.text != (Mathf.Clamp(source.time.delta < 0 ? (1 - Mathf.Abs(source.time.delta)) : source.time.delta,0,0.99f) * 100).ToString("00"))
					textObject.text = (Mathf.Clamp(source.time.delta < 0 ? (1 - Mathf.Abs(source.time.delta)) : source.time.delta,0,0.99f) * 100).ToString("00");
				if(textMeshObject && textMeshObject.text != (Mathf.Clamp(source.time.delta < 0 ? (1 - Mathf.Abs(source.time.delta)) : source.time.delta,0,0.99f) * 100).ToString("00"))
					textMeshObject.text = (Mathf.Clamp(source.time.delta < 0 ? (1 - Mathf.Abs(source.time.delta)) : source.time.delta,0,0.99f) * 100).ToString("00");
			}
		}
		public void SetUpdateMode (UpdateMode value) {if(updateMode != value)updateMode = value;}
		public void SetUpdateMode (int value)
		{
			UpdateMode convertedValue = (UpdateMode)value;
			if(updateMode != convertedValue)updateMode = convertedValue;
		}
		public void SetTimeUsage (TimeUsage value) {if(timeUsage != value)timeUsage = value;}
		public void SetTimeUsage (int value)
		{
			TimeUsage convertedValue = (TimeUsage)value;
			if(timeUsage != convertedValue)timeUsage = convertedValue;
		}
		public void SetTimePeriodNameUI (Transform value) {if(timePeriodNameUI != value)timePeriodNameUI = value;}
		public void SetTimePeriodNameUI (GameObject value) {if(timePeriodNameUI != value.transform)timePeriodNameUI = value.transform;}
		public void SetYearUI (Transform value) {if(yearUI != value)yearUI = value;}
		public void SetYearUI (GameObject value) {if(yearUI != value.transform)yearUI = value.transform;}
		public void SetMonthUI (Transform value) {if(monthUI != value)monthUI = value;}
		public void SetMonthUI (GameObject value) {if(monthUI != value.transform)monthUI = value.transform;}
		public void SetMonthNameUI (Transform value) {if(monthNameUI != value)monthNameUI = value;}
		public void SetMonthNameUI (GameObject value) {if(monthNameUI != value.transform)monthNameUI = value.transform;}
		public void SetWeekUI (Transform value) {if(weekUI != value)weekUI = value;}
		public void SetWeekUI (GameObject value) {if(weekUI != value.transform)weekUI = value.transform;}
		public void SetWeekNameUI (Transform value) {if(weekNameUI != value)weekNameUI = value;}
		public void SetWeekNameUI (GameObject value) {if(weekNameUI != value.transform)weekNameUI = value.transform;}
		public void SetDayUI (Transform value) {if(dayUI != value)dayUI = value;}
		public void SetDayUI (GameObject value) {if(dayUI != value.transform)dayUI = value.transform;}
		public void SetHourUI (Transform value) {if(hourUI != value)hourUI = value;}
		public void SetHourUI (GameObject value) {if(hourUI != value.transform)hourUI = value.transform;}
		public void SetMinuteUI (Transform value) {if(minuteUI != value)minuteUI = value;}
		public void SetMinuteUI (GameObject value) {if(minuteUI != value.transform)minuteUI = value.transform;}
		public void SetSecondUI (Transform value) {if(secondUI != value)secondUI = value;}
		public void SetSecondUI (GameObject value) {if(secondUI != value.transform)secondUI = value.transform;}
		public void SetDeltaUI (Transform value) {if(deltaUI != value)deltaUI = value;}
		public void SetDeltaUI (GameObject value) {if(deltaUI != value.transform)deltaUI = value.transform;}
		public void UpdateUIImmediately ()
		{
			if(!isUpdating)isUpdating = true;
			Update();
		}
		public void UpdateUIPending ()
		{
			if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && !isUpdating)
				isUpdating = true;
		}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeUI)),CanEditMultipleObjects]
	internal class TimeUIEditor : Editor
	{
		private TimeUI[] timeUIs
		{
			get
			{
				TimeUI[] timeUIs = new TimeUI[targets.Length];
				for(int timeUIsIndex = 0; timeUIsIndex < targets.Length; timeUIsIndex++)
					timeUIs[timeUIsIndex] = (TimeUI)targets[timeUIsIndex];
				return timeUIs;
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
				for(int timeUIsIndex = 0; timeUIsIndex < timeUIs.Length; timeUIsIndex++)
					EditorUtility.SetDirty(timeUIs[timeUIsIndex]);
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
					GUI.enabled = timeUIs[0].source;
					if(timeUIs[0].source)GUILayout.Label(timeUIs[0].source.time.year.ToString("0000") + " : " + timeUIs[0].source.time.month.ToString("00") + " : " + timeUIs[0].source.time.day.ToString("00"),style);
					else GUILayout.Label("0000 : 00 : 00",style);
					GUILayout.FlexibleSpace();
					if(!timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.timeType == Time.TimeType.TwentyFourHour)GUILayout.Label(" | ",style);
					if(timeUIs[0].source && timeUIs[0].source.time.timeType == Time.TimeType.TwelveHour)
					{
						if(timeUIs[0].source.time.isAm)GUILayout.Label("| AM |",style);
						if(timeUIs[0].source.time.isPm)GUILayout.Label("| PM |",style);
					}
					GUILayout.FlexibleSpace();
					if(timeUIs[0].source)GUILayout.Label(timeUIs[0].source.time.hour.ToString("00") + " : " + timeUIs[0].source.time.minute.ToString("00") + " : " + timeUIs[0].source.time.second.ToString("00") + " : " + timeUIs[0].source.time.delta.ToString("0.00"),style);
					else GUILayout.Label("00 : 00 : 00 : 0.00",style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Label("Fixed Time:",style);
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					GUI.enabled = timeUIs[0].source;
					if(timeUIs[0].source)GUILayout.Label(timeUIs[0].source.time.year.ToString("0000") + " : " + (timeUIs[0].source.time.month + 1).ToString("00") + " : " + (timeUIs[0].source.time.day + 1).ToString("00"),style);
					else GUILayout.Label("0000 : 01 : 01",style);
					GUILayout.FlexibleSpace();
					if(!timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.timeType == Time.TimeType.TwentyFourHour)GUILayout.Label(" | ",style);
					if(timeUIs[0].source && timeUIs[0].source.time.timeType == Time.TimeType.TwelveHour)
					{
						if(timeUIs[0].source.time.isAm)GUILayout.Label("| AM |",style);
						if(timeUIs[0].source.time.isPm)GUILayout.Label("| PM |",style);
					}
					GUILayout.FlexibleSpace();
					if(timeUIs[0].source)GUILayout.Label(timeUIs[0].source.time.hour.ToString("00") + " : " + timeUIs[0].source.time.minute.ToString("00") + " : " + timeUIs[0].source.time.second.ToString("00") + " : " + timeUIs[0].source.time.delta.ToString("0.00"),style);
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
					GUI.enabled = timeUIs[0].source && timeUIs[0].source.time.useSeason;
					GUILayout.Label(seasonNames[timeUIs[0].source ? (timeUIs[0].source.time.season - 1) : 3],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(" | ",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = timeUIs[0].source && timeUIs[0].source.time.useMonth;
					GUILayout.Label(monthNames[timeUIs[0].source ? (timeUIs[0].source.time.month % 12) : 0],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(" | ",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = timeUIs[0].source && timeUIs[0].source.time.useWeek;
					GUILayout.Label(weekNames[timeUIs[0].source ? (timeUIs[0].source.time.week - 1) : 6],style);
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
				EditorGUIUtility.labelWidth = 0;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("timeUsage"),true);
				if(timeUIs[0].source && timeUIs[0].source.time.timeType == Time.TimeType.TwelveHour)
					EditorGUILayout.PropertyField(serializedObject.FindProperty("timePeriodNameUI"),true);
				GUI.enabled = !timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.useYear;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("yearUI"),true);
				GUI.enabled = !timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.useSeason;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("seasonUI"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("seasonNameUI"),true);
				GUI.enabled = !timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.useMonth;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("monthUI"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("monthNameUI"),true);
				GUI.enabled = !timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.useWeek;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("weekUI"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("weekNameUI"),true);
				GUI.enabled = !timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.useDay;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("dayUI"),true);
				GUI.enabled = !timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.useHour;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("hourUI"),true);
				GUI.enabled = !timeUIs[0].source || timeUIs[0].source && timeUIs[0].source.time.useMinute;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("minuteUI"),true);
				GUI.enabled = true;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("secondUI"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("deltaUI"),true);
			}
			EditorGUILayout.EndVertical();
		}
	}
	#endif
}