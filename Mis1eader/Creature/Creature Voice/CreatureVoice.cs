namespace Mis1eader.Creature
{
	using UnityEngine;
	using UnityEngine.Events;
	using System.Collections.Generic;
	using Localization;
	[AddComponentMenu("Mis1eader/Creature/Creature Voice/Creature Voice",1),RequireComponent(typeof(CreatureIdentity)),ExecuteInEditMode]
	public class CreatureVoice : MonoBehaviour
	{
		public byte SpeakingLanguage {get {return (byte)speakingLanguage;} set {speakingLanguage = (Language)value;}}
		public enum Length : byte {Text,Clip}
		[System.Serializable] public class Line
		{
			public AudioClip clip = null;
			[TextArea] public string line = string.Empty;
			[TextArea] public string display = string.Empty;
			public Line () {}
			public Line (AudioClip clip,string line,string display)
			{
				this.clip = clip;
				this.line = line;
				this.display = display;
			}
		}
		[System.Serializable] public class UnityEventString : UnityEvent<string> {}
		[System.Serializable] public class UnityEventStringFloat : UnityEvent<string,float> {}
		public AudioSource voiceSource = null;
		public Language speakingLanguage = Language.EnglishUnitedStates;
		public byte voiceVariant = 0;
		public CreatureVoiceManager voiceManager = null;
		public int talk = -1;
		public Length length = Length.Text;
		public bool includeTranslation = true;
		public UnityEventString onTalk = new UnityEventString();
		public UnityEventStringFloat onTalkLength = new UnityEventStringFloat();
		public List<Line> lines = new List<Line>();
		[HideInInspector] private Language? lastSpeakingLanguage = null;
		[HideInInspector] private Language? lastDisplayLanguage = null;
		[HideInInspector] private Gender? lastGender = null;
		[HideInInspector] private byte? lastVariant = null;
		[HideInInspector] private bool? lastIncludeTranslation = null;
		[HideInInspector,SerializeField] private CreatureIdentity identity = null;
		private void Update ()
		{
			if(!identity || identity.gameObject != gameObject)
			{
				identity = GetComponent<CreatureIdentity>();
				if(!identity)identity = gameObject.AddComponent<CreatureIdentity>();
			}
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				talk = -1;
				return;
			}
			#endif
			if(talk < -1)talk = -1;
			if(voiceManager)
			{
				if(lastSpeakingLanguage != speakingLanguage || includeTranslation && (lastDisplayLanguage != voiceManager.displayLanguage || lastIncludeTranslation != includeTranslation) || lastGender != identity.gender || lastVariant != voiceVariant)
				{
					voiceManager.GetLines(ref lines,speakingLanguage,identity.gender,voiceVariant,includeTranslation);
					lastSpeakingLanguage = speakingLanguage;
					lastDisplayLanguage = voiceManager.displayLanguage;
					lastGender = identity.gender;
					lastVariant = voiceVariant;
					lastIncludeTranslation = includeTranslation;
				}
				if(!includeTranslation && lastIncludeTranslation != includeTranslation)
				{
					for(int a = 0,A = lines.Count; a < A; a++)
					{
						Line line = lines[a];
						line.display = line.line;
					}
					lastIncludeTranslation = includeTranslation;
				}
				if(talk != -1)
				{
					if(voiceSource && talk < lines.Count)
					{
						Line line = lines[talk];
						onTalk.Invoke(line.display);
						if(line.clip)
						{
							voiceSource.clip = line.clip;
							voiceSource.Play();
							onTalkLength.Invoke(line.display,length == Length.Clip ? line.clip.length : Mathf.Log(line.line.Length + 1));
						}
						else onTalkLength.Invoke(line.display,Mathf.Log(line.line.Length + 1));
					}
					talk = -1;
				}
			}
		}
		public void SetVoiceSource (AudioSource value) {voiceSource = value;}
		public void SetSpeakingLanguage (Language value) {speakingLanguage = value;}
		public void SetSpeakingLanguage (int value) {speakingLanguage = (Language)value;}
		public void SetVoiceVariant (byte value) {voiceVariant = value;}
		public void SetVoiceManager (CreatureVoiceManager value) {voiceManager = value;}
		public void SetTalk (int value) {talk = value;}
		public void SetTalk (string value)
		{
			for(int a = 0,A = lines.Count; a < A; a++)
			{
				Line line = lines[a];
				if(line.line != value && line.display != value)continue;
				talk = a;
				return;
			}
		}
		public void SetLength (Length value) {length = value;}
		public void SetLength (int value) {length = (Length)value;}
		public void SetIncludeTranslation (bool value) {includeTranslation = value;}
		public void SetOnTalk (UnityEventString value) {onTalk = value;}
		public void SetOnTalkLength (UnityEventStringFloat value) {onTalkLength = value;}
		public void SetLines (List<Line> value) {lines = value;}
		public void SetLines (Line[] value) {lines = new List<Line>(value);}
	}
}