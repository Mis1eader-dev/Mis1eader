namespace Mis1eader.Customization
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(ValueCustomizationSystem)),CanEditMultipleObjects]
	internal class ValueCustomizationSystemEditor : Editor<ValueCustomizationSystem>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector ()
		{
			Section("Editor",null,labelContent: () => PressButton(serializedObject.FindProperty("runInEditor"),"Executes everything in editor for visualization."));
			Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("valueCustomizations"),target.valueCustomizations,primary: MainSectionValueCustomizationsContainer),labelContent: () => ControlBar(() =>
			{
				LabelWidth(91);
				Property(serializedObject.FindProperty("updateMode"));
			}));
		}
		private void MainSectionValueCustomizationsContainer (ValueCustomizationSystem.ValueCustomization current,SerializedProperty currentProperty)
		{
			LabelWidth(50);
			if(Asset(ref current.profile,currentProperty.FindPropertyRelative("profile"),name: current.name))
			{
				ValueCustomizationProfile profile = current.profile;
				profile.name = current.name;
				profile.variable = current.variable;
				profile.overrideTypes = current.types.Count != 0;
				if(profile.overrideTypes)profile.SetTypesUnlinked(current.types);
			}
			OpenVerticalSubsection();
			{
				GUI.enabled = !current.profile;
				LabelWidth(44);
				Property(currentProperty.FindPropertyRelative("name"));
				GUI.enabled = true;
				LabelWidth(53);
				SerializedProperty sourceProperty = currentProperty.FindPropertyRelative("source");
				Selection(sourceProperty);
				Variable(sourceProperty,ref current.variable,ref current.variableNameIndex,currentProperty.FindPropertyRelative("variable"),variableState: !current.profile);
				Container2(currentProperty.FindPropertyRelative("types"),current.types,primary: MainSectionValueCustomizationsContainerTypesContainer,header: () => IndexContainer(currentProperty.FindPropertyRelative("index"),currentProperty.FindPropertyRelative("types"),design: 1),state: !current.profile);
			}
			CloseVertical();
		}
		private void MainSectionValueCustomizationsContainerTypesContainer (ValueCustomizationSystem.ValueCustomization.Type current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			LabelWidth(44);
			PropertyContainer1(currentProperty.FindPropertyRelative("value"));
		}
	}
}