namespace AdvancedAssets
{
	using UnityEngine;
	using UnityEngine.Events;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using System.Collections.Generic;
	[AddComponentMenu("Advanced Assets/Media/Media Player/Audio Source Player",16),ExecuteInEditMode]
	public class AudioSourcePlayer : MonoBehaviour
	{
		public enum Type {Manual,Random}
		public bool skipBackward = false;
		public bool fastBackward = false;
		public bool pause = false;
		public bool fastForward = false;
		public bool skipForward = false;
		public bool repeat = true;
		public bool preventRepeating = true;
		public Type type = Type.Manual;
		public List<AudioSource> audioSources = new List<AudioSource>();
		public float volume = 1;
		public float pitch = 1;
		public bool useFadeIn = false;
		public float fadeIn = 0.01f;
		public bool useFadeOut = false;
		public float fadeOut = 0.99f;
		public bool useSwitchTime = false;
		public float switchTime = 0.4f;
		public AudioSource switchSource = null;
		public UnityEvent onFinish = new UnityEvent();
		public UnityEvent isFinished = new UnityEvent();
		#if UNITY_EDITOR
		[HideInInspector] public bool audioSourcesIsExpanded = true;
		[System.NonSerialized] public Vector2 audioSourcesScrollView = Vector2.zero;
		[System.NonSerialized] public int audioSourcesScrollViewIndex = 0;
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
		[HideInInspector] private bool canGo = true;
		[HideInInspector] private bool canPlay = true;
		[HideInInspector] private bool isPaused = false;
		[HideInInspector] private bool isSwitching = false;
		[HideInInspector] private bool _isFinished = false;
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(Application.isPlaying)
			#endif
			ExecutionHandler();
		}
		private void ValidationHandler ()
		{
			volume = Mathf.Clamp01(volume);
			pitch = Mathf.Clamp(pitch,-10,10);
			fadeIn = Mathf.Clamp(fadeIn,0,0.25f);
			fadeOut = Mathf.Clamp(fadeOut,0.75f,1);
			switchTime = Mathf.Clamp(switchTime,0.1f,float.MaxValue);
			if(pause && (skipBackward || skipForward))pause = false;
			if(index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].clip)
			{
				if(sourceTime != (index >= 0 && index < audioSources.Count && audioSources[index] && !isSwitching ? audioSources[index].time : 0))
					sourceTime = index >= 0 && index < audioSources.Count && audioSources[index] && !isSwitching ? audioSources[index].time : 0;
				if(clipLength != audioSources[index].clip.length)
					clipLength = audioSources[index].clip.length;
				#if UNITY_EDITOR
				if(Application.isPlaying)
				#endif
				FadeHandler();
			}
			else
			{
				if(sourceTime != 0)sourceTime = 0;
				if(clipLength != 1)clipLength = 1;
			}
		}
		private void FadeHandler ()
		{
			if(!isSwitching)
			{
				if((!useFadeIn && !useFadeOut || useFadeIn && useFadeOut && sourceTime > clipLength * fadeIn && sourceTime < clipLength * fadeOut || useFadeIn && !useFadeOut && sourceTime > clipLength * fadeIn || !useFadeIn && useFadeOut && sourceTime < clipLength * fadeOut) && audioSources[index].volume != volume)
					audioSources[index].volume = volume;
				if(useFadeIn && sourceTime <= clipLength * fadeIn && audioSources[index].volume != Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume))
					audioSources[index].volume = Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume);
				if(useFadeOut && sourceTime >= clipLength * fadeOut && audioSources[index].volume != Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume))
					audioSources[index].volume = Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume);
			}
			else if(audioSources[index].volume != volume)
				audioSources[index].volume = volume;
		}
		private void ExecutionHandler ()
		{
			SourceHandler();
			if(isPaused || pause)return;
			SwitchHandler();
			if(type == Type.Manual && repeat && index == -1 && useSwitchTime && switchSource && _isFinished)index = 0;
			if(audioSources.Count > 0 && (index == -1 && useSwitchTime && switchSource || type == Type.Manual && index == -1 && (!useSwitchTime || useSwitchTime && !switchSource) && (!_isFinished || _isFinished && repeat) || index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && !audioSources[index].clip || index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].clip && !audioSources[index].isPlaying || index >= 0 && index < audioSources.Count && !audioSources[index] || index >= 0 && index < audioSources.Count && audioSources[index] && !audioSources[index].gameObject.activeInHierarchy || index >= 0 && index < audioSources.Count && audioSources[index] && !audioSources[index].enabled || skipBackward || skipForward))
			{
				if(index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].isPlaying && !isSwitching)
					audioSources[index].Stop();
				ControlHandler();
				if(useSwitchTime && switchSource && isSwitching && (index < 0 || index >= audioSources.Count))
				{
					isSwitching = false;
					canPlay = true;
				}
				IndexHandler();
				if(useSwitchTime && switchSource && !isSwitching && (index >= 0 && index < audioSources.Count && audioSources[index] || index < 0 || index >= audioSources.Count))
				{
					isSwitching = true;
					canPlay = false;
				}
				if(index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && canPlay && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
				{
					if(audioSources.Count > 1 && preventRepeatingIndex != index)
						preventRepeatingIndex = index;
					if(useSwitchTime && isSwitching)isSwitching = false;
					if(skipBackward)skipBackward = false;
					if(skipForward)skipForward = false;
					audioSources[index].Play();
				}
			}
		}
		private void SourceHandler ()
		{
			bool isActive = index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled;
			if(pause)
			{
				if(isActive && audioSources[index].clip && audioSources[index].isPlaying)audioSources[index].Pause();
				if(useSwitchTime && switchSource && switchSource.isPlaying)switchSource.Pause();
				if(!isPaused)isPaused = true;
			}
			else if(isPaused)
			{
				if(isActive && audioSources[index].clip && !audioSources[index].isPlaying)audioSources[index].Play();
				if(useSwitchTime && switchSource && !switchSource.isPlaying)switchSource.Play();
				isPaused = false;
			}
			if(isActive && audioSources[index].clip && audioSources[index].isPlaying && !pause && !isPaused)
			{
				if(!fastBackward && !fastForward && audioSources[index].pitch != pitch)
					audioSources[index].pitch = pitch;
				if(fastBackward && audioSources[index].pitch != pitch * -2)
					audioSources[index].pitch = pitch * -2;
				if(fastForward && audioSources[index].pitch != pitch * 2)
					audioSources[index].pitch = pitch * 2;
			}
			if(isActive && !audioSources[index].clip)
			{
				if(fastBackward)fastBackward = false;
				if(fastForward)fastForward = false;
			}
		}
		[HideInInspector] private float switchCounter = 0;
		private void SwitchHandler ()
		{
			if(useSwitchTime && switchSource)
			{
				if(isSwitching && switchCounter < switchTime)
				{
					if(index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].isPlaying)
						audioSources[index].Stop();
					if(!switchSource.isPlaying)switchSource.Play();
					if(!switchSource.loop)switchSource.loop = true;
					if(switchCounter < 0)switchCounter = 0;
					if(canPlay)canPlay = false;
					switchCounter = switchCounter + UnityEngine.Time.deltaTime;
				}
				if(switchCounter >= switchTime && (index >= 0 && index < audioSources.Count && audioSources[index] || repeat && index < 0 || repeat && index >= audioSources.Count))
				{
					if(switchSource.isPlaying)switchSource.Stop();
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
			if(fastBackward)fastBackward = false;
			if(fastForward)fastForward = false;
			if(!skipBackward && !skipForward && !isSwitching)
			{
				if(type == Type.Manual && canGo)index = index + 1;
				if(type == Type.Random)
				{
					if(preventRepeating)while(index == preventRepeatingIndex)
						index = Random.Range(0,audioSources.Count);
					else index = Random.Range(0,audioSources.Count);
				}
				return;
			}
			if(skipBackward)
			{
				if(type == Type.Manual && index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].clip)
					skipBackward = false;
				if(index >= 0 && index < audioSources.Count)
				{
					if(type == Type.Manual)index = index - 1;
					if(type == Type.Random)index = Random.Range(0,audioSources.Count);
				}
				if(type == Type.Random && index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].clip && (!preventRepeating || preventRepeating && index != preventRepeatingIndex))
					skipBackward = false;
			}
			if(skipForward)
			{
				if(type == Type.Manual && index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].clip)
					skipForward = false;
				if(index >= 0 && index < audioSources.Count)
				{
					if(type == Type.Manual)index = index + 1;
					if(type == Type.Random)index = Random.Range(0,audioSources.Count);
				}
				if(type == Type.Random && index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].clip && (!preventRepeating || preventRepeating && index != preventRepeatingIndex))
					skipForward = false;
			}
			if(!canGo)canGo = true;
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
						index = audioSources.Count - 1;
						if(audioSources.Count == 1 && preventRepeatingIndex != -1)
							preventRepeatingIndex = -1;
					}
					else
					{
						if(index != -1)index = -1;
						if(preventRepeatingIndex != -1)preventRepeatingIndex = -1;
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
				else index = Random.Range(0,audioSources.Count);
			}
			if(index >= audioSources.Count)
			{
				if(type == Type.Manual)
				{
					if(repeat)
					{
						index = 0;
						if(audioSources.Count == 1 && preventRepeatingIndex != -1)
							preventRepeatingIndex = -1;
					}
					else
					{
						index = -1;
						if(preventRepeatingIndex != -1)preventRepeatingIndex = -1;
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
				else index = Random.Range(0,audioSources.Count);
			}
		}
		private float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum);}
		public void Repeat (bool value) {if(repeat != value)repeat = value;}
		public void PreventRepeating (bool value) {if(preventRepeating != value)preventRepeating = value;}
		public void SetType (Type value) {if(type != value)type = value;}
		public void SetType (int value)
		{
			Type convertedValue = (Type)value;
			if(type != convertedValue)type = convertedValue;
		}
		public void SetAudioSources (List<AudioSource> value)
		{
			int A = value.Count;
			if(audioSources.Count != A)audioSources = new List<AudioSource>(new AudioSource[A]);
			for(int a = 0; a < A; a++)if(audioSources[a] != value[a])audioSources[a] = value[a];
		}
		public void SetAudioSources (AudioSource[] value)
		{
			List<AudioSource> convertedValue = new List<AudioSource>(value);
			if(audioSources != convertedValue)audioSources = convertedValue;
		}
		public void SetVolume (float value) {if(volume != value)volume = value;}
		public void SetPitch (float value) {if(pitch != value)pitch = value;}
		public void UseFadeIn (bool value) {if(useFadeIn != value)useFadeIn = value;}
		public void SetFadeIn (float value) {if(fadeIn != value)fadeIn = value;}
		public void UseFadeOut (bool value) {if(useFadeOut != value)useFadeOut = value;}
		public void SetFadeOut (float value) {if(fadeOut != value)fadeOut = value;}
		public void UseSwitchTime (bool value) {if(useSwitchTime != value)useSwitchTime = value;}
		public void SetSwitchTime (float value) {if(switchTime != value)switchTime = value;}
		public void SetSwitchSource (AudioSource value) {if(switchSource != value)switchSource = value;}
		public void SetOnFinish (UnityEvent value) {if(onFinish != value)onFinish = value;}
		public void SetIsFinished (UnityEvent value) {if(isFinished != value)isFinished = value;}
		public void SetIndex (int value)
		{
			if(index >= 0 && index < audioSources.Count && audioSources[index] && audioSources[index].gameObject.activeInHierarchy && audioSources[index].enabled && audioSources[index].clip && audioSources[index].isPlaying)
				audioSources[index].Stop();
			if(index == value)
			{
				if(useSwitchTime && switchSource)isSwitching = true;
				if(preventRepeatingIndex != -1)preventRepeatingIndex = -1;
			}
			else
			{
				if(type == Type.Manual)canGo = false;
				index = value;
			}
		}
		public void SkipBackward () {if(!skipBackward)skipBackward = true;}
		public void FastBackward () {if(!fastBackward)fastBackward = true;}
		public void Play () {if(pause)pause = false;}
		public void Pause () {if(!pause)pause = true;}
		public void FastForward () {if(!fastForward)fastForward = true;}
		public void SkipForward () {if(!skipForward)skipForward = true;}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(AudioSourcePlayer)),CanEditMultipleObjects]
	internal class AudioSourcePlayerInspector : Editor
	{
		private AudioSourcePlayer[] audioSourcePlayers
		{
			get
			{
				AudioSourcePlayer[] audioSourcePlayers = new AudioSourcePlayer[targets.Length];
				for(int audioSourcePlayersIndex = 0; audioSourcePlayersIndex < targets.Length; audioSourcePlayersIndex++)
					audioSourcePlayers[audioSourcePlayersIndex] = (AudioSourcePlayer)targets[audioSourcePlayersIndex];
				return audioSourcePlayers;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			StatsSection();
			ConsoleSection();
			MainSection();
			SourcesSection();
			ConfigurationSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int audioSourcePlayersIndex = 0; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)
					EditorUtility.SetDirty(audioSourcePlayers[audioSourcePlayersIndex]);
			}
		}
		private void StatsSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("STATS",EditorStyles.boldLabel);
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
						if(audioSourcePlayers[0].gameObject.activeInHierarchy && audioSourcePlayers[0].enabled)
						{
							if(audioSourcePlayers[0].index >= 0 && audioSourcePlayers[0].index < audioSourcePlayers[0].audioSources.Count && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].gameObject.activeInHierarchy && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].enabled && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].clip || audioSourcePlayers[0].repeat && audioSourcePlayers[0].pause && (audioSourcePlayers[0].index < 0 || audioSourcePlayers[0].index >= audioSourcePlayers[0].audioSources.Count))
							{
								GUI.color = audioSourcePlayers[0].pause ? Color.yellow : Color.green;
								GUILayout.Box(audioSourcePlayers[0].pause ? "[Paused]" : "[Playing]",GUILayout.ExpandWidth(true));
							}
							if(audioSourcePlayers[0].audioSources.Count == 0 || audioSourcePlayers[0].index >= 0 && audioSourcePlayers[0].index < audioSourcePlayers[0].audioSources.Count && !audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] || audioSourcePlayers[0].index >= 0 && audioSourcePlayers[0].index < audioSourcePlayers[0].audioSources.Count && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] && !audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].gameObject.activeInHierarchy || audioSourcePlayers[0].index >= 0 && audioSourcePlayers[0].index < audioSourcePlayers[0].audioSources.Count && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] && !audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].enabled || audioSourcePlayers[0].index >= 0 && audioSourcePlayers[0].index < audioSourcePlayers[0].audioSources.Count && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] && !audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].clip)
							{
								GUI.color = Color.red;
								GUILayout.Box("[Not Working]",GUILayout.ExpandWidth(true));
							}
							if(!audioSourcePlayers[0].repeat && audioSourcePlayers[0].audioSources.Count > 0 && (audioSourcePlayers[0].index < 0 || audioSourcePlayers[0].index >= audioSourcePlayers[0].audioSources.Count))
							{
								GUI.color = Color.red;
								GUILayout.Box("[Stopped]",GUILayout.ExpandWidth(true));
							}
						}
						else
						{
							GUI.color = Color.red;
							GUILayout.Box("[Disabled]",GUILayout.ExpandWidth(true));
						}
					}
					if(audioSourcePlayers[0].type == AudioSourcePlayer.Type.Manual)
					{
						GUI.color = audioSourcePlayers[0].repeat ? Color.green : Color.red;
						GUILayout.Box(audioSourcePlayers[0].repeat ? "[Repeats]" : "[Does Not Repeat]",GUILayout.ExpandWidth(true));
					}
					if(audioSourcePlayers[0].type == AudioSourcePlayer.Type.Random)
					{
						GUI.color = audioSourcePlayers[0].preventRepeating ? Color.green : Color.red;
						GUILayout.Box(audioSourcePlayers[0].preventRepeating ? "[Prevents Repeating]" : "[Does Not Prevent Repeating]",GUILayout.ExpandWidth(true));
					}
					GUI.color = color;
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		private void ConsoleSection ()
		{
			bool canCreateSwitchSource = false;
			bool canEnableSwitchSource = false;
			if(serializedObject.isEditingMultipleObjects)for(int audioSourcePlayersIndex = 0; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)
			{
				if(audioSourcePlayers[audioSourcePlayersIndex].useSwitchTime && !audioSourcePlayers[audioSourcePlayersIndex].switchSource)canCreateSwitchSource = true;
				if(audioSourcePlayers[audioSourcePlayersIndex].useSwitchTime && audioSourcePlayers[audioSourcePlayersIndex].gameObject.activeInHierarchy && audioSourcePlayers[audioSourcePlayersIndex].enabled && audioSourcePlayers[audioSourcePlayersIndex].switchSource && !audioSourcePlayers[audioSourcePlayersIndex].switchSource.enabled)canEnableSwitchSource = true;
				if(canCreateSwitchSource && canEnableSwitchSource)break;
			}
			else
			{
				if(audioSourcePlayers[0].useSwitchTime && !audioSourcePlayers[0].switchSource)canCreateSwitchSource = true;
				if(audioSourcePlayers[0].useSwitchTime && audioSourcePlayers[0].gameObject.activeInHierarchy && audioSourcePlayers[0].enabled && audioSourcePlayers[0].switchSource && !audioSourcePlayers[0].switchSource.enabled)canEnableSwitchSource = true;
			}
			if(canCreateSwitchSource)
			{
				EditorGUILayout.BeginVertical("Box");
				{
					GUILayout.Label("Create a recommended Switch Source?");
					if(GUILayout.Button("Create"))
					{
						for(int audioSourcePlayersIndex = 0; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)if(!audioSourcePlayers[audioSourcePlayersIndex].switchSource)
						{
							Undo.RecordObject(target,"Inspector");
							AudioSource audioSource = new GameObject("Switch Source",typeof(AudioSource)).GetComponent<AudioSource>();
							audioSource.transform.SetParent(audioSourcePlayers[audioSourcePlayersIndex].transform,false);
							audioSource.playOnAwake = false;
							audioSource.priority = 0;
							audioSource.rolloffMode = AudioRolloffMode.Linear;
							audioSource.minDistance = 0;
							audioSource.maxDistance = 10;
							audioSourcePlayers[audioSourcePlayersIndex].switchSource = audioSource;
							Undo.RegisterCreatedObjectUndo(audioSource.gameObject,"Inspector");
						}
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndVertical();
			}
			if(canEnableSwitchSource)
			{
				EditorGUILayout.BeginVertical("Box");
				{
					GUILayout.Label("Enable the Switch Source?");
					if(GUILayout.Button("Enable"))
					{
						for(int audioSourcePlayersIndex = 0; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)if(audioSourcePlayers[audioSourcePlayersIndex].gameObject.activeInHierarchy && audioSourcePlayers[audioSourcePlayersIndex].enabled && audioSourcePlayers[audioSourcePlayersIndex].switchSource && !audioSourcePlayers[audioSourcePlayersIndex].switchSource.enabled)
						{
							Undo.RecordObject(audioSourcePlayers[audioSourcePlayersIndex].switchSource,"Inspector");
							audioSourcePlayers[audioSourcePlayersIndex].switchSource.enabled = true;
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
				GUILayout.Label("MAIN",EditorStyles.boldLabel);
				if(Application.isPlaying && audioSourcePlayers[0].index >= 0 && audioSourcePlayers[0].index < audioSourcePlayers[0].audioSources.Count && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].gameObject.activeInHierarchy && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].enabled && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].clip)
				{
					GUIStyle style = new GUIStyle() {alignment = TextAnchor.MiddleCenter,fontStyle = FontStyle.Bold,fontSize = 10};
					EditorGUILayout.BeginVertical("Box");
					{
						GUILayout.Label("Playing",style);
						style.fontSize = 14;
						style.wordWrap = true;
						GUILayout.Label(audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].clip.name,style);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.BeginVertical("Box");
				{
					string second = string.Empty;
					string maximumSecond = string.Empty;
					string minute = string.Empty;
					string maximumMinute = string.Empty;
					if(audioSourcePlayers[0].index >= 0 && audioSourcePlayers[0].index < audioSourcePlayers[0].audioSources.Count && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].gameObject.activeInHierarchy && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].enabled && audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index].clip)
					{
						second = Mathf.Floor(audioSourcePlayers[0].sourceTime % 60).ToString("00");
						maximumSecond = Mathf.Floor(audioSourcePlayers[0].clipLength % 60).ToString("00");
						minute = Mathf.Floor(audioSourcePlayers[0].sourceTime / 60).ToString("00");
						maximumMinute = Mathf.Floor(audioSourcePlayers[0].clipLength / 60).ToString("00");
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
							GUILayout.Box(GUIContent.none,GUILayout.Width(RangeConversion(audioSourcePlayers[0].sourceTime,0,audioSourcePlayers[0].clipLength,3,132)),GUILayout.Height(4));
						}
						EditorGUILayout.EndHorizontal();
						GUI.color = color;
						GUILayout.FlexibleSpace();
						GUILayout.Label(maximumMinute + ":" + maximumSecond);
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					{
						Color color = GUI.color;
						GUILayout.FlexibleSpace();
						if(GUILayout.Button(audioSourcePlayers[0].skipBackwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							audioSourcePlayers[0].skipBackward = true;
							for(int audioSourcePlayersIndex = 1; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)
								audioSourcePlayers[audioSourcePlayersIndex].skipBackward = true;
							GUI.FocusControl(null);
						}
						bool fastBackward = audioSourcePlayers[0].fastBackward;
						audioSourcePlayers[0].fastBackward = GUILayout.RepeatButton(audioSourcePlayers[0].fastBackwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int audioSourcePlayersIndex = 1; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)
							audioSourcePlayers[audioSourcePlayersIndex].fastBackward = audioSourcePlayers[0].fastBackward;
						if(!fastBackward && audioSourcePlayers[0].fastBackward)
							GUI.FocusControl(null);
						if(GUILayout.Button(audioSourcePlayers[0].pause ? audioSourcePlayers[0].playTexture : audioSourcePlayers[0].pauseTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							audioSourcePlayers[0].pause = !audioSourcePlayers[0].pause;
							for(int audioSourcePlayersIndex = 1; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)if(audioSourcePlayers[audioSourcePlayersIndex].pause != audioSourcePlayers[0].pause)
								audioSourcePlayers[audioSourcePlayersIndex].pause = audioSourcePlayers[0].pause;
							GUI.FocusControl(null);
						}
						bool fastForward = audioSourcePlayers[0].fastForward;
						audioSourcePlayers[0].fastForward = GUILayout.RepeatButton(audioSourcePlayers[0].fastForwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int audioSourcePlayersIndex = 1; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)
							audioSourcePlayers[audioSourcePlayersIndex].fastForward = audioSourcePlayers[0].fastForward;
						if(!fastForward && audioSourcePlayers[0].fastForward)
							GUI.FocusControl(null);
						if(GUILayout.Button(audioSourcePlayers[0].skipForwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							audioSourcePlayers[0].skipForward = true;
							for(int audioSourcePlayersIndex = 1; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)
								audioSourcePlayers[audioSourcePlayersIndex].skipForward = true;
							GUI.FocusControl(null);
						}
						if(audioSourcePlayers[0].type == AudioSourcePlayer.Type.Manual)
						{
							GUI.color = (audioSourcePlayers[0].repeat ? Color.green : Color.red) * color;
							if(GUILayout.Button(audioSourcePlayers[0].repeatTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								audioSourcePlayers[0].repeat = !audioSourcePlayers[0].repeat;
								for(int audioSourcePlayersIndex = 1; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)if(audioSourcePlayers[audioSourcePlayersIndex].repeat != audioSourcePlayers[0].repeat)
									audioSourcePlayers[audioSourcePlayersIndex].repeat = audioSourcePlayers[0].repeat;
								GUI.FocusControl(null);
							}
							GUI.color = color;
						}
						if(audioSourcePlayers[0].type == AudioSourcePlayer.Type.Random)
						{
							GUI.color = (audioSourcePlayers[0].preventRepeating ? Color.green : Color.red) * color;
							if(GUILayout.Button(audioSourcePlayers[0].preventRepeatingTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								audioSourcePlayers[0].preventRepeating = !audioSourcePlayers[0].preventRepeating;
								for(int audioSourcePlayersIndex = 1; audioSourcePlayersIndex < audioSourcePlayers.Length; audioSourcePlayersIndex++)if(audioSourcePlayers[audioSourcePlayersIndex].preventRepeating != audioSourcePlayers[0].preventRepeating)
									audioSourcePlayers[audioSourcePlayersIndex].preventRepeating = audioSourcePlayers[0].preventRepeating;
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
		private void SourcesSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("SOURCES",EditorStyles.boldLabel);
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("type"),true);
				EditorGUIUtility.labelWidth = 0;
				if(!serializedObject.isEditingMultipleObjects)SourcesSectionAudioSourcesContainer();
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Audio Sources",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void SourcesSectionAudioSourcesContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Audio Sources","Box",GUILayout.ExpandWidth(true)))
					{
						audioSourcePlayers[0].audioSourcesIsExpanded = !audioSourcePlayers[0].audioSourcesIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = audioSourcePlayers[0].audioSources.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						audioSourcePlayers[0].audioSources.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(audioSourcePlayers[0].audioSourcesIsExpanded)
				{
					if(audioSourcePlayers[0].audioSources.Count >= 5)audioSourcePlayers[0].audioSourcesScrollView = EditorGUILayout.BeginScrollView(audioSourcePlayers[0].audioSourcesScrollView,GUILayout.Height(101));
					else
					{
						if(audioSourcePlayers[0].audioSourcesScrollView != Vector2.zero)
							audioSourcePlayers[0].audioSourcesScrollView = Vector2.zero;
						if(audioSourcePlayers[0].audioSourcesScrollViewIndex != 0)
							audioSourcePlayers[0].audioSourcesScrollViewIndex = 0;
					}
					if(audioSourcePlayers[0].audioSourcesScrollViewIndex > 0)GUILayout.Space(audioSourcePlayers[0].audioSourcesScrollViewIndex * 26);
					for(int a = audioSourcePlayers[0].audioSourcesScrollViewIndex; a <= Mathf.Clamp(audioSourcePlayers[0].audioSourcesScrollViewIndex + 4,0,audioSourcePlayers[0].audioSources.Count - 1); a++)
					{
						EditorGUILayout.BeginHorizontal("Box");
						{
							EditorGUILayout.BeginHorizontal("Box");
							GUILayout.Box(a.ToString("000"),new GUIStyle() {fontSize = 8},GUILayout.ExpandWidth(false));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSources").GetArrayElementAtIndex(a),GUIContent.none,true);
							GUI.enabled = a != 0;
							if(GUILayout.Button("▲",GUILayout.Width(16),GUILayout.Height(16)))
							{
								AudioSource currentSource = Application.isPlaying ? audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] : null;
								AudioSource current = audioSourcePlayers[0].audioSources[a];
								AudioSource previous = audioSourcePlayers[0].audioSources[a - 1];
								Undo.RecordObject(target,"Inspector");
								audioSourcePlayers[0].audioSources[a] = previous;
								audioSourcePlayers[0].audioSources[a - 1] = current;
								if(Application.isPlaying && currentSource)
								{
									if(currentSource == audioSourcePlayers[0].audioSources[a])audioSourcePlayers[0].index = a;
									if(currentSource == audioSourcePlayers[0].audioSources[a - 1])audioSourcePlayers[0].index = a - 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = a != audioSourcePlayers[0].audioSources.Count - 1;
							if(GUILayout.Button("▼",GUILayout.Width(16),GUILayout.Height(16)))
							{
								AudioSource currentSource = Application.isPlaying ? audioSourcePlayers[0].audioSources[audioSourcePlayers[0].index] : null;
								AudioSource current = audioSourcePlayers[0].audioSources[a];
								AudioSource next = audioSourcePlayers[0].audioSources[a + 1];
								Undo.RecordObject(target,"Inspector");
								audioSourcePlayers[0].audioSources[a] = next;
								audioSourcePlayers[0].audioSources[a + 1] = current;
								if(Application.isPlaying && currentSource)
								{
									if(currentSource == audioSourcePlayers[0].audioSources[a])audioSourcePlayers[0].index = a;
									if(currentSource == audioSourcePlayers[0].audioSources[a + 1])audioSourcePlayers[0].index = a + 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = audioSourcePlayers[0].audioSources[a];
							if(GUILayout.Button("X",GUILayout.Width(16),GUILayout.Height(16)))
							{
								GameObject audioSource = null;
								if(audioSourcePlayers[0].audioSources[a])audioSource = audioSourcePlayers[0].audioSources[a].gameObject;
								Undo.RecordObject(target,"Inspector");
								if(Application.isPlaying && audioSourcePlayers[0].audioSources[a] && audioSourcePlayers[0].audioSources[a].isPlaying)audioSourcePlayers[0].audioSources[a].Stop();
								audioSourcePlayers[0].audioSources.RemoveAt(a);
								if(audioSource)Undo.DestroyObjectImmediate(audioSource);
								if(Application.isPlaying && a < audioSourcePlayers[0].index)audioSourcePlayers[0].index = audioSourcePlayers[0].index - 1;
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = true;
							if(GUILayout.Button("-",GUILayout.Width(16),GUILayout.Height(16)))
							{
								Undo.RecordObject(target,"Inspector");
								if(Application.isPlaying && audioSourcePlayers[0].audioSources[a] && audioSourcePlayers[0].audioSources[a].isPlaying)audioSourcePlayers[0].audioSources[a].Stop();
								audioSourcePlayers[0].audioSources.RemoveAt(a);
								if(Application.isPlaying && a < audioSourcePlayers[0].index)audioSourcePlayers[0].index = audioSourcePlayers[0].index - 1;
								GUI.FocusControl(null);
								break;
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					if(audioSourcePlayers[0].audioSourcesScrollViewIndex + 5 < audioSourcePlayers[0].audioSources.Count)
						GUILayout.Space((audioSourcePlayers[0].audioSources.Count - (audioSourcePlayers[0].audioSourcesScrollViewIndex + 5)) * 26);
					if(audioSourcePlayers[0].audioSources.Count >= 5)
					{
						if(audioSourcePlayers[0].audioSourcesScrollViewIndex != audioSourcePlayers[0].audioSourcesScrollView.y / 26 && (Event.current.type == EventType.Repaint && Event.current.type == EventType.ScrollWheel || Event.current.type != EventType.Layout && Event.current.type != EventType.ScrollWheel))
							audioSourcePlayers[0].audioSourcesScrollViewIndex = (int)audioSourcePlayers[0].audioSourcesScrollView.y / 26;
						EditorGUILayout.EndScrollView();
					}
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Add a new Audio Source?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							audioSourcePlayers[0].audioSources.Add(null);
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Create a new Audio Source?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							int index = audioSourcePlayers[0].audioSources.Count;
							Undo.RecordObject(target,"Inspector");
							AudioSource audioSource = new GameObject("Audio Source " + (index + 1),typeof(AudioSource)).GetComponent<AudioSource>();
							audioSource.transform.SetParent(audioSourcePlayers[0].transform,false);
							audioSource.playOnAwake = false;
							audioSource.priority = 0;
							audioSource.rolloffMode = AudioRolloffMode.Linear;
							audioSource.minDistance = 0;
							audioSource.maxDistance = 10;
							audioSourcePlayers[0].audioSources.Add(audioSource);
							Undo.RegisterCreatedObjectUndo(audioSource.gameObject,"Inspector");
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigurationSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("CONFIGURATION",EditorStyles.boldLabel);
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 50;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"),true);
					EditorGUIUtility.labelWidth = 35;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("pitch"),true);
				}
				EditorGUILayout.EndHorizontal();
				ConfigurationSectionFadeInContainer();
				ConfigurationSectionFadeOutContainer();
				ConfigurationSectionSwitchTimeContainer();
				if(!audioSourcePlayers[0].repeat)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("onFinish"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("isFinished"),true);
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigurationSectionFadeInContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useFadeIn"),GUIContent.none,true,GUILayout.ExpandWidth(false));
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Fade In",EditorStyles.boldLabel);
				}
				EditorGUILayout.EndHorizontal();
				if(audioSourcePlayers[0].useFadeIn)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeIn"),true);
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigurationSectionFadeOutContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useFadeOut"),GUIContent.none,true,GUILayout.ExpandWidth(false));
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Fade Out",EditorStyles.boldLabel);
				}
				EditorGUILayout.EndHorizontal();
				if(audioSourcePlayers[0].useFadeOut)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOut"),true);
			}
			EditorGUILayout.EndVertical();
		}
		private void ConfigurationSectionSwitchTimeContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useSwitchTime"),GUIContent.none,true,GUILayout.ExpandWidth(false));
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Switch Time",EditorStyles.boldLabel);
				}
				EditorGUILayout.EndHorizontal();
				if(audioSourcePlayers[0].useSwitchTime)
				{
					GUI.enabled = audioSourcePlayers[0].switchSource;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchTime"),true);
					GUI.enabled = true;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchSource"),true);
				}
			}
			EditorGUILayout.EndVertical();
		}
		private float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum);}
	}
	#endif
}