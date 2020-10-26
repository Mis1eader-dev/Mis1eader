namespace Mis1eader.Gauge
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(GaugeTarget)),CanEditMultipleObjects]
	internal class GaugeTargetEditor : Editor<GaugeTarget>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector ()
		{
			Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("targets"),target.targets,primary: MainSectionTargetsContainer),labelContent: () => ControlBar(() =>
			{
				LabelWidth(90);
				Property(serializedObject.FindProperty("updateMode"));
			}));
		}
		private GaugeSystem currentGauge = null;
		private SerializedProperty currentTargetProperty = null;
		private void MainSectionTargetsContainer (GaugeTarget.Target current,SerializedProperty currentProperty)
		{
			LabelWidth(50);
			if(Asset(ref current.profile,currentProperty.FindPropertyRelative("profile"),name: current.name))
			{
				GaugeTargetProfile profile = current.profile;
				profile.name = current.name;
				profile.overrideMinorTicks = current.overrideMinorTicks;
				if(profile.overrideMinorTicks)profile.minorTicks = current.minorTicks;
				profile.overrideMajorTicks = current.overrideMajorTicks;
				if(profile.overrideMajorTicks)profile.SetMajorTicksUnlinked(current.majorTicks);
				profile.overrideOverrides = current.overrides.Count != 0;
				if(profile.overrideOverrides)profile.SetOverridesUnlinked(current.overrides);
			}
			Subsection("Editor",() => Container("Range Generation",() =>
			{
				OpenHorizontal();
				{
					PropertyContainer1(currentProperty.FindPropertyRelative("from"),group: true,design: 3);
					PropertyContainer1(currentProperty.FindPropertyRelative("to"),group: true,design: 3);
					PropertyContainer1(currentProperty.FindPropertyRelative("count"),group: true,width: 24,design: 3);
				}
				CloseHorizontal();
				OpenHorizontal();
				{
					if(PressButton("Generate",EditorContents.info,"In a standalone build you have to call GenerateRange() on it."))
					{
						Undo.RecordObject(target,"Inspector");
						current.GenerateRange(current.from,current.to,current.count,current.integerizeRange);
						serializedObject.Update();
					}
					GUI.enabled = GUI.enabled && current.majorTicks.Count != 0;
					if(PressButton("Clear"))
					{
						Undo.RecordObject(target,"Inspector");
						current.majorTicks.Clear();
						serializedObject.Update();
					}
					GUI.enabled = true;
				}
				CloseHorizontal();
			},labelContent: () =>
			{
				FieldWidth(1);
				LabelWidth(2);
				Property(string.Empty,currentProperty.FindPropertyRelative("integerizeRange"));
				FieldWidth();
				LabelWidth();
			},toggleProperty: currentProperty.FindPropertyRelative("rangeGeneration"),generalState: current.overrideMajorTicks && (!current.profile || current.profile && !current.profile.overrideMajorTicks),design: 4));
			OpenVerticalSubsection();
			{
				GUI.enabled = !current.profile;
				LabelWidth(44);
				Property(currentProperty.FindPropertyRelative("name"));
				GUI.enabled = true;
				LabelWidth(49);
				PropertyContainer1(currentProperty.FindPropertyRelative("gauge"));
				LabelWidth(53);
				Selection(currentProperty.FindPropertyRelative("source"));
				LabelWidth(82);
				PropertyContainer2(currentProperty.FindPropertyRelative("overrideMinorTicks"),currentProperty.FindPropertyRelative("minorTicks"),state: !current.profile || current.profile && !current.profile.overrideMinorTicks);
				LabelWidth();
				Container1(currentProperty.FindPropertyRelative("majorTicks"),current.majorTicks,toggleProperty: currentProperty.FindPropertyRelative("overrideMajorTicks"),@default: current.majorTicks.Count.ToString(),state: !current.profile || current.profile && !current.profile.overrideMajorTicks);
				currentGauge = current.gauge;
				currentTargetProperty = currentProperty;
				Container2(currentProperty.FindPropertyRelative("overrides"),current.overrides,primary: MainSectionTargetsContainerOverridesContainer,state: !current.profile || current.profile && !current.profile.overrideOverrides);
			}
			CloseVertical();
		}
		private void MainSectionTargetsContainerOverridesContainer (GaugeTarget.Target.Override current,SerializedProperty currentProperty)
		{
			LabelWidth(40);
			PropertyContainer1(currentProperty.FindPropertyRelative("type"));
			bool lastEnabled = GUI.enabled;
			GUI.enabled = currentGauge && currentGauge.additionalValues.Count != 0;
			MainSectionTargetsContainerOverridesContainerIndexContainer(currentProperty.FindPropertyRelative("index"));
			LabelWidth(44);
			GUI.enabled = lastEnabled && current.variable == string.Empty;
			PropertyContainer1(currentProperty.FindPropertyRelative("value"));
			GUI.enabled = lastEnabled;
			FieldWidth(1);
			LabelWidth();
			Variable(currentTargetProperty.FindPropertyRelative("source"),ref current.variable,ref current.nameIndex,currentProperty.FindPropertyRelative("variable"));
			FieldWidth();
		}
		private void MainSectionTargetsContainerOverridesContainerIndexContainer (SerializedProperty indexProperty,float width = 46)
		{
			OpenVerticalSubsection();
			{
				OpenHorizontalBar();
				{
					string[] valueNames = new string[currentGauge ? currentGauge.additionalValues.Count + 1 : 1];
					valueNames[0] = "Built-in";
					for(int a = 1,A = valueNames.Length; a < A; a++)
						valueNames[a] = "[" + (a - 1).ToString() + "] " + currentGauge.additionalValues[a - 1].name;
					LabelWidth(width);
					FieldWidth(23);
					Property(indexProperty);
					EditorGUI.BeginChangeCheck();
					LabelWidth();
					FieldWidth(1);
					int popup = EditorGUILayout.Popup(currentGauge && currentGauge.additionalValues.Count != 0 ? 1 + indexProperty.intValue : 0,valueNames) - 1;
					FieldWidth();
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						indexProperty.intValue = popup;
					}
				}
				CloseHorizontal();
			}
			CloseVertical();
		}
	}
}