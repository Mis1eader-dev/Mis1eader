namespace Mis1eader
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(HealthEvent)),CanEditMultipleObjects]
	internal class HealthEventEditor : Editor<HealthEvent>
	{
		private static bool mainSectionIsExpanded = true;
		private bool operatorIsExpanded = false;
		internal override void Inspector () {Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("events"),target.events,primary: MainSectionEventsContainer,isEvent: true));}
		private void MainSectionEventsContainer (HealthEvent.Event current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			Container2(currentProperty.FindPropertyRelative("conditions"),current.conditions,primary: MainSectionEventsContainerConditionsContainer);
			Property(currentProperty.FindPropertyRelative("onTrue"));
			Property(currentProperty.FindPropertyRelative("onFalse"));
			Property(currentProperty.FindPropertyRelative("isTrue"));
			Property(currentProperty.FindPropertyRelative("isFalse"));
		}
		private void MainSectionEventsContainerConditionsContainer (HealthEvent.Event.Condition current,SerializedProperty currentProperty)
		{
			LabelWidth(53);
			PropertyContainer1(currentProperty.FindPropertyRelative("source"));
			MainSectionEventsContainerConditionsContainerIndexContainer(current,currentProperty);
			Operator(currentProperty.FindPropertyRelative("operator"),ref operatorIsExpanded);
			LabelWidth(50);
			PropertyContainer1(currentProperty.FindPropertyRelative("health"));
		}
		private void MainSectionEventsContainerConditionsContainerIndexContainer (HealthEvent.Event.Condition current,SerializedProperty currentProperty)
		{
			OpenVerticalSubsection();
			{
				OpenHorizontalBar();
				{
					string[] healthNames = new string[current.source ? current.source.healths.Count + 1 : 1];
					healthNames[0] = "Not Specified";
					for(int a = 1,A = healthNames.Length; a < A; a++)
						healthNames[a] = "[" + (a - 1).ToString() + "] " + current.source.healths[a - 1].name;
					LabelWidth(46);
					FieldWidth(1);
					Property(currentProperty.FindPropertyRelative("index"));
					LabelWidth(40);
					FieldWidth(10);
					EditorGUI.BeginChangeCheck();
					int popup = EditorGUILayout.Popup("Health",current.source && current.source.healths.Count != 0 ? current.index + 1 : 0,healthNames);
					if(current.source && current.source.healths.Count != 0)popup = popup - 1;
					else if(current.index == -1)popup = -1;
					FieldWidth();
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						current.index = popup;
					}
				}
				CloseHorizontal();
			}
			CloseVertical();
		}
	}
}