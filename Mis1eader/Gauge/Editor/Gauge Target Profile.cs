namespace Mis1eader.Gauge
{
	using UnityEngine;
	using UnityEditor;
	/*internal class StartGaugeTargetProfile
	{
		[MenuItem("Assets/Create/Mis1eader/Gauge Target Profile",false,11)]
		private static void Action () {ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<EndGaugeTargetProfile>(),"New Gauge Target Profile.asset",EditorGUIUtility.FindTexture("ScriptableObject Icon"),string.Empty);}
	}
	internal class EndGaugeTargetProfile : UnityEditor.ProjectWindowCallback.EndNameEditAction
	{
		public override void Action (int instanceId,string pathName,string resourceFile)
		{
			GaugeTargetProfile gaugeTargetProfile = ScriptableObject.CreateInstance<GaugeTargetProfile>();
			gaugeTargetProfile.name = System.IO.Path.GetFileName(pathName.Replace(".asset",string.Empty));
			AssetDatabase.CreateAsset(gaugeTargetProfile,pathName);
			ProjectWindowUtil.ShowCreatedAsset(gaugeTargetProfile);
		}
	}*/
	[CustomEditor(typeof(GaugeTargetProfile)),CanEditMultipleObjects]
	internal class GaugeTargetProfileEditor : Editor<GaugeTargetProfile>
	{
		[MenuItem("Assets/Create/Mis1eader/Gauge Target Profile",false,11)]
		private static void Create ()
		{
			string name = string.Empty;
			CreateProfile(out name).name = name;
		}
		internal override void Inspector ()
		{
			Section("Editor",() => Container("Range Generation",() =>
			{
				OpenHorizontal();
				{
					PropertyContainer1(serializedObject.FindProperty("from"),group: true,design: 3);
					PropertyContainer1(serializedObject.FindProperty("to"),group: true,design: 3);
					PropertyContainer1(serializedObject.FindProperty("count"),group: true,design: 3);
				}
				CloseHorizontal();
				OpenHorizontal();
				{
					if(PressButton("Generate",EditorContents.info,"In a standalone build you have to call GenerateRange() on it."))
					{
						Undo.RecordObjects(targets,"Inspector");
						for(int a = 0,A = targets.Length; a < A; a++)
							targets[a].GenerateRange(targets[a].from,targets[a].to,targets[a].count,targets[a].integerizeRange);
						serializedObject.Update();
					}
					GUI.enabled = GUI.enabled && target.majorTicks.Count != 0;
					if(PressButton("Clear"))
					{
						Undo.RecordObjects(targets,"Inspector");
						for(int a = 0,A = targets.Length; a < A; a++)
							targets[a].majorTicks.Clear();
						serializedObject.Update();
					}
					GUI.enabled = true;
				}
				CloseHorizontal();
			},labelContent: () => Property(string.Empty,serializedObject.FindProperty("integerizeRange")),toggleProperty: serializedObject.FindProperty("rangeGeneration"),generalState: target.overrideMajorTicks,design: 3));
			Section("Profile",() =>
			{
				LabelWidth(40);
				Property(serializedObject.FindProperty("name"));
				LabelWidth();
				PropertyContainer2(serializedObject.FindProperty("overrideMinorTicks"),serializedObject.FindProperty("minorTicks"));
				Container1(serializedObject.FindProperty("majorTicks"),target.majorTicks,toggleProperty: serializedObject.FindProperty("overrideMajorTicks"),@default: target.majorTicks.Count.ToString());
				
				
				Container2(serializedObject.FindProperty("overrides"),target.overrides,toggleProperty: serializedObject.FindProperty("overrideOverrides"),primary: ProfileSectionOverridesContainer);
				
				/*OpenHorizontalSubsection();
				{
					PropertyContainer2(serializedObject.FindProperty("overrideMinimumValue"),serializedObject.FindProperty("minimumValue"),group: true,design: 2);
					GUI.enabled = target.overrideMinimumValue;
					PropertyContainer1("Name",serializedObject.FindProperty("minimumValueName"),group: true,design: 3);
					GUI.enabled = true;
				}
				CloseHorizontal();
				OpenHorizontalSubsection();
				{
					PropertyContainer2(serializedObject.FindProperty("overrideValue"),serializedObject.FindProperty("value"),labelContent: () =>
					{
						OpenHorizontal();
						{
							GUI.enabled = target.overrideValue;
							GUI.backgroundColor = target.overrideAbsolute ? Color.green : Color.red;
							Toggle(serializedObject.FindProperty("overrideAbsolute"),EditorContents.miniButtonLeft,design: 1);
							GUI.enabled = target.overrideValue && target.overrideAbsolute;
							GUI.backgroundColor = target.absolute ? Color.green : Color.red;
							if(PressButton("Absolute",EditorContents.miniButtonRight))
							{
								Undo.RecordObjects(targets,"Inspector");
								target.absolute = !target.absolute;
							}
							GUI.backgroundColor = Color.white;
						}
						CloseHorizontal();
					},group: true,design: 2);
					GUI.enabled = target.overrideValue;
					PropertyContainer1("Name",serializedObject.FindProperty("valueName"),group: true,design: 3);
					GUI.enabled = true;
				}
				CloseHorizontal();
				OpenHorizontalSubsection();
				{
					PropertyContainer2(serializedObject.FindProperty("overrideMaximumValue"),serializedObject.FindProperty("maximumValue"),group: true,design: 2);
					GUI.enabled = target.overrideMaximumValue;
					PropertyContainer1("Name",serializedObject.FindProperty("maximumValueName"),group: true,design: 3);
					GUI.enabled = true;
				}
				CloseHorizontal();*/
			});
		}
		private void ProfileSectionOverridesContainer (GaugeTarget.Target.Override current,SerializedProperty currentProperty)
		{
			LabelWidth(40);
			PropertyContainer1(currentProperty.FindPropertyRelative("type"));
			//IndexProperty(currentProperty.FindPropertyRelative("index"),);
			LabelWidth(46);
			PropertyContainer1(currentProperty.FindPropertyRelative("index"));
			LabelWidth(44);
			PropertyContainer1(currentProperty.FindPropertyRelative("value"));
			FieldWidth(1);
			LabelWidth(62);
			PropertyContainer1(currentProperty.FindPropertyRelative("variable"));
			FieldWidth();
			LabelWidth();
		}
	}
}