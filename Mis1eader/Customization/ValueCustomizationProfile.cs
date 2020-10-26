namespace Mis1eader.Customization
{
	using UnityEngine;
	using System.Collections.Generic;
	public class ValueCustomizationProfile : ScriptableObject
	{
		public new string name = string.Empty;
		#if UNITY_5_3_OR_NEWER
		[Delayed]
		#endif
		public string variable = string.Empty;
		[Tooltip("Override the types of the value customization.")]
		public bool overrideTypes = false;
		public List<ValueCustomizationSystem.ValueCustomization.Type> types = new List<ValueCustomizationSystem.ValueCustomization.Type>();
		private void OnValidate () {ValueCustomizationSystem.ValueCustomization.VariableName(ref variable);}
		public void SetName (string value) {name = value;}
		public void SetVariable (string value) {variable = value;}
		public void OverrideTypes (bool value) {overrideTypes = value;}
		public void SetTypes (List<ValueCustomizationSystem.ValueCustomization.Type> value) {types = value;}
		public void SetTypesUnlinked (List<ValueCustomizationSystem.ValueCustomization.Type> value) {int A = value.Count;if(types.Count != A)types = new List<ValueCustomizationSystem.ValueCustomization.Type>(new ValueCustomizationSystem.ValueCustomization.Type[A]);for(int a = 0; a < A; a++)types[a] = value[a];}
		public void SetTypes (ValueCustomizationSystem.ValueCustomization.Type[] value) {types = new List<ValueCustomizationSystem.ValueCustomization.Type>(value);}
		#if UNITY_EDITOR
		[HideInInspector] public string typesName = "Untitled";
		#endif
	}
}