namespace AdvancedAssets
{
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	[AddComponentMenu("Advanced Assets/Time/Time Light",23),ExecuteInEditMode]
	public class TimeLight : MonoBehaviour
	{
		public enum UpdateMode {OnAwake,EveryFrame,ViaScripting}
		public TimeSystem source = null;
		public bool usePosition = false;
		public UpdateMode positionUpdateMode = UpdateMode.EveryFrame;
		public Vector3 position = Vector3.zero;
		public float distance = 100;
		public bool useRotation = false;
		public UpdateMode rotationUpdateMode = UpdateMode.EveryFrame;
		public float directionalAngle = 25;
		public bool useIntensity = false;
		public UpdateMode intensityUpdateMode = UpdateMode.EveryFrame;
		public new Light light = null;
		public float intensity = 1;
		public Time startFrom = new Time() {isAm = true,isPm = false,hour = 6,wasAm = true,wasPm = false};
		public Time startTo = new Time() {isAm = true,isPm = false,hour = 6,minute = 30,wasAm = true,wasPm = false};
		public Time endFrom = new Time() {isAm = false,isPm = true,hour = 17,minute = 30,wasAm = false,wasPm = true};
		public Time endTo = new Time() {isAm = false,isPm = true,hour = 18,wasAm = false,wasPm = true};
		#if UNITY_EDITOR
		[HideInInspector] public bool positionIsExpanded = false;
		[HideInInspector] public bool rotationIsExpanded = false;
		[HideInInspector] public bool intensityIsExpanded = false;
		#endif
		[HideInInspector] public bool positionIsUpdating = false;
		[HideInInspector] public bool rotationIsUpdating = false;
		[HideInInspector] public bool intensityIsUpdating = false;
		[HideInInspector,SerializeField] private bool onTwelveHourSwitch = false;
		[HideInInspector,SerializeField] private bool onTwentyFourHourSwitch = false;
		[HideInInspector,SerializeField] private bool isTwelveHourSwitch = false;
		[HideInInspector,SerializeField] private bool isTwentyFourHourSwitch = false;
		[System.Serializable] public class Time
		{
			public bool isAm = false;
			public bool isPm = false;
			#if UNITY_5_3_OR_NEWER
			[Delayed] public int hour = 0;
			[Delayed] public int minute = 0;
			[Delayed] public int second = 0;
			#else
			public int hour = 0;
			public int minute = 0;
			public int second = 0;
			#endif
			[HideInInspector,SerializeField] internal bool wasAm = false;
			[HideInInspector,SerializeField] internal bool wasPm = false;
			#if UNITY_EDITOR
			[HideInInspector] public bool isExpanded = false;
			#endif
			public void SetHour (int value) {if(hour != value)hour = value;}
			public void DecreaseHour (int value) {hour = hour - Mathf.Abs(value);}
			public void IncreaseHour (int value) {hour = hour + Mathf.Abs(value);}
			public void SetMinute (int value) {if(minute != value)minute = value;}
			public void DecreaseMinute (int value) {minute = minute - Mathf.Abs(value);}
			public void IncreaseMinute (int value) {minute = minute + Mathf.Abs(value);}
			public void SetSecond (int value) {if(second != value)second = value;}
			public void DecreaseSecond (int value) {second = second - Mathf.Abs(value);}
			public void IncreaseSecond (int value) {second = second + Mathf.Abs(value);}
			public void SetAm ()
			{
				if(!isAm)isAm = true;
				if(isPm)isPm = false;
				if(hour == 0)
				{
					wasAm = true;
					wasPm = false;
				}
				if(hour >= 1 && hour < 12)
				{
					wasAm = false;
					wasPm = true;
				}
			}
			public void SetPm ()
			{
				if(isAm)isAm = false;
				if(!isPm)isPm = true;
				if(hour > 12 && hour < 24)
				{
					wasAm = true;
					wasPm = false;
				}
				if(hour == 12)
				{
					wasAm = false;
					wasPm = true;
				}
			}
		}
		private void Awake ()
		{
			if(
			#if UNITY_EDITOR
			!Application.isPlaying ||
			#endif
			positionUpdateMode != UpdateMode.OnAwake || rotationUpdateMode != UpdateMode.OnAwake || intensityUpdateMode != UpdateMode.OnAwake)return;
			if(positionUpdateMode == UpdateMode.OnAwake)positionIsUpdating = true;
			if(rotationUpdateMode == UpdateMode.OnAwake)rotationIsUpdating = true;
			if(intensityUpdateMode == UpdateMode.OnAwake)intensityIsUpdating = true;
			Update();
		}
		private void Update ()
		{
			bool positionIsUpdating = positionUpdateMode == UpdateMode.EveryFrame || (positionUpdateMode == UpdateMode.OnAwake || positionUpdateMode == UpdateMode.ViaScripting) && this.positionIsUpdating;
			ValidationHandler();
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if(!usePosition || !positionIsUpdating)return;
				transform.position = transform.rotation * new Vector3(0,0,-distance) + position;
				return;
			}
			#endif
			if(!source)return;
			bool rotationIsUpdating = rotationUpdateMode == UpdateMode.EveryFrame || (rotationUpdateMode == UpdateMode.OnAwake || rotationUpdateMode == UpdateMode.ViaScripting) && this.rotationIsUpdating;
			bool intensityIsUpdating = intensityUpdateMode == UpdateMode.EveryFrame || (intensityUpdateMode == UpdateMode.OnAwake || intensityUpdateMode == UpdateMode.ViaScripting) && this.intensityIsUpdating;
			if(positionIsUpdating || rotationIsUpdating || intensityIsUpdating)
				ExecutionHandler(positionIsUpdating,rotationIsUpdating,intensityIsUpdating);
		}
		private void ValidationHandler ()
		{
			directionalAngle = Mathf.Clamp(directionalAngle,-360,360);
			if(!useIntensity)return;
			SwitchHandler(source ? source.time.timeType == AdvancedAssets.Time.TimeType.TwelveHour : false,source ? source.time.timeType == AdvancedAssets.Time.TimeType.TwentyFourHour : true);
			if(source && source.time.useHour)
			{
				SwitchHandler(ref startFrom.hour,ref startFrom.isAm,ref startFrom.isPm,ref startFrom.wasAm,ref startFrom.wasPm);
				SwitchHandler(ref startTo.hour,ref startTo.isAm,ref startTo.isPm,ref startTo.wasAm,ref startTo.wasPm);
				SwitchHandler(ref endFrom.hour,ref endFrom.isAm,ref endFrom.isPm,ref endFrom.wasAm,ref endFrom.wasPm);
				SwitchHandler(ref endTo.hour,ref endTo.isAm,ref endTo.isPm,ref endTo.wasAm,ref endTo.wasPm);
			}
			CalculationHandler(ref startFrom.second,ref startFrom.minute,ref startFrom.hour,ref startFrom.isAm,ref startFrom.isPm,ref startFrom.wasAm,ref startFrom.wasPm);
			CalculationHandler(ref startTo.second,ref startTo.minute,ref startTo.hour,ref startTo.isAm,ref startTo.isPm,ref startTo.wasAm,ref startTo.wasPm);
			CalculationHandler(ref endFrom.second,ref endFrom.minute,ref endFrom.hour,ref endFrom.isAm,ref endFrom.isPm,ref endFrom.wasAm,ref endFrom.wasPm);
			CalculationHandler(ref endTo.second,ref endTo.minute,ref endTo.hour,ref endTo.isAm,ref endTo.isPm,ref endTo.wasAm,ref endTo.wasPm);
		}
		private void SwitchHandler (bool isTwelveHour,bool isTwentyFourHour)
		{
			if(onTwelveHourSwitch)onTwelveHourSwitch = false;
			if(onTwentyFourHourSwitch)onTwentyFourHourSwitch = false;
			if(isTwelveHourSwitch != isTwelveHour)
			{
				isTwelveHourSwitch = isTwelveHour;
				if(isTwelveHour)onTwelveHourSwitch = true;
			}
			if(isTwentyFourHourSwitch != isTwentyFourHour)
			{
				isTwentyFourHourSwitch = isTwentyFourHour;
				if(isTwentyFourHour)onTwentyFourHourSwitch = true;
			}
		}
		private void SwitchHandler (ref int hour,ref bool isAm,ref bool isPm,ref bool wasAm,ref bool wasPm)
		{
			if(onTwelveHourSwitch)
			{
				if(hour < 12)
				{
					if(!isAm)isAm = true;
					if(isPm)isPm = false;
					if(hour == 0)
					{
						hour = hour + 12;
						wasAm = true;
						wasPm = false;
					}
					if(hour >= 1 && hour < 12)
					{
						wasAm = false;
						wasPm = true;
					}
					return;
				}
				if(hour >= 12)
				{
					if(isAm)isAm = false;
					if(!isPm)isPm = true;
					if(hour > 12 && hour < 24)
					{
						hour = hour - 12;
						wasAm = true;
						wasPm = false;
					}
					if(hour == 12)
					{
						wasAm = false;
						wasPm = true;
					}
					return;
				}
			}
			if(onTwentyFourHourSwitch)
			{
				if(isAm && hour == 12)hour = hour - 12;
				if(isPm && hour >= 1 && hour < 12)hour = hour + 12;
			}
		}
		private void CalculationHandler (ref int second,ref int minute,ref int hour,ref bool isAm,ref bool isPm,ref bool wasAm,ref bool wasPm)
		{
			while(second >= 60)
			{
				second = second - 60;
				minute = minute + 1;
			}
			while(minute >= 60)
			{
				minute = minute - 60;
				hour = hour + 1;
			}
			if(source && source.time.timeType == AdvancedAssets.Time.TimeType.TwelveHour)
			{
				if(hour >= 12 && wasAm)
				{
					if(!isAm)isAm = true;
					if(isPm)isPm = false;
				}
				if(hour >= 12 && wasPm)
				{
					if(isAm)isAm = false;
					if(!isPm)isPm = true;
				}
				if(hour >= 1 && hour < 12 && wasAm)
				{
					if(isAm)isAm = false;
					if(!isPm)isPm = true;
				}
				if(hour >= 1 && hour < 12 && wasPm)
				{
					if(!isAm)isAm = true;
					if(isPm)isPm = false;
				}
				while(hour <= 0)
				{
					hour = hour + 12;
					if(wasAm)
					{
						if(isAm)isAm = false;
						if(!isPm)isPm = true;
						wasAm = false;
						wasPm = true;
						continue;
					}
					if(wasPm)
					{
						if(!isAm)isAm = true;
						if(isPm)isPm = false;
						wasAm = true;
						wasPm = false;
					}
				}
				while(hour > 12)
				{
					hour = hour - 12;
					if(wasAm)
					{
						if(!isAm)isAm = true;
						if(isPm)isPm = false;
						wasAm = false;
						wasPm = true;
						continue;
					}
					if(wasPm)
					{
						if(isAm)isAm = false;
						if(!isPm)isPm = true;
						wasAm = true;
						wasPm = false;
					}
				}
			}
			if(!source || source && source.time.timeType == AdvancedAssets.Time.TimeType.TwentyFourHour)
			{
				if(hour >= 0 && hour < 12)
				{
					if(!isAm)isAm = true;
					if(isPm)isPm = false;
				}
				if(hour >= 12 && hour < 24)
				{
					if(isAm)isAm = false;
					if(!isPm)isPm = true;
				}
				while(hour >= 24)
				{
					if(!isAm)isAm = true;
					if(isPm)isPm = false;
					hour = hour - 24;
				}
			}
			while(second < 0 && minute > 0 || second < 0 && minute <= 0 && hour > 0)
			{
				second = 59;
				minute = minute - 1;
			}
			while(minute < 0 && hour > 0)
			{
				minute = 59;
				hour = hour - 1;
			}
		}
		private void ExecutionHandler (bool positionIsUpdating,bool rotationIsUpdating,bool intensityIsUpdating)
		{
			int totalTime = TotalTime(source.time.second,source.time.minute,source.time.hour,source.time.isAm,source.time.isPm);
			if(usePosition && positionIsUpdating || useRotation && rotationIsUpdating)
			{
				Quaternion rotation = Quaternion.Euler(new Vector3(((source.speedType == TimeSystem.SpeedType.Rotation ? totalTime * source.speed : totalTime) / 360 * 1.5f) - 90,directionalAngle,0));
				if(usePosition && positionIsUpdating)
				{
					if(this.positionIsUpdating)this.positionIsUpdating = false;
					transform.position = rotation * new Vector3(0,0,-distance) + position;
				}
				if(useRotation && rotationIsUpdating)
				{
					if(this.rotationIsUpdating)this.rotationIsUpdating = false;
					transform.rotation = rotation;
				}
			}
			if(!useIntensity || !light || !intensityIsUpdating)return;
			if(this.intensityIsUpdating)this.intensityIsUpdating = false;
			if(source.time.useMinute && source.time.useHour)
			{
				int startFrom = TotalTime(this.startFrom.second,this.startFrom.minute,this.startFrom.hour,this.startFrom.isAm,this.startFrom.isPm);
				int startTo = TotalTime(this.startTo.second,this.startTo.minute,this.startTo.hour,this.startTo.isAm,this.startTo.isPm);
				int endFrom = TotalTime(this.endFrom.second,this.endFrom.minute,this.endFrom.hour,this.endFrom.isAm,this.endFrom.isPm);
				int endTo = TotalTime(this.endTo.second,this.endTo.minute,this.endTo.hour,this.endTo.isAm,this.endTo.isPm);
				if((totalTime < startFrom || totalTime > endTo) && light.intensity != 0)
					light.intensity = 0;
				if(totalTime >= startTo && totalTime <= endFrom && light.intensity != this.intensity)
					light.intensity = this.intensity;
				if(totalTime >= startFrom && totalTime < startTo)
				{
					float intensity = RangeConversion(totalTime,startFrom,startTo,0,1) * this.intensity;
					if(light.intensity != intensity)
						light.intensity = intensity;
				}
				if(totalTime > endFrom && totalTime <= endTo)
				{
					float intensity = RangeConversion(totalTime,endFrom,endTo,1,0) * this.intensity;
					if(light.intensity != intensity)
						light.intensity = intensity;
				}
			}
			else if(light.intensity != this.intensity)
				light.intensity = this.intensity;
		}
		private int TotalTime (int second,int minute,int hour,bool isAm,bool isPm)
		{
			while(second >= 60)
			{
				second = second - 60;
				minute = minute + 1;
			}
			while(minute >= 60)
			{
				minute = minute - 60;
				hour = hour + 1;
			}
			hour = hour % 24;
			if(source.time.timeType == AdvancedAssets.Time.TimeType.TwelveHour)
			{
				if(isAm && hour == 12)hour = hour - 12;
				if(isPm && hour >= 1 && hour < 12)hour = hour + 12;
			}
			return second + minute * 60 + hour * 3600;
		}
		private static float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimumValue != maximumValue ? minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum) : minimum;}
		public void SetSource (TimeSystem value) {if(source != value)source = value;}
		public void UsePosition (bool value) {if(usePosition != value)usePosition = value;}
		public void SetPositionUpdateMode (UpdateMode value) {if(positionUpdateMode != value)positionUpdateMode = value;}
		public void SetPositionUpdateMode (int value)
		{
			UpdateMode convertedValue = (UpdateMode)value;
			if(positionUpdateMode != convertedValue)positionUpdateMode = convertedValue;
		}
		public void SetPosition (Vector2 value)
		{
			Vector3 convertedValue = new Vector3(value.x,value.y,0);
			if(position != convertedValue)position = convertedValue;
		}
		public void SetPosition (Vector3 value) {if(position != value)position = value;}
		public void SetPosition (Vector4 value)
		{
			Vector3 convertedValue = new Vector3(value.x,value.y,value.z);
			if(position != convertedValue)position = convertedValue;
		}
		public void SetDistance (float value) {if(distance != value)distance = value;}
		public void UseRotation (bool value) {if(useRotation != value)useRotation = value;}
		public void SetRotationUpdateMode (UpdateMode value) {if(rotationUpdateMode != value)rotationUpdateMode = value;}
		public void SetRotationUpdateMode (int value)
		{
			UpdateMode convertedValue = (UpdateMode)value;
			if(rotationUpdateMode != convertedValue)rotationUpdateMode = convertedValue;
		}
		public void SetDirectionalAngle (float value) {if(directionalAngle != value)directionalAngle = value;}
		public void UseIntensity (bool value) {if(useIntensity != value)useIntensity = value;}
		public void SetIntensityUpdateMode (UpdateMode value) {if(intensityUpdateMode != value)intensityUpdateMode = value;}
		public void SetIntensityUpdateMode (int value)
		{
			UpdateMode convertedValue = (UpdateMode)value;
			if(intensityUpdateMode != convertedValue)intensityUpdateMode = convertedValue;
		}
		public void SetLight (Light value) {if(light != value)light = value;}
		public void SetIntensity (float value) {if(intensity != value)intensity = value;}
		public void DecreaseIntensity (float value) {if(intensity > 0)intensity = Mathf.Clamp(intensity - value,0,float.MaxValue);}
		public void IncreaseIntensity (float value) {if(intensity < float.MaxValue)intensity = Mathf.Clamp(intensity + value,0,float.MaxValue);}
		public void SetStartFrom (Time value) {if(startFrom != value)startFrom = value;}
		public void SetStartTo (Time value) {if(startTo != value)startTo = value;}
		public void SetEndFrom (Time value) {if(endFrom != value)endFrom = value;}
		public void SetEndTo (Time value) {if(endTo != value)endTo = value;}
		public void UpdateAllImmediately ()
		{
			UpdatePositionImmediately();
			UpdateRotationImmediately();
			UpdateIntensityImmediately();
		}
		public void UpdatePositionImmediately ()
		{
			if(!positionIsUpdating)positionIsUpdating = true;
			Update();
		}
		public void UpdateRotationImmediately ()
		{
			if(!rotationIsUpdating)rotationIsUpdating = true;
			Update();
		}
		public void UpdateIntensityImmediately ()
		{
			if(!intensityIsUpdating)intensityIsUpdating = true;
			Update();
		}
		public void UpdateAllPending ()
		{
			UpdatePositionPending();
			UpdateRotationPending();
			UpdateIntensityPending();
		}
		public void UpdatePositionPending ()
		{
			if((positionUpdateMode == UpdateMode.OnAwake || positionUpdateMode == UpdateMode.ViaScripting) && !positionIsUpdating)
				positionIsUpdating = true;
		}
		public void UpdateRotationPending ()
		{
			if((rotationUpdateMode == UpdateMode.OnAwake || rotationUpdateMode == UpdateMode.ViaScripting) && !rotationIsUpdating)
				rotationIsUpdating = true;
		}
		public void UpdateIntensityPending ()
		{
			if((intensityUpdateMode == UpdateMode.OnAwake || intensityUpdateMode == UpdateMode.ViaScripting) && !intensityIsUpdating)
				intensityIsUpdating = true;
		}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeLight)),CanEditMultipleObjects]
	internal class TimeLightEditor : Editor
	{
		private TimeLight[] timeLights
		{
			get
			{
				TimeLight[] timeLights = new TimeLight[targets.Length];
				for(int timeLightsIndex = 0; timeLightsIndex < targets.Length; timeLightsIndex++)
					timeLights[timeLightsIndex] = (TimeLight)targets[timeLightsIndex];
				return timeLights;
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
				for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)
					EditorUtility.SetDirty(timeLights[timeLightsIndex]);
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
					GUI.enabled = timeLights[0].source;
					if(timeLights[0].source)GUILayout.Label(timeLights[0].source.time.year.ToString("0000") + " : " + timeLights[0].source.time.month.ToString("00") + " : " + timeLights[0].source.time.day.ToString("00"),style);
					else GUILayout.Label("0000 : 00 : 00",style);
					GUILayout.FlexibleSpace();
					if(!timeLights[0].source || timeLights[0].source && timeLights[0].source.time.timeType == AdvancedAssets.Time.TimeType.TwentyFourHour)GUILayout.Label(" | ",style);
					if(timeLights[0].source && timeLights[0].source.time.timeType == AdvancedAssets.Time.TimeType.TwelveHour)
					{
						if(timeLights[0].source.time.isAm)GUILayout.Label("| AM |",style);
						if(timeLights[0].source.time.isPm)GUILayout.Label("| PM |",style);
					}
					GUILayout.FlexibleSpace();
					if(timeLights[0].source)GUILayout.Label(timeLights[0].source.time.hour.ToString("00") + " : " + timeLights[0].source.time.minute.ToString("00") + " : " + timeLights[0].source.time.second.ToString("00") + " : " + timeLights[0].source.time.delta.ToString("0.00"),style);
					else GUILayout.Label("00 : 00 : 00 : 0.00",style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Label("Fixed Time:",style);
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					GUI.enabled = timeLights[0].source;
					if(timeLights[0].source)GUILayout.Label(timeLights[0].source.time.year.ToString("0000") + " : " + (timeLights[0].source.time.month + 1).ToString("00") + " : " + (timeLights[0].source.time.day + 1).ToString("00"),style);
					else GUILayout.Label("0000 : 01 : 01",style);
					GUILayout.FlexibleSpace();
					if(!timeLights[0].source || timeLights[0].source && timeLights[0].source.time.timeType == AdvancedAssets.Time.TimeType.TwentyFourHour)GUILayout.Label(" | ",style);
					if(timeLights[0].source && timeLights[0].source.time.timeType == AdvancedAssets.Time.TimeType.TwelveHour)
					{
						if(timeLights[0].source.time.isAm)GUILayout.Label("| AM |",style);
						if(timeLights[0].source.time.isPm)GUILayout.Label("| PM |",style);
					}
					GUILayout.FlexibleSpace();
					if(timeLights[0].source)GUILayout.Label(timeLights[0].source.time.hour.ToString("00") + " : " + timeLights[0].source.time.minute.ToString("00") + " : " + timeLights[0].source.time.second.ToString("00") + " : " + timeLights[0].source.time.delta.ToString("0.00"),style);
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
					GUI.enabled = timeLights[0].source && timeLights[0].source.time.useSeason;
					GUILayout.Label(seasonNames[timeLights[0].source ? (timeLights[0].source.time.season - 1) : 3],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(" | ",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = timeLights[0].source && timeLights[0].source.time.useMonth;
					GUILayout.Label(monthNames[timeLights[0].source ? (timeLights[0].source.time.month % 12) : 0],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(" | ",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = timeLights[0].source && timeLights[0].source.time.useWeek;
					GUILayout.Label(weekNames[timeLights[0].source ? (timeLights[0].source.time.week - 1) : 6],style);
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
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("source"),true);
				EditorGUIUtility.labelWidth = 0;
				MainSectionPositionContainer();
				MainSectionRotationContainer();
				MainSectionLightContainer();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionPositionContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("usePosition"),true,GUILayout.Width(15));
					EditorGUIUtility.labelWidth = 0;
					if(GUILayout.Button("Position",EditorStyles.label,GUILayout.ExpandWidth(true)))
					{
						timeLights[0].positionIsExpanded = !timeLights[0].positionIsExpanded;
						for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(timeLights[timeLightsIndex].positionIsExpanded != timeLights[0].positionIsExpanded)
							timeLights[timeLightsIndex].positionIsExpanded = timeLights[0].positionIsExpanded;
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndHorizontal();
				if(timeLights[0].positionIsExpanded)
				{
					GUI.enabled = timeLights[0].usePosition;
					EditorGUIUtility.labelWidth = 140;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("positionUpdateMode"),true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("position"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"),true);
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionRotationContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useRotation"),true,GUILayout.Width(15));
					EditorGUIUtility.labelWidth = 0;
					if(GUILayout.Button("Rotation",EditorStyles.label,GUILayout.ExpandWidth(true)))
					{
						timeLights[0].rotationIsExpanded = !timeLights[0].rotationIsExpanded;
						for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(timeLights[timeLightsIndex].rotationIsExpanded != timeLights[0].rotationIsExpanded)
							timeLights[timeLightsIndex].rotationIsExpanded = timeLights[0].rotationIsExpanded;
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndHorizontal();
				if(timeLights[0].rotationIsExpanded)
				{
					GUI.enabled = timeLights[0].useRotation;
					EditorGUIUtility.labelWidth = 140;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationUpdateMode"),true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("directionalAngle"),true);
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionLightContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useIntensity"),true,GUILayout.Width(15));
					EditorGUIUtility.labelWidth = 0;
					if(GUILayout.Button("Light",EditorStyles.label,GUILayout.ExpandWidth(true)))
					{
						timeLights[0].intensityIsExpanded = !timeLights[0].intensityIsExpanded;
						for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(timeLights[timeLightsIndex].intensityIsExpanded != timeLights[0].intensityIsExpanded)
							timeLights[timeLightsIndex].intensityIsExpanded = timeLights[0].intensityIsExpanded;
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndHorizontal();
				if(timeLights[0].intensityIsExpanded)
				{
					GUI.enabled = timeLights[0].useIntensity;
					EditorGUIUtility.labelWidth = 140;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("intensityUpdateMode"),true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("light"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("intensity"),true);
					MainSectionLightContainerTimeContainer("Start From",timeLights[0].startFrom,serializedObject.FindProperty("startFrom"));
					MainSectionLightContainerTimeContainer("Start To",timeLights[0].startTo,serializedObject.FindProperty("startTo"));
					MainSectionLightContainerTimeContainer("End From",timeLights[0].endFrom,serializedObject.FindProperty("endFrom"));
					MainSectionLightContainerTimeContainer("End To",timeLights[0].endTo,serializedObject.FindProperty("endTo"));
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionLightContainerTimeContainer (string name,TimeLight.Time currentTime,SerializedProperty currentTimeProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				if(GUILayout.Button(name,"Box",GUILayout.ExpandWidth(true)))
				{
					currentTime.isExpanded = !currentTime.isExpanded;
					if(name == "Start From")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(timeLights[timeLightsIndex].startFrom.isExpanded != currentTime.isExpanded)
						timeLights[timeLightsIndex].startFrom.isExpanded = currentTime.isExpanded;
					if(name == "Start To")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(timeLights[timeLightsIndex].startTo.isExpanded != currentTime.isExpanded)
						timeLights[timeLightsIndex].startTo.isExpanded = currentTime.isExpanded;
					if(name == "End From")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(timeLights[timeLightsIndex].endFrom.isExpanded != currentTime.isExpanded)
						timeLights[timeLightsIndex].endFrom.isExpanded = currentTime.isExpanded;
					if(name == "End To")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(timeLights[timeLightsIndex].endTo.isExpanded != currentTime.isExpanded)
						timeLights[timeLightsIndex].endTo.isExpanded = currentTime.isExpanded;
					GUI.FocusControl(null);
				}
				if(currentTime.isExpanded)
				{
					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Space(20);
						EditorGUILayout.BeginVertical();
						{
							if(timeLights[0].source && timeLights[0].source.time.timeType == AdvancedAssets.Time.TimeType.TwelveHour)
							{
								bool canChangeAm = false;
								bool canChangePm = false;
								if(serializedObject.isEditingMultipleObjects)for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)
								{
									if(name == "Start From")
									{
										if(!timeLights[timeLightsIndex].startFrom.isAm && !canChangeAm)canChangeAm = true;
										if(!timeLights[timeLightsIndex].startFrom.isPm && !canChangePm)canChangePm = true;
									}
									if(name == "Start To")
									{
										if(!timeLights[timeLightsIndex].startTo.isAm && !canChangeAm)canChangeAm = true;
										if(!timeLights[timeLightsIndex].startTo.isPm && !canChangePm)canChangePm = true;
									}
									if(name == "End From")
									{
										if(!timeLights[timeLightsIndex].endFrom.isAm && !canChangeAm)canChangeAm = true;
										if(!timeLights[timeLightsIndex].endFrom.isPm && !canChangePm)canChangePm = true;
									}
									if(name == "End To")
									{
										if(!timeLights[timeLightsIndex].endTo.isAm && !canChangeAm)canChangeAm = true;
										if(!timeLights[timeLightsIndex].endTo.isPm && !canChangePm)canChangePm = true;
									}
									if(canChangeAm && canChangePm)break;
								}
								else
								{
									if(!currentTime.isAm)canChangeAm = true;
									if(!currentTime.isPm)canChangePm = true;
								}
								EditorGUILayout.BeginHorizontal();
								{
									GUI.backgroundColor = currentTime.isAm ? Color.green : Color.red;
									if(GUILayout.Button("AM") && canChangeAm)
									{
										Undo.RecordObjects(targets,"Inspector");
										if(name == "Start From")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].startFrom.isAm)
										{
											timeLights[timeLightsIndex].startFrom.isAm = true;
											timeLights[timeLightsIndex].startFrom.isPm = false;
											timeLights[timeLightsIndex].startFrom.wasAm = !timeLights[timeLightsIndex].startFrom.wasAm;
											timeLights[timeLightsIndex].startFrom.wasPm = !timeLights[timeLightsIndex].startFrom.wasPm;
										}
										if(name == "Start To")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].startTo.isAm)
										{
											timeLights[timeLightsIndex].startTo.isAm = true;
											timeLights[timeLightsIndex].startTo.isPm = false;
											timeLights[timeLightsIndex].startTo.wasAm = !timeLights[timeLightsIndex].startTo.wasAm;
											timeLights[timeLightsIndex].startTo.wasPm = !timeLights[timeLightsIndex].startTo.wasPm;
										}
										if(name == "End From")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].endFrom.isAm)
										{
											timeLights[timeLightsIndex].endFrom.isAm = true;
											timeLights[timeLightsIndex].endFrom.isPm = false;
											timeLights[timeLightsIndex].endFrom.wasAm = !timeLights[timeLightsIndex].endFrom.wasAm;
											timeLights[timeLightsIndex].endFrom.wasPm = !timeLights[timeLightsIndex].endFrom.wasPm;
										}
										if(name == "End To")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].endTo.isAm)
										{
											timeLights[timeLightsIndex].endTo.isAm = true;
											timeLights[timeLightsIndex].endTo.isPm = false;
											timeLights[timeLightsIndex].endTo.wasAm = !timeLights[timeLightsIndex].endTo.wasAm;
											timeLights[timeLightsIndex].endTo.wasPm = !timeLights[timeLightsIndex].endTo.wasPm;
										}
										GUI.FocusControl(null);
									}
									GUI.backgroundColor = currentTime.isPm ? Color.green : Color.red;
									if(GUILayout.Button("PM") && canChangePm)
									{
										Undo.RecordObjects(targets,"Inspector");
										if(name == "Start From")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].startFrom.isPm)
										{
											timeLights[timeLightsIndex].startFrom.isAm = false;
											timeLights[timeLightsIndex].startFrom.isPm = true;
											timeLights[timeLightsIndex].startFrom.wasAm = !timeLights[timeLightsIndex].startFrom.wasAm;
											timeLights[timeLightsIndex].startFrom.wasPm = !timeLights[timeLightsIndex].startFrom.wasPm;
										}
										if(name == "Start To")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].startTo.isPm)
										{
											timeLights[timeLightsIndex].startTo.isAm = false;
											timeLights[timeLightsIndex].startTo.isPm = true;
											timeLights[timeLightsIndex].startTo.wasAm = !timeLights[timeLightsIndex].startTo.wasAm;
											timeLights[timeLightsIndex].startTo.wasPm = !timeLights[timeLightsIndex].startTo.wasPm;
										}
										if(name == "End From")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].endFrom.isPm)
										{
											timeLights[timeLightsIndex].endFrom.isAm = false;
											timeLights[timeLightsIndex].endFrom.isPm = true;
											timeLights[timeLightsIndex].endFrom.wasAm = !timeLights[timeLightsIndex].endFrom.wasAm;
											timeLights[timeLightsIndex].endFrom.wasPm = !timeLights[timeLightsIndex].endFrom.wasPm;
										}
										if(name == "End To")for(int timeLightsIndex = 0; timeLightsIndex < timeLights.Length; timeLightsIndex++)if(!timeLights[timeLightsIndex].endTo.isPm)
										{
											timeLights[timeLightsIndex].endTo.isAm = false;
											timeLights[timeLightsIndex].endTo.isPm = true;
											timeLights[timeLightsIndex].endTo.wasAm = !timeLights[timeLightsIndex].endTo.wasAm;
											timeLights[timeLightsIndex].endTo.wasPm = !timeLights[timeLightsIndex].endTo.wasPm;
										}
										GUI.FocusControl(null);
									}
									GUI.backgroundColor = Color.white;
								}
								EditorGUILayout.EndHorizontal();
							}
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										GUILayout.FlexibleSpace();
										GUILayout.Label("Hour");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("hour"),GUIContent.none,true);
								}
								EditorGUILayout.EndVertical();
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										GUILayout.FlexibleSpace();
										GUILayout.Label("Minute");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("minute"),GUIContent.none,true);
								}
								EditorGUILayout.EndVertical();
								EditorGUILayout.BeginVertical("Box");
								{
									EditorGUILayout.BeginHorizontal();
									{
										GUILayout.FlexibleSpace();
										GUILayout.Label("Second");
										GUILayout.FlexibleSpace();
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.PropertyField(currentTimeProperty.FindPropertyRelative("second"),GUIContent.none,true);
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
	}
	#endif
}