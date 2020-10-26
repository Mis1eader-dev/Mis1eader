namespace AdvancedAssets
{
	using UnityEngine;
	using UnityEngine.Events;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using System.Collections.Generic;
	[AddComponentMenu("Advanced Assets/Time/Time Event",21),ExecuteInEditMode]
	public class TimeEvent : MonoBehaviour
	{
		#if UNITY_EDITOR
		[HideInInspector] public string eventsName = "Untitled";
		#endif
		public List<Event> events = new List<Event>();
		#if UNITY_EDITOR
		[HideInInspector] public bool eventsIsExpanded = true;
		#endif
		[System.Serializable] public class Event
		{
			public string name = string.Empty;
			public List<Condition> conditions = new List<Condition>();
			public UnityEvent onTrue = new UnityEvent();
			public UnityEvent onFalse = new UnityEvent();
			public UnityEvent isTrue = new UnityEvent();
			public UnityEvent isFalse = new UnityEvent();
			[HideInInspector] public bool state = false;
			#if UNITY_EDITOR
			[HideInInspector] public bool isExpanded = false;
			[HideInInspector] public bool conditionsIsExpanded = true;
			#endif
			[System.Serializable] public class Condition : Time
			{
				public enum Statement {And,Or}
				public enum Operator {LessThan,LessThanOrEqualTo,NotEqualTo,EqualTo,GreaterThanOrEqualTo,GreaterThan}
				public TimeSystem source = null;
				public Statement statement = Statement.And;
				public Operator @operator = Operator.EqualTo;
				public bool useSeason = false;
				#if UNITY_5_3_OR_NEWER
				[Delayed] public byte season = 4;
				#else
				public byte season = 4;
				#endif
				public bool useWeek = false;
				#if UNITY_5_3_OR_NEWER
				[Delayed] public byte week = 7;
				#else
				public byte week = 7;
				#endif
				public bool useSecond = false;
				public bool useDelta = false;
				#if UNITY_EDITOR
				[HideInInspector] public bool isExpanded = false;
				#endif
				public new void Update ()
				{
					base.Update();
					if(useWeek)
					{
						if(useDelta)useDelta = false;
						if(useSecond)useSecond = false;
						if(useMinute)useMinute = false;
						if(useHour)useHour = false;
						if(useDay)useDay = false;
						if(useMonth)useMonth = false;
						if(useSeason)useSeason = false;
						if(useYear)useYear = false;
					}
					if(useSeason)
					{
						if(useDelta)useDelta = false;
						if(useSecond)useSecond = false;
						if(useMinute)useMinute = false;
						if(useHour)useHour = false;
						if(useDay)useDay = false;
						if(useWeek)useWeek = false;
						if(useMonth)useMonth = false;
						if(useYear)useYear = false;
					}
					if(source)
					{
						if(!source.time.useMinute && useMinute)useMinute = false;
						if(!source.time.useHour && useHour)useHour = false;
						if(!source.time.useDay && useDay)useDay = false;
						if(!source.time.useWeek && useWeek)useWeek = false;
						if(!source.time.useMonth && useMonth)useMonth = false;
						if(!source.time.useSeason && useSeason)useSeason = false;
						if(!source.time.useYear && useYear)useYear = false;
					}
					if(@operator == Operator.LessThan && (useDelta || useSecond || useMinute || useHour || useDay || useMonth || useYear) && delta == 0 && second == 0 && minute == 0 && hour == 0 && day == 0 && month == 0 && year == 0)
					{
						#if UNITY_EDITOR
						Debug.LogError("The event time cannot be less than 0");
						#endif
						@operator = Operator.EqualTo;
					}
					if((@operator == Operator.NotEqualTo || @operator == Operator.EqualTo) && useDelta)
					{
						#if UNITY_EDITOR
						Debug.LogError("The event will mostly return " + (@operator == Operator.NotEqualTo ? "true" : "false") + " with delta being enabled");
						#endif
						useDelta = false;
					}
					#if UNITY_EDITOR
					if(!useWeek || !useSeason)
					{
						if(useDelta || useSecond || useMinute || useHour || useDay || useMonth || useYear)
						{
							uint _year = year;
							int _month = month;
							int _day = day;
							ConversionHandler(useYear,delta,second,minute,hour,day,month,ref _year,timeType == TimeType.TwelveHour,isAm,isPm);
							ConversionHandler(useYear,delta,second,minute,hour,ref _day,ref _month,ref _year,timeType == TimeType.TwelveHour,isAm,isPm);
							if(!useWeek)week = Week(_day,_month,_year);
							if(!useSeason)season = Season(_month);
						}
						else
						{
							if(!useWeek && week != 7)week = 7;
							if(!useSeason && season != 4)season = 4;
						}
					}
					if(useWeek)
					{
						while(week > 7)week -= 7;
						while(week < 1)week += 7;
					}
					if(useSeason)
					{
						while(season > 4)season -= 4;
						while(season < 1)season += 4;
					}
					#else
					while(week > 7)week -= 7;
					while(week < 1)week += 7;
					while(season > 4)season -= 4;
					while(season < 1)season += 4;
					#endif
					if(!useDelta && delta != 0)delta = 0;
					if(!useSecond && second != 0)second = 0;
				}
				#if UNITY_EDITOR
				private byte Week (int day,int month,uint year)
				{
					int result = day * 864;
					byte[] days = new byte[] {31,28,31,30,31,30,31,31,30,31,30,31};
					for(int a = 0; a < month % 12; a++)
						result = result + 864 * days[a];
					year = year % 2000;
					return (byte)(1 + (6 + (result + year * 315360 + (year / 4) * 864 + (year / 400) * 864  - (year / 100) * 864 + (year % 4 == 0 && month >= 2 ? 864 : 0) + (year % 4 > 0 ? 864 : 0)) / 864) % 7);
				}
				private byte Season (int month)
				{
					if(month == 2 || month == 3 || month == 4)return 1;
					if(month == 5 || month == 6 || month == 7)return 2;
					if(month == 8 || month == 9 || month == 10)return 3;
					if(month == 11 || month == 0 || month == 1)return 4;
					return 4;
				}
				#endif
				public void SetSource (TimeSystem value) {if(source != value)source = value;}
				public void SetStatement (Statement value) {if(statement != value)statement = value;}
				public void SetStatement (int value)
				{
					Statement convertedValue = (Statement)value;
					if(statement != convertedValue)statement = convertedValue;
				}
				public void SetOperator (Operator value) {if(@operator != value)@operator = value;}
				public void SetOperator (int value)
				{
					Operator convertedValue = (Operator)value;
					if(@operator != convertedValue)@operator = convertedValue;
				}
				public void UseSeason (bool value) {if(useSeason != value)useSeason = value;}
				public void UseWeek (bool value) {if(useWeek != value)useWeek = value;}
				public void UseSecond (bool value) {if(useSecond != value)useSecond = value;}
				public void UseDelta (bool value) {if(useDelta != value)useDelta = value;}
			}
			public void Update ()
			{
				ValidationHandler();
				#if UNITY_EDITOR
				if(!Application.isPlaying)return;
				#endif
				if(conditions.Count == 0)return;
				EventHandler();
			}
			private void ValidationHandler () {for(int a = 0,A = conditions.Count; a < A; a++)conditions[a].Update();}
			private void EventHandler ()
			{
				bool isPassed = false;
				for(int a = 0,A = conditions.Count; a < A; a++)
				{
					Condition condition = conditions[a];
					if(a != 0)
					{
						if(condition.statement == Condition.Statement.And && !isPassed)continue;
						if(condition.statement == Condition.Statement.Or && isPassed)break;
					}
					if(!condition.source)continue;
					Condition.Operator @operator = condition.@operator;
					if(!condition.useWeek && !condition.useSeason)
					{
						if(@operator == Condition.Operator.NotEqualTo)
						{
							isPassed = condition.useYear && condition.source.time.year != condition.year ||
									   condition.useMonth && condition.source.time.month != condition.month ||
									   condition.useDay && condition.source.time.day != condition.day ||
									   condition.useHour && condition.source.time.hour != condition.hour ||
									   condition.useMinute && condition.source.time.minute != condition.minute ||
									   condition.useSecond && condition.source.time.second != condition.second;
							continue;
						}
						if((@operator == Condition.Operator.LessThanOrEqualTo || @operator == Condition.Operator.EqualTo || @operator == Condition.Operator.GreaterThanOrEqualTo) && (condition.useYear || condition.useMonth || condition.useDay || condition.useHour || condition.useMinute || condition.useSecond || condition.useDelta))
						{
							isPassed = (!condition.useYear || condition.source.time.year == condition.year) &&
									   (!condition.useMonth || condition.source.time.month == condition.month) &&
									   (!condition.useDay || condition.source.time.day == condition.day) &&
									   (!condition.useHour || condition.source.time.hour == condition.hour) &&
									   (!condition.useMinute || condition.source.time.minute == condition.minute) &&
									   (!condition.useSecond || condition.source.time.second == condition.second) &&
									   (!condition.useDelta || condition.source.time.delta == condition.delta);
							if(isPassed)continue;
						}
						if(@operator == Condition.Operator.LessThan || @operator == Condition.Operator.LessThanOrEqualTo)
						{
							if(condition.useYear)
							{
								if(condition.source.time.year < condition.year)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.year > condition.year)continue;
							}
							if(condition.useMonth)
							{
								if(condition.source.time.month < condition.month)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.month > condition.month)continue;
							}
							if(condition.useDay)
							{
								if(condition.source.time.day < condition.day)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.day > condition.day)continue;
							}
							if(condition.useHour)
							{
								int sourceHour = condition.source.time.hour;
								int conditionHour = condition.hour;
								if(condition.source.time.timeType == Time.TimeType.TwelveHour)
								{
									if(condition.source.time.isAm && sourceHour == 12)sourceHour = sourceHour - 12;
									if(condition.source.time.isPm && sourceHour >= 1 && sourceHour < 12)sourceHour = sourceHour + 12;
									if(condition.isAm && conditionHour == 12)conditionHour = conditionHour - 12;
									if(condition.isPm && conditionHour >= 1 && conditionHour < 12)conditionHour = conditionHour + 12;
								}
								if(sourceHour < conditionHour)
								{
									isPassed = true;
									continue;
								}
								if(sourceHour > conditionHour)continue;
							}
							if(condition.useMinute)
							{
								if(condition.source.time.minute < condition.minute)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.minute > condition.minute)continue;
							}
							if(condition.useSecond)
							{
								if(condition.source.time.second < condition.second)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.second > condition.second)continue;
							}
							if(condition.useDelta)
							{
								if(condition.source.time.delta < condition.delta)
								{
									isPassed = true;
									continue;
								}
								else continue;
							}
							continue;
						}
						if(@operator == Condition.Operator.GreaterThanOrEqualTo || @operator == Condition.Operator.GreaterThan)
						{
							if(condition.useYear)
							{
								if(condition.source.time.year > condition.year)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.year < condition.year)continue;
							}
							if(condition.useMonth)
							{
								if(condition.source.time.month > condition.month)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.month < condition.month)continue;
							}
							if(condition.useDay)
							{
								if(condition.source.time.day > condition.day)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.day < condition.day)continue;
							}
							if(condition.useHour)
							{
								int sourceHour = condition.source.time.hour;
								int conditionHour = condition.hour;
								if(condition.source.time.timeType == Time.TimeType.TwelveHour)
								{
									if(condition.source.time.isAm && sourceHour == 12)sourceHour = sourceHour - 12;
									if(condition.source.time.isPm && sourceHour >= 1 && sourceHour < 12)sourceHour = sourceHour + 12;
									if(condition.isAm && conditionHour == 12)conditionHour = conditionHour - 12;
									if(condition.isPm && conditionHour >= 1 && conditionHour < 12)conditionHour = conditionHour + 12;
								}
								if(sourceHour > conditionHour)
								{
									isPassed = true;
									continue;
								}
								if(sourceHour < conditionHour)continue;
							}
							if(condition.useMinute)
							{
								if(condition.source.time.minute > condition.minute)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.minute < condition.minute)continue;
							}
							if(condition.useSecond)
							{
								if(condition.source.time.second > condition.second)
								{
									isPassed = true;
									continue;
								}
								if(condition.source.time.second < condition.second)continue;
							}
							if(condition.useDelta)
							{
								if(condition.source.time.delta > condition.delta)
								{
									isPassed = true;
									continue;
								}
								else continue;
							}
							continue;
						}
					}
					else
					{
						if(@operator == Condition.Operator.LessThan)
						{
							isPassed = condition.useWeek ? condition.source.time.week < condition.week : condition.source.time.season < condition.season;
							continue;
						}
						if(@operator == Condition.Operator.LessThanOrEqualTo)
						{
							isPassed = condition.useWeek ? condition.source.time.week <= condition.week : condition.source.time.season <= condition.season;
							continue;
						}
						if(@operator == Condition.Operator.NotEqualTo)
						{
							isPassed = condition.useWeek ? condition.source.time.week != condition.week : condition.source.time.season != condition.season;
							continue;
						}
						if(@operator == Condition.Operator.EqualTo)
						{
							isPassed = condition.useWeek ? condition.source.time.week == condition.week : condition.source.time.season == condition.season;
							continue;
						}
						if(@operator == Condition.Operator.GreaterThanOrEqualTo)
						{
							isPassed = condition.useWeek ? condition.source.time.week >= condition.week : condition.source.time.season >= condition.season;
							continue;
						}
						if(@operator == Condition.Operator.GreaterThan)
						{
							isPassed = condition.useWeek ? condition.source.time.week > condition.week : condition.source.time.season > condition.season;
							continue;
						}
					}
				}
				if(isPassed)
				{
					if(!state)
					{
						onTrue.Invoke();
						state = true;
					}
					isTrue.Invoke();
				}
				else
				{
					if(state)
					{
						onFalse.Invoke();
						state = false;
					}
					isFalse.Invoke();
				}
			}
			public void SetName (string value) {if(name != value)name = value;}
			public void SetConditions (List<Condition> value)
			{
				int A = value.Count;
				if(conditions.Count != A)conditions = new List<Condition>(new Condition[A]);
				for(int a = 0; a < A; a++)if(conditions[a] != value[a])conditions[a] = value[a];
			}
			public void SetConditions (Condition[] value)
			{
				List<Condition> convertedValue = new List<Condition>(value);
				if(conditions != convertedValue)conditions = convertedValue;
			}
			public void SetOnTrue (UnityEvent value) {if(onTrue != value)onTrue = value;}
			public void SetOnFalse (UnityEvent value) {if(onFalse != value)onFalse = value;}
			public void SetIsTrue (UnityEvent value) {if(isTrue != value)isTrue = value;}
			public void SetIsFalse (UnityEvent value) {if(isFalse != value)isFalse = value;}
		}
		private void Update () {for(int a = 0,A = events.Count; a < A; a++)events[a].Update();}
		public void SetEvents (List<Event> value)
		{
			int A = value.Count;
			if(events.Count != A)events = new List<Event>(new Event[A]);
			for(int a = 0; a < A; a++)if(events[a] != value[a])events[a] = value[a];
		}
		public void SetEvents (Event[] value)
		{
			List<Event> convertedValue = new List<Event>(value);
			if(events != convertedValue)events = convertedValue;
		}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeEvent)),CanEditMultipleObjects]
	internal class TimeEventEditor : Editor
	{
		private TimeEvent timeEvent {get {return (TimeEvent)target;}}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			MainSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(timeEvent);
			}
		}
		private void MainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("MAIN",EditorStyles.boldLabel);
				if(!serializedObject.isEditingMultipleObjects)MainSectionEventsContainer();
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Events",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionEventsContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Events","Box",GUILayout.ExpandWidth(true)))
					{
						timeEvent.eventsIsExpanded = !timeEvent.eventsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = timeEvent.events.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						timeEvent.events.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(timeEvent.eventsIsExpanded)
				{
					for(int a = 0; a < timeEvent.events.Count; a++)
					{
						TimeEvent.Event currentEvent = timeEvent.events[a];
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								Color defaultColor = GUI.color;
								GUI.color = Application.isPlaying ? (currentEvent.state ? Color.green : Color.red) : Color.yellow;
								GUILayout.Box(a.ToString("000"));
								GUI.color = defaultColor;
								if(GUILayout.Button(currentEvent.name,"Box",GUILayout.ExpandWidth(true)))
								{
									currentEvent.isExpanded = !currentEvent.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeEvent.Event previous = timeEvent.events[a - 1];
									timeEvent.events[a - 1] = currentEvent;
									timeEvent.events[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != timeEvent.events.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeEvent.Event next = timeEvent.events[a + 1];
									timeEvent.events[a + 1] = currentEvent;
									timeEvent.events[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									timeEvent.events.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentEvent.isExpanded)
							{
								SerializedProperty currentEventProperty = serializedObject.FindProperty("events").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 40;
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("name"),true);
										EditorGUIUtility.labelWidth = 0;
										MainSectionEventsContainerConditionsContainer(currentEvent,currentEventProperty);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("onTrue"),true);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("onFalse"),true);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("isTrue"),true);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("isFalse"),true);
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
						EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsName"),new GUIContent("Name"),true);
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							timeEvent.events.Add(new TimeEvent.Event() {name = timeEvent.eventsName});
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionEventsContainerConditionsContainer (TimeEvent.Event currentEvent,SerializedProperty currentEventProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Conditions","Box",GUILayout.ExpandWidth(true)))
					{
						currentEvent.conditionsIsExpanded = !currentEvent.conditionsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = currentEvent.conditions.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						currentEvent.conditions.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(currentEvent.conditionsIsExpanded)
				{
					for(int a = 0; a < currentEvent.conditions.Count; a++)
					{
						TimeEvent.Event.Condition currentCondition = currentEvent.conditions[a];
						if(a > 0)
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.FlexibleSpace();
								GUI.backgroundColor = currentCondition.statement == TimeEvent.Event.Condition.Statement.And ? Color.green : Color.red;
								if(GUILayout.Button("&& : And"))
								{
									if(currentCondition.statement != TimeEvent.Event.Condition.Statement.And)
									{
										Undo.RecordObject(target,"Inspector");
										currentCondition.statement = TimeEvent.Event.Condition.Statement.And;
									}
									GUI.FocusControl(null);
								}
								GUI.backgroundColor = currentCondition.statement == TimeEvent.Event.Condition.Statement.Or ? Color.green : Color.red;
								if(GUILayout.Button("|| : Or"))
								{
									if(currentCondition.statement != TimeEvent.Event.Condition.Statement.Or)
									{
										Undo.RecordObject(target,"Inspector");
										currentCondition.statement = TimeEvent.Event.Condition.Statement.Or;
									}
									GUI.FocusControl(null);
								}
								GUI.backgroundColor = Color.white;
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button("Condition " + (a + 1),"Box",GUILayout.ExpandWidth(true)))
								{
									currentCondition.isExpanded = !currentCondition.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeEvent.Event.Condition previous = currentEvent.conditions[a - 1];
									currentEvent.conditions[a - 1] = currentCondition;
									currentEvent.conditions[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != currentEvent.conditions.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									TimeEvent.Event.Condition next = currentEvent.conditions[a + 1];
									currentEvent.conditions[a + 1] = currentCondition;
									currentEvent.conditions[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									currentEvent.conditions.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentCondition.isExpanded)
							{
								SerializedProperty currentConditionProperty = currentEventProperty.FindPropertyRelative("conditions").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 60;
										EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("source"),true);
										EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("operator"),true);
										if((currentCondition.source ? currentCondition.source.time.timeType == Time.TimeType.TwelveHour : false))
										{
											EditorGUILayout.BeginHorizontal();
											{
												GUI.enabled = currentCondition.useHour;
												GUI.backgroundColor = currentCondition.isAm ? Color.green : Color.red;
												if(GUILayout.Button("AM") && !currentCondition.isAm)
												{
													Undo.RecordObject(target,"Inspector");
													currentCondition.isAm = true;
													currentCondition.isPm = false;
													currentCondition.wasAm = !currentCondition.wasAm;
													currentCondition.wasPm = !currentCondition.wasPm;
													GUI.FocusControl(null);
												}
												GUI.backgroundColor = currentCondition.isPm ? Color.green : Color.red;
												if(GUILayout.Button("PM") && !currentCondition.isPm)
												{
													Undo.RecordObject(target,"Inspector");
													currentCondition.isAm = false;
													currentCondition.isPm = true;
													currentCondition.wasAm = !currentCondition.wasAm;
													currentCondition.wasPm = !currentCondition.wasPm;
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
												GUI.enabled = (currentCondition.source && currentCondition.source.time.useSeason || !currentCondition.source) && !currentCondition.useDelta && !currentCondition.useSecond && !currentCondition.useMinute && !currentCondition.useHour && !currentCondition.useDay && !currentCondition.useWeek && !currentCondition.useMonth && !currentCondition.useYear;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useSeason"),GUIContent.none,true);
												GUI.enabled = true;
												EditorGUIUtility.labelWidth = 0;
												GUILayout.Label("Season");
												GUILayout.FlexibleSpace();
											}
											EditorGUILayout.EndHorizontal();
											GUI.enabled = currentCondition.useSeason && !currentCondition.useDelta && !currentCondition.useSecond && !currentCondition.useMinute && !currentCondition.useHour && !currentCondition.useDay && !currentCondition.useWeek && !currentCondition.useMonth && !currentCondition.useYear;
											EditorGUILayout.BeginHorizontal();
											{
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("season"),GUIContent.none,true);
												EditorGUI.BeginChangeCheck();
												int season = EditorGUILayout.Popup(currentCondition.season - 1,new string[] {"Spring","Summer","Autumn","Winter"});
												if(EditorGUI.EndChangeCheck())
												{
													Undo.RecordObject(target,"Inspector");
													currentCondition.season = (byte)(season + 1);
												}
											}
											EditorGUILayout.EndHorizontal();
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
													GUI.enabled = (currentCondition.source && currentCondition.source.time.useYear || !currentCondition.source) && !currentCondition.useWeek && !currentCondition.useSeason;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useYear"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Year");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useYear && !currentCondition.useWeek && !currentCondition.useSeason;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("year"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = (currentCondition.source && currentCondition.source.time.useMonth || !currentCondition.source) && !currentCondition.useWeek && !currentCondition.useSeason;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useMonth"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Month");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useMonth && !currentCondition.useWeek && !currentCondition.useSeason;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("month"),GUIContent.none,true);
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
													GUI.enabled = (currentCondition.source && currentCondition.source.time.useWeek || !currentCondition.source) && !currentCondition.useDelta && !currentCondition.useSecond && !currentCondition.useMinute && !currentCondition.useHour && !currentCondition.useDay && !currentCondition.useMonth && !currentCondition.useSeason && !currentCondition.useYear;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useWeek"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Week");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useWeek && !currentCondition.useDelta && !currentCondition.useSecond && !currentCondition.useMinute && !currentCondition.useHour && !currentCondition.useDay && !currentCondition.useMonth && !currentCondition.useSeason && !currentCondition.useYear;
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.fieldWidth = 3;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("week"),GUIContent.none,true);
													EditorGUIUtility.fieldWidth = 0;
													EditorGUI.BeginChangeCheck();
													int week = EditorGUILayout.Popup(currentCondition.week - 1,new string[] {"Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"});
													if(EditorGUI.EndChangeCheck())
													{
														Undo.RecordObject(target,"Inspector");
														currentCondition.week = (byte)(week + 1);
													}
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = (currentCondition.source && currentCondition.source.time.useDay || !currentCondition.source) && !currentCondition.useWeek && !currentCondition.useSeason;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useDay"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Day");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useDay && !currentCondition.useWeek && !currentCondition.useSeason;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("day"),GUIContent.none,true);
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
													GUI.enabled = (currentCondition.source && currentCondition.source.time.useHour || !currentCondition.source) && !currentCondition.useWeek && !currentCondition.useSeason;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useHour"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Hour");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useHour && !currentCondition.useWeek && !currentCondition.useSeason;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("hour"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = (currentCondition.source && currentCondition.source.time.useMinute || !currentCondition.source) && !currentCondition.useWeek && !currentCondition.useSeason;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useMinute"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Minute");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useMinute && !currentCondition.useWeek && !currentCondition.useSeason;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("minute"),GUIContent.none,true);
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
													GUI.enabled = !currentCondition.useWeek && !currentCondition.useSeason;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useSecond"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Second");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useSecond && !currentCondition.useWeek && !currentCondition.useSeason;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("second"),GUIContent.none,true);
												GUI.enabled = true;
											}
											EditorGUILayout.EndVertical();
											EditorGUILayout.BeginVertical("Box");
											{
												EditorGUILayout.BeginHorizontal();
												{
													EditorGUIUtility.labelWidth = 1;
													GUI.enabled = !currentCondition.useWeek && !currentCondition.useSeason;
													EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("useDelta"),GUIContent.none,true);
													GUI.enabled = true;
													EditorGUIUtility.labelWidth = 0;
													GUILayout.Label("Delta");
													GUILayout.FlexibleSpace();
												}
												EditorGUILayout.EndHorizontal();
												GUI.enabled = currentCondition.useDelta && !currentCondition.useWeek && !currentCondition.useSeason;
												EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("delta"),GUIContent.none,true);
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
							GUILayout.Label("Add a new Condition?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							currentEvent.conditions.Add(new TimeEvent.Event.Condition());
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