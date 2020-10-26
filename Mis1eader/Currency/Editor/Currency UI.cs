/*
To use a feature from the following three #define lines below, uncomment the forward slashes "//" that come before #define preprocessors, and to disable one, comment it with two forward slashes.
Keep in mind that only one feature must be used at a time, or the highest one in the order will take priority.
Also it is only possible to switch to a lower feature and not the other way around, for example if you enable ENABLE_UNITY_TEXT and go back to ENABLE_REFLECTION, then you will have to re-assign the text mesh field manually, or you can re-import the package.
As for ENABLE_TEXT_MESH_PRO, you will have to re-assign the text fields manually.
The following scripts must use the same features as each other:
	1. Assets/Mis1eader/Currency/CurrencyUI.cs
	2. Assets/Mis1eader/Currency/Editor/Currency UI.cs
*/
#define ENABLE_REFLECTION //Uses reflections, it works on every component that has a text field or property, but when the text of the text component changes continuously, it produces garbage that triggers the garbage collection in order to make the changes, results in slight performance loss.
//#define ENABLE_UNITY_TEXT //Uses Unity's built-in text components, Text and Text Mesh, it directly modifies text and color properties of these components.
//#define ENABLE_TEXT_MESH_PRO //Uses Text Mesh Pro, it has components for both 2D and 3D text.

#if ENABLE_REFLECTION
#undef ENABLE_UNITY_TEXT
#undef ENABLE_TEXT_MESH_PRO
#else
#if	ENABLE_UNITY_TEXT
#undef ENABLE_TEXT_MESH_PRO
#else
#if	!ENABLE_TEXT_MESH_PRO
#define ENABLE_REFLECTION
#endif
#endif
#endif
namespace Mis1eader.Currency
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(CurrencyUI)),CanEditMultipleObjects]
	internal class CurrencyUIEditor : Editor<CurrencyUI>
	{
		private static bool mainSectionIsExpanded = true;
		#if ENABLE_UNITY_TEXT
		private static string[] componentNames = new string[] {"Text (2D)","Text Mesh (3D)"};
		#endif
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
						GUILayout.Label("Currency Digits");
						Property(string.Empty,serializedObject.FindProperty("currencyDigits.left"),GUILayout.Width(30));
						GUILayout.Label(".",GUILayout.ExpandWidth(false));
						Property(string.Empty,serializedObject.FindProperty("currencyDigits.right"),GUILayout.Width(30));
					}
					CloseHorizontal();
				}
				CloseVertical();
				OpenVerticalSubsection();
				{
					OpenHorizontalBar();
					{
						GUILayout.Label("Maximum Currency Digits",EditorContents.label3);
						Property(string.Empty,serializedObject.FindProperty("maximumCurrencyDigits.left"),GUILayout.Width(30));
						GUILayout.Label(".",GUILayout.ExpandWidth(false));
						Property(string.Empty,serializedObject.FindProperty("maximumCurrencyDigits.right"),GUILayout.Width(30));
					}
					CloseHorizontal();
				}
				CloseVertical();
				#if ENABLE_REFLECTION
				LabelWidth(101);
				Selection("Currency Text",serializedObject.FindProperty("currencyText.text"),requiredVariable: "text",requiredVariableType: typeof(string));
				LabelWidth(132);
				Selection("Maximum Currency Text",serializedObject.FindProperty("maximumCurrencyText.text"),requiredVariable: "text",requiredVariableType: typeof(string));
				#else
				#if ENABLE_UNITY_TEXT
				if(target.component == 0)
				{
					#endif
					LabelWidth(101);
					PropertyContainer1("Currency Text",serializedObject.FindProperty("currencyText.text"));
					LabelWidth(132);
					PropertyContainer1("Maximum Currency Text",serializedObject.FindProperty("maximumCurrencyText.text"));
					#if ENABLE_UNITY_TEXT
				}
				else
				{
					LabelWidth(101);
					PropertyContainer1("Currency Text",serializedObject.FindProperty("currencyText.textMesh"));
					LabelWidth(132);
					PropertyContainer1("Maximum Currency Text",serializedObject.FindProperty("maximumCurrencyText.textMesh"));
				}
				#endif
				#endif
			}
			#if ENABLE_UNITY_TEXT
			,labelContent: () => ControlBar(() =>
			{
				LabelWidth(74);
				FieldWidth(84);
				EditorGUI.BeginChangeCheck();
				byte component = (byte)EditorGUILayout.Popup("Component",target.component,componentNames);
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target,"Inspector");
					target.component = component;
				}
				FieldWidth();
			})
			#endif
			);
		}
		private void MainSectionIndexContainer ()
		{
			OpenVerticalSubsection();
			{
				OpenHorizontalBar();
				{
					string[] currencyNames = new string[target.source ? target.source.currencies.Count + 1 : 1];
					currencyNames[0] = "Not Specified";
					for(int a = 1,A = currencyNames.Length; a < A; a++)
						currencyNames[a] = "[" + (a - 1).ToString() + "] " + target.source.currencies[a - 1].name;
					LabelWidth(46);
					FieldWidth(1);
					Property(serializedObject.FindProperty("index"));
					LabelWidth(59);
					FieldWidth();
					EditorGUI.BeginChangeCheck();
					int popup = EditorGUILayout.Popup("Currency",target.source && target.source.currencies.Count != 0 ? target.index + 1 : 0,currencyNames);
					if(target.source && target.source.currencies.Count != 0)popup = popup - 1;
					else if(target.index == -1)popup = -1;
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						target.index = (sbyte)popup;
					}
				}
				CloseHorizontal();
			}
			CloseVertical();
		}
	}
}