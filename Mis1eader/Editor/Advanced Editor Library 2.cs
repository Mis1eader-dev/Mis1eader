namespace Mis1eader.Editor
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEditor;
	using System.Collections.Generic;
	public static class Manager
	{
		public static readonly bool isPro = EditorGUIUtility.isProSkin;
		public static readonly Color red = isPro ? Color.red : new Color(1F,0.5F,0.5F,1F),
		green = isPro ? Color.green : new Color(0.5F,1F,0.5F,1F),
		yellow = new Color(1F,0.86F,0.25F,1F);
		public static readonly float indent = 15;
	}
	public static class Style
	{
		private static readonly GUIStyle defaultButton = GUI.skin.button;
		public static readonly GUIStyle label = EditorStyles.label,
		labelCentered = new GUIStyle(label) {alignment = TextAnchor.MiddleCenter},
		boldLabel = new GUIStyle(label) {fontStyle = FontStyle.Bold},
		boldLabelCentered = new GUIStyle(boldLabel) {alignment = TextAnchor.MiddleCenter},
		toggle = EditorStyles.toggle,
		foldout = EditorStyles.foldout,
		boldLabelFoldout = new GUIStyle(foldout) {fontStyle = FontStyle.Bold},
		boldLabelFoldoutToggle = new GUIStyle(boldLabelFoldout) {padding = new RectOffset(boldLabelFoldout.padding.left + toggle.padding.horizontal,boldLabelFoldout.padding.right,boldLabelFoldout.padding.top,boldLabelFoldout.padding.bottom)},
		button = new GUIStyle(EditorStyles.toolbarButton)
		{
			fixedHeight = 0F,
			margin = new RectOffset(EditorStyles.toolbarButton.margin.left,EditorStyles.toolbarButton.margin.right,defaultButton.margin.top,defaultButton.margin.bottom),
			padding = new RectOffset(EditorStyles.toolbarButton.padding.left,EditorStyles.toolbarButton.padding.right,defaultButton.padding.top,defaultButton.padding.bottom),
			stretchHeight = false
		},
		miniLabelButton = new GUIStyle(button) {fontSize = 8},
		arraySize = new GUIStyle(EditorStyles.numberField) {margin = button.margin,fontSize = 9};
		/*public static readonly GUIStyle section = new GUIStyle(button)
		{
			active = new GUIStyleState() {background = Image.Pressed,textColor = button.active.textColor},
			focused = new GUIStyleState() {background = Image.Normal,textColor = button.focused.textColor},
			hover = new GUIStyleState() {background = Image.Normal,textColor = label.hover.textColor},
			normal = new GUIStyleState() {background = Image.Normal,textColor = button.normal.textColor},
			
			onActive = new GUIStyleState() {background = Image.Pressed,textColor = button.onActive.textColor},
			onFocused = new GUIStyleState() {background = Image.Normal,textColor = button.onFocused.textColor},
			onHover = new GUIStyleState() {background = Image.Normal,textColor = label.onHover.textColor},
			onNormal = new GUIStyleState() {background = Image.Normal,textColor = button.onNormal.textColor}
		};*/
	}
	public static class Image
	{
		public static Texture2D Normal
		{
			get
			{
				if(!normal)
				{
					normal = new Texture2D(1,1);
					normal.SetPixel(0,0,Manager.isPro ? new Color(0.16F,0.16F,0.16F,1F) : new Color(1F,1F,1F,0.25F));
					normal.Apply(false);
				}
				return normal;
			}
		}
		private static Texture2D normal = null;
		
		public static Texture2D Pressed
		{
			get
			{
				if(!pressed)
				{
					pressed = new Texture2D(1,1);
					pressed.SetPixel(0,0,Manager.isPro ? new Color(0.18F,0.18F,0.18F,1F) : new Color(1F,1F,1F,0.25F));
					pressed.Apply(false);
				}
				return pressed;
			}
		}
		private static Texture2D pressed = null;
	}
	public static class Library
	{
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
		public static Vector2 Size (string text) {return Style.label.CalcSize(new GUIContent(text));}
		public static Vector2 Size (Texture image) {return Style.label.CalcSize(new GUIContent(image));}
		public static Vector2 Size (string text,Texture image) {return Style.label.CalcSize(new GUIContent(text,image));}
		public static Vector2 Size (GUIContent content) {return Style.label.CalcSize(content);}
		public static Vector2 Size (GUIStyle style) {return style.CalcSize(GUIContent.none);}
		public static Vector2 Size (GUIStyle style,string text) {return style.CalcSize(new GUIContent(text));}
		public static Vector2 Size (GUIStyle style,Texture image) {return style.CalcSize(new GUIContent(image));}
		public static Vector2 Size (GUIStyle style,string text,Texture image) {return style.CalcSize(new GUIContent(text,image));}
		public static Vector2 Size (GUIStyle style,GUIContent content) {return style.CalcSize(content);}
	}
	public class Editor<T0> : UnityEditor.Editor
	{
		public new T0 target = default(T0);
		public new T0[] targets = null;
		private void OnEnable ()
		{
			int A = base.targets.Length;
			T0[] targets = new T0[A];
			for(int a = 0; a < A; a++)
				targets[a] = (T0)(object)base.targets[a];
			this.target = targets[0];
			this.targets = targets;
			Initialization();
		}
		private byte view = 0;
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			/*OpenHorizontal(EditorContents.section);
			{
				Label(EditorContents.inspector);
				GUILayout.Space(1);
				view = GUILayout.Toolbar(view,new string[] {"Advanced","Simple","Standard"});
			}
			CloseHorizontal();*/
			if(view == 0 || view == 1)Inspector();
			else base.OnInspectorGUI();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int a = 0,A = base.targets.Length; a < A; a++)
					EditorUtility.SetDirty(base.targets[a]);
			}
		}
		public virtual void Initialization () {}
		public virtual void Inspector () {base.OnInspectorGUI();}
		
		public static void print (object message) {MonoBehaviour.print(message);}
		
		public static SerializedProperty Find (SerializedObject serializedObject,string name) {return serializedObject.FindProperty(name);}
		public static SerializedProperty Find (SerializedProperty serializedProperty,string name) {return serializedProperty.FindPropertyRelative(name);}
		
		public static GUIContent Content () {return GUIContent.none;}
		public static GUIContent Content (string text)
		{
			if(text != null && text != string.Empty)
			{
				string[] split = text.Split('|');
				if(split.Length > 1)return new GUIContent(split[0],split[1]);
				return new GUIContent(split[0]);
			}
			return GUIContent.none;
		}
		public static GUIContent Content (Texture image) {return image != null ? new GUIContent(image) : GUIContent.none;}
		public static GUIContent Content (string text,Texture image)
		{
			if(text != null && text != string.Empty)
			{
				string[] split = text.Split('|');
				if(split.Length > 1)return new GUIContent(image == null ? split[0] : split[0].Insert(0," "),image,split[1]);
				return new GUIContent(image == null ? split[0] : split[0].Insert(0," "),image);
			}
			return image != null ? new GUIContent(image) : GUIContent.none;
		}
		
		public static Rect Rect (params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(options);}
		public static Rect Rect (GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(true,EditorGUIUtility.singleLineHeight,style,options);}
		public static Rect Rect (bool hasLabel,params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(hasLabel,options);}
		public static Rect Rect (bool hasLabel,GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(hasLabel,EditorGUIUtility.singleLineHeight,style,options);}
		public static Rect Rect (float height,params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(true,height,options);}
		public static Rect Rect (float height,GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(true,height,style,options);}
		public static Rect Rect (bool hasLabel,float height,params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(hasLabel,height,options);}
		public static Rect Rect (bool hasLabel,float height,GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.GetControlRect(hasLabel,height,style,options);}
		public static Rect Rect (string text,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(Content(text),Style.label,options);}
		public static Rect Rect (Texture image,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(Content(image),Style.label,options);}
		public static Rect Rect (string text,Texture image,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(Content(text,image),Style.label,options);}
		public static Rect Rect (GUIContent content,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(content,Style.label,options);}
		public static Rect Rect (string text,GUIStyle style,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(Content(text),style,options);}
		public static Rect Rect (Texture image,GUIStyle style,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(Content(image),style,options);}
		public static Rect Rect (string text,Texture image,GUIStyle style,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(Content(text,image),style,options);}
		public static Rect Rect (GUIContent content,GUIStyle style,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(content,style,options);}
		public static Rect Rect (float width,float height,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(width,height,options);}
		public static Rect Rect (float width,float height,GUIStyle style,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(width,height,style,options);}
		public static Rect Rect (float minimumWidth,float maximumWidth,float minimumHeight,float maximumHeight,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(minimumWidth,maximumWidth,minimumHeight,maximumHeight,options);}
		public static Rect Rect (float minimumWidth,float maximumWidth,float minimumHeight,float maximumHeight,GUIStyle style,params GUILayoutOption[] options) {return GUILayoutUtility.GetRect(minimumWidth,maximumWidth,minimumHeight,maximumHeight,style,options);}
		
		public static Rect AspectRect (float aspect,params GUILayoutOption[] options) {return GUILayoutUtility.GetAspectRect(aspect,options);}
		public static Rect AspectRect (float aspect,GUIStyle style,params GUILayoutOption[] options) {return GUILayoutUtility.GetAspectRect(aspect,style,options);}
		public static Rect LastRect () {return GUILayoutUtility.GetLastRect();}
		
		public static void Label (Rect rect) {GUI.Label(rect,Content());}
		public static void Label (Rect rect,string text) {GUI.Label(rect,Content(text));}
		public static void Label (Rect rect,Texture image) {GUI.Label(rect,Content(image));}
		public static void Label (Rect rect,string text,Texture image) {GUI.Label(rect,Content(text,image));}
		public static void Label (Rect rect,GUIContent content) {GUI.Label(rect,content);}
		public static void Label (Rect rect,GUIStyle style) {GUI.Label(rect,Content(),style);}
		public static void Label (Rect rect,string text,GUIStyle style) {GUI.Label(rect,Content(text),style);}
		public static void Label (Rect rect,Texture image,GUIStyle style) {GUI.Label(rect,Content(image),style);}
		public static void Label (Rect rect,string text,Texture image,GUIStyle style) {GUI.Label(rect,Content(text,image),style);}
		public static void Label (Rect rect,GUIContent content,GUIStyle style) {GUI.Label(rect,content,style);}
		
		public static void LabelField (Rect rect) {EditorGUI.LabelField(rect,Content());}
		public static void LabelField (Rect rect,string text) {EditorGUI.LabelField(rect,Content(text));}
		public static void LabelField (Rect rect,Texture image) {EditorGUI.LabelField(rect,Content(image));}
		public static void LabelField (Rect rect,string text,Texture image) {EditorGUI.LabelField(rect,Content(text,image));}
		public static void LabelField (Rect rect,GUIContent content) {EditorGUI.LabelField(rect,content);}
		public static void LabelField (Rect rect,GUIStyle style) {EditorGUI.LabelField(rect,Content(),style);}
		public static void LabelField (Rect rect,string text,GUIStyle style) {EditorGUI.LabelField(rect,Content(text),style);}
		public static void LabelField (Rect rect,Texture image,GUIStyle style) {EditorGUI.LabelField(rect,Content(image),style);}
		public static void LabelField (Rect rect,string text,Texture image,GUIStyle style) {EditorGUI.LabelField(rect,Content(text,image),style);}
		public static void LabelField (Rect rect,GUIContent content,GUIStyle style) {EditorGUI.LabelField(rect,content,style);}
		public static void LabelField (Rect rect,string text,GUIContent content2) {EditorGUI.LabelField(rect,Content(text),content2);}
		public static void LabelField (Rect rect,Texture image,GUIContent content2) {EditorGUI.LabelField(rect,Content(image),content2);}
		public static void LabelField (Rect rect,string text,Texture image,GUIContent content2) {EditorGUI.LabelField(rect,Content(text,image),content2);}
		public static void LabelField (Rect rect,GUIContent content,string text2) {EditorGUI.LabelField(rect,content,Content(text2));}
		public static void LabelField (Rect rect,GUIContent content,Texture image2) {EditorGUI.LabelField(rect,content,Content(image2));}
		public static void LabelField (Rect rect,GUIContent content,string text2,Texture image2) {EditorGUI.LabelField(rect,content,Content(text2,image2));}
		public static void LabelField (Rect rect,GUIContent content,GUIContent content2) {EditorGUI.LabelField(rect,content,content2);}
		public static void LabelField (Rect rect,string text,string text2,GUIStyle style) {EditorGUI.LabelField(rect,Content(text),Content(text2),style);}
		public static void LabelField (Rect rect,Texture image,Texture image2,GUIStyle style) {EditorGUI.LabelField(rect,Content(image),Content(image2),style);}
		public static void LabelField (Rect rect,string text,Texture image,string text2,GUIStyle style) {EditorGUI.LabelField(rect,Content(text,image),Content(text2),style);}
		public static void LabelField (Rect rect,string text,string text2,Texture image2,GUIStyle style) {EditorGUI.LabelField(rect,Content(text),Content(text2,image2),style);}
		public static void LabelField (Rect rect,string text,Texture image,string text2,Texture image2,GUIStyle style) {EditorGUI.LabelField(rect,Content(text,image),Content(text2,image2),style);}
		public static void LabelField (Rect rect,GUIContent content,GUIContent content2,GUIStyle style) {EditorGUI.LabelField(rect,content,content2,style);}
		
		public static bool Button (string text,params GUILayoutOption[] options) {return Button(Content(text),Style.button,options);}
		public static bool Button (Texture image,params GUILayoutOption[] options) {return Button(Content(image),Style.button,options);}
		public static bool Button (string text,Texture image,params GUILayoutOption[] options) {return Button(Content(text,image),Style.button,options);}
		public static bool Button (string text,GUIStyle style,params GUILayoutOption[] options) {return Button(Content(text),style,options);}
		public static bool Button (Texture image,GUIStyle style,params GUILayoutOption[] options) {return Button(Content(image),style,options);}
		public static bool Button (string text,Texture image,GUIStyle style,params GUILayoutOption[] options) {return Button(Content(text,image),style,options);}
		public static bool Button (GUIContent content,GUIStyle style,params GUILayoutOption[] options)
		{
			if(GUILayout.Button(content.text != string.Empty && content.image != null ? new GUIContent(content.text.Insert(0," "),content.image,content.tooltip) : content,style,options))
			{
				GUI.FocusControl(null);
				return true;
			}
			return false;
		}
		
		public static void Label (params GUILayoutOption[] options) {GUILayout.Label(Content(),options);}
		public static void Label (string text,params GUILayoutOption[] options) {GUILayout.Label(Content(text),options);}
		public static void Label (Texture image,params GUILayoutOption[] options) {GUILayout.Label(Content(image),options);}
		public static void Label (string text,Texture image,params GUILayoutOption[] options) {GUILayout.Label(Content(text,image),options);}
		public static void Label (GUIContent content,params GUILayoutOption[] options) {GUILayout.Label(content,options);}
		public static void Label (GUIStyle style,params GUILayoutOption[] options) {GUILayout.Label(Content(),style,options);}
		public static void Label (string text,GUIStyle style,params GUILayoutOption[] options) {GUILayout.Label(Content(text),style,options);}
		public static void Label (Texture image,GUIStyle style,params GUILayoutOption[] options) {GUILayout.Label(Content(image),style,options);}
		public static void Label (string text,Texture image,GUIStyle style,params GUILayoutOption[] options) {GUILayout.Label(Content(text,image),style,options);}
		public static void Label (GUIContent content,GUIStyle style,params GUILayoutOption[] options) {GUILayout.Label(content,style,options);}
		
		public static void LabelField (params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(),options);}
		public static void LabelField (string text,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text),options);}
		public static void LabelField (Texture image,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(image),options);}
		public static void LabelField (string text,Texture image,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text,image),options);}
		public static void LabelField (GUIContent content,params GUILayoutOption[] options) {EditorGUILayout.LabelField(content,options);}
		public static void LabelField (GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(),style,options);}
		public static void LabelField (string text,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text),style,options);}
		public static void LabelField (Texture image,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(image),style,options);}
		public static void LabelField (string text,Texture image,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text,image),style,options);}
		public static void LabelField (GUIContent content,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(content,style,options);}
		public static void LabelField (string text,GUIContent content2,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text),content2,options);}
		public static void LabelField (Texture image,GUIContent content2,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(image),content2,options);}
		public static void LabelField (string text,Texture image,GUIContent content2,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text,image),content2,options);}
		public static void LabelField (GUIContent content,string text2,params GUILayoutOption[] options) {EditorGUILayout.LabelField(content,Content(text2),options);}
		public static void LabelField (GUIContent content,Texture image2,params GUILayoutOption[] options) {EditorGUILayout.LabelField(content,Content(image2),options);}
		public static void LabelField (GUIContent content,string text2,Texture image2,params GUILayoutOption[] options) {EditorGUILayout.LabelField(content,Content(text2,image2),options);}
		public static void LabelField (GUIContent content,GUIContent content2,params GUILayoutOption[] options) {EditorGUILayout.LabelField(content,content2,options);}
		public static void LabelField (string text,string text2,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text),Content(text2),style,options);}
		public static void LabelField (Texture image,Texture image2,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(image),Content(image2),style,options);}
		public static void LabelField (string text,Texture image,string text2,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text,image),Content(text2),style,options);}
		public static void LabelField (string text,string text2,Texture image2,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text),Content(text2,image2),style,options);}
		public static void LabelField (string text,Texture image,string text2,Texture image2,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(Content(text,image),Content(text2,image2),style,options);}
		public static void LabelField (GUIContent content,GUIContent content2,GUIStyle style,params GUILayoutOption[] options) {EditorGUILayout.LabelField(content,content2,style,options);}
		
		public static float FieldWidth (float width = 0F)
		{
			float lastWidth = EditorGUIUtility.fieldWidth;
			EditorGUIUtility.fieldWidth = width;
			return lastWidth;
		}
		public static float LabelWidth (float width = 0F)
		{
			float lastWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = width;
			return lastWidth;
		}
		
		public static Rect OpenHorizontal (params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(options);}
		public static Rect OpenHorizontal (GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(style,options);}
		public static Rect OpenVertical (params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(options);}
		public static Rect OpenVertical (GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(style,options);}
		public static void CloseHorizontal () {EditorGUILayout.EndHorizontal();}
		public static void CloseVertical () {EditorGUILayout.EndVertical();}
		public static Rect OH (params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(options);}
		public static Rect OH (GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.BeginHorizontal(style,options);}
		public static Rect OV (params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(options);}
		public static Rect OV (GUIStyle style,params GUILayoutOption[] options) {return EditorGUILayout.BeginVertical(style,options);}
		public static void CH () {EditorGUILayout.EndHorizontal();}
		public static void CV () {EditorGUILayout.EndVertical();}
		
		public static void Indent (bool indent = false)
		{
			if(indent)
			{
				OpenHorizontal();
				GUILayout.Space(Manager.indent);
				OpenVertical();
			}
			else
			{
				CloseVertical();
				CloseHorizontal();
			}
		}
		//public static void PropertyLabel (GUIContent content) {EditorGUILayout.PrefixLabel(content);}
		
		public static void Property (SerializedProperty property,params GUILayoutOption[] options)
		{
			if(property == null) {Debug.LogError("No property was found.");return;}
			EditorGUILayout.PropertyField(property,true,options);
		}
		public static void Property (string text,SerializedProperty property,params GUILayoutOption[] options) {Property(Content(text),property,options);}
		public static void Property (Texture image,SerializedProperty property,params GUILayoutOption[] options) {Property(Content(image),property,options);}
		public static void Property (string text,Texture image,SerializedProperty property,params GUILayoutOption[] options) {Property(Content(text,image),property,options);}
		public static void Property (GUIContent content,SerializedProperty property,params GUILayoutOption[] options)
		{
			if(property == null) {Debug.LogError("No property was found.");return;}
			EditorGUILayout.PropertyField(property,content,true,options);
		}
		
		public static void Toggle (Rect rect,SerializedProperty property)
		{
			if(property == null) {Debug.LogError("No property was found.");return;}
			if(property.type != "bool")
			{
				Debug.LogError("Property is not a bool.");
				Color color = GUI.color;
				GUI.color = Manager.red;
				EditorGUI.Toggle(rect,false);
				GUI.color = color;
				return;
			}
			property.boolValue = EditorGUI.Toggle(rect,property.boolValue);
		}
		public static void Toggle (SerializedProperty property)
		{
			if(property == null) {Debug.LogError("No property was found.");return;}
			float width = Library.Size(Style.toggle).y;
			Rect rect = Rect(GUILayout.Width(width - Style.toggle.margin.right * 0.5F));
			rect.width = width;
			if(property.type != "bool")
			{
				Debug.LogError("Property is not a bool.");
				Color color = GUI.color;
				GUI.color = Manager.red;
				EditorGUI.Toggle(rect,false);
				GUI.color = color;
				return;
			}
			property.boolValue = EditorGUI.Toggle(rect,property.boolValue);
		}
		
		public static void FloatSlider (SerializedProperty property,float minimum,float maximum,params GUILayoutOption[] options) {FloatSlider(Content(),property,minimum,maximum,options);}
		public static void FloatSlider (string text,SerializedProperty property,float minimum,float maximum,params GUILayoutOption[] options) {FloatSlider(Content(text),property,minimum,maximum,options);}
		public static void FloatSlider (Texture image,SerializedProperty property,float minimum,float maximum,params GUILayoutOption[] options) {FloatSlider(Content(image),property,minimum,maximum,options);}
		public static void FloatSlider (string text,Texture image,SerializedProperty property,float minimum,float maximum,params GUILayoutOption[] options) {FloatSlider(Content(text,image),property,minimum,maximum,options);}
		public static void FloatSlider (GUIContent content,SerializedProperty property,float minimum,float maximum,params GUILayoutOption[] options)
		{
			if(property == null) {Debug.LogError("No property was found.");return;}
			if(property.type != "float")
			{
				Debug.LogError("Property is not a float.");
				Color color = GUI.color;
				GUI.color = Manager.red;
				EditorGUILayout.Slider(content,0F,0F,1F,options);
				GUI.color = color;
				return;
			}
			property.floatValue = EditorGUILayout.Slider(content,property.floatValue,minimum,maximum,options);
		}
		
		public static void IntSlider (SerializedProperty property,int minimum,int maximum,params GUILayoutOption[] options) {IntSlider(Content(),property,minimum,maximum,options);}
		public static void IntSlider (string text,SerializedProperty property,int minimum,int maximum,params GUILayoutOption[] options) {IntSlider(Content(text),property,minimum,maximum,options);}
		public static void IntSlider (Texture image,SerializedProperty property,int minimum,int maximum,params GUILayoutOption[] options) {IntSlider(Content(image),property,minimum,maximum,options);}
		public static void IntSlider (string text,Texture image,SerializedProperty property,int minimum,int maximum,params GUILayoutOption[] options) {IntSlider(Content(text,image),property,minimum,maximum,options);}
		public static void IntSlider (GUIContent content,SerializedProperty property,int minimum,int maximum,params GUILayoutOption[] options)
		{
			if(property == null) {Debug.LogError("No property was found.");return;}
			if(property.type != "int")
			{
				Debug.LogError("Property is not an int.");
				Color color = GUI.color;
				GUI.color = Manager.red;
				EditorGUILayout.IntSlider(content,0,0,1,options);
				GUI.color = color;
				return;
			}
			property.intValue = EditorGUILayout.IntSlider(content,property.intValue,minimum,maximum,options);
		}
		
		public static void Wrap (GUIStyle style,UnityAction container)
		{
			OpenVertical(style);
			if(container != null)container.Invoke();
			CloseVertical();
		}
		
		public static void Section (string text,UnityAction container,ref bool isExpanded,SerializedProperty toggle = null,UnityAction label = null) {Section(Content(text),container,ref isExpanded,toggle,label);}
		public static void Section (Texture image,UnityAction container,ref bool isExpanded,SerializedProperty toggle = null,UnityAction label = null) {Section(Content(image),container,ref isExpanded,toggle,label);}
		public static void Section (string text,Texture image,UnityAction container,ref bool isExpanded,SerializedProperty toggle = null,UnityAction label = null) {Section(Content(text,image),container,ref isExpanded,toggle,label);}
		public static void Section (GUIContent content,UnityAction container,ref bool isExpanded,SerializedProperty toggle = null,UnityAction label = null)
		{
			OpenVertical();
			{
				OpenHorizontal();
				Rect rect = Rect(GUILayout.Width(Library.Size(toggle == null ? Style.boldLabelFoldout : Style.boldLabelFoldoutToggle,content).x - Style.boldLabelFoldout.padding.horizontal));
				if(label != null)label.Invoke();
				CloseHorizontal();
				rect.x = 0F;
				rect.width = Screen.width;
				EditorGUI.DrawPreviewTexture(rect,Image.Normal);
				if(toggle == null)
				{
					rect.x = Style.boldLabelFoldout.padding.left;
					rect.width = rect.width - Style.boldLabelFoldout.padding.left;
					isExpanded = EditorGUI.Foldout(rect,isExpanded,content,true,Style.boldLabelFoldout);
				}
				else
				{
					rect.x = Style.boldLabelFoldout.padding.horizontal;
					rect.y = rect.y - 1F;
					rect.width = Library.Size(Style.toggle).y;
					Toggle(rect,toggle);
					rect.x = Style.boldLabelFoldout.padding.left;
					rect.y = rect.y + 1F;
					rect.width = Screen.width - Style.boldLabelFoldout.padding.left;
					isExpanded = EditorGUI.Foldout(rect,isExpanded,content,true,Style.boldLabelFoldoutToggle);
				}
				if(isExpanded && container != null)
				{
					GUI.enabled = toggle != null && toggle.type == "bool" ? toggle.boolValue : GUI.enabled;
					Indent(true);
					container.Invoke();
					Indent();
					GUI.enabled = true;
				}
			}
			CloseVertical();
		}
		public static void ContainerToggle (SerializedProperty property)
		{
			if(property == null) {Debug.LogError("No property was found.");return;}
			float width = Library.Size(Style.toggle).y;
			Rect rect = Rect(Style.button,GUILayout.Width(width + Style.button.padding.horizontal * 0.5F));
			Label(rect,Style.button);
			rect.x = rect.x + Style.button.padding.horizontal * 0.375F;
			rect.width = width;
			if(property.type != "bool")
			{
				Debug.LogError("Property is not a bool.");
				Color color = GUI.color;
				GUI.color = Manager.red;
				EditorGUI.Toggle(rect,false);
				GUI.color = color;
				return;
			}
			property.boolValue = EditorGUI.Toggle(rect,property.boolValue);
		}
		public static void Container (SerializedProperty list,SerializedProperty toggle = null,UnityAction<SerializedProperty> customClass = null,string conditionalProperty = null,bool enabled = true)
		{
			if(list == null) {Debug.LogError("No property was found.");return;}
			if(!list.isArray) {Debug.LogError("Property is not an Array or List.");return;}
			string name = list.displayName;
			OV(Style.button);
			
			
			bool lastEnabled = GUI.enabled;
			
			
			OH();
			{
				if(toggle != null)
				{
					ContainerToggle(toggle);
					enabled = toggle.boolValue;
				}
				GUI.enabled = enabled;
				ContainerSize(list);
				GUI.enabled = lastEnabled;
				if(Button(name))list.isExpanded = !list.isExpanded;
				GUI.enabled = enabled;
				if(Button("×",GUILayout.ExpandWidth(false)))list.ClearArray();
			}
			CH();
			if(list.isExpanded)
			{
				int size = list.arraySize;
				string singularName = Library.SingularName(name);
				if(size != 0)
				{
					//bool repaint = Event.current.type == EventType.Repaint;
					/*Rect rect = */OV();
					{
						if(list.type.ToLower() == "class") {for(int a = 0; a < size; a++)
						{
							SerializedProperty currentProperty = list.GetArrayElementAtIndex(a);
							OV(Style.button);
							{
								OH();
								{
									SerializedProperty nameProperty = Find(currentProperty,"name");
									IndexButton(a,currentProperty);
									if(Button(nameProperty != null ? nameProperty.stringValue : singularName + " " + (a + 1)))
										currentProperty.isExpanded = !currentProperty.isExpanded;
									if(Button("-",GUILayout.ExpandWidth(false)))
									{
										list.DeleteArrayElementAtIndex(a);
										size = size - 1;
									}
								}
								CH();
								if(currentProperty.isExpanded)
								{
									Indent(true);
									if(customClass != null)
									{
										customClass.Invoke(currentProperty);
									}
									else
									{
										Property(currentProperty);
									}
									Indent();
								}
							}
							CV();
						}}
						else for(int a = 0; a < size; a++)
						{
							OH();
							{
								OH(Style.button);
								{
									Index(a);
									Property(Content(),conditionalProperty == null ? list.GetArrayElementAtIndex(a) : list.GetArrayElementAtIndex(a).FindPropertyRelative(conditionalProperty));
									if(Button("-",GUILayout.ExpandWidth(false)))
									{
										list.DeleteArrayElementAtIndex(a);
										size = size - 1;
									}
								}
								CH();
							}
							CH();
						}
					}
					CV();
				}
				OH();
				{
					Label("Add a new " + singularName + "?",Style.button);
					if(Button("+",GUILayout.ExpandWidth(false)))
					{
						list.InsertArrayElementAtIndex(size);
					}
				}
				CH();
			}
			CV();
			GUI.enabled = lastEnabled;
		}
		public static void Index (int index,byte digits = 3) {Label(digits > 0 ? index.ToString(new string('0',digits)) : index.ToString(),Style.miniLabelButton,GUILayout.ExpandWidth(false),GUILayout.Height(Library.Size(Style.button).y));}
		public static bool IndexButton (int index,SerializedProperty list,byte digits = 3)
		{
			if(Button(digits > 0 ? index.ToString(new string('0',digits)) : index.ToString(),Style.miniLabelButton,GUILayout.ExpandWidth(false),GUILayout.Height(Library.Size(Style.button).y)))
			{
				list.isExpanded = !list.isExpanded;
				return true;
			}
			return false;
		}
		private static void ContainerSize (SerializedProperty list)
		{
			Rect rect = Rect(Style.arraySize,GUILayout.Width(Library.Size(Style.arraySize,"000").x + Style.button.padding.horizontal + Style.arraySize.padding.right * 0.5F));
			LabelField(rect,Style.button);
			rect.x = rect.x + Style.button.padding.left;
			rect.y = rect.y + 1F;
			rect.width = rect.width - Style.button.padding.horizontal;
			rect.height = rect.height - 3F;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = list.hasMultipleDifferentValues;
			int size = EditorGUI.IntField(rect,list.arraySize,Style.arraySize);
			EditorGUI.showMixedValue = false;
			if(EditorGUI.EndChangeCheck())
				list.arraySize = size;
		}
	}
}