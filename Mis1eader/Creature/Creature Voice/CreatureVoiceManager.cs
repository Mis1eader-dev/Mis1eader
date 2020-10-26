namespace Mis1eader.Creature
{
	using UnityEngine;
	using System.Collections.Generic;
	using Localization;
	[AddComponentMenu("Mis1eader/Creature/Creature Voice/Creature Voice Manager",0)]
	public class CreatureVoiceManager : MonoBehaviour
	{
		[System.Serializable] public class Line
		{
			[System.Serializable] public class Voice
			{
				[System.Serializable] public class Variant
				{
					public AudioClip male = null;
					public AudioClip female = null;
				}
				public Language language = Language.EnglishUnitedStates;
				[TextArea] public string dialog = string.Empty;
				public List<Variant> variants = new List<Variant>(new Variant[1]);
				public void SetLanguage (Language value) {language = value;}
				public void SetLanguage (int value) {language = (Language)value;}
				public void SetDialog (string value) {dialog = value;}
				public void SetVariants (List<Variant> value) {variants = value;}
				public void SetVariants (Variant[] value) {variants = new List<Variant>(value);}
			}
			public string label = string.Empty;
			public List<Voice> voices = new List<Voice>(new Voice[1]);
			public void SetLabel (string value) {label = value;}
			public void SetVoices (List<Voice> value) {voices = value;}
			public void SetVoices (Voice[] value) {voices = new List<Voice>(value);}
		}
		public LanguageInstance synchronization = null;
		public Language displayLanguage = Language.EnglishUnitedStates;
		public List<Line> lines = new List<Line>();
		private void Update () {if(synchronization)displayLanguage = synchronization.language;}
		public void GetLines (ref List<CreatureVoice.Line> lines,Language language,Gender gender,byte voiceVariant,bool translate = true)
		{
			int index = 0;
			for(int a = 0,A = this.lines.Count; a < A; a++)
			{
				Line line = this.lines[a];
				if(translate)
				{
					bool incremented = false;
					string display = null;
					for(int b = 0,B = line.voices.Count; b < B; b++)
					{
						Line.Voice voice = line.voices[b];
						if(voice.language == language)
						{
							if(index >= lines.Count)
							{
								lines.Add(new CreatureVoice.Line(voiceVariant < voice.variants.Count ? (gender == Gender.Male ? voice.variants[voiceVariant].male : voice.variants[voiceVariant].female) : null,voice.dialog,display));
								index = index + 1;
								incremented = true;
							}
							else
							{
								if(voiceVariant < voice.variants.Count)lines[index].clip = gender == Gender.Male ? voice.variants[voiceVariant].male : voice.variants[voiceVariant].female;
								lines[index].line = voice.dialog;
								lines[index].display = display;
								index = index + 1;
								incremented = true;
							}
						}
						if(voice.language == displayLanguage && display == null)
						{
							display = voice.dialog;
							if((incremented ? index - 1 : index) < lines.Count)
								lines[incremented ? index - 1 : index].display = display;
						}
					}
				}
				else for(int b = 0,B = line.voices.Count; b < B; b++)
				{
					Line.Voice voice = line.voices[b];
					if(voice.language != language)continue;
					if(index >= lines.Count)
					{
						lines.Add(new CreatureVoice.Line(voiceVariant < voice.variants.Count ? (gender == Gender.Male ? voice.variants[voiceVariant].male : voice.variants[voiceVariant].female) : null,voice.dialog,voice.dialog));
						index = index + 1;
					}
					else
					{
						if(voiceVariant < voice.variants.Count)lines[index].clip = gender == Gender.Male ? voice.variants[voiceVariant].male : voice.variants[voiceVariant].female;
						lines[index].line = voice.dialog;
						lines[index].display = voice.dialog;
						index = index + 1;
					}
				}
			}
			while(lines.Count > index)lines.RemoveAt(index);
		}
		public void SetSynchronization (LanguageInstance value) {synchronization = value;}
		public void SetDisplayLanguage (Language value) {displayLanguage = value;}
		public void SetDisplayLanguage (int value) {displayLanguage = (Language)value;}
		public void SetLines (List<Line> value) {lines = value;}
		public void SetLines (Line[] value) {lines = new List<Line>(value);}
		public void UpdateDisplayLanguage () {if(synchronization)displayLanguage = synchronization.language;}
	}
}