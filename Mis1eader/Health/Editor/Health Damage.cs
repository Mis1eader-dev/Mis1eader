namespace Mis1eader
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(HealthDamage)),CanEditMultipleObjects]
	internal class HealthDamageEditor : Editor<HealthDamage>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector () {Section("Main",ref mainSectionIsExpanded,MainSection);}
		private void MainSection ()
		{
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			/*Property(serializedObject.FindProperty("source"));
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useVelocity"),GUIContent.none,true,GUILayout.ExpandWidth(false));
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Velocity",EditorStyles.boldLabel);
				}
				EditorGUILayout.EndHorizontal();
				if(targets[0].useVelocity)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("body"),true);
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(serializedObject.FindProperty("useVelocityX"),GUIContent.none,true,GUILayout.ExpandWidth(false));
							EditorGUIUtility.labelWidth = 0;
							EditorGUILayout.LabelField("X",EditorStyles.boldLabel);
						}
						EditorGUILayout.EndHorizontal();
						if(targets[0].useVelocityX)
						{
							SerializedProperty currentVelocityProperty = serializedObject.FindProperty("velocityX");
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("velocityRange"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("minimumVelocity"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("maximumVelocity"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("minimumDamage"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("maximumDamage"),true);
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(serializedObject.FindProperty("useVelocityY"),GUIContent.none,true,GUILayout.ExpandWidth(false));
							EditorGUIUtility.labelWidth = 0;
							EditorGUILayout.LabelField("Y",EditorStyles.boldLabel);
						}
						EditorGUILayout.EndHorizontal();
						if(targets[0].useVelocityY)
						{
							SerializedProperty currentVelocityProperty = serializedObject.FindProperty("velocityY");
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("velocityRange"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("minimumVelocity"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("maximumVelocity"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("minimumDamage"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("maximumDamage"),true);
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(serializedObject.FindProperty("useVelocityZ"),GUIContent.none,true,GUILayout.ExpandWidth(false));
							EditorGUIUtility.labelWidth = 0;
							EditorGUILayout.LabelField("Z",EditorStyles.boldLabel);
						}
						EditorGUILayout.EndHorizontal();
						if(targets[0].useVelocityZ)
						{
							SerializedProperty currentVelocityProperty = serializedObject.FindProperty("velocityZ");
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("velocityRange"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("minimumVelocity"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("maximumVelocity"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("minimumDamage"),true);
							EditorGUILayout.PropertyField(currentVelocityProperty.FindPropertyRelative("maximumDamage"),true);
						}
					}
					EditorGUILayout.EndVertical();
				}
			}
			EditorGUILayout.EndVertical();*/
		}
	}
}