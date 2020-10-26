/*
To use a feature from the following three #define lines below, uncomment the forward slashes "//" that come before #define preprocessors, and to disable one, comment it with two forward slashes.
Keep in mind that only one feature must be used at a time, or the highest one in the order will take priority.
Also it is only possible to switch to a lower feature and not the other way around, for example if you enable ENABLE_UNITY_TEXT and go back to ENABLE_REFLECTION, then you will have to re-assign the text mesh field manually, or you can re-import the package.
As for ENABLE_TEXT_MESH_PRO, you will have to remove existing text components and add Text Mesh Pro components to the prefabs manually, and then assign the text fields.
The following scripts must use the same features as each other:
	1. Assets/Mis1eader/Gauge/GaugeBodyPart.cs
	2. Assets/Mis1eader/Gauge/GaugeSystem.cs
	3. Assets/Mis1eader/Gauge/Editor/Gauge Body Part.cs
	4. Assets/Mis1eader/Gauge/Editor/Gauge System.cs
*/
#define ENABLE_REFLECTION //Uses reflections, it works on every component that has a text field or property, but when the text or color of the text component changes continuously, it produces garbage that triggers the garbage collection in order to make the changes, results in slight performance loss.
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
namespace Mis1eader.Gauge
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(GaugeBodyPart))]
	internal class GaugeBodyPartEditor : Editor<GaugeBodyPart>
	{
		private static bool mainSectionIsExpanded = true;
		private static string[] typeNames = new string[] {"Tick","Needle","Text"};
		private static string[] componentNames = new string[] {"Sprite Renderer","Image","Raw Image","Mesh Renderer"};
		#if ENABLE_UNITY_TEXT
		private static string[] textComponentNames = new string[] {"Text (2D)","Text Mesh (3D)"};
		#endif
		internal override void Inspector ()
		{
			Section("Main",ref mainSectionIsExpanded,() =>
			{
				if(target.type != 2)
				{
					LabelWidth(69);
					PropertyContainer1(serializedObject.FindProperty("movables"));
					LabelWidth(70);
					PropertyContainer1(serializedObject.FindProperty("scalables"));
					LabelWidth();
				}
				if(target.type == 1)
				{
					LabelWidth(42);
					PropertyContainer1(serializedObject.FindProperty("pivot"));
				}
				if(target.type != 2)Container1(serializedObject.FindProperty("colorables"),target.colorables,propertyName: (target.component == 0 ? "spriteRenderer" : (target.component == 1 ? "image" : (target.component == 2 ? "rawImage" : "meshRenderer"))));
				else
				{
					#if ENABLE_REFLECTION
					LabelWidth(38);
					Selection(serializedObject.FindProperty("text.text"),requiredVariable: "text",requiredVariableType: typeof(string));
					#else
					#if ENABLE_UNITY_TEXT
					if(target.textComponent == 0)
					{
						#endif
						LabelWidth(38);
						PropertyContainer1(serializedObject.FindProperty("text.text"));
						#if ENABLE_UNITY_TEXT
					}
					else
					{
						LabelWidth(74);
						PropertyContainer1(serializedObject.FindProperty("text.textMesh"));
					}
					#endif
					#endif
				}
			},labelContent: () => ControlBar(() =>
			{
				FieldWidth(50);
				LabelWidth(36);
				EditorGUI.BeginChangeCheck();
				byte type = (byte)EditorGUILayout.Popup("Type",target.type,typeNames);
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target,"Inspector");
					target.type = type;
				}
				if(type != 2)
				{
					FieldWidth(84);
					EditorGUI.BeginChangeCheck();
					byte component = (byte)EditorGUILayout.Popup(target.component,componentNames);
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						target.component = component;
					}
				}
				#if ENABLE_UNITY_TEXT
				else
				{
					FieldWidth(84);
					EditorGUI.BeginChangeCheck();
					byte textComponent = (byte)EditorGUILayout.Popup(target.textComponent,textComponentNames);
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						target.textComponent = textComponent;
					}
				}
				#endif
				FieldWidth();
			}));
		}
	}
}