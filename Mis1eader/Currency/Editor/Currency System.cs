namespace Mis1eader.Currency
{
	using UnityEngine;
	using UnityEditor;
	[CustomEditor(typeof(CurrencySystem)),CanEditMultipleObjects]
	internal class CurrencySystemEditor : Editor<CurrencySystem>
	{
		private static bool mainSectionIsExpanded = true;
		internal override void Inspector () {Section("Main",ref mainSectionIsExpanded,() => Container2(serializedObject.FindProperty("currencies"),target.currencies,secondary: MainSectionCurrenciesContainer));}
		private void MainSectionCurrenciesContainer (SerializedProperty currentProperty)
		{
			LabelWidth(42);
			Property(currentProperty.FindPropertyRelative("name"));
			LabelWidth();
			OpenHorizontalSubsection();
			{
				PropertyContainer1(currentProperty.FindPropertyRelative("currency"),group: true,design: 3);
				PropertyContainer1(currentProperty.FindPropertyRelative("maximumCurrency"),group: true,design: 3);
			}
			CloseHorizontal();
		}
	}
}