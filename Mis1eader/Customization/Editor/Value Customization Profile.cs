namespace Mis1eader.Customization
{
	using UnityEngine;
	using UnityEditor;
	/*internal class ValueCustomizationProfileAsset : Asset<ValueCustomizationProfile>
	{
		[MenuItem("Assets/Create/Mis1eader/Value Customization Profile",false,0)]
		private static void Make () {Action();}
	}*/
	/*internal class StartValueCustomizationProfile
	{
		[MenuItem("Assets/Create/Mis1eader/Value Customization Profile",false,0)]
		private static void Action () {ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<EndValueCustomizationProfile>(),"New Value Customization Profile.asset",EditorGUIUtility.FindTexture("ScriptableObject Icon"),string.Empty);}
	}
	internal class EndValueCustomizationProfile : UnityEditor.ProjectWindowCallback.EndNameEditAction
	{
		public override void Action (int instanceId,string pathName,string resourceFile)
		{
			ValueCustomizationProfile profile = ScriptableObject.CreateInstance<ValueCustomizationProfile>();
			profile.name = System.IO.Path.GetFileName(pathName.Replace(".asset",string.Empty));
			AssetDatabase.CreateAsset(profile,pathName);
			ProjectWindowUtil.ShowCreatedAsset(profile);
		}
	}*/
	[CustomEditor(typeof(ValueCustomizationProfile)),CanEditMultipleObjects]
	internal class ValueCustomizationProfileEditor : Editor<ValueCustomizationProfile>
	{
		[MenuItem("Assets/Create/Mis1eader/Value Customization Profile",false,0)]
		private static void Create ()
		{
			string name = string.Empty;
			CreateProfile(out name).name = name;
		}
		internal override void Inspector ()
		{
			Section("Profile",() =>
			{
				LabelWidth(40);
				Property(serializedObject.FindProperty("name"));
				LabelWidth(78);
				PropertyContainer1(serializedObject.FindProperty("variable"));
				LabelWidth(40);
				Container2(serializedObject.FindProperty("types"),target.types,toggleProperty: serializedObject.FindProperty("overrideTypes"),secondary: ProfileSectionTypesContainer);
			});
		}
		private void ProfileSectionTypesContainer (SerializedProperty currentProperty)
		{
			Property(currentProperty.FindPropertyRelative("name"));
			PropertyContainer1(currentProperty.FindPropertyRelative("value"));
		}
	}
}