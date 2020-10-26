namespace Mis1eader.Localization
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Localization/Localization",1)]
	public class Localization : MonoBehaviour
	{
		[System.Serializable] public class Locale
		{
			public string text = string.Empty;
			public Texture image = null;
			public Language language = Language.EnglishUnitedStates;
			public UnityEvent onSelect = new UnityEvent();
			public void SetText (string value) {text = value;}
			public void SetImage (Texture value) {image = value;}
			public void SetLanguage (Language value) {language = value;}
			public void SetLanguage (int value) {language = (Language)value;}
			public void SetOnSelect (UnityEvent value) {onSelect = value;}
		}
		[System.Serializable] public class UnityEventString : UnityEvent<string> {}
		[System.Serializable] public class UnityEventTexture : UnityEvent<Texture> {}
		public LanguageInstance synchronization = null;
		public Language displayLanguage = Language.EnglishUnitedStates;
		public UnityEventString onLanguageChanged = new UnityEventString();
		public UnityEventTexture _onLanguageChanged = new UnityEventTexture();
		public List<Locale> locales = new List<Locale>();
		[HideInInspector] private Language? lastDisplayLanguage = null;
		private void Update ()
		{
			if(synchronization)displayLanguage = synchronization.language;
			if(lastDisplayLanguage != displayLanguage)
			{
				for(int a = 0,A = locales.Count; a < A; a++)
				{
					Locale locale = locales[a];
					if(locale.language != displayLanguage)continue;
					onLanguageChanged.Invoke(locale.text);
					_onLanguageChanged.Invoke(locale.image);
					locale.onSelect.Invoke();
					break;
				}
				lastDisplayLanguage = displayLanguage;
			}
		}
		public void SetSynchronization (LanguageInstance value) {synchronization = value;}
		public void SetDisplayLanguage (Language value) {displayLanguage = value;}
		public void SetDisplayLanguage (int value) {displayLanguage = (Language)value;}
		public void SetOnLanguageChanged (UnityEventString value) {onLanguageChanged = value;}
		public void SetLocales (List<Locale> value) {locales = value;}
		public void SetLocales (Locale[] value) {locales = new List<Locale>(value);}
	}
}