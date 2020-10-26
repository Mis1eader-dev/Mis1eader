namespace Mis1eader
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(HealthUI)),CanEditMultipleObjects]
	internal class HealthUIEditor : Editor<HealthUI>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector ()
		{
			Section("Main",ref mainSectionIsExpanded,() =>
			{
				LabelWidth(53);
				PropertyContainer1(serializedObject.FindProperty("source"));
				MainSectionIndexContainer();
				OpenVerticalSubsection();
				{
					OpenHorizontalBar();
					{
						GUILayout.Label("Health Digits");
						Property(string.Empty,serializedObject.FindProperty("healthDigits.left"),GUILayout.Width(30));
						GUILayout.Label(".",GUILayout.ExpandWidth(false));
						Property(string.Empty,serializedObject.FindProperty("healthDigits.right"),GUILayout.Width(30));
					}
					CloseHorizontal();
				}
				CloseVertical();
				OpenVerticalSubsection();
				{
					OpenHorizontalBar();
					{
						GUILayout.Label("Maximum Health Digits",EditorContents.label3);
						Property(string.Empty,serializedObject.FindProperty("maximumHealthDigits.left"),GUILayout.Width(30));
						GUILayout.Label(".",GUILayout.ExpandWidth(false));
						Property(string.Empty,serializedObject.FindProperty("maximumHealthDigits.right"),GUILayout.Width(30));
					}
					CloseHorizontal();
				}
				CloseVertical();
				LabelWidth(84);
				Selection(serializedObject.FindProperty("healthText"),requiredVariable: "text",requiredVariableType: typeof(string));
				LabelWidth(138);
				Selection(serializedObject.FindProperty("maximumHealthText"),requiredVariable: "text",requiredVariableType: typeof(string));
			},labelContent: () => ControlBar(() =>
			{
				LabelWidth(90);
				Property(serializedObject.FindProperty("updateMode"));
			}));
		}
		private void MainSectionIndexContainer ()
		{
			OpenVerticalSubsection();
			{
				OpenHorizontalBar();
				{
					string[] healthNames = new string[target.source ? target.source.healths.Count + 1 : 1];
					healthNames[0] = "Not Specified";
					for(int a = 1,A = healthNames.Length; a < A; a++)
						healthNames[a] = "[" + (a - 1).ToString() + "] " + target.source.healths[a - 1].name;
					LabelWidth(46);
					FieldWidth(1);
					Property(serializedObject.FindProperty("index"));
					LabelWidth(43);
					FieldWidth();
					EditorGUI.BeginChangeCheck();
					int popup = EditorGUILayout.Popup("Health",target.source && target.source.healths.Count != 0 ? target.index + 1 : 0,healthNames);
					if(target.source && target.source.healths.Count != 0)popup = popup - 1;
					else if(target.index == -1)popup = -1;
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						target.index = popup;
					}
				}
				CloseHorizontal();
			}
			CloseVertical();
		}
	}
}