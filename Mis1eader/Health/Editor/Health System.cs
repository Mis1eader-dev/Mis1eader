namespace Mis1eader
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(HealthSystem)),CanEditMultipleObjects]
	internal class HealthSystemEditor : Editor<HealthSystem>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector () {Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("healths"),target.healths,primary: MainSectionHealthsContainer));}
		private void MainSectionHealthsContainer (HealthSystem.Health current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			LabelWidth();
			OpenHorizontalSubsection();
			{
				PropertyContainer1(currentProperty.FindPropertyRelative("health"),group: true,design: 3);
				PropertyContainer1(currentProperty.FindPropertyRelative("maximumHealth"),group: true,design: 3);
			}
			CloseHorizontal();
			MainSectionHealthsContainerIndexContainer(current,currentProperty);
		}
		private void MainSectionHealthsContainerIndexContainer (HealthSystem.Health current,SerializedProperty currentProperty)
		{
			OpenVerticalSubsection();
			{
				OpenHorizontalBar();
				{
					int index = target.healths.IndexOf(current);
					string[] healthNames = new string[target.healths.Count];
					healthNames[0] = "None";
					for(int a = 1,A = healthNames.Length; a < A; a++)
						healthNames[a] = "[" + (index > a - 1 ? a - 1 : a).ToString() + "] " + target.healths[index > a - 1 ? a - 1 : a].name;
					LabelWidth(34);
					FieldWidth(1);
					Property(currentProperty.FindPropertyRelative("link"));
					LabelWidth(73);
					FieldWidth(18);
					EditorGUI.BeginChangeCheck();
					int popup = EditorGUILayout.Popup("Linked With",current.link > index ? current.link : (current.link == index ? 0 : current.link + 1),healthNames);
					if(popup <= index)popup = popup - 1;
					FieldWidth();
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						current.link = popup;
					}
				}
				CloseHorizontal();
			}
			CloseVertical();
		}
	}
}