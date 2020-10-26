namespace AdvancedAssets
{
	using UnityEngine;
	using UnityEngine.Events;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using System.Collections.Generic;
	[AddComponentMenu("Advanced Assets/Media/Media Player/Audio Clip Player",15),ExecuteInEditMode]
	public class AudioClipPlayer : MonoBehaviour
	{
		public enum Type {Manual,Random}
		public AudioSource source = null;
		public bool skipBackward = false;
		public bool fastBackward = false;
		public bool pause = false;
		public bool fastForward = false;
		public bool skipForward = false;
		public bool repeat = true;
		public bool preventRepeating = true;
		public Type type = Type.Manual;
		public List<AudioClip> audioClips = new List<AudioClip>();
		public float volume = 1;
		public float pitch = 1;
		public bool useFadeIn = false;
		public float fadeIn = 0.01f;
		public bool useFadeOut = false;
		public float fadeOut = 0.99f;
		public bool useSwitchTime = false;
		public float switchTime = 0.4f;
		public AudioClip switchClip = null;
		public UnityEvent onFinish = new UnityEvent();
		public UnityEvent isFinished = new UnityEvent();
		#if UNITY_EDITOR
		[HideInInspector] public int audioClipsScrollViewIndex = 0;
		[HideInInspector] public bool audioClipsIsExpanded = true;
		[HideInInspector] public Vector2 audioClipsScrollView = Vector2.zero;
		[HideInInspector] public Texture2D skipBackwardTexture = null;
		[HideInInspector] public Texture2D fastBackwardTexture = null;
		[HideInInspector] public Texture2D playTexture = null;
		[HideInInspector] public Texture2D pauseTexture = null;
		[HideInInspector] public Texture2D fastForwardTexture = null;
		[HideInInspector] public Texture2D skipForwardTexture = null;
		[HideInInspector] public Texture2D repeatTexture = null;
		[HideInInspector] public Texture2D preventRepeatingTexture = null;
		#endif
		[HideInInspector] public int index = -1;
		[HideInInspector] public int preventRepeatingIndex = -1;
		[HideInInspector] public float sourceTime = 0;
		[HideInInspector] public float clipLength = 1;
		[HideInInspector] private bool canPlay = true;
		[HideInInspector] private bool isPaused = false;
		[HideInInspector] private bool isSwitching = false;
		[HideInInspector] private bool _isFinished = false;
		private void Update ()
		{
			EditorHandler();
			if(Application.isPlaying && source && source.gameObject.activeInHierarchy && source.enabled)
			{
				SourceHandler();
				if(!isPaused && !pause)
				{
					SwitchHandler();
					if(audioClips.Count > 0 && (!source.clip || source.clip && !source.isPlaying || index >= 0 && index < audioClips.Count && !audioClips[index] || skipBackward || skipForward))
					{
						if(source.isPlaying && !isSwitching)source.Stop();
						ControlHandler();
						if(useSwitchTime && switchClip && isSwitching && (index < 0 || index >= audioClips.Count))
						{
							isSwitching = false;
							canPlay = true;
						}
						IndexHandler();
						if(useSwitchTime && switchClip && !isSwitching && (index >= 0 && index < audioClips.Count && audioClips[index] || index < 0 || index >= audioClips.Count))
						{
							isSwitching = true;
							canPlay = false;
						}
						if(index >= 0 && index < audioClips.Count && audioClips[index] && canPlay && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
						{
							if(audioClips.Count > 1 && preventRepeatingIndex != index)
								preventRepeatingIndex = index;
							if(useSwitchTime && isSwitching)isSwitching = false;
							if(skipBackward)skipBackward = false;
							if(skipForward)skipForward = false;
							source.clip = audioClips[index];
							source.Play();
						}
					}
				}
			}
		}
		private void EditorHandler ()
		{
			volume = Mathf.Clamp01(volume);
			pitch = Mathf.Clamp(pitch,-10,10);
			fadeIn = Mathf.Clamp(fadeIn,0,0.25f);
			fadeOut = Mathf.Clamp(fadeOut,0.75f,1);
			switchTime = Mathf.Clamp(switchTime,0.1f,float.MaxValue);
			if(pause && (skipBackward || skipForward))pause = false;
			if(source)
			{
				if(audioClips.Count > 0 && index >= 0 && index < audioClips.Count && audioClips[index])
				{
					if(sourceTime != (!isSwitching ? source.time : 0))
						sourceTime = !isSwitching ? source.time : 0;
					if(clipLength != audioClips[index].length)
						clipLength = (float)audioClips[index].length;
					if(Application.isPlaying)FadeHandler();
				}
				else
				{
					if(sourceTime != 0)sourceTime = 0;
					if(clipLength != 1)clipLength = 1;
				}
			}
		}
		private void FadeHandler ()
		{
			if(!isSwitching)
			{
				if((!useFadeIn && !useFadeOut || useFadeIn && useFadeOut && sourceTime > clipLength * fadeIn && sourceTime < clipLength * fadeOut || useFadeIn && !useFadeOut && sourceTime > clipLength * fadeIn || !useFadeIn && useFadeOut && sourceTime < clipLength * fadeOut) && source.volume != volume)
					source.volume = volume;
				if(useFadeIn && sourceTime <= clipLength * fadeIn && source.volume != Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume))
					source.volume = Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume);
				if(useFadeOut && sourceTime >= clipLength * fadeOut && source.volume != Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume))
					source.volume = Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume);
			}
			else if(source.volume != volume)
				source.volume = volume;
		}
		private void SourceHandler ()
		{
			if(index >= 0 && index < audioClips.Count && source.clip != audioClips[index] && !isSwitching)source.clip = audioClips[index];
			if(source.clip)
			{
				if(audioClips.Count == 0)source.clip = null;
				if(pause)
				{
					if(source.isPlaying)source.Pause();
					if(!isPaused)isPaused = true;
				}
				else if(isPaused)
				{
					if(!source.isPlaying)source.Play();
					isPaused = false;
				}
				if(source.isPlaying && !pause && !isPaused)
				{
					if(!fastBackward && !fastForward && source.pitch != pitch)
						source.pitch = pitch;
					if(fastBackward && source.pitch != pitch * -2)
						source.pitch = pitch * -2;
					if(fastForward && source.pitch != pitch * 2)
						source.pitch = pitch * 2;
				}
			}
			else
			{
				if(fastBackward)fastBackward = false;
				if(fastForward)fastForward = false;
			}
		}
		[HideInInspector] private float switchCounter = 0;
		private void SwitchHandler ()
		{
			if(useSwitchTime && switchClip)
			{
				if(isSwitching && switchCounter < switchTime)
				{
					if(source.clip != switchClip)
					{
						source.clip = switchClip;
						source.Play();
					}
					if(!source.loop)source.loop = true;
					if(switchCounter < 0)switchCounter = 0;
					if(canPlay)canPlay = false;
					switchCounter = switchCounter + UnityEngine.Time.deltaTime;
				}
				if(switchCounter >= switchTime && (index >= 0 && index < audioClips.Count && audioClips[index] || repeat && index < 0 || repeat && index >= audioClips.Count))
				{
					if(source.clip)
					{
						source.clip = null;
						source.Stop();
					}
					if(source.loop)source.loop = false;
					if(!canPlay)canPlay = true;
					switchCounter = -1;
				}
			}
			else
			{
				if(!canPlay)canPlay = true;
				if(isSwitching)isSwitching = false;
				if(switchCounter != -1)switchCounter = -1;
			}
		}
		private void ControlHandler ()
		{
			if(!skipBackward && !skipForward && !isSwitching)
			{
				if(type == Type.Manual && (index == -1 || index >= 0 && index < audioClips.Count))index = index + 1;
				if(type == Type.Random)
				{
					if(preventRepeating)while(index == preventRepeatingIndex)
						index = Random.Range(0,audioClips.Count);
					else index = Random.Range(0,audioClips.Count);
				}
			}
			if(fastBackward)fastBackward = false;
			if(fastForward)fastForward = false;
			if(skipBackward)
			{
				if(index >= 0 && index < audioClips.Count)
				{
					if(type == Type.Manual)index = index - 1;
					if(type == Type.Random)index = Random.Range(0,audioClips.Count);
				}
				if(index >= 0 && index < audioClips.Count && audioClips[index] && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
					skipBackward = false;
			}
			if(skipForward)
			{
				if(index >= 0 && index < audioClips.Count)
				{
					if(type == Type.Manual)index = index + 1;
					if(type == Type.Random)index = Random.Range(0,audioClips.Count);
				}
				if(index >= 0 && index < audioClips.Count && audioClips[index] && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
					skipForward = false;
			}
		}
		private void IndexHandler ()
		{
			if(repeat && _isFinished)_isFinished = false;
			if(index < 0)
			{
				if(type == Type.Manual)
				{
					if(repeat)
					{
						index = audioClips.Count - 1;
						if(audioClips.Count == 1 && preventRepeatingIndex != -1)
							preventRepeatingIndex = -1;
						if(useSwitchTime && audioClips[index])
						{
							if(skipBackward)skipBackward = false;
							if(skipForward)skipForward = false;
						}
					}
					else
					{
						index = audioClips.Count;
						if(preventRepeatingIndex != -1)preventRepeatingIndex = -1;
						if(source.clip)source.clip = null;
						if(!_isFinished)
						{
							onFinish.Invoke();
							_isFinished = true;
						}
						else isFinished.Invoke();
						if(skipBackward)skipBackward = false;
						if(skipForward)skipForward = false;
					}
				}
				if(type == Type.Random)index = Random.Range(0,audioClips.Count);
			}
			if(index >= audioClips.Count)
			{
				if(type == Type.Manual)
				{
					if(repeat)
					{
						index = 0;
						if(audioClips.Count == 1 && preventRepeatingIndex != -1)
							preventRepeatingIndex = -1;
						if(useSwitchTime && audioClips[index])
						{
							if(skipBackward)skipBackward = false;
							if(skipForward)skipForward = false;
						}
					}
					else
					{
						if(index != audioClips.Count)index = audioClips.Count;
						if(preventRepeatingIndex != -1)preventRepeatingIndex = -1;
						if(source.clip)source.clip = null;
						if(!_isFinished)
						{
							onFinish.Invoke();
							_isFinished = true;
						}
						else isFinished.Invoke();
						if(skipBackward)skipBackward = false;
						if(skipForward)skipForward = false;
					}
				}
				if(type == Type.Random)index = Random.Range(0,audioClips.Count);
			}
		}
		private float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum);}
		public void SetSource (AudioSource value) {if(source != value)source = value;}
		public void Repeat (bool value) {if(repeat != value)repeat = value;}
		public void PreventRepeating (bool value) {if(preventRepeating != value)preventRepeating = value;}
		public void SetType (Type value) {if(type != value)type = value;}
		public void SetType (int value)
		{
			Type convertedValue = (Type)value;
			if(type != convertedValue)type = convertedValue;
		}
		public void SetAudioClips (List<AudioClip> value) {if(audioClips != value)audioClips = value;}
		public void SetAudioClips (AudioClip[] value)
		{
			List<AudioClip> convertedValue = new List<AudioClip>(value);
			if(audioClips != convertedValue)audioClips = convertedValue;
		}
		public void SetVolume (float value) {if(volume != value)volume = value;}
		public void SetPitch (float value) {if(pitch != value)pitch = value;}
		public void UseFadeIn (bool value) {if(useFadeIn != value)useFadeIn = value;}
		public void SetFadeIn (float value) {if(fadeIn != value)fadeIn = value;}
		public void UseFadeOut (bool value) {if(useFadeOut != value)useFadeOut = value;}
		public void SetFadeOut (float value) {if(fadeOut != value)fadeOut = value;}
		public void UseSwitchTime (bool value) {if(useSwitchTime != value)useSwitchTime = value;}
		public void SetSwitchTime (float value) {if(switchTime != value)switchTime = value;}
		public void SetSwitchClip (AudioClip value) {if(switchClip != value)switchClip = value;}
		public void SetOnFinish (UnityEvent value) {if(onFinish != value)onFinish = value;}
		public void SetIsFinished (UnityEvent value) {if(isFinished != value)isFinished = value;}
		public void SetIndex (int value)
		{
			if(index == value)
			{
				if(source && source.isPlaying)source.Stop();
				if(useSwitchTime && switchClip)isSwitching = true;
				if(preventRepeatingIndex != -1)preventRepeatingIndex = -1;
			}
			else index = value;
		}
		public void SkipBackward () {if(!skipBackward)skipBackward = true;}
		public void FastBackward () {if(!fastBackward)fastBackward = true;}
		public void Play () {if(pause)pause = false;}
		public void Pause () {if(!pause)pause = true;}
		public void FastForward () {if(!fastForward)fastForward = true;}
		public void SkipForward () {if(!skipForward)skipForward = true;}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(AudioClipPlayer)),CanEditMultipleObjects]
	internal class AudioClipPlayerInspector : Editor
	{
		private AudioClipPlayer[] audioClipPlayers
		{
			get
			{
				AudioClipPlayer[] audioClipPlayers = new AudioClipPlayer[targets.Length];
				for(int audioClipPlayersIndex = 0; audioClipPlayersIndex < targets.Length; audioClipPlayersIndex++)
					audioClipPlayers[audioClipPlayersIndex] = (AudioClipPlayer)targets[audioClipPlayersIndex];
				return audioClipPlayers;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			StatsSection();
			ConsoleSection();
			MainSection();
			ClipsSection();
			ConfigureSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int audioClipPlayersIndex = 0; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)
					EditorUtility.SetDirty(audioClipPlayers[audioClipPlayersIndex]);
			}
		}
		private void StatsSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("Stats",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					Color color = GUI.color;
					if(!Application.isPlaying)
					{
						GUI.color = Color.yellow;
						GUILayout.Box("[In Editor]",GUILayout.ExpandWidth(true));
					}
					else
					{
						if(audioClipPlayers[0].gameObject.activeInHierarchy && audioClipPlayers[0].enabled)
						{
							if(audioClipPlayers[0].source && audioClipPlayers[0].source.enabled && (audioClipPlayers[0].index >= 0 && audioClipPlayers[0].index < audioClipPlayers[0].audioClips.Count && audioClipPlayers[0].audioClips[audioClipPlayers[0].index] || audioClipPlayers[0].repeat && audioClipPlayers[0].pause && (audioClipPlayers[0].index < 0 || audioClipPlayers[0].index >= audioClipPlayers[0].audioClips.Count)))
							{
								GUI.color = audioClipPlayers[0].pause ? Color.yellow : Color.green;
								GUILayout.Box(audioClipPlayers[0].pause ? "[Paused]" : "[Playing]",GUILayout.ExpandWidth(true));
							}
							if(!audioClipPlayers[0].source || audioClipPlayers[0].audioClips.Count == 0 || audioClipPlayers[0].index >= 0 && audioClipPlayers[0].index < audioClipPlayers[0].audioClips.Count && !audioClipPlayers[0].audioClips[audioClipPlayers[0].index])
							{
								GUI.color = Color.red;
								GUILayout.Box("[Not Working]",GUILayout.ExpandWidth(true));
							}
							if(audioClipPlayers[0].source && audioClipPlayers[0].source.gameObject.activeInHierarchy && audioClipPlayers[0].source.enabled && !audioClipPlayers[0].repeat && audioClipPlayers[0].audioClips.Count > 0 && (audioClipPlayers[0].index < 0 || audioClipPlayers[0].index >= audioClipPlayers[0].audioClips.Count))
							{
								GUI.color = Color.red;
								GUILayout.Box("[Stopped]",GUILayout.ExpandWidth(true));
							}
						}
						if(!audioClipPlayers[0].gameObject.activeInHierarchy || !audioClipPlayers[0].enabled || audioClipPlayers[0].source && !audioClipPlayers[0].source.gameObject.activeInHierarchy || audioClipPlayers[0].source && !audioClipPlayers[0].source.enabled)
						{
							GUI.color = Color.red;
							GUILayout.Box("[Disabled]",GUILayout.ExpandWidth(true));
						}
					}
					if(audioClipPlayers[0].type == AudioClipPlayer.Type.Manual)
					{
						GUI.color = audioClipPlayers[0].repeat ? Color.green : Color.red;
						GUILayout.Box(audioClipPlayers[0].repeat ? "[Repeats]" : "[Does Not Repeat]",GUILayout.ExpandWidth(true));
					}
					if(audioClipPlayers[0].type == AudioClipPlayer.Type.Random)
					{
						GUI.color = audioClipPlayers[0].preventRepeating ? Color.green : Color.red;
						GUILayout.Box(audioClipPlayers[0].preventRepeating ? "[Prevents Repeating]" : "[Does Not Prevent Repeating]",GUILayout.ExpandWidth(true));
					}
					GUI.color = color;
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		private void ConsoleSection ()
		{
			bool canCreateSource = false;
			bool canEnableSource = false;
			if(serializedObject.isEditingMultipleObjects)for(int audioClipPlayersIndex = 0; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)
			{
				if(!audioClipPlayers[audioClipPlayersIndex].source && !audioClipPlayers[audioClipPlayersIndex].GetComponent<AudioSource>())canCreateSource = true;
				if(audioClipPlayers[audioClipPlayersIndex].gameObject.activeInHierarchy && audioClipPlayers[audioClipPlayersIndex].enabled && audioClipPlayers[audioClipPlayersIndex].source && !audioClipPlayers[audioClipPlayersIndex].source.enabled)canEnableSource = true;
				if(canCreateSource && canEnableSource)break;
			}
			else
			{
				if(!audioClipPlayers[0].source && !audioClipPlayers[0].GetComponent<AudioSource>())canCreateSource = true;
				if(audioClipPlayers[0].gameObject.activeInHierarchy && audioClipPlayers[0].enabled && audioClipPlayers[0].source && !audioClipPlayers[0].source.enabled)canEnableSource = true;
			}
			if(canCreateSource)
			{
				EditorGUILayout.BeginVertical("Box");
				{
					GUILayout.Label("Create a recommended source?");
					if(GUILayout.Button("Create"))
					{
						for(int audioClipPlayersIndex = 0; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)if(!audioClipPlayers[audioClipPlayersIndex].source && !audioClipPlayers[audioClipPlayersIndex].GetComponent<AudioSource>())
						{
							AudioSource audioSource = Undo.AddComponent(audioClipPlayers[audioClipPlayersIndex].gameObject,typeof(AudioSource)).GetComponent<AudioSource>();
							audioSource.playOnAwake = false;
							audioSource.priority = 0;
							audioSource.rolloffMode = AudioRolloffMode.Linear;
							audioSource.minDistance = 0;
							audioSource.maxDistance = 10;
							audioClipPlayers[audioClipPlayersIndex].source = audioSource;
						}
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndVertical();
			}
			if(canEnableSource)
			{
				EditorGUILayout.BeginVertical("Box");
				{
					GUILayout.Label("Enable the source?");
					if(GUILayout.Button("Enable"))
					{
						for(int audioClipPlayersIndex = 0; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)if(audioClipPlayers[audioClipPlayersIndex].gameObject.activeInHierarchy && audioClipPlayers[audioClipPlayersIndex].enabled && audioClipPlayers[audioClipPlayersIndex].source && !audioClipPlayers[audioClipPlayersIndex].source.enabled)
						{
							Undo.RecordObject(audioClipPlayers[audioClipPlayersIndex].source,"Inspector");
							audioClipPlayers[audioClipPlayersIndex].source.enabled = true;
						}
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndVertical();
			}
		}
		private void MainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Main","BoldLabel");
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("source"),true);
				EditorGUIUtility.labelWidth = 0;
				if(Application.isPlaying && audioClipPlayers[0].source && audioClipPlayers[0].source.gameObject.activeInHierarchy && audioClipPlayers[0].source.enabled && audioClipPlayers[0].audioClips.Count > 0 && audioClipPlayers[0].index >= 0 && audioClipPlayers[0].index < audioClipPlayers[0].audioClips.Count && audioClipPlayers[0].audioClips[audioClipPlayers[0].index])
				{
					GUIStyle style = new GUIStyle() {alignment = TextAnchor.MiddleCenter,fontStyle = FontStyle.Bold,fontSize = 10};
					EditorGUILayout.BeginVertical("Box");
					{
						GUILayout.Label("Playing",style);
						style.fontSize = 14;
						style.wordWrap = true;
						GUILayout.Label(audioClipPlayers[0].audioClips[audioClipPlayers[0].index].name,style);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.BeginVertical("Box");
				{
					if(audioClipPlayers[0].source)
					{
						string second = string.Empty;
						string maximumSecond = string.Empty;
						string minute = string.Empty;
						string maximumMinute = string.Empty;
						if(audioClipPlayers[0].audioClips.Count > 0 && audioClipPlayers[0].index >= 0 && audioClipPlayers[0].index < audioClipPlayers[0].audioClips.Count && audioClipPlayers[0].audioClips[audioClipPlayers[0].index])
						{
							second = Mathf.Floor(audioClipPlayers[0].sourceTime % 60).ToString("00");
							maximumSecond = Mathf.Floor(audioClipPlayers[0].clipLength % 60).ToString("00");
							minute = Mathf.Floor(audioClipPlayers[0].sourceTime / 60).ToString("00");
							maximumMinute = Mathf.Floor(audioClipPlayers[0].clipLength / 60).ToString("00");
						}
						else
						{
							second = "--";
							maximumSecond = second;
							minute = second;
							maximumMinute = second;
						}
						EditorGUILayout.BeginHorizontal();
						{
							Color color = GUI.color;
							GUILayout.Label(minute + ":" + second);
							GUILayout.FlexibleSpace();
							GUI.color = Color.white;
							EditorGUILayout.BeginHorizontal("Box",GUILayout.Width(140),GUILayout.Height(4));
							{
								GUI.color = Color.black;
								GUILayout.Box(GUIContent.none,GUILayout.Width(RangeConversion(audioClipPlayers[0].sourceTime,0,audioClipPlayers[0].clipLength,3,132)),GUILayout.Height(4));
							}
							EditorGUILayout.EndHorizontal();
							GUI.color = color;
							GUILayout.FlexibleSpace();
							GUILayout.Label(maximumMinute + ":" + maximumSecond);
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.BeginHorizontal();
					{
						Color color = GUI.color;
						GUILayout.FlexibleSpace();
						if(GUILayout.Button(audioClipPlayers[0].skipBackwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							audioClipPlayers[0].skipBackward = true;
							for(int audioClipPlayersIndex = 1; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)
								audioClipPlayers[audioClipPlayersIndex].skipBackward = true;
							GUI.FocusControl(null);
						}
						bool fastBackward = audioClipPlayers[0].fastBackward;
						audioClipPlayers[0].fastBackward = GUILayout.RepeatButton(audioClipPlayers[0].fastBackwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int audioClipPlayersIndex = 1; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)
							audioClipPlayers[audioClipPlayersIndex].fastBackward = audioClipPlayers[0].fastBackward;
						if(!fastBackward && audioClipPlayers[0].fastBackward)
							GUI.FocusControl(null);
						if(GUILayout.Button(audioClipPlayers[0].pause ? audioClipPlayers[0].playTexture : audioClipPlayers[0].pauseTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							audioClipPlayers[0].pause = !audioClipPlayers[0].pause;
							for(int audioClipPlayersIndex = 1; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)if(audioClipPlayers[audioClipPlayersIndex].pause != audioClipPlayers[0].pause)
								audioClipPlayers[audioClipPlayersIndex].pause = audioClipPlayers[0].pause;
							GUI.FocusControl(null);
						}
						bool fastForward = audioClipPlayers[0].fastForward;
						audioClipPlayers[0].fastForward = GUILayout.RepeatButton(audioClipPlayers[0].fastForwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int audioClipPlayersIndex = 1; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)
							audioClipPlayers[audioClipPlayersIndex].fastForward = audioClipPlayers[0].fastForward;
						if(!fastForward && audioClipPlayers[0].fastForward)
							GUI.FocusControl(null);
						if(GUILayout.Button(audioClipPlayers[0].skipForwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							audioClipPlayers[0].skipForward = true;
							for(int audioClipPlayersIndex = 1; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)
								audioClipPlayers[audioClipPlayersIndex].skipForward = true;
							GUI.FocusControl(null);
						}
						if(audioClipPlayers[0].type == AudioClipPlayer.Type.Manual)
						{
							GUI.color = (audioClipPlayers[0].repeat ? Color.green : Color.red) * color;
							if(GUILayout.Button(audioClipPlayers[0].repeatTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								audioClipPlayers[0].repeat = !audioClipPlayers[0].repeat;
								for(int audioClipPlayersIndex = 1; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)if(audioClipPlayers[audioClipPlayersIndex].repeat != audioClipPlayers[0].repeat)
									audioClipPlayers[audioClipPlayersIndex].repeat = audioClipPlayers[0].repeat;
								GUI.FocusControl(null);
							}
							GUI.color = color;
						}
						if(audioClipPlayers[0].type == AudioClipPlayer.Type.Random)
						{
							GUI.color = (audioClipPlayers[0].preventRepeating ? Color.green : Color.red) * color;
							if(GUILayout.Button(audioClipPlayers[0].preventRepeatingTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								audioClipPlayers[0].preventRepeating = !audioClipPlayers[0].preventRepeating;
								for(int audioClipPlayersIndex = 1; audioClipPlayersIndex < audioClipPlayers.Length; audioClipPlayersIndex++)if(audioClipPlayers[audioClipPlayersIndex].preventRepeating != audioClipPlayers[0].preventRepeating)
									audioClipPlayers[audioClipPlayersIndex].preventRepeating = audioClipPlayers[0].preventRepeating;
								GUI.FocusControl(null);
							}
							GUI.color = color;
						}
						GUILayout.FlexibleSpace();
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}
		private void ClipsSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Clips","BoldLabel");
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("type"),true);
				EditorGUIUtility.labelWidth = 0;
				if(!serializedObject.isEditingMultipleObjects)ClipsSectionAudioClipsContainer();
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Audio Clips",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void ClipsSectionAudioClipsContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Audio Clips","Box",GUILayout.ExpandWidth(true)))
					{
						audioClipPlayers[0].audioClipsIsExpanded = !audioClipPlayers[0].audioClipsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = audioClipPlayers[0].audioClips.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						audioClipPlayers[0].audioClips.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(audioClipPlayers[0].audioClipsIsExpanded)
				{
					if(audioClipPlayers[0].audioClips.Count >= 5)audioClipPlayers[0].audioClipsScrollView = EditorGUILayout.BeginScrollView(audioClipPlayers[0].audioClipsScrollView,GUILayout.Height(101));
					else
					{
						if(audioClipPlayers[0].audioClipsScrollView != Vector2.zero)
							audioClipPlayers[0].audioClipsScrollView = Vector2.zero;
						if(audioClipPlayers[0].audioClipsScrollViewIndex != 0)
							audioClipPlayers[0].audioClipsScrollViewIndex = 0;
					}
					if(audioClipPlayers[0].audioClipsScrollViewIndex > 0)GUILayout.Space(audioClipPlayers[0].audioClipsScrollViewIndex * 26);
					for(int a = audioClipPlayers[0].audioClipsScrollViewIndex; a <= Mathf.Clamp(audioClipPlayers[0].audioClipsScrollViewIndex + 4,0,audioClipPlayers[0].audioClips.Count - 1); a++)
					{
						EditorGUILayout.BeginHorizontal("Box");
						{
							EditorGUILayout.BeginHorizontal("Box");
							GUILayout.Box(a.ToString("000"),new GUIStyle() {fontSize = 8},GUILayout.ExpandWidth(false));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClips").GetArrayElementAtIndex(a),GUIContent.none,true);
							GUI.enabled = a != 0;
							if(GUILayout.Button("▲",GUILayout.Width(16),GUILayout.Height(16)))
							{
								AudioClip current = audioClipPlayers[0].audioClips[a];
								AudioClip previous = audioClipPlayers[0].audioClips[a - 1];
								Undo.RecordObject(target,"Inspector");
								audioClipPlayers[0].audioClips[a] = previous;
								audioClipPlayers[0].audioClips[a - 1] = current;
								if(Application.isPlaying && audioClipPlayers[0].source)
								{
									if(audioClipPlayers[0].source.clip == audioClipPlayers[0].audioClips[a])audioClipPlayers[0].index = a;
									if(audioClipPlayers[0].source.clip == audioClipPlayers[0].audioClips[a - 1])audioClipPlayers[0].index = a - 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = a != audioClipPlayers[0].audioClips.Count - 1;
							if(GUILayout.Button("▼",GUILayout.Width(16),GUILayout.Height(16)))
							{
								AudioClip current = audioClipPlayers[0].audioClips[a];
								AudioClip next = audioClipPlayers[0].audioClips[a + 1];
								Undo.RecordObject(target,"Inspector");
								audioClipPlayers[0].audioClips[a] = next;
								audioClipPlayers[0].audioClips[a + 1] = current;
								if(Application.isPlaying && audioClipPlayers[0].source)
								{
									if(audioClipPlayers[0].source.clip == audioClipPlayers[0].audioClips[a])audioClipPlayers[0].index = a;
									if(audioClipPlayers[0].source.clip == audioClipPlayers[0].audioClips[a + 1])audioClipPlayers[0].index = a + 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = true;
							if(GUILayout.Button("-",GUILayout.Width(16),GUILayout.Height(16)))
							{
								Undo.RecordObject(target,"Inspector");
								audioClipPlayers[0].audioClips.RemoveAt(a);
								if(Application.isPlaying && a < audioClipPlayers[0].index)audioClipPlayers[0].index = audioClipPlayers[0].index - 1;
								GUI.FocusControl(null);
								break;
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					if(audioClipPlayers[0].audioClipsScrollViewIndex + 5 < audioClipPlayers[0].audioClips.Count)
						GUILayout.Space((audioClipPlayers[0].audioClips.Count - (audioClipPlayers[0].audioClipsScrollViewIndex + 5)) * 26);
					if(audioClipPlayers[0].audioClips.Count >= 5)
					{
						if(audioClipPlayers[0].audioClipsScrollViewIndex != audioClipPlayers[0].audioClipsScrollView.y / 26 && (Event.current.type == EventType.Repaint && Event.current.type == EventType.ScrollWheel || Event.current.type != EventType.Layout && Event.current.type != EventType.ScrollWheel))
							audioClipPlayers[0].audioClipsScrollViewIndex = (int)audioClipPlayers[0].audioClipsScrollView.y / 26;
						EditorGUILayout.EndScrollView();
					}
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Add a new Audio Clip?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							audioClipPlayers[0].audioClips.Add(null);
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigureSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Configuration","BoldLabel");
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 50;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"),true);
					EditorGUIUtility.labelWidth = 35;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("pitch"),true);
				}
				EditorGUILayout.EndHorizontal();
				ConfigureSectionFadeInContainer();
				ConfigureSectionFadeOutContainer();
				ConfigureSectionSwitchTimeContainer();
				if(!audioClipPlayers[0].repeat)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("onFinish"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("isFinished"),true);
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigureSectionFadeInContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useFadeIn"),GUIContent.none,true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Fade In",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				if(audioClipPlayers[0].useFadeIn)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeIn"),true);
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigureSectionFadeOutContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useFadeOut"),GUIContent.none,true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Fade Out",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				if(audioClipPlayers[0].useFadeOut)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOut"),true);
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigureSectionSwitchTimeContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useSwitchTime"),GUIContent.none,true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Switch Time",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				if(audioClipPlayers[0].useSwitchTime)
				{
					GUI.enabled = audioClipPlayers[0].switchClip;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchTime"),true);
					GUI.enabled = true;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchClip"),true);
				}
			}
			EditorGUILayout.EndVertical();
		}
		private float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum);}
	}
	#endif
}