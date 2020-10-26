namespace Mis1eader
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(HealthBodyPart)),CanEditMultipleObjects]
	internal class HealthBodyPartEditor : Editor<HealthBodyPart>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector ()
		{
			Section("Main",ref mainSectionIsExpanded,() =>
			{
				LabelWidth(53);
				PropertyContainer1(serializedObject.FindProperty("source"));
				MainSectionIndexContainer();
			},labelContent: () =>
			{
				if(PressButton("Search For Parent",GUILayout.ExpandWidth(false)))
				{
					Undo.RecordObjects(targets,"Inspector");
					for(int a = 0,A = targets.Length; a < A; a++)
						targets[a].SearchForParent();
				}
			});
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