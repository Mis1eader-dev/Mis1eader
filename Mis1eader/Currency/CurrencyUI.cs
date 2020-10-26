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
	[AddComponentMenu("Mis1eader/Currency/Currency UI",2),ExecuteInEditMode]
	public class CurrencyUI : MonoBehaviour
	{
		[System.Serializable] public class Text
		{
			#if ENABLE_REFLECTION
			public Component text = null;
			public Text (Component text = null) {this.text = text;}
			private System.Reflection.FieldInfo field = null;
			private System.Reflection.PropertyInfo property = null;
			#elif ENABLE_UNITY_TEXT
			public UnityEngine.UI.Text text = null;
			[UnityEngine.Serialization.FormerlySerializedAs("text")]
			public TextMesh textMesh = null;
			public Text () {}
			public Text (UnityEngine.UI.Text text) {this.text = text;}
			public Text (TextMesh textMesh) {this.textMesh = textMesh;}
			#else
			public TMPro.TMP_Text text = null;
			public Text (TMPro.TMP_Text text = null) {this.text = text;}
			#endif
			[HideInInspector,SerializeField] private string lastText = null;
			public void Handle (string text)
			{
				if(lastText == text)return;
				#if ENABLE_REFLECTION
				if(!this.text)return;
				System.Type type = this.text.GetType();
				if(type.BaseType == typeof(MonoBehaviour))
				{
					if(field == null)field = type.GetField("text");
					if(field != null)field.SetValue(this.text,text);
					lastText = text;
				}
				else if(type != typeof(Transform) && type != typeof(RectTransform))
				{
					if(property == null)property = type.GetProperty("text");
					if(property != null)property.SetValue(this.text,text,null);
					lastText = text;
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
			public void SetText (
								 #if ENABLE_REFLECTION
								 Component
								 #elif ENABLE_UNITY_TEXT
								 UnityEngine.UI.Text
								 #else
								 TMPro.TMP_Text
								 #endif
								 value) {text = value;}
			#if ENABLE_UNITY_TEXT
			public void SetTextMesh (TextMesh value) {textMesh = value;}
			#endif
		}
		[System.Serializable] public class Digit
		{
			public byte left = 1;
			public sbyte right = 2;
			public Digit (byte left = 1,sbyte right = -1)
			{
				this.left = left;
				this.right = right;
			}
			public void Update ()
			{
				if(left == 0)left = 1;
				if(right < -1)right = -1;
			}
			public void SetLeft (byte value) {left = value;}
			public void SetRight (sbyte value) {right = value;}
		}
		public CurrencySystem source = null;
		public sbyte index = 0;
		public Text currencyText = new Text();
		public Digit currencyDigits = new Digit(1,2);
		public Text maximumCurrencyText = new Text();
		public Digit maximumCurrencyDigits = new Digit(1,2);
		[HideInInspector,SerializeField] private double? lastCurrency = null;
		[HideInInspector,SerializeField] private Digit lastCurrencyDigits = new Digit();
		[HideInInspector,SerializeField] private double? lastMaximumCurrency = null;
		[HideInInspector,SerializeField] private Digit lastMaximumCurrencyDigits = new Digit();
		private void Update ()
		{
			if(index < -1)index = -1;
			else if(source && source.currencies.Count != 0) {if(index >= source.currencies.Count)index = (sbyte)(source.currencies.Count - 1);}
			else if(index > 0)index = 0;
			currencyDigits.Update();
			maximumCurrencyDigits.Update();
			if(
			#if UNITY_EDITOR
			!Application.isPlaying || 
			#endif
			!source || index == -1)return;
			CurrencySystem.Currency currency = source.currencies[index];
			if(currencyText != null)
			{
				if(lastCurrency != currency.currency)
				{
					currencyText.Handle(currency.currency.ToString(new string('0',currencyDigits.left) + "." + new string('0',currencyDigits.right)));
					lastCurrency = currency.currency;
				}
				if(lastCurrencyDigits.left != currencyDigits.left)
				{
					currencyText.Handle(currency.currency.ToString(new string('0',currencyDigits.left) + "." + new string('0',currencyDigits.right)));
					lastCurrencyDigits.left = currencyDigits.left;
				}
				if(lastCurrencyDigits.right != currencyDigits.right)
				{
					currencyText.Handle(currency.currency.ToString(new string('0',currencyDigits.left) + "." + new string('0',currencyDigits.right)));
					lastCurrencyDigits.right = currencyDigits.right;
				}
			}
			if(maximumCurrencyText != null)
			{
				if(lastMaximumCurrency != currency.maximumCurrency)
				{
					maximumCurrencyText.Handle(currency.maximumCurrency.ToString(new string('0',maximumCurrencyDigits.left) + "." + new string('0',maximumCurrencyDigits.right)));
					lastMaximumCurrency = currency.maximumCurrency;
				}
				if(lastMaximumCurrencyDigits.left != maximumCurrencyDigits.left)
				{
					maximumCurrencyText.Handle(currency.maximumCurrency.ToString(new string('0',maximumCurrencyDigits.left) + "." + new string('0',maximumCurrencyDigits.right)));
					lastMaximumCurrencyDigits.left = maximumCurrencyDigits.left;
				}
				if(lastMaximumCurrencyDigits.right != maximumCurrencyDigits.right)
				{
					maximumCurrencyText.Handle(currency.maximumCurrency.ToString(new string('0',maximumCurrencyDigits.left) + "." + new string('0',maximumCurrencyDigits.right)));
					lastMaximumCurrencyDigits.right = maximumCurrencyDigits.right;
				}
			}
		}
		public void SetSource (CurrencySystem value) {source = value;}
		public void SetIndex (sbyte value) {index = value;}
		public void SetCurrencyText (
							 #if ENABLE_REFLECTION
							 Component
							 #elif ENABLE_UNITY_TEXT
							 UnityEngine.UI.Text
							 #else
							 TMPro.TMP_Text
							 #endif
							 value) {if(currencyText != null)currencyText.text = value;}
		#if ENABLE_UNITY_TEXT
		public void SetCurrencyTextMesh (TextMesh value) {if(currencyText != null)currencyText.textMesh = value;}
		#endif
		public void SetCurrencyDigits (Digit value) {currencyDigits = value;}
		public void SetCurrencyLeftDigits (byte value) {currencyDigits.left = value;}
		public void SetCurrencyRightDigits (sbyte value) {currencyDigits.right = value;}
		public void SetMaximumCurrencyText (
							 #if ENABLE_REFLECTION
							 Component
							 #elif ENABLE_UNITY_TEXT
							 UnityEngine.UI.Text
							 #else
							 TMPro.TMP_Text
							 #endif
							 value) {if(maximumCurrencyText != null)maximumCurrencyText.text = value;}
		#if ENABLE_UNITY_TEXT
		public void SetMaximumCurrencyTextMesh (TextMesh value) {if(maximumCurrencyText != null)maximumCurrencyText.textMesh = value;}
		#endif
		public void SetMaximumCurrencyDigits (Digit value) {maximumCurrencyDigits = value;}
		public void SetMaximumCurrencyLeftDigits (byte value) {maximumCurrencyDigits.left = value;}
		public void SetMaximumCurrencyRightDigits (sbyte value) {maximumCurrencyDigits.right = value;}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR && ENABLE_UNITY_TEXT
		[HideInInspector] public byte component = 0;
		#endif
	}
}