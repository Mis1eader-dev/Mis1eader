namespace Mis1eader.Customization
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(MeshCustomizationSystem)),CanEditMultipleObjects]
	internal class MeshCustomizationSystemEditor : Editor<MeshCustomizationSystem>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector ()
		{
			Section("Editor",null,labelContent: () => PressButton(serializedObject.FindProperty("runInEditor"),"Executes everything in editor for visualization."));
			Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("meshCustomizations"),target.meshCustomizations,primary: MainSectionMeshCustomizationsContainer),labelContent: () => ControlBar(() =>
			{
				LabelWidth(91);
				Property(serializedObject.FindProperty("updateMode"));
			}));
		}
		private void MainSectionMeshCustomizationsContainer (MeshCustomizationSystem.MeshCustomization current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			Container2(currentProperty.FindPropertyRelative("types"),current.types,primary: MainSectionMeshCustomizationsContainerTypesContainer,header: () =>
			{
				IndexContainer(currentProperty.FindPropertyRelative("index"),currentProperty.FindPropertyRelative("types"),design: 1);
				IndexContainer(currentProperty.FindPropertyRelative("preview"),currentProperty.FindPropertyRelative("types"),width: 60,@default: "Disabled Preview",design: 1);
			});
		}
		private void MainSectionMeshCustomizationsContainerTypesContainer (MeshCustomizationSystem.MeshCustomization.Type current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			LabelWidth(48);
			PropertyContainer1(currentProperty.FindPropertyRelative("group"));
			Container1(currentProperty.FindPropertyRelative("singles"),current.singles);
		}
	}
}