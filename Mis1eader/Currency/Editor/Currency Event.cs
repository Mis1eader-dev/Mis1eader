namespace Mis1eader.Currency
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(CurrencyEvent)),CanEditMultipleObjects]
	internal class CurrencyEventEditor : Editor<CurrencyEvent>
	{
		private static bool mainSectionIsExpanded = true;
		private bool operatorIsExpanded = false;
		internal override void Inspector () {Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("events"),target.events,primary: MainSectionEventsContainer,isEvent: true));}
		private void MainSectionEventsContainer (CurrencyEvent.Event current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			Container2(currentProperty.FindPropertyRelative("conditions"),current.conditions,primary: MainSectionEventsContainerConditionsContainer);
			Property(currentProperty.FindPropertyRelative("onTrue"));
			Property(currentProperty.FindPropertyRelative("onFalse"));
			Property(currentProperty.FindPropertyRelative("isTrue"));
			Property(currentProperty.FindPropertyRelative("isFalse"));
		}
		private void MainSectionEventsContainerConditionsContainer (CurrencyEvent.Event.Condition current,SerializedProperty currentProperty)
		{
			LabelWidth(53);
			PropertyContainer1(currentProperty.FindPropertyRelative("source"));
			
			MainSectionEventsContainerConditionsContainerIndexContainer(current,currentProperty);
			//IndexContainer(currentProperty.FindPropertyRelative("index"),current.source ? currentProperty.FindPropertyRelative("source").FindPropertyRelative("currencies") : null,46);
			
			
			Operator(currentProperty.FindPropertyRelative("operator"),ref operatorIsExpanded);
			LabelWidth(67);
			PropertyContainer1(currentProperty.FindPropertyRelative("currency"),width: -1);
		}
		private void MainSectionEventsContainerConditionsContainerIndexContainer (CurrencyEvent.Event.Condition current,SerializedProperty currentProperty)
		{
			OpenVerticalSubsection();
			{
				OpenHorizontalBar();
				{
					string[] currencyNames = new string[current.source ? current.source.currencies.Count + 1 : 1];
					currencyNames[0] = "Not Specified";
					for(int a = 1,A = currencyNames.Length; a < A; a++)
						currencyNames[a] = "[" + (a - 1).ToString() + "] " + current.source.currencies[a - 1].name;
					LabelWidth(46);
					FieldWidth(1);
					Property(currentProperty.FindPropertyRelative("index"));
					LabelWidth(40);
					FieldWidth(10);
					EditorGUI.BeginChangeCheck();
					int popup = EditorGUILayout.Popup("Currency",current.source && current.source.currencies.Count != 0 ? current.index + 1 : 0,currencyNames);
					if(current.source && current.source.currencies.Count != 0)popup = popup - 1;
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