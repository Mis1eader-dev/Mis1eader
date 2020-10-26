namespace Mis1eader
{
	using UnityEngine;
	using UnityEngine.UI;
	[AddComponentMenu("Mis1eader/Health/Health UI",4),ExecuteInEditMode]
	public class HealthUI : MonoBehaviour
	{
		public enum UpdateMode {OnAwake,EveryFrame,ViaScripting}
		[System.Serializable] public struct Digit
		{
			public byte left;
			public sbyte right;
			public Digit (byte left,sbyte right = 0)
			{
				this.left = left;
				this.right = right;
			}
			public void Update ()
			{
				if(left < 1)left = 1;
				if(right < -1)right = -1;
			}
			public void SetLeft (byte value) {if(left != value)left = value;}
			public void SetRight (sbyte value) {if(right != value)right = value;}
		}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		public HealthSystem source = null;
		public int index = 0;
		public Component healthText = null;
		public Digit healthDigits = new Digit(3,0);
		public Component maximumHealthText = null;
		public Digit maximumHealthDigits = new Digit(3,0);
		[HideInInspector] public bool isUpdating = false;
		private void Awake ()
		{
			if(
			#if UNITY_EDITOR
			!Application.isPlaying || 
			#endif
			updateMode != UpdateMode.OnAwake)return;
			isUpdating = true;
			Update();
		}
		private void Update ()
		{
			if(index < -1)index = -1;
			else if(source && source.healths.Count != 0) {if(index >= source.healths.Count)index = source.healths.Count - 1;}
			else if(index > 0)index = 0;
			healthDigits.Update();
			maximumHealthDigits.Update();
			if(
			#if UNITY_EDITOR
			!Application.isPlaying || 
			#endif
			!source || index == -1 || (updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && !isUpdating)return;
			if(isUpdating)isUpdating = false;
			if(healthText)TextHandler(healthText,source.healths[index].health.ToString(new string('0',healthDigits.left) + "." + new string('0',healthDigits.right)));
			if(maximumHealthText)TextHandler(maximumHealthText,source.healths[index].maximumHealth.ToString(new string('0',maximumHealthDigits.left) + "." + new string('0',maximumHealthDigits.right)));
		}
		private void TextHandler (Component component,string text)
		{
			if(!component)return;
			System.Type type = component.GetType();
			if(type.BaseType == typeof(MonoBehaviour))
			{
				System.Reflection.FieldInfo fieldInfo = type.GetField("text");
				if(fieldInfo == null || fieldInfo.FieldType != typeof(string))return;
				fieldInfo.SetValue(component,text);
			}
			else if(type != typeof(Transform) && type != typeof(RectTransform))
			{
				System.Reflection.PropertyInfo propertyInfo = type.GetProperty("text");
				if(propertyInfo == null || propertyInfo.PropertyType != typeof(string))return;
				propertyInfo.SetValue(component,text,null);
			}
		}
		public void SetUpdateMode (UpdateMode value) {if(updateMode != value)updateMode = value;}
		public void SetUpdateMode (int value)
		{
			UpdateMode convertedValue = (UpdateMode)value;
			if(updateMode != convertedValue)updateMode = convertedValue;
		}
		public void SetSource (HealthSystem value) {if(source != value)source = value;}
		public void SetIndex (int value) {if(index != value)index = value;}
		public void SetHealthText (Component value) {if(healthText != value)healthText = value;}
		public void SetHealthDigits (Digit value) {if(!healthDigits.Equals(value))healthDigits = value;}
		public void SetHealthLeftDigits (byte value) {if(healthDigits.left != value)healthDigits.left = value;}
		public void SetHealthRightDigits (sbyte value) {if(healthDigits.right != value)healthDigits.right = value;}
		public void SetMaximumHealthText (Component value) {if(maximumHealthText != value)maximumHealthText = value;}
		public void SetMaximumHealthDigits (Digit value) {if(!maximumHealthDigits.Equals(value))maximumHealthDigits = value;}
		public void SetMaximumHealthLeftDigits (byte value) {if(maximumHealthDigits.left != value)maximumHealthDigits.left = value;}
		public void SetMaximumHealthRightDigits (sbyte value) {if(maximumHealthDigits.right != value)maximumHealthDigits.right = value;}
		public void UpdateUIImmediately ()
		{
			if(!isUpdating)isUpdating = true;
			Update();
		}
		public void UpdateUIPending ()
		{
			if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && !isUpdating)
				isUpdating = true;
		}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
	}
}