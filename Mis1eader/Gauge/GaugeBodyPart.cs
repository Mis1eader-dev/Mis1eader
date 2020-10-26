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
	using UnityEngine.UI;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Gauge/Gauge Body Part",2),ExecuteInEditMode]
	public class GaugeBodyPart : MonoBehaviour
	{
		[System.Serializable] public class Text
		{
			#if ENABLE_REFLECTION
			public Component text = null;
			public Text (Component text = null) {this.text = text;if(text)root = text.transform.root;}
			private System.Reflection.FieldInfo textField = null;
			private System.Reflection.FieldInfo colorField = null;
			private System.Reflection.PropertyInfo textProperty = null;
			private System.Reflection.PropertyInfo colorProperty = null;
			#elif ENABLE_UNITY_TEXT
			public UnityEngine.UI.Text text = null;
			[UnityEngine.Serialization.FormerlySerializedAs("text")]
			public TextMesh textMesh = null;
			public Text () {}
			public Text (UnityEngine.UI.Text text) {this.text = text;if(text)root = text.transform.root;}
			public Text (TextMesh textMesh) {this.textMesh = textMesh;if(textMesh)root = textMesh.transform.root;}
			#else
			public TMPro.TMP_Text text = null;
			public Text (TMPro.TMP_Text text = null) {this.text = text;if(text)root = text.transform.root;}
			#endif
			[HideInInspector] public Transform root = null;
			[HideInInspector,SerializeField] private string lastText = null;
			[HideInInspector,SerializeField] private Color? lastColor = null;
			public void Handle (string text)
			{
				if(lastText == text)return;
				#if ENABLE_REFLECTION
				if(!this.text)return;
				System.Type type = this.text.GetType();
				if(type.BaseType == typeof(MonoBehaviour))
				{
					if(textField == null)textField = type.GetField("text");
					if(textField != null)textField.SetValue(this.text,text);
				}
				else if(type != typeof(Transform) && type != typeof(RectTransform))
				{
					if(textProperty == null)textProperty = type.GetProperty("text");
					if(textProperty != null)textProperty.SetValue(this.text,text,null);
				}
				#else
				#if ENABLE_UNITY_TEXT
				if(this.text)
				#endif
				this.text.text = text;
				#if ENABLE_UNITY_TEXT
				else if(textMesh)
					textMesh.text = text;
				#endif
				#endif
				lastText = text;
			}
			public void Handle (Color color)
			{
				if(lastColor == color)return;
				#if ENABLE_REFLECTION
				if(!this.text)return;
				System.Type type = this.text.GetType();
				if(type.BaseType == typeof(MonoBehaviour))
				{
					if(colorField == null)colorField = type.GetField("color");
					if(colorField != null)colorField.SetValue(this.text,color);
				}
				else if(type != typeof(Transform) && type != typeof(RectTransform))
				{
					if(colorProperty == null)colorProperty = type.GetProperty("color");
					if(colorProperty != null)colorProperty.SetValue(this.text,color,null);
				}
				#else
				#if ENABLE_UNITY_TEXT
				if(this.text)
				#endif
				this.text.color = color;
				#if ENABLE_UNITY_TEXT
				else if(textMesh)
					textMesh.color = color;
				#endif
				#endif
				lastColor = color;
			}
			public void Handle (string text,Color color)
			{
				if(lastText == text && lastColor == color)return;
				#if ENABLE_REFLECTION
				if(!this.text)return;
				System.Type type = this.text.GetType();
				if(type.BaseType == typeof(MonoBehaviour))
				{
					if(lastText != text)
					{
						if(textField == null)textField = type.GetField("text");
						if(textField != null)textField.SetValue(this.text,text);
						lastText = text;
					}
					if(lastColor != color)
					{
						if(colorField == null)colorField = type.GetField("color");
						if(colorField != null)colorField.SetValue(this.text,color);
						lastColor = color;
					}
				}
				else if(type != typeof(Transform) && type != typeof(RectTransform))
				{
					if(lastText != text)
					{
						if(textProperty == null)textProperty = type.GetProperty("text");
						if(textProperty != null)textProperty.SetValue(this.text,text,null);
						lastText = text;
					}
					if(lastColor != color)
					{
						if(colorProperty == null)colorProperty = type.GetProperty("color");
						if(colorProperty != null)colorProperty.SetValue(this.text,color,null);
						lastColor = color;
					}
				}
				#else
				#if ENABLE_UNITY_TEXT
				if(this.text)
				{
					#endif
					if(lastText != text)
					{
						this.text.text = text;
						lastText = text;
					}
					if(lastColor != color)
					{
						this.text.color = color;
						lastColor = color;
					}
					#if ENABLE_UNITY_TEXT
				}
				else if(textMesh)
				{
					if(lastText != text)
					{
						textMesh.text = text;
						lastText = text;
					}
					if(lastColor != color)
					{
						textMesh.color = color;
						lastColor = color;
					}
				}
				#endif
				#endif
			}
			public void SetText (
								 #if ENABLE_REFLECTION
								 Component
								 #elif ENABLE_UNITY_TEXT
								 UnityEngine.UI.Text
								 #else
								 TMPro.TMP_Text
								 #endif
								 value) {text = value;}
			public void SetText (GaugeBodyPart value)
			{
				if(!value || value.text == null)return;text = value.text.text;
				#if UNITY_EDITOR
				if(!Application.isPlaying)DestroyImmediate(value);
				else
				#endif
				Destroy(value);
			}
			#if ENABLE_UNITY_TEXT
			public void SetTextMesh (TextMesh value) {textMesh = value;}
			public void SetTextMesh (GaugeBodyPart value)
			{
				if(!value || value.text == null)return;textMesh = value.text.textMesh;
				#if UNITY_EDITOR
				if(!Application.isPlaying)DestroyImmediate(value);
				else
				#endif
				Destroy(value);
			}
			#endif
		}
		[System.Serializable] public class Colorable
		{
			public SpriteRenderer spriteRenderer = null;
			public Image image = null;
			public RawImage rawImage = null;
			public MeshRenderer meshRenderer = null;
			public void SetSpriteRenderer (SpriteRenderer value) {spriteRenderer = value;}
			public void SetImage (Image value) {image = value;}
			public void SetRawImage (RawImage value) {rawImage = value;}
			public void SetMeshRenderer (MeshRenderer value) {meshRenderer = value;}
		}
		public Transform movables = null;
		public Transform scalables = null;
		public Transform pivot = null;
		public List<Colorable> colorables = new List<Colorable>();
		public Text text = null;
		public void SetMovables (Transform value) {movables = value;}
		public void SetScalables (Transform value) {scalables = value;}
		public void SetPivot (Transform value) {pivot = value;}
		public void SetColorables (List<Colorable> value) {colorables = value;}
		public void SetColorablesUnlinked (List<Colorable> value) {int A = value.Count;if(colorables.Count != A)colorables = new List<Colorable>(new Colorable[A]);for(int a = 0; a < A; a++)colorables[a] = value[a];}
		public void SetColorables (Colorable[] value) {colorables = new List<Colorable>(value);}
		public void SetText (
							 #if ENABLE_REFLECTION
							 Component
							 #elif ENABLE_UNITY_TEXT
							 UnityEngine.UI.Text
							 #else
							 TMPro.TMP_Text
							 #endif
							 value) {if(text != null)text.text = value;}
		public void SetText (GaugeBodyPart value)
		{
			if(!value || value.text == null || text == null)return;text.text = value.text.text;
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(value);
			else
			#endif
			Destroy(value);
		}
		#if ENABLE_UNITY_TEXT
		public void SetTextMesh (TextMesh value) {if(text != null)text.textMesh = value;}
		public void SetTextMesh (GaugeBodyPart value)
		{
			if(!value || value.text == null || text == null)return;text.textMesh = value.text.textMesh;
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(value);
			else
			#endif
			Destroy(value);
		}
		#endif
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public byte type = 0;
		[HideInInspector] public byte component = 0;
		#if ENABLE_UNITY_TEXT
		[HideInInspector] public byte textComponent = 0;
		#endif
		[HideInInspector] public Vector2 colorablesScrollView = Vector2.zero;
		#endif
	}
}