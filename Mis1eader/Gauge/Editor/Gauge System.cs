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
	[CustomEditor(typeof(GaugeSystem)),CanEditMultipleObjects]
	internal class GaugeSystemEditor : Editor<GaugeSystem>
	{
		private static bool editorSectionIsExpanded = true,mainSectionIsExpanded = true,ticksSectionIsExpanded = true,shapesSectionIsExpanded = true,majorTicksSectionIsExpanded = true,configurationsSectionIsExpanded = true,frameSchedulingSectionIsExpanded = false;
		internal override void Inspector ()
		{
			Section("Editor",ref editorSectionIsExpanded,() => Container("Range Generation",() =>
			{
				OpenHorizontal();
				{
					PropertyContainer1(serializedObject.FindProperty("from"),group: true,design: 3);
					PropertyContainer1(serializedObject.FindProperty("to"),group: true,design: 3);
					PropertyContainer1(serializedObject.FindProperty("count"),group: true,design: 3);
				}
				CloseHorizontal();
				OpenHorizontal();
				{
					if(PressButton("Generate",EditorContents.info,"In a standalone build you have to call GenerateRange() on it."))
					{
						Undo.RecordObjects(targets,"Inspector");
						for(int a = 0,A = targets.Length; a < A; a++)
							targets[a].GenerateRange(targets[a].from,targets[a].to,targets[a].count,targets[a].integerizeRange);
						serializedObject.Update();
					}
					GUI.enabled = GUI.enabled && target.majorTicks.Count != 0;
					if(PressButton("Clear"))
					{
						Undo.RecordObjects(targets,"Inspector");
						for(int a = 0,A = targets.Length; a < A; a++)
							targets[a].majorTicks.Clear();
						serializedObject.Update();
					}
					GUI.enabled = true;
				}
				CloseHorizontal();
			},labelContent: () => Property(string.Empty,serializedObject.FindProperty("integerizeRange")),toggleProperty: serializedObject.FindProperty("rangeGeneration"),design: 3),labelContent: () => PressButton(serializedObject.FindProperty("runInEditor"),"Executes everything in editor for visualization."));
			Section("Main",ref mainSectionIsExpanded,MainSection,labelContent: () => ControlBar(() =>
			{
				FieldWidth(65);
				Property(string.Empty,serializedObject.FindProperty("dimension"));
				FieldWidth(1);
				Property(string.Empty,serializedObject.FindProperty("type"));
				FieldWidth();
			},design: 1));
			Section("Ticks",ref ticksSectionIsExpanded,TicksSection);
			Section("Shape",ref shapesSectionIsExpanded,ShapeSection,labelContent: () => PressButton(serializedObject.FindProperty("connectEndpoints"),"Connects the last and first tick by adding additional minor ticks."));
			Section("Values",ref majorTicksSectionIsExpanded,ValuesSection);
			Section("Configurations",ref configurationsSectionIsExpanded,ConfigurationsSection,labelContent: () => ControlBar(() =>
			{
				LabelWidth(48);
				Property(serializedObject.FindProperty("factor"));
			},design: 1));
			Section("Frame Scheduling",ref frameSchedulingSectionIsExpanded,FrameSchedulingSection);
		}
		private void MainSection ()
		{
			OpenVerticalSubsection();
			{
				if(target.dimension == GaugeSystem.Dimension.TwoDimensional)
				{
					LabelWidth(116);
					Property(serializedObject.FindProperty("twoDimensional"));
				}
				else
				{
					FieldWidth(1);
					LabelWidth(128);
					Property(serializedObject.FindProperty("threeDimensional"));
					FieldWidth();
					if(target.threeDimensional == GaugeSystem.ThreeDimensional.MeshRenderer)
					{
						LabelWidth(114);
						PropertyContainer1(serializedObject.FindProperty("sampleMaterial"),design: 1);
						LabelWidth();
						OpenHorizontal();
						{
							PropertyContainer1(serializedObject.FindProperty("primaryMaterial"),group: true,design: 3);
							PropertyContainer1(serializedObject.FindProperty("secondaryMaterial"),group: true,design: 3);
						}
						CloseHorizontal();
					}
				}
			}
			CloseVertical();
			if(target.type == GaugeSystem.Type.Digital)
			{
				OpenVerticalSubsection();
				{
					LabelWidth(48);
					Property(serializedObject.FindProperty("digital"));
					if(target.digital == GaugeSystem.Digital.Color)
					{
						OpenHorizontal();
						{
							LabelWidth();
							PropertyContainer1(serializedObject.FindProperty("ticksColorOn"),group: true,design: 3);
							PropertyContainer1(serializedObject.FindProperty("ticksColorOff"),group: true,design: 3);
						}
						CloseHorizontal();
					}
				}
				CloseVertical();
			}
		}
		private void TicksSection ()
		{
			OpenVerticalSubsection();
			{
				FieldWidth(1);
				LabelWidth(164);
				PropertyContainer1(serializedObject.FindProperty("tickTransformUpdates"),design: 1);
				LabelWidth(130);
				PropertyContainer1(serializedObject.FindProperty("tickColorUpdates"),design: 1);
				LabelWidth(125);
				PropertyContainer1(serializedObject.FindProperty("tickTextUpdates"),design: 1);
				LabelWidth(164);
				PropertyContainer1(serializedObject.FindProperty("tickTextColorUpdates"),design: 1);
				FieldWidth();
			}
			CloseVertical();
			LabelWidth(82);
			PropertyContainer1(serializedObject.FindProperty("minorTicks"));
			LabelWidth();
			Container1(serializedObject.FindProperty("majorTicks"),target.majorTicks,required: target.runInEditor || Application.isPlaying,@default: target.majorTicks.Count.ToString());
			OpenVerticalSubsection();
			{
				OpenHorizontal();
				{
					PropertyContainer1(serializedObject.FindProperty("majorTickPrefab"),group: true,design: 3);
					GUI.enabled = target.useText || target.useTickText;
					PropertyContainer1(serializedObject.FindProperty("textPrefab"),group: true,design: 3);
					GUI.enabled = true;
				}
				CloseHorizontal();
				PropertyContainer1(serializedObject.FindProperty("minorTickPrefab"),design: 3);
			}
			CloseVertical();
		}
		private void ShapeSection ()
		{
			LabelWidth(81);
			PropertyContainer1(serializedObject.FindProperty("orientation"));
			LabelWidth();
			GUI.enabled = !target.connectEndpoints;
			PropertyContainer1(serializedObject.FindProperty("minimumAngle"),design: 2);
			PropertyContainer1(serializedObject.FindProperty("maximumAngle"),design: 2);
			GUI.enabled = true;
			Container2(serializedObject.FindProperty("shapes"),target.shapes,primary: ShapeSectionShapesContainer,header: () => PropertyContainer1(serializedObject.FindProperty("circleTolerance"),design: 2));
		}
		private void ShapeSectionShapesContainer (GaugeSystem.Shape current,SerializedProperty currentProperty)
		{
			LabelWidth(40);
			PropertyContainer1(currentProperty.FindPropertyRelative("type"),content: () => PressButton(currentProperty.FindPropertyRelative("straightenBorder"),"Makes sure that the border of the shape is straight, so that no parts of the ticks reach outside that border."),width: -1);
			FieldWidth(31);
			LabelWidth(1);
			PropertyContainer1(currentProperty.FindPropertyRelative("tolerance"),design: 2);
			OpenVerticalSubsection();
			{
				LabelWidth(66);
				PropertyContainer1(currentProperty.FindPropertyRelative("starness"),design: 1);
				LabelWidth(1);
				GUI.enabled = current.starness != 0;
				PropertyContainer1(currentProperty.FindPropertyRelative("starSharpness"),design: 3);
				GUI.enabled = true;
			}
			CloseVertical();
			FieldWidth();
			LabelWidth(81);
			PropertyContainer1(currentProperty.FindPropertyRelative("orientation"),design: 0);
			LabelWidth();
		}
		private void ValuesSection ()
		{
			LabelWidth(106);
			PropertyContainer1(serializedObject.FindProperty("minimumValue"));
			OpenHorizontalSubsection();
			{
				LabelWidth(45);
				PropertyContainer1(serializedObject.FindProperty("value"),group: true,design: 1);
				ControlBar(() =>
				{
					LabelWidth(47);
					Property(serializedObject.FindProperty("range"));
				});
			}
			CloseHorizontal();
			LabelWidth(110);
			PropertyContainer1(serializedObject.FindProperty("maximumValue"));
			Container2(serializedObject.FindProperty("additionalValues"),target.additionalValues,primary: ValuesSectionAdditionalValuesContainer);
			Container("Needle",ValuesSectionNeedlesContainer,toggleProperty: serializedObject.FindProperty("useNeedle"));
			Container("Text",MajorTicksSectionTextContainer,toggleProperty: serializedObject.FindProperty("useText"));
		}
		private void ValuesSectionAdditionalValuesContainer (GaugeSystem.AdditionalValue current,SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			LabelWidth(106);
			PropertyContainer1(currentProperty.FindPropertyRelative("minimumValue"));
			OpenHorizontalSubsection();
			{
				FieldWidth(1);
				LabelWidth(44);
				PropertyContainer1(currentProperty.FindPropertyRelative("value"),group: true,design: 1);
				ControlBar(() =>
				{
					FieldWidth(30);
					LabelWidth(47);
					Property(currentProperty.FindPropertyRelative("range"));
					FieldWidth();
				});
			}
			CloseHorizontal();
			LabelWidth(110);
			PropertyContainer1(currentProperty.FindPropertyRelative("maximumValue"));
		}
		private void ValuesSectionNeedlesContainer ()
		{
			OpenVerticalSubsection();
			{
				FieldWidth(1);
				LabelWidth(158);
				PropertyContainer1(serializedObject.FindProperty("needleTransformUpdates"),design: 1);
				LabelWidth(147);
				PropertyContainer1(serializedObject.FindProperty("needleColorUpdates"),design: 1);
				FieldWidth();
			}
			CloseVertical();
			LabelWidth(100);
			PropertyContainer1(serializedObject.FindProperty("needlePrefab"));
			Container2(serializedObject.FindProperty("needles"),target.needles,primary: ValuesSectionNeedlesContainerNeedlesContainer,onDestroy: ValuesSectionNeedlesContainerNeedlesContainerDestroy);
		}
		private void ValuesSectionNeedlesContainerNeedlesContainerDestroy (int index) {target.DestroyNeedleAtIndex(index);}
		private void ValuesSectionNeedlesContainerNeedlesContainer (GaugeSystem.Needle current,SerializedProperty currentProperty)
		{
			OpenVerticalSubsection();
			{
				LabelWidth(76);
				PropertyContainer1(currentProperty.FindPropertyRelative("integerize"),group: true,design: 1);
				GUI.enabled = target.additionalValues.Count != 0;
				IndexContainer(currentProperty.FindPropertyRelative("index"),serializedObject.FindProperty("additionalValues"),@default: "Built-in",design: 1);
				GUI.enabled = true;
			}
			CloseVertical();
			if(target.dimension == GaugeSystem.Dimension.ThreeDimensional && target.threeDimensional == GaugeSystem.ThreeDimensional.MeshRenderer)
			{
				LabelWidth(61);
				PropertyContainer1(currentProperty.FindPropertyRelative("material"));
			}
			Container("Offset",() =>
			{
				FieldWidth(1);
				LabelWidth(78);
				PropertyContainer1(currentProperty.FindPropertyRelative("pivotPoint"),design: 1);
				LabelWidth(85);
				PropertyContainer1(currentProperty.FindPropertyRelative("pivotOffset"),design: 1);
				LabelWidth(127);
				PropertyContainer1(currentProperty.FindPropertyRelative("pivotOffsetFactor"),design: 1);
				LabelWidth(85);
				PropertyContainer1(currentProperty.FindPropertyRelative("headOffset"),design: 1);
				LabelWidth(127);
				PropertyContainer1(currentProperty.FindPropertyRelative("headOffsetFactor"),design: 1);
				FieldWidth();
			},generalState: target.useNeedle,design: 2);
			Container("Scale",() =>
			{
				LabelWidth();
				PropertyContainer1(currentProperty.FindPropertyRelative("scale"),design: 3);
				PropertyContainer1(currentProperty.FindPropertyRelative("scaleMultiplier"),design: 3);
				FieldWidth(1);
				LabelWidth(90);
				PropertyContainer1(currentProperty.FindPropertyRelative("scaleFactor"),design: 1);
				LabelWidth(80);
				PropertyContainer1(currentProperty.FindPropertyRelative("pivotScale"),design: 1);
			},generalState: target.useNeedle,design: 2);
			LabelWidth(42);
			PropertyContainer2("Color",currentProperty.FindPropertyRelative("overrideColor"),currentProperty.FindPropertyRelative("color"),generalState: target.useNeedle,design: 1);
		}
		private void MajorTicksSectionTextContainer ()
		{
			OpenVerticalSubsection();
			{
				FieldWidth(1);
				LabelWidth(94);
				PropertyContainer1(serializedObject.FindProperty("textUpdates"),design: 1);
				LabelWidth(158);
				PropertyContainer1(serializedObject.FindProperty("textTransformUpdates"),design: 1);
				LabelWidth(132);
				PropertyContainer1(serializedObject.FindProperty("textColorUpdates"),design: 1);
				FieldWidth();
			}
			CloseVertical();
			OpenVerticalSubsection();
			{
				LabelWidth(76);
				PropertyContainer1(serializedObject.FindProperty("integerize"),design: 1);
				GUI.enabled = target.additionalValues.Count != 0;
				IndexContainer(serializedObject.FindProperty("index"),serializedObject.FindProperty("additionalValues"),@default: "Built-in",design: 1);
				GUI.enabled = true;
				#if ENABLE_REFLECTION
				LabelWidth(38);
				Selection(serializedObject.FindProperty("text.text"),requiredVariable: "text",requiredVariableType: typeof(string),shrink: true,content: () => PressButton(serializedObject.FindProperty("absolute")),design: 1);
				#else
				#if ENABLE_UNITY_TEXT
				if(target.dimension == GaugeSystem.Dimension.TwoDimensional)
				{
					#endif
					LabelWidth(38);
					PropertyContainer1(serializedObject.FindProperty("text.text"),content: () => PressButton(serializedObject.FindProperty("absolute")),design: 1);
					#if ENABLE_UNITY_TEXT
				}
				else
				{
					FieldWidth(1);
					LabelWidth(74);
					PropertyContainer1(serializedObject.FindProperty("text.textMesh"),content: () => PressButton(serializedObject.FindProperty("absolute")),design: 1);
					FieldWidth();
				}
				#endif
				#endif
			}
			CloseVertical();
			Container("Offset",() =>
			{
				LabelWidth();
				PropertyContainer1(serializedObject.FindProperty("textOffset"),design: 3);
				LabelWidth(126);
				PropertyContainer1(serializedObject.FindProperty("textOffsetFactor"),design: 1);
			},design: 2);
			Container("Scale",() =>
			{
				LabelWidth();
				PropertyContainer1(serializedObject.FindProperty("textScale"),design: 3);
				LabelWidth(122);
				PropertyContainer1(serializedObject.FindProperty("textScaleFactor"),design: 1);
			},design: 2);
			LabelWidth(76);
			PropertyContainer2("Color",serializedObject.FindProperty("overrideTextColor"),serializedObject.FindProperty("textColor"),design: 1);
		}
		private void ConfigurationsSection ()
		{
			LabelWidth(51);
			PropertyContainer1(serializedObject.FindProperty("radius"));
			LabelWidth(98);
			PropertyContainer1(serializedObject.FindProperty("radiusFactor"));
			Container("Major Tick",() =>
			{
				LabelWidth(113);
				PropertyContainer1(serializedObject.FindProperty("majorTickPivot"));
				Container("Offset",() =>
				{
					LabelWidth();
					PropertyContainer1(serializedObject.FindProperty("majorTickOffset"),design: 3);
					FieldWidth(1);
					LabelWidth(150);
					PropertyContainer1(serializedObject.FindProperty("majorTickOffsetFactor"),design: 1);
					FieldWidth();
				},design: 2);
				Container("Scale",() =>
				{
					LabelWidth();
					PropertyContainer1(serializedObject.FindProperty("majorTickScale"),design: 3);
					FieldWidth(1);
					LabelWidth(150);
					PropertyContainer1(serializedObject.FindProperty("majorTickScaleFactor"),design: 1);
					FieldWidth();
				},design: 2);
				if(target.type != GaugeSystem.Type.Digital || target.digital != GaugeSystem.Digital.Color)
				{
					LabelWidth(114);
					PropertyContainer2("Color",serializedObject.FindProperty("overrideMajorTickColor"),serializedObject.FindProperty("majorTickColor"),design: 1);
				}
			});
			Container("Tick Text",() =>
			{
				LabelWidth(119);
				GUI.enabled = target.useTickText;
				PropertyContainer1(serializedObject.FindProperty("tickTextAnchor"),design: 2);
				Container("Offset",() =>
				{
					LabelWidth();
					PropertyContainer1(serializedObject.FindProperty("tickTextOffset"),design: 3);
					FieldWidth(1);
					LabelWidth(150);
					PropertyContainer1(serializedObject.FindProperty("tickTextOffsetFactor"),design: 1);
					FieldWidth();
				},generalState: target.useTickText,design: 2);
				Container("Scale",() =>
				{
					LabelWidth();
					PropertyContainer1(serializedObject.FindProperty("tickTextScale"),design: 3);
					FieldWidth(1);
					LabelWidth(150);
					PropertyContainer1(serializedObject.FindProperty("tickTextScaleFactor"),design: 1);
					FieldWidth();
				},generalState: target.useTickText,design: 2);
				LabelWidth(107);
				PropertyContainer2("Color",serializedObject.FindProperty("overrideTickTextColor"),serializedObject.FindProperty("tickTextColor"),generalState: target.useTickText,design: 1);
			},labelContent: () => PressButton(serializedObject.FindProperty("verticalRotation"),target.useTickText),toggleProperty: serializedObject.FindProperty("useTickText"),design: 1);
			Container("Minor Tick",() =>
			{
				LabelWidth(113);
				PropertyContainer1(serializedObject.FindProperty("minorTickPivot"));
				Container("Offset",() =>
				{
					LabelWidth();
					PropertyContainer1(serializedObject.FindProperty("minorTickOffset"),design: 3);
					FieldWidth(1);
					LabelWidth(150);
					PropertyContainer1(serializedObject.FindProperty("minorTickOffsetFactor"),design: 1);
					FieldWidth();
				},design: 2);
				Container("Scale",() =>
				{
					LabelWidth();
					PropertyContainer1(serializedObject.FindProperty("minorTickScale"),design: 3);
					FieldWidth(1);
					LabelWidth(150);
					GUI.enabled = target.type == GaugeSystem.Type.Analog;
					PropertyContainer1(serializedObject.FindProperty("minorTickScaleAnalogFactor"),design: 1);
					GUI.enabled = target.type == GaugeSystem.Type.Digital;
					PropertyContainer1(serializedObject.FindProperty("minorTickScaleDigitalFactor"),design: 1);
					GUI.enabled = true;
					FieldWidth();
				},design: 2);
				if(target.type != GaugeSystem.Type.Digital || target.digital != GaugeSystem.Digital.Color)
				{
					LabelWidth(114);
					PropertyContainer2("Color",serializedObject.FindProperty("overrideMinorTickColor"),serializedObject.FindProperty("minorTickColor"),design: 1);
				}
			});
		}
		private void FrameSchedulingSection ()
		{
			FieldWidth(1);
			LabelWidth(180);
			PropertyContainer1(serializedObject.FindProperty("tickTransformFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("tickColorFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("tickTextFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("tickTextColorFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("needleTransformFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("needleColorFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("textFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("textTransformFrames"),design: 1);
			PropertyContainer1(serializedObject.FindProperty("textColorFrames"),design: 1);
			FieldWidth();
			LabelWidth();
		}
	}
}