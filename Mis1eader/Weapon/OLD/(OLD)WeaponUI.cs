namespace AdvancedAssets.Weapon
{
	using UnityEngine;
	using UnityEngine.UI;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	[ExecuteInEditMode]
	public class WeaponUI : MonoBehaviour
	{
		public WeaponSystem source = null;
		public Transform shotsUI = null;
		public byte shotsDigits = 1;
		public Transform storageUI = null;
		public byte storageDigits = 1;
		private void Update ()
		{
			shotsDigits = Clamp(shotsDigits,1,10);
			storageDigits = Clamp(storageDigits,1,10);
			if(Application.isPlaying && source)
			{
				if(shotsUI)
				{
					string digits = string.Empty;
					Text textObject = shotsUI.GetComponent<Text>();
					TextMesh textMeshObject = shotsUI.GetComponent<TextMesh>();
					while(digits.Length < shotsDigits)digits += "0";
					if(textObject && textObject.text != source.shots.ToString(digits))
						textObject.text = source.shots.ToString(digits);
					if(textMeshObject && textMeshObject.text != source.shots.ToString(digits))
						textMeshObject.text = source.shots.ToString(digits);
				}
				if(storageUI)
				{
					string digits = string.Empty;
					Text textObject = storageUI.GetComponent<Text>();
					TextMesh textMeshObject = storageUI.GetComponent<TextMesh>();
					while(digits.Length < storageDigits)digits += "0";
					if(textObject && textObject.text != source.storage.ToString(digits))
						textObject.text = source.storage.ToString(digits);
					if(textMeshObject && textMeshObject.text != source.storage.ToString(digits))
						textMeshObject.text = source.storage.ToString(digits);
				}
			}
		}
		private byte Clamp (byte value,byte minimum,byte maximum)
		{
			if(value < minimum)value = minimum;
			if(value > maximum)value = maximum;
			return value;
		}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(WeaponUI)),CanEditMultipleObjects]
	internal class WeaponUIEditor : Editor
	{
		private WeaponUI[] weaponUIs
		{
			get
			{
				WeaponUI[] weaponUIs = new WeaponUI[targets.Length];
				for(int weaponUIsIndex = 0; weaponUIsIndex < targets.Length; weaponUIsIndex++)
					weaponUIs[weaponUIsIndex] = (WeaponUI)targets[weaponUIsIndex];
				return weaponUIs;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			StatsSection();
			MainSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int weaponUIsIndex = 0; weaponUIsIndex < weaponUIs.Length; weaponUIsIndex++)
					EditorUtility.SetDirty(weaponUIs[weaponUIsIndex]);
			}
		}
		private void StatsSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("Stats",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("Shots: " + (weaponUIs[0].source ? weaponUIs[0].source.shots : 0));
					GUILayout.FlexibleSpace();
					GUILayout.Label(" | ");
					GUILayout.FlexibleSpace();
					GUILayout.Label("Storage: " + (weaponUIs[0].source ? weaponUIs[0].source.storage : 0));
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Main","BoldLabel");
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("source"),true);
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 60;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("shotsUI"),true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("shotsDigits"),GUIContent.none,true,GUILayout.Width(20));
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 70;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("storageUI"),true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("storageDigits"),GUIContent.none,true,GUILayout.Width(20));
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
	}
	#endif
}