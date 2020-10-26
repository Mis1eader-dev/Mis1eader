namespace Mis1eader.Localization
{
	using UnityEngine;
	using UnityEngine.Events;
	public enum Language : byte
	{
		Afrikaans,
		Arabic,
		Chinese,
		EnglishAustralia,
		EnglishUnitedKingdom,
		EnglishUnitedStates,
		Dutch,
		French,
		German,
		Hindi,
		Italian,
		Japanese,
		Korean,
		KurdishKurmanji,
		KurdishSorani,
		Persian,
		Polish,
		Portuguese,
		Russian,
		Spanish,
		Swedish,
		Thai,
		Turkish,
		UrduIndia,
		UrduPakistan,
		Vietnamese
	}
	[AddComponentMenu("Mis1eader/Localization/Language Instance",0)]
	public class LanguageInstance : MonoBehaviour
	{
		public byte Language {get {return (byte)language;} set {language = (Mis1eader.Localization.Language)value;}}
		public Mis1eader.Localization.Language language = Mis1eader.Localization.Language.EnglishUnitedStates;
		public UnityEvent onLanguageChanged = new UnityEvent();
		[HideInInspector] private Mis1eader.Localization.Language lastLanguage = Mis1eader.Localization.Language.EnglishUnitedStates;
		private void Awake () {lastLanguage = language;}
		private void Update ()
		{
			if(lastLanguage != language)
			{
				onLanguageChanged.Invoke();
				lastLanguage = language;
			}
		}
		public void SetLanguage (Mis1eader.Localization.Language value) {language = value;}
		public void SetLanguage (int value) {language = (Mis1eader.Localization.Language)value;}
		public void SetOnLanguageChanged (UnityEvent value) {onLanguageChanged = value;}
	}
}