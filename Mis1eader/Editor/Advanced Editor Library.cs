namespace Mis1eader
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEditor;
	using System.Collections.Generic;
	internal static class EditorContents
	{
		public static readonly bool isPro = EditorGUIUtility.isProSkin;
		public static readonly Color red = isPro ? Color.red : new Color(1,0.5f,0.5f,1),
		green = isPro ? Color.green : new Color(0.5f,1,0.5f,1),
		yellow = new Color(1,0.86f,0.25f,1);
		public static readonly GUIContent inspector = new GUIContent("Inspector",EditorGUIUtility.IconContent(isPro ? "d_UnityEditor.InspectorWindow" : "UnityEditor.InspectorWindow").image);
		public static readonly Texture viewOn = EditorGUIUtility.IconContent(isPro ? "d_ViewToolOrbit On" : "ViewToolOrbit On").image,
		viewOff = EditorGUIUtility.IconContent(isPro ? "d_ViewToolOrbit" : "ViewToolOrbit").image,
		error = EditorGUIUtility.IconContent("console.erroricon.sml").image,
		info = EditorGUIUtility.IconContent("console.infoicon.sml").image,
		positive = EditorGUIUtility.IconContent("sv_icon_dot3_pix16_gizmo").image,
		negative = EditorGUIUtility.IconContent("sv_icon_dot14_pix16_gizmo").image,
		uncertain = EditorGUIUtility.IconContent("sv_icon_dot13_pix16_gizmo").image;
		public static readonly GUIStyle section = GUI.skin.box,
		subsection = EditorStyles.helpBox,
		container = GUI.skin.button,
		label1 = EditorStyles.label,
		label2 = new GUIStyle(label1) {fontSize = 10},
		label3 = new GUIStyle(label1) {fontSize = 9},
		label4 = new GUIStyle(label1) {fontSize = 8},
		boldLabel = new GUIStyle(label1) {fontStyle = FontStyle.Bold},
		boldLabelCenter = new GUIStyle(label1) {alignment = TextAnchor.MiddleCenter,fontStyle = FontStyle.Bold},
		button1 = EditorStyles.toolbarButton,
		button2 = new GUIStyle(button1) {fontSize = 8},
		buttonLeft = new GUIStyle(button1) {alignment = TextAnchor.MiddleLeft},
		buttonBoldLabel = new GUIStyle(button1) {fontStyle = FontStyle.Bold},
		buttonLeftBoldLabel = new GUIStyle(buttonLeft) {fontStyle = FontStyle.Bold},
		buttonMiniLabel1 = new GUIStyle(button1) {fontSize = 8},
		buttonMiniLabel2 = new GUIStyle(button1) {fontSize = 7},
		miniButton = EditorStyles.miniButton,
		miniButtonLeft = EditorStyles.miniButtonLeft,
		miniButtonMid = EditorStyles.miniButtonMid,
		miniButtonRight = EditorStyles.miniButtonRight;
	}
	internal class Editor<T0> : UnityEditor.Editor
	{
		internal new T0 target = default(T0);
		internal new T0[] targets = null;
		internal void OnEnable ()
		{
			int A = base.targets.Length;
			T0[] targets = new T0[A];
			for(int a = 0; a < A; a++)
				targets[a] = (T0)(object)base.targets[a];
			this.target = targets[0];
			this.targets = targets;
		}
		internal static T0 CreateProfile (out string name)
		{
			//ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<End>(),"New " + TitleName(typeof(T0).Name) + ".asset",EditorGUIUtility.FindTexture("ScriptableObject Icon"),string.Empty);
			string path = EditorUtility.SaveFilePanelInProject("Save Asset","New " + TitleName(typeof(T0).Name),"asset",string.Empty);
			if(path != string.Empty)
			{
				Object profile = ScriptableObject.CreateInstance(typeof(T0));
				AssetDatabase.CreateAsset(profile,path);
				ProjectWindowUtil.ShowCreatedAsset(profile);
				name = System.IO.Path.GetFileName(path.Replace(".asset",string.Empty));
				return (T0)(object)profile;
			}
			name = null;
			return default(T0);
		}
		/*internal class End : UnityEditor.ProjectWindowCallback.EndNameEditAction
		{
			public override void Action (int instanceId,string pathName,string resourceFile)
			{
				T0 profile = (T0)(object)ScriptableObject.CreateInstance(typeof(T0));
				//profile.name = System.IO.Path.GetFileName(pathName.Replace(".asset",string.Empty));
				AssetDatabase.CreateAsset(profile as Object,pathName);
				//ProjectWindowUtil.ShowCreatedAsset(profile as Object);
			}
		}*/
		private bool isAdvanced = true;
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			OpenHorizontal(EditorContents.section);
			{
				Label(EditorContents.inspector);
				GUILayout.Space(1);
				isAdvanced = GUILayout.Toolbar(isAdvanced ? 0 : 1,new string[] {"Advanced","Standard"}) == 0;
			}
			CloseHorizontal();
			if(isAdvanced)Inspector();
			else base.OnInspectorGUI();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int a = 0,A = base.targets.Length; a < A; a++)
					EditorUtility.SetDirty(base.targets[a]);
			}
		}
		internal virtual void Inspector () {base.OnInspectorGUI();}
		public static void FieldWidth (float width = 0) {EditorGUIUtility.fieldWidth = width;}
		public static void LabelWidth (float width = 0) {EditorGUIUtility.labelWidth = width;}
		public static void Property (SerializedProperty property,params GUILayoutOption[] options) {Property(new GUIContent(property.displayName),property,options);}
		public static void Property (SerializedProperty property,string tooltip,params GUILayoutOption[] options) {Property(new GUIContent(property.displayName,tooltip),property,options);}
		public static void Property (string title,SerializedProperty property,params GUILayoutOption[] options) {Property(new GUIContent(title),property,options);}
		public static void Property (string title,SerializedProperty property,string tooltip,params GUILayoutOption[] options) {Property(new GUIContent(title,tooltip),property,options);}
		public static void Property (GUIContent content,SerializedProperty property,params GUILayoutOption[] options)
		{
			if(property != null)EditorGUILayout.PropertyField(property,content,true,options);
			else Debug.LogError("No property was found");
		}
		public static Rect OpenHorizontal (params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(options);}
		public static Rect OpenHorizontalBar (params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(EditorContents.button1,options);}
		public static Rect OpenHorizontalContainer (params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(EditorContents.container,options);}
		public static Rect OpenHorizontalSubsection (params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(EditorContents.subsection,options);}
		public static Rect OpenHorizontal (GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(style,options);}
		public static void CloseHorizontal () {EditorGUILayout.EndHorizontal();}
		public static Rect OpenVertical (params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(options);}
		public static Rect OpenVerticalContainer (params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(EditorContents.container,options);}
		public static Rect OpenVerticalSubsection (params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(EditorContents.subsection,options);}
		public static Rect OpenVertical (GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(style,options);}
		public static void CloseVertical () {EditorGUILayout.EndVertical();}
		public static void FlexibleSpace () {GUILayout.FlexibleSpace();}
		public static void VerticalSpace () {GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);}
		public static void IconSize (float size = 0) {EditorGUIUtility.SetIconSize(Vector2.one * size);}
		public static void Label (string text,params GUILayoutOption[] options) {Label(new GUIContent(text),EditorContents.label1,options);}
		public static void Label (string text,string tooltip,params GUILayoutOption[] options) {Label(new GUIContent(text,tooltip),EditorContents.label1,options);}
		public static void Label (Texture image,params GUILayoutOption[] options) {Label(new GUIContent(image),EditorContents.label1,options);}
		public static void Label (Texture image,string tooltip,params GUILayoutOption[] options) {Label(new GUIContent(image,tooltip),EditorContents.label1,options);}
		public static void Label (string text,Texture image,params GUILayoutOption[] options) {Label(new GUIContent(text,image),EditorContents.label1,options);}
		public static void Label (string text,Texture image,string tooltip,params GUILayoutOption[] options) {Label(new GUIContent(text,image,tooltip),EditorContents.label1,options);}
		public static void Label (GUIContent content,params GUILayoutOption[] options) {Label(content,EditorContents.label1,options);}
		public static void Label (string text,GUIStyle style,params GUILayoutOption[] options) {Label(new GUIContent(text),style,options);}
		public static void Label (string text,string tooltip,GUIStyle style,params GUILayoutOption[] options) {Label(new GUIContent(text,tooltip),style,options);}
		public static void Label (Texture image,GUIStyle style,params GUILayoutOption[] options) {Label(new GUIContent(image),style,options);}
		public static void Label (Texture image,string tooltip,GUIStyle style,params GUILayoutOption[] options) {Label(new GUIContent(image,tooltip),style,options);}
		public static void Label (string text,Texture image,GUIStyle style,params GUILayoutOption[] options) {Label(new GUIContent(text,image),style,options);}
		public static void Label (string text,Texture image,string tooltip,GUIStyle style,params GUILayoutOption[] options) {Label(new GUIContent(text,image,tooltip),style,options);}
		public static void Label (GUIContent content,GUIStyle style,params GUILayoutOption[] options)
		{
			if(content.text != string.Empty && content.image != null)
				content = new GUIContent(content.text.Insert(0," "),content.image,content.tooltip);
			GUILayout.Label(content,style,options);
		}
		public static bool PressButton (string text,params GUILayoutOption[] options) {return PressButton(new GUIContent(text),EditorContents.button1,options);}
		public static bool PressButton (string text,string tooltip,params GUILayoutOption[] options) {return PressButton(new GUIContent(text,tooltip),EditorContents.button1,options);}
		public static bool PressButton (Texture image,params GUILayoutOption[] options) {return PressButton(new GUIContent(image),EditorContents.button1,options);}
		public static bool PressButton (Texture image,string tooltip,params GUILayoutOption[] options) {return PressButton(new GUIContent(image,tooltip),EditorContents.button1,options);}
		public static bool PressButton (string text,Texture image,params GUILayoutOption[] options) {return PressButton(new GUIContent(text,image),EditorContents.button1,options);}
		public static bool PressButton (string text,Texture image,string tooltip,params GUILayoutOption[] options) {return PressButton(new GUIContent(text,image,tooltip),EditorContents.button1,options);}
		public static bool PressButton (GUIContent content,params GUILayoutOption[] options) {return PressButton(content,EditorContents.button1,options);}
		public static bool PressButton (string text,GUIStyle style,params GUILayoutOption[] options) {return PressButton(new GUIContent(text),style,options);}
		public static bool PressButton (string text,string tooltip,GUIStyle style,params GUILayoutOption[] options) {return PressButton(new GUIContent(text,tooltip),style,options);}
		public static bool PressButton (Texture image,GUIStyle style,params GUILayoutOption[] options) {return PressButton(new GUIContent(image),style,options);}
		public static bool PressButton (Texture image,string tooltip,GUIStyle style,params GUILayoutOption[] options) {return PressButton(new GUIContent(image,tooltip),style,options);}
		public static bool PressButton (string text,Texture image,GUIStyle style,params GUILayoutOption[] options) {return PressButton(new GUIContent(text,image),style,options);}
		public static bool PressButton (string text,Texture image,string tooltip,GUIStyle style,params GUILayoutOption[] options) {return PressButton(new GUIContent(text,image,tooltip),style,options);}
		public static bool PressButton (GUIContent content,GUIStyle style,params GUILayoutOption[] options)
		{
			if(content.text != string.Empty && content.image != null)
				content = new GUIContent(content.text.Insert(0," "),content.image,content.tooltip);
			if(GUILayout.Button(content,style,options))
			{
				GUI.FocusControl(null);
				return true;
			}
			return false;
		}
		public static bool PressButton (SerializedProperty property,bool state = true) {return PressButton(new GUIContent(property.displayName),property,EditorContents.miniButton,state);}
		public static bool PressButton (SerializedProperty property,string tooltip,bool state = true) {return PressButton(new GUIContent(property.displayName,tooltip),property,EditorContents.miniButton,state);}
		public static bool PressButton (string name,SerializedProperty property,bool state = true) {return PressButton(new GUIContent(name),property,EditorContents.miniButton,state);}
		public static bool PressButton (string name,SerializedProperty property,string tooltip,bool state = true) {return PressButton(new GUIContent(name,tooltip),property,EditorContents.miniButton,state);}
		//public static bool PressButton (SerializedProperty property,GUIStyle style,bool state = true) {return PressButton(new GUIContent(property.displayName),property,style,state);}
		public static bool PressButton (SerializedProperty property,string tooltip,GUIStyle style,bool state = true) {return PressButton(new GUIContent(property.displayName,tooltip),property,style,state);}
		//public static bool PressButton (string name,SerializedProperty property,GUIStyle style,bool state = true) {return PressButton(new GUIContent(name),property,style,state);}
		public static bool PressButton (string name,SerializedProperty property,string tooltip,GUIStyle style,bool state = true) {return PressButton(new GUIContent(name,tooltip),property,style,state);}
		public static bool PressButton (GUIContent content,SerializedProperty property,GUIStyle style,bool state = true)
		{
			GUI.enabled = state;
			GUI.backgroundColor = !property.hasMultipleDifferentValues ? (property.boolValue ? EditorContents.green : EditorContents.red) : EditorContents.yellow;
			if(GUILayout.Button(content,style,GUILayout.ExpandWidth(false)))
			{
				Undo.RecordObjects(property.serializedObject.targetObjects,"Inspector");
				property.boolValue = !property.boolValue;
				GUI.enabled = true;
				GUI.backgroundColor = Color.white;
				GUI.FocusControl(null);
				return true;
			}
			GUI.enabled = true;
			GUI.backgroundColor = Color.white;
			return false;
		}
		
		
		
		
		
		
		
		
		
		public static bool HoldButton (Texture image,params GUILayoutOption[] options) {return HoldButton(null,image,null,EditorContents.button1,options);}
		public static bool HoldButton (string text,Texture image,string tooltip,GUIStyle style,params GUILayoutOption[] options)
		{
			if(GUILayout.RepeatButton(new GUIContent(text,image,tooltip),style,options))
			{
				GUI.FocusControl(null);
				return true;
			}
			return false;
		}
		
		
		
		
		
		public static SerializedProperty PreviousProperty (SerializedProperty property)
		{
			if(property == null)return null;
			return property.serializedObject.FindProperty(property.propertyPath.Remove(property.propertyPath.Length - property.name.Length,property.name.Length));
		}
		public static string TitleName (string name)
		{
			if(name == string.Empty)return name;
			for(int a = 1,A = name.Length; a < A; a++)
			{
				if(char.IsUpper(name[a - 1]) || char.IsLower(name[a]))continue;
				name = name.Insert(a," ");
				a = a + 2;
				A = A + 1;
			}
			if(char.IsLower(name[0]))name = char.ToUpper(name[0]) + name.Substring(1);
			return name;
		}
		public static string SingularName (string name)
		{
			if(name == string.Empty)return name;
			int length = name.Length;
			if(name[length - 1] == 's')
			{
				if(length >= 3 && name[length - 2] == 'e' && name[length - 3] == 'i')return name.Remove(length - 3,3) + "y";
				return name.Remove(length - 1);
			}
			return name;
		}
		
		
		
		
		
		public static void Toggle (SerializedProperty toggleProperty,byte design = 0) {Toggle(toggleProperty,EditorContents.button1,design);}
		public static void Toggle (SerializedProperty toggleProperty,GUIStyle style,byte design = 0)
		{
			if(design == 1)OpenHorizontal(style,GUILayout.Width(1));
			Property(string.Empty,toggleProperty,GUILayout.Width(EditorContents.button1.CalcSize(GUIContent.none).y - 5));
			if(design == 1)CloseHorizontal();
		}
		public static void Section (string name,UnityAction content,UnityAction labelContent = null)
		{
			OpenVertical(EditorContents.section);
			{
				OpenHorizontal();
				{
					LabelWidth(1);
					Label(name.ToUpper(),EditorContents.boldLabel);
					LabelWidth();
					if(labelContent != null)labelContent();
				}
				CloseHorizontal();
				if(content != null)content();
			}
			CloseVertical();
		}
		public static void Section (string name,ref bool isExpanded,UnityAction content,UnityAction labelContent = null,bool space = true)
		{
			if(space)GUILayout.Space(10);
			OpenVertical(EditorContents.section);
			{
				OpenHorizontal();
				{
					if(PressButton(name.ToUpper(),EditorContents.boldLabel))isExpanded = !isExpanded;
					if(labelContent != null)labelContent();
				}
				CloseHorizontal();
				if(content != null && isExpanded)content();
			}
			CloseVertical();
		}
		public static void Subsection (string name,UnityAction content,UnityAction labelContent = null)
		{
			OpenVerticalSubsection();
			{
				OpenHorizontal();
				{
					LabelWidth(1);
					Label(name.ToUpper(),EditorContents.boldLabel);
					LabelWidth();
					if(labelContent != null)labelContent();
				}
				CloseHorizontal();
				if(content != null)content();
			}
			CloseVertical();
		}
		public static void ControlBar (UnityAction content,byte design = 0)
		{
			if(design == 0)OpenHorizontalBar(GUILayout.Width(1));
			else if(design == 1)OpenHorizontalBar();
			if(content != null)content();
			CloseHorizontal();
		}
		public static void Index (int index,byte digits = 3) {Label(index.ToString(new string('0',digits)),EditorContents.buttonMiniLabel1,GUILayout.ExpandWidth(false));}
		public static void Index (int index,Color color,byte digits = 3)
		{
			Color previous = GUI.color;
			GUI.color = color;
			Label(index.ToString(new string('0',digits)),EditorContents.buttonMiniLabel1,GUILayout.ExpandWidth(false));
			GUI.color = previous;
		}
		public static bool IndexButton (int index,byte digits = 3) {return PressButton(index.ToString(new string('0',digits)),EditorContents.buttonMiniLabel1,GUILayout.ExpandWidth(false));}
		public static bool IndexButton (int index,SerializedProperty property,byte digits = 3)
		{
			if(PressButton(index.ToString(new string('0',digits)),EditorContents.buttonMiniLabel1,GUILayout.ExpandWidth(false)))
			{
				property.isExpanded = !property.isExpanded;
				return true;
			}
			return false;
		}
		public static bool IndexButton (int index,SerializedProperty property,Color color,byte digits = 3)
		{
			Color previous = GUI.color;
			GUI.color = color;
			if(PressButton(index.ToString(new string('0',digits)),EditorContents.buttonMiniLabel1,GUILayout.ExpandWidth(false)))
			{
				property.isExpanded = !property.isExpanded;
				GUI.color = previous;
				return true;
			}
			GUI.color = previous;
			return false;
		}
		public static void Operator (SerializedProperty property,ref bool operatorIsExpanded,bool shrink = true)
		{
			OpenVerticalSubsection();
			{
				OpenHorizontal();
				{
					IconSize(10);
					if(PressButton(operatorIsExpanded ? EditorContents.viewOn : EditorContents.viewOff,GUILayout.ExpandWidth(false)) || PressButton("Operator",EditorContents.buttonLeft,GUILayout.ExpandWidth(true)))
						operatorIsExpanded = !operatorIsExpanded;
					IconSize();
				}
				CloseHorizontal();
				OpenHorizontal();
				{
					GUIStyle style = !operatorIsExpanded ? (!shrink ? EditorContents.buttonMiniLabel1 : EditorContents.buttonMiniLabel2) : EditorContents.button1;
					if(!operatorIsExpanded)GUILayout.Space(0.5f);
					GUI.backgroundColor = property.enumValueIndex == 0 ? EditorContents.green : EditorContents.red;
					if(PressButton("<","Less Than (<)",style,GUILayout.ExpandWidth(!operatorIsExpanded)) && property.enumValueIndex != 0)property.enumValueIndex = 0;
					GUI.backgroundColor = property.enumValueIndex == 1 ? EditorContents.green : EditorContents.red;
					if(PressButton("<=","Less Than Or Equal To (≤)",style,GUILayout.ExpandWidth(!operatorIsExpanded)) && property.enumValueIndex != 1)property.enumValueIndex = 1;
					if(operatorIsExpanded)
					{
						GUI.backgroundColor = EditorContents.red;
						Label(GUIContent.none,EditorContents.button1,GUILayout.ExpandWidth(true));
						CloseHorizontal();
						OpenHorizontalBar();
						FlexibleSpace();
					}
					GUI.backgroundColor = property.enumValueIndex == 2 ? EditorContents.green : EditorContents.red;
					if(PressButton("!=","Not Equal To (≠)",style,GUILayout.ExpandWidth(!operatorIsExpanded)) && property.enumValueIndex != 2)property.enumValueIndex = 2;
					GUI.backgroundColor = property.enumValueIndex == 3 ? EditorContents.green : EditorContents.red;
					if(PressButton("==","Equal To (=)",style,GUILayout.ExpandWidth(!operatorIsExpanded)) && property.enumValueIndex != 3)property.enumValueIndex = 3;
					if(operatorIsExpanded)
					{
						FlexibleSpace();
						CloseHorizontal();
						OpenHorizontal();
						GUI.backgroundColor = EditorContents.red;
						Label(GUIContent.none,EditorContents.button1,GUILayout.ExpandWidth(true));
					}
					GUI.backgroundColor = property.enumValueIndex == 4 ? EditorContents.green : EditorContents.red;
					if(PressButton(">=","Greater Than Or Equal To (≥)",style,GUILayout.ExpandWidth(!operatorIsExpanded)) && property.enumValueIndex != 4)property.enumValueIndex = 4;
					GUI.backgroundColor = property.enumValueIndex == 5 ? EditorContents.green : EditorContents.red;
					if(PressButton(">","Greater Than (>)",style,GUILayout.ExpandWidth(!operatorIsExpanded)) && property.enumValueIndex != 5)property.enumValueIndex = 5;
					GUI.backgroundColor = Color.white;
				}
				CloseHorizontal();
			}
			CloseVertical();
		}
		public static void PropertyContainer1 (SerializedProperty property,UnityAction content = null,bool group = false,sbyte width = 0,byte design = 0) {PropertyContainer1(property.displayName,property,content,group,width,design);}
		public static void PropertyContainer1 (string name,SerializedProperty property,UnityAction content = null,bool group = false,sbyte width = 0,byte design = 0)
		{
			if(design == 1 || design == 3)
			{
				if(!group)VerticalSpace();
				OpenVertical();
			}
			else OpenVerticalSubsection();
			{
				if(design == 0 || design == 1)
				{
					OpenHorizontalBar();
					{
						if(width == -1)FieldWidth(1);
						Property(name,property);
						if(width == -1)FieldWidth();
						if(content != null)content();
					}
					CloseHorizontal();
				}
				else
				{
					Label(name,!property.prefabOverride ? EditorContents.button1 : EditorContents.buttonBoldLabel);
					OpenHorizontalBar();
					{
						if(width <= 0)Property(string.Empty,property);
						else Property(string.Empty,property,GUILayout.MinWidth(width));
					}
					CloseHorizontal();
				}
			}
			CloseVertical();
		}
		public static void PropertyContainer2 (SerializedProperty toggleProperty,SerializedProperty property,UnityAction labelContent = null,bool group = false,bool generalState = true,bool state = true,byte design = 0) {PropertyContainer2(property.displayName,toggleProperty,property,labelContent,group,generalState,state,design);}
		public static void PropertyContainer2 (string name,SerializedProperty toggleProperty,SerializedProperty property,UnityAction labelContent = null,bool group = false,bool generalState = true,bool state = true,byte design = 0)
		{
			GUI.enabled = generalState;
			if(!group)OpenVerticalSubsection();
			else OpenVertical();
			{
				if(design == 0)
				{
					OpenHorizontalBar();
					{
						Toggle(toggleProperty);
						GUI.enabled = generalState && state && toggleProperty.boolValue;
						FieldWidth(1);
						Property(property);
						FieldWidth();
					}
					CloseHorizontal();
				}
				else if(design == 1 || design == 2 || design == 3)
				{
					if(design == 1)
					{
						OpenHorizontalBar();
						{
							FlexibleSpace();
							Label(name,EditorContents.boldLabel);
							FlexibleSpace();
						}
						CloseHorizontal();
						OpenHorizontalBar();
						{
							Toggle(toggleProperty);
							GUI.enabled = generalState && state && toggleProperty.boolValue;
							FieldWidth(1);
							Property(property);
							FieldWidth();
						}
						CloseHorizontal();
					}
					else
					{
						OpenHorizontalBar();
						{
							Toggle(toggleProperty);
							Label(name);
							if(labelContent != null)labelContent();
						}
						CloseHorizontal();
						GUI.enabled = generalState && state && toggleProperty.boolValue;
						if(design == 2 || design == 3 && toggleProperty.boolValue)
						{
							OpenHorizontalBar();
							Property(string.Empty,property);
							CloseHorizontal();
						}
					}
				}
			}
			CloseVertical();
			GUI.enabled = true;
		}
		public static void Container(string name,UnityAction content,UnityAction labelContent = null,SerializedProperty toggleProperty = null,bool generalState = true,bool state = true,byte design = 0)
		{
			GUI.enabled = generalState;
			OpenVerticalSubsection();
			{
				OpenHorizontal();
				{
					if(toggleProperty != null)Toggle(toggleProperty);
					if(design != 4)Label(name,design != 2 ? EditorContents.boldLabel : EditorContents.boldLabelCenter);
					else
					{
						LabelWidth(1);
						EditorGUILayout.LabelField(name,design != 2 ? EditorContents.boldLabel : EditorContents.boldLabelCenter);
						LabelWidth();
					}
					GUI.enabled = generalState && state;
					if(labelContent != null)labelContent();
				}
				CloseHorizontal();
				if(content != null && ((toggleProperty == null || toggleProperty != null && toggleProperty.boolValue) || design == 1))
				{
					if(design == 0 || design == 1)
					{
						OpenHorizontal();
						GUILayout.Space(20);
						OpenVertical();
					}
					content();
					if(design == 0 || design == 1)
					{
						CloseVertical();
						CloseHorizontal();
					}
				}
			}
			CloseVertical();
			GUI.enabled = true;
		}
		public static void Container1<T> (SerializedProperty property,List<T> content,SerializedProperty toggleProperty = null,bool required = false,T @default = default(T),string propertyName = null,bool state = true)
		{
			SerializedObject serializedObject = property.serializedObject;
			SerializedProperty scrollViewProperty = serializedObject.FindProperty(property.propertyPath + "ScrollView");
			Object target = serializedObject.targetObject;
			VerticalSpace();
			GUI.enabled = !serializedObject.isEditingMultipleObjects;
			OpenVerticalSubsection();
			{
				GUILayout.Space(1);
				OpenHorizontal();
				{
					if(toggleProperty != null)
					{
						Toggle(toggleProperty,1);
						state = state && toggleProperty.boolValue;
					}
					if(PressButton(required && content.Count == 0 ? new GUIContent(property.displayName,EditorContents.error,"This should not be left empty.") : new GUIContent(property.displayName),EditorContents.buttonBoldLabel,GUILayout.ExpandWidth(true)))
						property.isExpanded = !property.isExpanded;
					GUI.enabled = !serializedObject.isEditingMultipleObjects && content.Count != 0 && state;
					if(PressButton("×",GUILayout.ExpandWidth(false)))
					{
						Undo.RecordObject(target,"Inspector");
						content.Clear();
						serializedObject.Update();
					}
					GUI.enabled = true;
				}
				CloseHorizontal();
				if(!serializedObject.isEditingMultipleObjects && property.isExpanded)
				{
					float height = EditorContents.button1.CalcSize(GUIContent.none).y;
					byte count = 5;
					int index = 0;
					if(scrollViewProperty != null)
					{
						if(content.Count > count)
						{
							if(Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel)index = (int)(scrollViewProperty.vector2Value.y / height);
							scrollViewProperty.vector2Value = EditorGUILayout.BeginScrollView(scrollViewProperty.vector2Value,GUILayout.Height(count * height));
							serializedObject.ApplyModifiedProperties();
							if(Event.current.type != EventType.Repaint && Event.current.type != EventType.ScrollWheel)index = (int)(scrollViewProperty.vector2Value.y / height);
							if(index != 0)GUILayout.Space(index * height);
						}
						else
						{
							if(scrollViewProperty.vector2Value != Vector2.zero)
								scrollViewProperty.vector2Value = Vector2.zero;
							FieldWidth(1);
						}
					}
					GUI.enabled = state;
					for(int a = index,A = scrollViewProperty != null ? Mathf.Min(index + count + 1,content.Count) : content.Count; a < A; a++)
					{
						OpenHorizontalBar();
						{
							Index(a);
							Property(string.Empty,propertyName == null ? property.GetArrayElementAtIndex(a) : property.GetArrayElementAtIndex(a).FindPropertyRelative(propertyName));
							GUI.enabled = a != 0 && state;
							if(PressButton("▲",GUILayout.ExpandWidth(false)))
							{
								Undo.RecordObject(target,"Inspector");
								T other = content[a - 1];
								content[a - 1] = content[a];
								content[a] = other;
								break;
							}
							GUI.enabled = a != content.Count - 1 && state;
							if(PressButton("▼",GUILayout.ExpandWidth(false)))
							{
								Undo.RecordObject(target,"Inspector");
								T other = content[a + 1];
								content[a + 1] = content[a];
								content[a] = other;
								break;
							}
							GUI.enabled = state;
							if(PressButton("-",GUILayout.ExpandWidth(false)))
							{
								Undo.RecordObject(target,"Inspector");
								content.RemoveAt(a);
								break;
							}
						}
						CloseHorizontal();
					}
					if(scrollViewProperty != null)
					{
						if(content.Count > count)
						{
							if(index + count + 1 < content.Count)
								GUILayout.Space((content.Count - index - count - 1) * height);
							EditorGUILayout.EndScrollView();
						}
						else FieldWidth();
					}
					OpenHorizontal();
					{
						Label("Add a new " + SingularName(property.displayName) + "?",EditorContents.button1);
						if(PressButton("+",GUILayout.ExpandWidth(false)))
						{
							Undo.RecordObject(target,"Inspector");
							content.Add(@default);
						}
					}
					CloseHorizontal();
					if(toggleProperty != null)GUI.enabled = true;
				}
			}
			CloseVertical();
		}
		public static void Container2<T> (SerializedProperty property,List<T> content,SerializedProperty toggleProperty = null,UnityAction<T,SerializedProperty> primary = null,UnityAction<SerializedProperty> secondary = null,UnityAction header = null,UnityAction<int> onDestroy = null,bool isEvent = false,bool state = true) where T : new()
		{
			SerializedObject serializedObject = property.serializedObject;
			SerializedProperty nameProperty = serializedObject.FindProperty(property.propertyPath + "Name");
			Object target = serializedObject.targetObject;
			VerticalSpace();
			GUI.enabled = !serializedObject.isEditingMultipleObjects;
			OpenVerticalSubsection();
			{
				if(header != null)
				{
					GUILayout.Space(1);
					header();
				}
				GUILayout.Space(1);
				OpenHorizontal();
				{
					if(toggleProperty != null)
					{
						Toggle(toggleProperty,1);
						state = state && toggleProperty.boolValue;
					}
					if(PressButton(property.displayName,EditorContents.buttonBoldLabel,GUILayout.ExpandWidth(true)))
						property.isExpanded = !property.isExpanded;
					GUI.enabled = !serializedObject.isEditingMultipleObjects && content.Count != 0 && state;
					if(PressButton("×",GUILayout.ExpandWidth(false)))
					{
						Undo.RecordObject(target,"Inspector");
						if(onDestroy != null)for(int a = 0; a < content.Count; a++)onDestroy(a);
						content.Clear();
					}
					GUI.enabled = true;
				}
				CloseHorizontal();
				if(!serializedObject.isEditingMultipleObjects && property.isExpanded)
				{
					if(!state)GUI.enabled = false;
					for(int a = 0; a < content.Count; a++)
					{
						SerializedProperty currentProperty = property.GetArrayElementAtIndex(a);
						if(a > 0 && nameProperty == null)
						{
							SerializedProperty statementProperty = currentProperty.FindPropertyRelative("statement");
							if(statementProperty != null)
							{
								OpenHorizontal();
								{
									FlexibleSpace();
									GUI.backgroundColor = statementProperty.enumValueIndex == 0 ? EditorContents.green : EditorContents.red;
									if(PressButton("And","&&",EditorContents.miniButtonLeft) && statementProperty.enumValueIndex != 0)
									{
										Undo.RecordObject(target,"Inspector");
										statementProperty.enumValueIndex = 0;
									}
									GUI.backgroundColor = statementProperty.enumValueIndex == 1 ? EditorContents.green : EditorContents.red;
									if(PressButton("Or","||",EditorContents.miniButtonRight) && statementProperty.enumValueIndex != 1)
									{
										Undo.RecordObject(target,"Inspector");
										statementProperty.enumValueIndex = 1;
									}
									GUI.backgroundColor = Color.white;
									FlexibleSpace();
								}
								CloseHorizontal();
							}
						}
						OpenVerticalContainer();
						{
							OpenHorizontal();
							{
								GUI.enabled = true;
								if(!isEvent)IndexButton(a,currentProperty);
								else IndexButton(a,currentProperty,Application.isPlaying ? (currentProperty.FindPropertyRelative("state").boolValue ? EditorContents.green : EditorContents.red) : EditorContents.yellow);
								if(PressButton(nameProperty != null ? currentProperty.FindPropertyRelative("name").stringValue : SingularName(property.displayName) + " " + (a + 1),GUILayout.ExpandWidth(true)))
									currentProperty.isExpanded = !currentProperty.isExpanded;
								GUI.enabled = a != 0 && state;
								if(PressButton("▲",GUILayout.ExpandWidth(false)))
								{
									Undo.RecordObject(target,"Inspector");
									T other = content[a - 1];
									content[a - 1] = content[a];
									content[a] = other;
									break;
								}
								GUI.enabled = a != content.Count - 1 && state;
								if(PressButton("▼",GUILayout.ExpandWidth(false)))
								{
									Undo.RecordObject(target,"Inspector");
									T other = content[a + 1];
									content[a + 1] = content[a];
									content[a] = other;
									break;
								}
								GUI.enabled = state;
								if(PressButton("-",GUILayout.ExpandWidth(false)))
								{
									Undo.RecordObject(target,"Inspector");
									if(onDestroy != null)onDestroy(a);
									content.RemoveAt(a);
									break;
								}
							}
							CloseHorizontal();
							if(currentProperty.isExpanded)
							{
								OpenHorizontal();
								GUILayout.Space(20);
								OpenVertical();
								if(primary != null)primary(content[a],currentProperty);
								if(secondary != null)secondary(currentProperty);
								CloseVertical();
								CloseHorizontal();
							}
						}
						CloseVertical();
					}
					OpenHorizontal();
					{
						if(nameProperty != null)
						{
							OpenHorizontalBar();
							{
								LabelWidth(46);
								EditorGUI.BeginChangeCheck();
								string text = EditorGUILayout.TextField("Name",nameProperty.stringValue);
								if(EditorGUI.EndChangeCheck())
								{
									Undo.RecordObject(target,"Inspector");
									nameProperty.stringValue = text;
								}
								LabelWidth();
							}
							CloseHorizontal();
						}
						else Label("Add a new " + SingularName(property.displayName) + "?",EditorContents.button1);
						if(nameProperty != null ? PressButton("+") : PressButton("+",GUILayout.ExpandWidth(false)))
						{
							Undo.RecordObject(target,"Inspector");
							content.Add(new T());
							serializedObject.Update();
							property = property.GetArrayElementAtIndex(content.Count - 1);
							property.isExpanded = false;
							if(nameProperty != null)property.FindPropertyRelative("name").stringValue = nameProperty.stringValue;
						}
					}
					CloseHorizontal();
					if(!state)GUI.enabled = true;
				}
			}
			CloseVertical();
		}
		public static bool Asset<T> (ref T asset,SerializedProperty assetProperty,string name = null)
		{
			bool isExporting = false;
			OpenVerticalSubsection();
			{
				Property(assetProperty);
				OpenHorizontal();
				{
					if(PressButton("Import","Load from an asset file"))
					{
						string path = EditorUtility.OpenFilePanel("Load Asset","Assets","asset");
						if(path != string.Empty)
						{
							if(path.StartsWith(Application.dataPath))
							{
								T assetSource = (T)(object)AssetDatabase.LoadAssetAtPath(path.Replace(Application.dataPath,"Assets"),typeof(T));
								if(assetSource != null)
								{
									Undo.RecordObjects(assetProperty.serializedObject.targetObjects,"Inspector");
									asset = assetSource;
								}
								else Debug.LogError("The selected asset is not of type " + TitleName(typeof(T).Name));
							}
							else Debug.LogError("The selected asset is not located in this project");
						}
					}
					if(PressButton("Export","Save to an asset file"))
					{
						if(name == null)name = "New " + TitleName(typeof(T).Name);
						string path = EditorUtility.SaveFilePanelInProject("Save Asset",name,"asset",string.Empty);
						if(path != string.Empty)
						{
							T assetSource = (T)(object)ScriptableObject.CreateInstance(typeof(T));
							isExporting = true;
							asset = assetSource;
							AssetDatabase.CreateAsset(assetSource as Object,path);
						}
					}
				}
				CloseHorizontal();
			}
			CloseVertical();
			return isExporting;
		}
		public static void Selection (SerializedProperty sourceProperty,string requiredVariable = null,System.Type requiredVariableType = null,bool shrink = false,UnityAction content = null,byte design = 0) {Selection(sourceProperty.displayName,sourceProperty,requiredVariable,requiredVariableType,shrink,content,design);}
		public static void Selection (string name,SerializedProperty sourceProperty,string requiredVariable = null,System.Type requiredVariableType = null,bool shrink = false,UnityAction content = null,byte design = 0)
		{
			if(design == 0)OpenVerticalSubsection();
			{
				Object source = sourceProperty.objectReferenceValue;
				OpenHorizontalBar();
				{
					FieldWidth(1);
					Property(name,sourceProperty);
					FieldWidth();
					GUI.enabled = !sourceProperty.hasMultipleDifferentValues ? source : true;
					if(PressButton("×",GUILayout.ExpandWidth(false)))
					{
						Undo.RecordObjects(sourceProperty.serializedObject.targetObjects,"Inspector");
						sourceProperty.objectReferenceValue = null;
						source = null;
					}
					GUI.enabled = true;
					if(content != null)content();
				}
				CloseHorizontal();
				if(!sourceProperty.serializedObject.isEditingMultipleObjects && source)
				{
					System.Type type = source.GetType();
					if(type == typeof(Transform) || type == typeof(RectTransform))
					{
						bool isFound = false;
						Component[] components = ((Transform)source).GetComponents<Component>();
						Label("Select Component",EditorContents.buttonBoldLabel);
						IconSize(13);
						for(int a = 1; a < components.Length; a++)
						{
							type = components[a].GetType();
							if(requiredVariable != null)
							{
								bool isType = false;
								if(type.BaseType == typeof(MonoBehaviour))
								{
									System.Reflection.FieldInfo fieldInfo = type.GetField(requiredVariable);
									isType = fieldInfo != null && fieldInfo.FieldType == requiredVariableType;
									if(!isFound && isType)isFound = true;
								}
								else if(type != typeof(Transform) && type != typeof(RectTransform))
								{
									System.Reflection.PropertyInfo propertyInfo = type.GetProperty(requiredVariable);
									isType = propertyInfo != null && propertyInfo.PropertyType == requiredVariableType;
									if(!isFound && isType)isFound = true;
								}
								GUI.enabled = isType;
							}
							OpenHorizontal();
							{
								if(IndexButton(a,1) || PressButton(TitleName(type.Name),EditorGUIUtility.ObjectContent(components[a],type).image,EditorContents.buttonLeft))
								{
									Undo.RecordObjects(sourceProperty.serializedObject.targetObjects,"Inspector");
									sourceProperty.objectReferenceValue = components[a];
								}
							}
							CloseHorizontal();
							if(requiredVariable != null)GUI.enabled = true;
						}
						IconSize();
						if(requiredVariable != null && !isFound)Label(new GUIContent("No " + requiredVariable + " field or property was found.",EditorContents.error),!shrink ? EditorContents.button1 : EditorContents.button2);
					}
					else if(PressButton("Revert"))
					{
						Undo.RecordObjects(sourceProperty.serializedObject.targetObjects,"Inspector");
						sourceProperty.objectReferenceValue = ((Component)source).transform;
					}
				}
			}
			if(design == 0)CloseVertical();
		}
		public static void Variable (SerializedProperty sourceProperty,ref string variableName,ref int variableNameIndex,SerializedProperty valueNameProperty,bool variableState = true)
		{
			string[] variableNames = new string[] {variableName};
			int[] variableNameIndexes = new int[] {variableNameIndex};
			Variables(sourceProperty,ref variableNames,ref variableNameIndexes,new SerializedProperty[] {valueNameProperty},new bool[] {variableState});
			variableName = variableNames[0];
			variableNameIndex = variableNameIndexes[0];
		}
		public static void Variables (SerializedProperty sourceProperty,ref string[] variableNames,ref int[] variableNameIndexes,SerializedProperty[] valueNameProperties,bool[] variableStates = null)
		{
			Object source = sourceProperty.objectReferenceValue;
			System.Type type = source ? source.GetType() : null;
			int length = variableNames.Length;
			bool isSingleState = length == variableStates.Length;
			bool isComponent = type != typeof(Transform) && type != typeof(RectTransform);
			List<string> names = new List<string>() {"Not Specified"};
			string[] variableNamesFound = new string[length];
			bool[] variableNamesExist = new bool[length];
			if(source)
			{
				if(type.BaseType == typeof(MonoBehaviour))
				{
					System.Reflection.FieldInfo[] fieldInfos = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
					for(int a = 0,A = fieldInfos.Length; a < A; a++)
					{
						type = fieldInfos[a].FieldType;
						bool isPublic = fieldInfos[a].IsPublic;
						bool isPrivate = fieldInfos[a].IsPrivate;
						if(!isPublic && !isPrivate)continue;
						if(type == typeof(float))
						{
							if(isPublic)names.Add("[Public] [Float] " + fieldInfos[a].Name);
							else if(isPrivate)names.Add("[Private] [Float] " + fieldInfos[a].Name);
							int b = names.Count - 1;
							while(b > 0 && (isPrivate ? names[b - 1].Contains("[Private]") : true) && (isPublic && names[b - 1].Contains("[Private]") || names[b - 1].Contains("[Int]") || names[b - 1].Contains("[Vector2]") || names[b - 1].Contains("[Vector3]")))
							{
								string previous = names[b - 1];
								names[b - 1] = names[b];
								names[b] = previous;
								b = b - 1;
							}
							continue;
						}
						if(type == typeof(int))
						{
							if(isPublic)names.Add("[Public] [Int] " + fieldInfos[a].Name);
							else if(isPrivate)names.Add("[Private] [Int] " + fieldInfos[a].Name);
							int b = names.Count - 1;
							while(b > 0 && (isPrivate ? names[b - 1].Contains("[Private]") : true) && (isPublic && names[b - 1].Contains("[Private]") || names[b - 1].Contains("[Vector2]") || names[b - 1].Contains("[Vector3]")))
							{
								string previous = names[b - 1];
								names[b - 1] = names[b];
								names[b] = previous;
								b = b - 1;
							}
							continue;
						}
						if(type == typeof(Vector2))
						{
							if(isPublic)
							{
								names.Add("[Public] [Vector2] " + fieldInfos[a].Name + ".x");
								names.Add("[Public] [Vector2] " + fieldInfos[a].Name + ".y");
							}
							else if(isPrivate)
							{
								names.Add("[Private] [Vector2] " + fieldInfos[a].Name + ".x");
								names.Add("[Private] [Vector2] " + fieldInfos[a].Name + ".y");
							}
							int b = names.Count - 1;
							while(b > 0 && (isPrivate ? names[b - 2].Contains("[Private]") : true) && (isPublic && names[b - 2].Contains("[Private]") || names[b - 2].Contains("[Vector3]")))
							{
								string previous = names[b - 2];
								names[b - 2] = names[b - 1];
								names[b - 1] = names[b];
								names[b] = previous;
								b = b - 1;
							}
							continue;
						}
						if(type == typeof(Vector3))
						{
							if(isPublic)
							{
								names.Add("[Public] [Vector3] " + fieldInfos[a].Name + ".x");
								names.Add("[Public] [Vector3] " + fieldInfos[a].Name + ".y");
								names.Add("[Public] [Vector3] " + fieldInfos[a].Name + ".z");
								int b = names.Count - 1;
								while(b > 0 && names[b - 3].Contains("[Private]"))
								{
									string previous = names[b - 3];
									names[b - 3] = names[b - 2];
									names[b - 2] = names[b - 1];
									names[b - 1] = names[b];
									names[b] = previous;
									b = b - 1;
								}
							}
							else if(isPrivate)
							{
								names.Add("[Private] [Vector3] " + fieldInfos[a].Name + ".x");
								names.Add("[Private] [Vector3] " + fieldInfos[a].Name + ".y");
								names.Add("[Private] [Vector3] " + fieldInfos[a].Name + ".z");
							}
							continue;
						}
					}
				}
				else if(isComponent)
				{
					System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties();
					for(int a = 0,A = propertyInfos.Length; a < A; a++)
					{
						type = propertyInfos[a].PropertyType;
						if(type == typeof(float))
						{
							names.Add("[Float] " + propertyInfos[a].Name);
							int b = names.Count - 1;
							while(b > 0 && (names[b - 1].Contains("[Int]") || names[b - 1].Contains("[Vector2]") || names[b - 1].Contains("[Vector3]")))
							{
								string previous = names[b - 1];
								names[b - 1] = names[b];
								names[b] = previous;
								b = b - 1;
							}
							continue;
						}
						if(type == typeof(int))
						{
							names.Add("[Int] " + propertyInfos[a].Name);
							int b = names.Count - 1;
							while(b > 0 && (names[b - 1].Contains("[Vector2]") || names[b - 1].Contains("[Vector3]")))
							{
								string previous = names[b - 1];
								names[b - 1] = names[b];
								names[b] = previous;
								b = b - 1;
							}
							continue;
						}
						if(type == typeof(Vector2))
						{
							names.Add("[Vector2] " + propertyInfos[a].Name + ".x");
							names.Add("[Vector2] " + propertyInfos[a].Name + ".y");
							int b = names.Count - 1;
							while(b > 0 && names[b - 2].Contains("[Vector3]"))
							{
								string previous = names[b - 2];
								names[b - 2] = names[b - 1];
								names[b - 1] = names[b];
								names[b] = previous;
								b = b - 1;
							}
							continue;
						}
						if(type == typeof(Vector3))
						{
							names.Add("[Vector3] " + propertyInfos[a].Name + ".x");
							names.Add("[Vector3] " + propertyInfos[a].Name + ".y");
							names.Add("[Vector3] " + propertyInfos[a].Name + ".z");
							continue;
						}
					}
				}
				if(variableStates != null)for(int a = 0; a < length; a++)
				{
					if(variableStates[isSingleState ? a : a * 2])break;
					else if(a == length - 1)goto Stop;
				}
				type = source.GetType();
				if(type.BaseType == typeof(MonoBehaviour))
				{
					System.Reflection.FieldInfo[] fieldInfos = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
					for(int a = 0,A = fieldInfos.Length; a < A; a++)
					{
						type = fieldInfos[a].FieldType;
						if(type == typeof(float) || type == typeof(int))
						{
							string name = fieldInfos[a].Name;
							for(int b = 0; b < length; b++)
							{
								if(variableNames[b] != name)continue;
								if(variableNameIndexes[b] == 0)
									variableNamesFound[b] = name;
								variableNamesExist[b] = true;
							}
							for(int b = 0; b < length; b++)
							{
								if(!variableNamesExist[b])break;
								else if(b == length - 1)goto Stop;
							}
						}
						else if(type == typeof(Vector2))
						{
							string name = fieldInfos[a].Name;
							for(int b = 0; b < length; b++)
							{
								if(variableNames[b] != name + ".x" && variableNames[b] != name + ".y")continue;
								if(variableNameIndexes[b] == 0)
									variableNamesFound[b] = variableNames[b];
								variableNamesExist[b] = true;
							}
							for(int b = 0; b < length; b++)
							{
								if(!variableNamesExist[b])break;
								else if(b == length - 1)goto Stop;
							}
						}
						else if(type == typeof(Vector3))
						{
							string name = fieldInfos[a].Name;
							for(int b = 0; b < length; b++)
							{
								if(variableNames[b] != name + ".x" && variableNames[b] != name + ".y" && variableNames[b] != name + ".z")continue;
								if(variableNameIndexes[b] == 0)
									variableNamesFound[b] = variableNames[b];
								variableNamesExist[b] = true;
							}
							for(int b = 0; b < length; b++)
							{
								if(!variableNamesExist[b])break;
								else if(b == length - 1)goto Stop;
							}
						}
					}
				}
				else if(isComponent)
				{
					System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties();
					for(int a = 0,A = propertyInfos.Length; a < A; a++)
					{
						type = propertyInfos[a].PropertyType;
						if(type == typeof(float) || type == typeof(int))
						{
							string name = propertyInfos[a].Name;
							for(int b = 0; b < length; b++)
							{
								if(variableNames[b] != name)continue;
								if(variableNameIndexes[b] == 0)
									variableNamesFound[b] = name;
								variableNamesExist[b] = true;
							}
							for(int b = 0; b < length; b++)
							{
								if(!variableNamesExist[b])break;
								else if(b == length - 1)goto Stop;
							}
						}
						else if(type == typeof(Vector2))
						{
							string name = propertyInfos[a].Name;
							for(int b = 0; b < length; b++)
							{
								if(variableNames[b] != name + ".x" && variableNames[b] != name + ".y")continue;
								if(variableNameIndexes[b] == 0)
									variableNamesFound[b] = variableNames[b];
								variableNamesExist[b] = true;
							}
							for(int b = 0; b < length; b++)
							{
								if(!variableNamesExist[b])break;
								else if(b == length - 1)goto Stop;
							}
						}
						else if(type == typeof(Vector3))
						{
							string name = propertyInfos[a].Name;
							for(int b = 0; b < length; b++)
							{
								if(variableNames[b] != name + ".x" && variableNames[b] != name + ".y" && variableNames[b] != name + ".z")continue;
								if(variableNameIndexes[b] == 0)
									variableNamesFound[b] = variableNames[b];
								variableNamesExist[b] = true;
							}
							for(int b = 0; b < length; b++)
							{
								if(!variableNamesExist[b])break;
								else if(b == length - 1)goto Stop;
							}
						}
					}
				}
			}
			Stop:
			IconSize(12);
			for(int a = 0; a < length; a++)
			{
				bool state = variableStates != null ? variableStates[isSingleState ? a : a * 2] && (isSingleState ? true : variableStates[a * 2 + 1]) : true;
				GUI.enabled = state;
				OpenVerticalSubsection();
				{
					if(state && variableNameIndexes[a] > 0 && !variableNamesExist[a])
						variableNameIndexes[a] = 0;
					Label(new GUIContent(valueNameProperties[a].displayName,state && source ? (variableNameIndexes[a] > 0 && variableNamesExist[a] ? EditorContents.positive : EditorContents.negative) : EditorContents.uncertain),!valueNameProperties[a].prefabOverride ? EditorContents.buttonLeft : EditorContents.buttonLeftBoldLabel);
					if(source && variableNamesFound[a] != string.Empty)for(int b = 1,B = names.Count; b < B; b++)if(variableNamesFound[a] == names[b].Replace("[Public] ",string.Empty).Replace("[Private] ",string.Empty).Replace("[Float] ",string.Empty).Replace("[Int] ",string.Empty).Replace("[Vector2] ",string.Empty).Replace("[Vector3] ",string.Empty))
						variableNameIndexes[a] = b;
					OpenHorizontalBar();
					{
						Property(string.Empty,valueNameProperties[a]);
						GUI.enabled = state && isComponent;
						EditorGUI.BeginChangeCheck();
						int index = EditorGUILayout.Popup(variableNameIndexes[a],names.ToArray());
						if(index >= 0 && EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(valueNameProperties[a].serializedObject.targetObject,"Inspector");
							variableNames[a] = index < names.Count ? names[index].Replace("Not Specified",string.Empty).Replace("[Public] ",string.Empty).Replace("[Private] ",string.Empty).Replace("[Float] ",string.Empty).Replace("[Int] ",string.Empty).Replace("[Vector2] ",string.Empty).Replace("[Vector3] ",string.Empty) : string.Empty;
							variableNameIndexes[a] = index;
						}
					}
					CloseHorizontal();
				}
				CloseVertical();
				GUI.enabled = true;
			}
			IconSize();
		}
		public static void IndexContainer (SerializedProperty indexProperty,SerializedProperty property,float width = 46,bool startsAtZero = false,bool hideDefault = false,string @default = "Not Specified",byte design = 0)
		{
			if(design == 0)OpenVerticalSubsection();
			OpenHorizontalBar();
			{
				/*int index = target.healths.IndexOf(current);
				string[] names = new string[target.healths.Count];
				names[0] = "None";
				for(int a = 1,A = names.Length; a < A; a++)
					names[a] = "[" + (index > a - 1 ? a - 1 : a).ToString() + "] " + target.healths[index > a - 1 ? a - 1 : a].name;
				
				
				int popup = EditorGUILayout.Popup(current.link > index ? current.link : (current.link == index ? 0 : current.link + 1),names);
				if(popup <= index)popup = popup - 1;*/
				
				
				int start = !startsAtZero ? (hideDefault && property.arraySize != 0 ? 0 : 1) : (property.arraySize != 0 ? 0 : 1);
				string[] names = new string[property != null ? start + Mathf.Min(property.arraySize,sbyte.MaxValue) : 1];
				if(start == 1)names[0] = @default;
				for(int a = start,A = names.Length; a < A; a++)
				{
					int b = a - start;
					SerializedProperty nameProperty = property.GetArrayElementAtIndex(b).FindPropertyRelative("name");
					if(nameProperty != null)names[a] = "[" + (b).ToString() + "] " + nameProperty.stringValue;
					else
					{
						Object current = property.GetArrayElementAtIndex(b).objectReferenceValue;
						names[a] = "[" + (b).ToString() + "] " + (current ? current.name : "Null");
					}
				}
				LabelWidth(width);
				FieldWidth(23);
				Property(indexProperty);
				EditorGUI.BeginChangeCheck();
				LabelWidth();
				FieldWidth(1);
				int popup = !startsAtZero && (!hideDefault || property.arraySize == 0) ? EditorGUILayout.Popup(1 + indexProperty.intValue,names) - 1 : EditorGUILayout.Popup(indexProperty.intValue,names);
				FieldWidth();
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(indexProperty.serializedObject.targetObject,"Inspector");
					indexProperty.intValue = popup;
				}
			}
			CloseHorizontal();
			if(design == 0)CloseVertical();
		}
		
		
		
		/*public static void IndexContainerWithConditions (byte design = 0)
		{
			if(design == 1)OpenVerticalSubsection();
			OpenHorizontalBar();
			{
				string[] currencyNames = new string[target.source ? target.source.currencies.Count + 1 : 1];
				currencyNames[0] = "Not Specified";
				for(int a = 1,A = currencyNames.Length; a < A; a++)
					currencyNames[a] = "[" + (a - 1).ToString() + "] " + target.source.currencies[a - 1].name;
				LabelWidth(46;
				FieldWidth(1);
				Property(serializedObject.FindProperty("index"));
				LabelWidth(59;
				FieldWidth();
				EditorGUI.BeginChangeCheck();
				int popup = EditorGUILayout.Popup("Currency",target.source && target.source.currencies.Count != 0 ? target.index + 1 : 0,currencyNames);
				if(target.source && target.source.currencies.Count != 0)popup = popup - 1;
				else if(target.index == -1)popup = -1;
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target,"Inspector");
					target.index = popup;
				}
			}
			CloseHorizontal();
			if(design == 1)CloseVertical();
		}
		private void IndexContainerWithExclusion (HealthSystem.Health current,SerializedProperty currentProperty)
		{
			OpenVerticalSubsection();
			{
				OpenHorizontalBar();
				{
					int index = target.healths.IndexOf(current);
					string[] healthNames = new string[target.healths.Count];
					healthNames[0] = "None";
					for(int a = 1,A = healthNames.Length; a < A; a++)
						healthNames[a] = "[" + (index > a - 1 ? a - 1 : a).ToString() + "] " + target.healths[index > a - 1 ? a - 1 : a].name;
					LabelWidth(34;
					FieldWidth(1);
					Property(currentProperty.FindPropertyRelative("link"));
					LabelWidth(73;
					FieldWidth(18;
					EditorGUI.BeginChangeCheck();
					int popup = EditorGUILayout.Popup("Linked With",current.link > index ? current.link : (current.link == index ? 0 : current.link + 1),healthNames);
					if(popup <= index)popup = popup - 1;
					FieldWidth(0;
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						current.link = popup;
					}
				}
				CloseHorizontal();
			}
			CloseVertical();
		}*/
	}
}