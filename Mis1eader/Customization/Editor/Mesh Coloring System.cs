namespace Mis1eader.Customization
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(MeshColoringSystem)),CanEditMultipleObjects]
	internal class MeshColoringSystemEditor : Editor<MeshColoringSystem>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector ()
		{
			Section("Editor",null,labelContent: () => PressButton(serializedObject.FindProperty("runInEditor"),"Executes everything in editor for visualization."));
			Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("meshColorings"),target.meshColorings,primary: MainSectionMeshColoringsContainer),labelContent: () => ControlBar(() =>
			{
				LabelWidth(91);
				Property(serializedObject.FindProperty("updateMode"));
			}));
		}
		private void MainSectionMeshColoringsContainer (MeshColoringSystem.MeshColoring current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			Container1(currentProperty.FindPropertyRelative("materials"),current.materials);
			Container2(currentProperty.FindPropertyRelative("parts"),current.parts,primary: MainSectionMeshColoringsContainerPartsContainer,header: () =>
			{
				IndexContainer(currentProperty.FindPropertyRelative("index"),currentProperty.FindPropertyRelative("materials"),design: 1);
				IndexContainer(currentProperty.FindPropertyRelative("preview"),currentProperty.FindPropertyRelative("materials"),width: 60,@default: "Disabled Preview",design: 1);
				IndexContainer(currentProperty.FindPropertyRelative("part"),currentProperty.FindPropertyRelative("parts"),width: 35,hideDefault: true,design: 1);
			});
		}
		private void MainSectionMeshColoringsContainerPartsContainer (MeshColoringSystem.MeshColoring.Part current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			//IndexContainer(currentProperty.FindPropertyRelative("index"),PreviousProperty(currentProperty).FindPropertyRelative("materials"),46,design: 1);
			LabelWidth(48);
			PropertyContainer1(currentProperty.FindPropertyRelative("group"));
			Container1(currentProperty.FindPropertyRelative("singles"),current.singles);
		}
	}
}