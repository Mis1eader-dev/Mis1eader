namespace Mis1eader.Customization
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Customization/Value Customization System",2),ExecuteInEditMode]
	public class ValueCustomizationSystem : MonoBehaviour
	{
		public enum UpdateMode : byte {OnAwake,EveryFrame,ViaScripting}
		[System.Serializable] public class ValueCustomization
		{
			[System.Serializable] public class Type
			{
				public string name = string.Empty;
				public float value = 0F;
				public void SetName (string value) {name = value;}
				public void SetValue (float value) {this.value = value;}
			}
			public string name = string.Empty;
			public ValueCustomizationProfile profile = null;
			public Component source = null;
			#if UNITY_5_3_OR_NEWER
			[Delayed]
			#endif
			public string variable = string.Empty;
			public sbyte index = -1;
			public List<Type> types = new List<Type>();
			[HideInInspector] public bool isUpdating = false;
			public void Update (bool run)
			{
				if(index < -1)index = -1;
				else if(index >= types.Count)index = (sbyte)(types.Count - 1);
				if(profile)
				{
					if(profile.variable != string.Empty)variable = profile.variable;
					if(profile.overrideTypes)SetTypesUnlinked(profile.types);
				}
				else VariableName(ref variable);
				if(!run || index == -1)return;
				isUpdating = false;
				Handle(types[index].value);
			}
			internal static void VariableName (ref string name)
			{
				if(name == string.Empty)return;
				bool hasDot = false;
				for(int a = 0,A = name.Length; a < A; a++)
				{
					char character = name[a];
					if(a > 0)
					{
						if(!hasDot && character == '.')hasDot = true;
						if(hasDot && character == '.' && a + 1 < A && name[a + 1] != 'x' && name[a + 1] != 'y' && name[a + 1] != 'z')
						{
							name = name.Remove(a + 1,1);
							a = a - 1;
							A = A - 1;
							continue;
						}
						if(hasDot && a + 1 < A && (name[a + 1] == 'x' || name[a + 1] == 'y' || name[a + 1] == 'z') && a + 2 < A)
						{
							name = name.Remove(a + 2,1);
							a = a - 1;
							A = A - 1;
							continue;
						}
					}
					if(!char.IsLetter(character) && character != '_' && (character != '.' || a == 0 && character == '.') && ((character < '0' || character > '9') || a == 0 && character >= '0' && character <= '9'))
					{
						name = name.Remove(a,1);
						a = a - 1;
						A = A - 1;
						continue;
					}
				}
			}
			private System.Reflection.FieldInfo field = null;
			private System.Reflection.PropertyInfo property = null;
			[HideInInspector,SerializeField] private Component lastSource = null;
			[HideInInspector,SerializeField] private string lastVariable = null;
			[HideInInspector,SerializeField] private float? lastValue = null;
			private void Handle (float value)
			{
				if(!source || variable == string.Empty || lastSource == source && lastVariable == variable && lastValue == value)return;
				System.Type type = source.GetType();
				if(type.BaseType == typeof(MonoBehaviour))
				{
					if(field == null || lastVariable != variable)
					{
						field = type.GetField(variable.Replace(".x",string.Empty).Replace(".y",string.Empty).Replace(".z",string.Empty).Replace(".",string.Empty),System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
						lastVariable = variable;
					}
					if(field != null)
					{
						type = field.FieldType;
						if(type == typeof(float))field.SetValue(source,value);
						else if(type == typeof(int))field.SetValue(source,(int)value);
						else if(type == typeof(Vector2))
						{
							Vector2 vector2 = (Vector2)field.GetValue(source);
							if(variable.Contains(".x")) {vector2.x = value;field.SetValue(source,vector2);}
							else if(variable.Contains(".y")) {vector2.y = value;field.SetValue(source,vector2);}
						}
						else if(type == typeof(Vector3))
						{
							Vector3 vector3 = (Vector3)field.GetValue(source);
							if(variable.Contains(".x")) {vector3.x = value;field.SetValue(source,vector3);}
							else if(variable.Contains(".y")) {vector3.y = value;field.SetValue(source,vector3);}
							else if(variable.Contains(".z")) {vector3.z = value;field.SetValue(source,vector3);}
						}
					}
					lastValue = value;
				}
				else if(type != typeof(Transform) && type != typeof(RectTransform))
				{
					if(property == null || lastVariable != variable)
					{
						property = type.GetProperty(variable.Replace(".x",string.Empty).Replace(".y",string.Empty).Replace(".z",string.Empty).Replace(".",string.Empty));
						lastVariable = variable;
					}
					if(property != null)
					{
						type = property.PropertyType;
						if(type == typeof(float))property.SetValue(source,value,null);
						else if(type == typeof(int))property.SetValue(source,(int)value,null);
						else if(type == typeof(Vector2))
						{
							Vector2 vector2 = (Vector2)property.GetValue(source,null);
							if(variable.Contains(".x")) {vector2.x = value;property.SetValue(source,vector2,null);}
							else if(variable.Contains(".y")) {vector2.y = value;property.SetValue(source,vector2,null);}
						}
						else if(type == typeof(Vector3))
						{
							Vector3 vector3 = (Vector3)property.GetValue(source,null);
							if(variable.Contains(".x")) {vector3.x = value;property.SetValue(source,vector3,null);}
							else if(variable.Contains(".y")) {vector3.y = value;property.SetValue(source,vector3,null);}
							else if(variable.Contains(".z")) {vector3.z = value;property.SetValue(source,vector3,null);}
						}
					}
					lastValue = value;
				}
				lastSource = source;
			}
			public void SetName (string value) {name = value;}
			public void SetProfile (ValueCustomizationProfile value) {profile = value;}
			public void SetSource (Component value) {source = value;}
			public void SetVariable (string value) {variable = value;}
			public void SetIndex (sbyte value) {index = value;}
			public void SetTypes (List<Type> value) {types = value;}
			public void SetTypesUnlinked (List<Type> value) {int A = value.Count;if(types.Count != A)types = new List<Type>(new Type[A]);for(int a = 0; a < A; a++)types[a] = value[a];}
			public void SetTypes (Type[] value) {types = new List<Type>(value);}
			#if UNITY_EDITOR
			[HideInInspector] public string typesName = "Untitled";
			[HideInInspector] public int variableNameIndex = 0;
			#endif
		}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		public List<ValueCustomization> valueCustomizations = new List<ValueCustomization>();
		private void Awake ()
		{
			if(
			#if UNITY_EDITOR
			!Application.isPlaying ||
			#endif
			updateMode != UpdateMode.OnAwake)return;
			for(int a = 0,A = valueCustomizations.Count; a < A; a++)
				valueCustomizations[a].Update(true);
		}
		private void Update ()
		{
			for(int a = 0,A = valueCustomizations.Count; a < A; a++)
				valueCustomizations[a].Update(
				#if UNITY_EDITOR
				(runInEditor || !runInEditor && Application.isPlaying) &&
				#endif
				(updateMode == UpdateMode.EveryFrame || (updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && valueCustomizations[a].isUpdating));
		}
		public void SetUpdateMode (UpdateMode value) {updateMode = value;}
		public void SetUpdateMode (int value) {updateMode = (UpdateMode)value;}
		public void SetValueCustomizations (List<ValueCustomization> value) {valueCustomizations = value;}
		public void SetValueCustomizationsUnlinked (List<ValueCustomization> value) {int A = value.Count;if(valueCustomizations.Count != A)valueCustomizations = new List<ValueCustomization>(new ValueCustomization[A]);for(int a = 0; a < A; a++)valueCustomizations[a] = value[a];}
		public void SetValueCustomizations (ValueCustomization[] value) {valueCustomizations = new List<ValueCustomization>(value);}
		[System.NonSerialized] private int valueCustomizationsPointer = 0;
		public void SetValueCustomizationsPointer (int value) {valueCustomizationsPointer = Mathf.Clamp(value,0,valueCustomizations.Count - 1);}
		public void SetValueCustomizationsPointerIndex (sbyte value) {if(valueCustomizationsPointer >= 0 && valueCustomizationsPointer < valueCustomizations.Count)valueCustomizations[valueCustomizationsPointer].SetIndex(value);}
		public void UpdateValueCustomizationAtIndexImmediately (int index) {if(index >= 0 && index < valueCustomizations.Count)valueCustomizations[index].Update(true);}
		public void UpdateAllValueCustomizationsImmediately () {for(int a = 0,A = valueCustomizations.Count; a < A; a++)valueCustomizations[a].Update(true);}
		public void UpdateValueCustomizationAtIndexPending (int index) {if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && index >= 0 && index < valueCustomizations.Count)valueCustomizations[index].isUpdating = true;}
		public void UpdateAllValueCustomizationsPending () {if(updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting)for(int a = 0,A = valueCustomizations.Count; a < A; a++)valueCustomizations[a].isUpdating = true;}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public bool runInEditor = false;
		[HideInInspector] public string valueCustomizationsName = "Untitled";
		#endif
	}
}