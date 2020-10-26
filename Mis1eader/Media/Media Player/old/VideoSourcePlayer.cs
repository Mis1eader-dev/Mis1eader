namespace AdvancedAssets
{
	using UnityEngine;
	#if UNITY_5_6_OR_NEWER
	using UnityEngine.Video;
	using UnityEngine.Events;
	#endif
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	#if UNITY_5_6_OR_NEWER
	using System.Collections.Generic;
	#endif
	[AddComponentMenu("Advanced Assets/Media/Media Player/Video Source Player",19),ExecuteInEditMode]
	public class VideoSourcePlayer : MonoBehaviour
	{
		#if UNITY_5_6_OR_NEWER
		public enum Type {Manual,Random}
		public bool skipBackward = false;
		public bool fastBackward = false;
		public bool pause = false;
		public bool fastForward = false;
		public bool skipForward = false;
		public bool repeat = true;
		public bool preventRepeating = true;
		public Type type = Type.Manual;
		public List<VideoPlayer> videoSources = new List<VideoPlayer>();
		public float volume = 1;
		public float speed = 1;
		public bool useFadeIn = false;
		public float fadeIn = 0.01f;
		public bool useFadeOut = false;
		public float fadeOut = 0.99f;
		public bool useSwitchTime = false;
		public float switchTime = 0.4f;
		public VideoPlayer switchSource = null;
		public UnityEvent onFinish = new UnityEvent();
		public UnityEvent isFinished = new UnityEvent();
		#endif
		#if UNITY_EDITOR
		#if UNITY_5_6_OR_NEWER
		[HideInInspector] public bool videoSourcesIsExpanded = true;
		[System.NonSerialized] public Vector2 videoSourcesScrollView = Vector2.zero;
		[System.NonSerialized] public int videoSourcesScrollViewIndex = 0;
		#endif
		[HideInInspector] public Texture2D skipBackwardTexture = null;
		[HideInInspector] public Texture2D fastBackwardTexture = null;
		[HideInInspector] public Texture2D playTexture = null;
		[HideInInspector] public Texture2D pauseTexture = null;
		[HideInInspector] public Texture2D fastForwardTexture = null;
		[HideInInspector] public Texture2D skipForwardTexture = null;
		[HideInInspector] public Texture2D repeatTexture = null;
		[HideInInspector] public Texture2D preventRepeatingTexture = null;
		#endif
		#if UNITY_5_6_OR_NEWER
		[HideInInspector] public int index = -1;
		[HideInInspector] public int preventRepeatingIndex = -1;
		[HideInInspector] public float sourceTime = 0;
		[HideInInspector] public float clipLength = 1;
		[HideInInspector] private bool canGo = true;
		[HideInInspector] private bool canPlay = true;
		[HideInInspector] private bool isPaused = false;
		[HideInInspector] private bool isSwitching = false;
		[HideInInspector] private bool _isFinished = false;
		[HideInInspector] private AudioSource audioSource
		{
			get
			{
				AudioSource audioSource = null;
				if(index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].audioOutputMode == VideoAudioOutputMode.AudioSource && videoSources[index].clip)
					audioSource = videoSources[index].GetTargetAudioSource(0);
				return audioSource;
			}
		}
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
			speed = Mathf.Clamp(speed,-10,10);
			fadeIn = Mathf.Clamp(fadeIn,0,0.25f);
			fadeOut = Mathf.Clamp(fadeOut,0.75f,1);
			switchTime = Mathf.Clamp(switchTime,0.1f,float.MaxValue);
			if(pause && (skipBackward || skipForward))pause = false;
			if(index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].clip)
			{
				if(sourceTime != (index >= 0 && index < videoSources.Count && videoSources[index] && !isSwitching ? videoSources[index].time : 0))
					sourceTime = index >= 0 && index < videoSources.Count && videoSources[index] && !isSwitching ? (float)videoSources[index].time : 0;
				if(clipLength != videoSources[index].clip.length)
					clipLength = (float)videoSources[index].clip.length;
				if(
				#if UNITY_EDITOR
				Application.isPlaying &&
				#endif
				audioSource)FadeHandler();
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
				if((!useFadeIn && !useFadeOut || useFadeIn && useFadeOut && sourceTime > clipLength * fadeIn && sourceTime < clipLength * fadeOut || useFadeIn && !useFadeOut && sourceTime > clipLength * fadeIn || !useFadeIn && useFadeOut && sourceTime < clipLength * fadeOut) && audioSource.volume != volume)
					audioSource.volume = volume;
				if(useFadeIn && sourceTime <= clipLength * fadeIn && audioSource.volume != Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume))
					audioSource.volume = Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume);
				if(useFadeOut && sourceTime >= clipLength * fadeOut && audioSource.volume != Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume))
					audioSource.volume = Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume);
			}
			else if(audioSource.volume != volume)
				audioSource.volume = volume;
		}
		private void ExecutionHandler ()
		{
			SourceHandler();
			if(isPaused || pause)return;
			SwitchHandler();
			if(type == Type.Manual && repeat && index == -1 && useSwitchTime && switchSource && _isFinished)index = 0;
			if(videoSources.Count > 0 && (index == -1 && useSwitchTime && switchSource || type == Type.Manual && index == -1 && (!useSwitchTime || useSwitchTime && !switchSource) && (!_isFinished || _isFinished && repeat) || index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && !videoSources[index].clip || index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].clip && !videoSources[index].isPlaying || index >= 0 && index < videoSources.Count && !videoSources[index] || index >= 0 && index < videoSources.Count && videoSources[index] && !videoSources[index].gameObject.activeInHierarchy || index >= 0 && index < videoSources.Count && videoSources[index] && !videoSources[index].enabled || skipBackward || skipForward))
			{
				if(index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].isPlaying && !isSwitching)
					videoSources[index].Stop();
				ControlHandler();
				if(useSwitchTime && switchSource && isSwitching && (index < 0 || index >= videoSources.Count))
				{
					isSwitching = false;
					canPlay = true;
				}
				IndexHandler();
				if(useSwitchTime && switchSource && !isSwitching && (index >= 0 && index < videoSources.Count && videoSources[index] || index < 0 || index >= videoSources.Count))
				{
					isSwitching = true;
					canPlay = false;
				}
				if(index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && canPlay && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
				{
					if(videoSources.Count > 1 && preventRepeatingIndex != index)
						preventRepeatingIndex = index;
					if(useSwitchTime && isSwitching)isSwitching = false;
					if(skipBackward)skipBackward = false;
					if(skipForward)skipForward = false;
					videoSources[index].Play();
				}
			}
		}
		private void SourceHandler ()
		{
			bool isActive = index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled;
			if(pause)
			{
				if(isActive && videoSources[index].clip && videoSources[index].isPlaying)videoSources[index].Pause();
				if(useSwitchTime && switchSource && switchSource.isPlaying)switchSource.Pause();
				if(!isPaused)isPaused = true;
			}
			else if(isPaused)
			{
				if(isActive && videoSources[index].clip && !videoSources[index].isPlaying)videoSources[index].Play();
				if(useSwitchTime && switchSource && !switchSource.isPlaying)switchSource.Play();
				isPaused = false;
			}
			if(isActive && videoSources[index].clip && videoSources[index].isPlaying && !pause && !isPaused)
			{
				if(!fastBackward && !fastForward)
				{
					if(videoSources[index].playbackSpeed != speed)
						videoSources[index].playbackSpeed = speed;
					if(audioSource && audioSource.pitch != speed)
						audioSource.pitch = speed;
				}
				if(fastBackward)
				{
					if(videoSources[index].playbackSpeed != speed * -2)
						videoSources[index].playbackSpeed = speed * -2;
					if(audioSource && audioSource.pitch != speed * -2)
						audioSource.pitch = speed * -2;
				}
				if(fastForward)
				{
					if(videoSources[index].playbackSpeed != speed * 2)
						videoSources[index].playbackSpeed = speed * 2;
					if(audioSource && audioSource.pitch != speed * 2)
						audioSource.pitch = speed * 2;
				}
			}
			if(isActive && !videoSources[index].clip)
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
					if(index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].isPlaying)
						videoSources[index].Stop();
					if(!switchSource.isPlaying)switchSource.Play();
					if(!switchSource.isLooping)switchSource.isLooping = true;
					if(switchCounter < 0)switchCounter = 0;
					if(canPlay)canPlay = false;
					switchCounter = switchCounter + Time.deltaTime;
				}
				if(switchCounter >= switchTime && (index >= 0 && index < videoSources.Count && videoSources[index] || repeat && index < 0 || repeat && index >= videoSources.Count))
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
						index = Random.Range(0,videoSources.Count);
					else index = Random.Range(0,videoSources.Count);
				}
				return;
			}
			if(skipBackward)
			{
				if(type == Type.Manual && index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].clip)
					skipBackward = false;
				if(index >= 0 && index < videoSources.Count)
				{
					if(type == Type.Manual)index = index - 1;
					if(type == Type.Random)index = Random.Range(0,videoSources.Count);
				}
				if(type == Type.Random && index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].clip && (!preventRepeating || preventRepeating && index != preventRepeatingIndex))
					skipBackward = false;
			}
			if(skipForward)
			{
				if(type == Type.Manual && index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].clip)
					skipForward = false;
				if(index >= 0 && index < videoSources.Count)
				{
					if(type == Type.Manual)index = index + 1;
					if(type == Type.Random)index = Random.Range(0,videoSources.Count);
				}
				if(type == Type.Random && index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].clip && (!preventRepeating || preventRepeating && index != preventRepeatingIndex))
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
						index = videoSources.Count - 1;
						if(videoSources.Count == 1 && preventRepeatingIndex != -1)
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
				else index = Random.Range(0,videoSources.Count);
			}
			if(index >= videoSources.Count)
			{
				if(type == Type.Manual)
				{
					if(repeat)
					{
						index = 0;
						if(videoSources.Count == 1 && preventRepeatingIndex != -1)
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
				else index = Random.Range(0,videoSources.Count);
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
		public void SetVideoSources (List<VideoPlayer> value)
		{
			int A = value.Count;
			if(videoSources.Count != A)videoSources = new List<VideoPlayer>(new VideoPlayer[A]);
			for(int a = 0; a < A; a++)if(videoSources[a] != value[a])videoSources[a] = value[a];
		}
		public void SetVideoSources (VideoPlayer[] value)
		{
			List<VideoPlayer> convertedValue = new List<VideoPlayer>(value);
			if(videoSources != convertedValue)videoSources = convertedValue;
		}
		public void SetVolume (float value) {if(volume != value)volume = value;}
		public void SetSpeed (float value) {if(speed != value)speed = value;}
		public void UseFadeIn (bool value) {if(useFadeIn != value)useFadeIn = value;}
		public void SetFadeIn (float value) {if(fadeIn != value)fadeIn = value;}
		public void UseFadeOut (bool value) {if(useFadeOut != value)useFadeOut = value;}
		public void SetFadeOut (float value) {if(fadeOut != value)fadeOut = value;}
		public void UseSwitchTime (bool value) {if(useSwitchTime != value)useSwitchTime = value;}
		public void SetSwitchTime (float value) {if(switchTime != value)switchTime = value;}
		public void SetSwitchSource (VideoPlayer value) {if(switchSource != value)switchSource = value;}
		public void SetOnFinish (UnityEvent value) {if(onFinish != value)onFinish = value;}
		public void SetIsFinished (UnityEvent value) {if(isFinished != value)isFinished = value;}
		public void SetIndex (int value)
		{
			if(index >= 0 && index < videoSources.Count && videoSources[index] && videoSources[index].gameObject.activeInHierarchy && videoSources[index].enabled && videoSources[index].clip && videoSources[index].isPlaying)
				videoSources[index].Stop();
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
		public void FastBackward ()
		{
			/*if(!fastBackward)fastBackward = true;*/
			#if UNITY_EDITOR
			Debug.LogWarning("Fast backward is not available for the current version of Unity");
			#endif
		}
		public void Play () {if(pause)pause = false;}
		public void Pause () {if(!pause)pause = true;}
		public void FastForward () {if(!fastForward)fastForward = true;}
		public void SkipForward () {if(!skipForward)skipForward = true;}
		#endif
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(VideoSourcePlayer)),CanEditMultipleObjects]
	internal class VideoSourcePlayerInspector : Editor
	{
		#if UNITY_5_6_OR_NEWER
		private VideoSourcePlayer[] videoSourcePlayers
		{
			get
			{
				VideoSourcePlayer[] videoSourcePlayers = new VideoSourcePlayer[targets.Length];
				for(int videoSourcePlayersIndex = 0; videoSourcePlayersIndex < targets.Length; videoSourcePlayersIndex++)
					videoSourcePlayers[videoSourcePlayersIndex] = (VideoSourcePlayer)targets[videoSourcePlayersIndex];
				return videoSourcePlayers;
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
				for(int videoSourcePlayersIndex = 0; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)
					EditorUtility.SetDirty(videoSourcePlayers[videoSourcePlayersIndex]);
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
						if(videoSourcePlayers[0].gameObject.activeInHierarchy && videoSourcePlayers[0].enabled)
						{
							if(videoSourcePlayers[0].index >= 0 && videoSourcePlayers[0].index < videoSourcePlayers[0].videoSources.Count && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].gameObject.activeInHierarchy && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].enabled && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].clip || videoSourcePlayers[0].repeat && videoSourcePlayers[0].pause && (videoSourcePlayers[0].index < 0 || videoSourcePlayers[0].index >= videoSourcePlayers[0].videoSources.Count))
							{
								GUI.color = videoSourcePlayers[0].pause ? Color.yellow : Color.green;
								GUILayout.Box(videoSourcePlayers[0].pause ? "[Paused]" : "[Playing]",GUILayout.ExpandWidth(true));
							}
							if(videoSourcePlayers[0].videoSources.Count == 0 || videoSourcePlayers[0].index >= 0 && videoSourcePlayers[0].index < videoSourcePlayers[0].videoSources.Count && !videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] || videoSourcePlayers[0].index >= 0 && videoSourcePlayers[0].index < videoSourcePlayers[0].videoSources.Count && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] && !videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].gameObject.activeInHierarchy || videoSourcePlayers[0].index >= 0 && videoSourcePlayers[0].index < videoSourcePlayers[0].videoSources.Count && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] && !videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].enabled || videoSourcePlayers[0].index >= 0 && videoSourcePlayers[0].index < videoSourcePlayers[0].videoSources.Count && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] && !videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].clip || videoSourcePlayers[0].repeat && videoSourcePlayers[0].index < 0 || videoSourcePlayers[0].repeat && videoSourcePlayers[0].index >= videoSourcePlayers[0].videoSources.Count)
							{
								GUI.color = Color.red;
								GUILayout.Box("[Not Working]",GUILayout.ExpandWidth(true));
							}
							if(!videoSourcePlayers[0].repeat && videoSourcePlayers[0].videoSources.Count > 0 && (videoSourcePlayers[0].index < 0 || videoSourcePlayers[0].index >= videoSourcePlayers[0].videoSources.Count))
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
					if(videoSourcePlayers[0].type == VideoSourcePlayer.Type.Manual)
					{
						GUI.color = videoSourcePlayers[0].repeat ? Color.green : Color.red;
						GUILayout.Box(videoSourcePlayers[0].repeat ? "[Repeats]" : "[Does Not Repeat]",GUILayout.ExpandWidth(true));
					}
					if(videoSourcePlayers[0].type == VideoSourcePlayer.Type.Random)
					{
						GUI.color = videoSourcePlayers[0].preventRepeating ? Color.green : Color.red;
						GUILayout.Box(videoSourcePlayers[0].preventRepeating ? "[Prevents Repeating]" : "[Does Not Prevent Repeating]",GUILayout.ExpandWidth(true));
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
			if(serializedObject.isEditingMultipleObjects)for(int videoSourcePlayersIndex = 0; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)
			{
				if(videoSourcePlayers[videoSourcePlayersIndex].useSwitchTime && !videoSourcePlayers[videoSourcePlayersIndex].switchSource)canCreateSwitchSource = true;
				if(videoSourcePlayers[videoSourcePlayersIndex].useSwitchTime && videoSourcePlayers[videoSourcePlayersIndex].gameObject.activeInHierarchy && videoSourcePlayers[videoSourcePlayersIndex].enabled && videoSourcePlayers[videoSourcePlayersIndex].switchSource && !videoSourcePlayers[videoSourcePlayersIndex].switchSource.enabled)canEnableSwitchSource = true;
				if(canCreateSwitchSource && canEnableSwitchSource)break;
			}
			else
			{
				if(videoSourcePlayers[0].useSwitchTime && !videoSourcePlayers[0].switchSource)canCreateSwitchSource = true;
				if(videoSourcePlayers[0].useSwitchTime && videoSourcePlayers[0].gameObject.activeInHierarchy && videoSourcePlayers[0].enabled && videoSourcePlayers[0].switchSource && !videoSourcePlayers[0].switchSource.enabled)canEnableSwitchSource = true;
			}
			if(canCreateSwitchSource)
			{
				EditorGUILayout.BeginVertical("Box");
				{
					GUILayout.Label("Create a recommended Switch Source?");
					if(GUILayout.Button("Create"))
					{
						for(int videoSourcePlayersIndex = 0; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)if(!videoSourcePlayers[videoSourcePlayersIndex].switchSource)
						{
							Undo.RecordObject(target,"Inspector");
							VideoPlayer videoSource = new GameObject("Switch Source",typeof(VideoPlayer),typeof(AudioSource)).GetComponent<VideoPlayer>();
							AudioSource audioSource = videoSource.GetComponent<AudioSource>();
							videoSource.transform.SetParent(videoSourcePlayers[videoSourcePlayersIndex].transform,false);
							videoSource.playOnAwake = false;
							videoSource.waitForFirstFrame = false;
							videoSource.renderMode = VideoRenderMode.CameraNearPlane;
							videoSource.targetCamera = Camera.main;
							videoSource.aspectRatio = VideoAspectRatio.FitInside;
							videoSource.SetTargetAudioSource(0,audioSource);
							audioSource.playOnAwake = false;
							audioSource.priority = 0;
							audioSource.rolloffMode = AudioRolloffMode.Linear;
							audioSource.minDistance = 0;
							audioSource.maxDistance = 10;
							videoSourcePlayers[videoSourcePlayersIndex].switchSource = videoSource;
							Undo.RegisterCreatedObjectUndo(videoSource.gameObject,"Inspector");
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
						for(int videoSourcePlayersIndex = 0; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)if(videoSourcePlayers[videoSourcePlayersIndex].gameObject.activeInHierarchy && videoSourcePlayers[videoSourcePlayersIndex].enabled && videoSourcePlayers[videoSourcePlayersIndex].switchSource && !videoSourcePlayers[videoSourcePlayersIndex].switchSource.enabled)
						{
							Undo.RecordObject(videoSourcePlayers[videoSourcePlayersIndex].switchSource,"Inspector");
							videoSourcePlayers[videoSourcePlayersIndex].switchSource.enabled = true;
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
				if(Application.isPlaying && videoSourcePlayers[0].index >= 0 && videoSourcePlayers[0].index < videoSourcePlayers[0].videoSources.Count && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].gameObject.activeInHierarchy && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].enabled && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].clip)
				{
					GUIStyle style = new GUIStyle() {alignment = TextAnchor.MiddleCenter,fontStyle = FontStyle.Bold,fontSize = 10};
					EditorGUILayout.BeginVertical("Box");
					{
						GUILayout.Label("Playing",style);
						style.fontSize = 14;
						style.wordWrap = true;
						GUILayout.Label(videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].clip.name,style);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.BeginVertical("Box");
				{
					string second = string.Empty;
					string maximumSecond = string.Empty;
					string minute = string.Empty;
					string maximumMinute = string.Empty;
					if(videoSourcePlayers[0].index >= 0 && videoSourcePlayers[0].index < videoSourcePlayers[0].videoSources.Count && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].gameObject.activeInHierarchy && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].enabled && videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index].clip)
					{
						second = Mathf.Floor(videoSourcePlayers[0].sourceTime % 60).ToString("00");
						maximumSecond = Mathf.Floor(videoSourcePlayers[0].clipLength % 60).ToString("00");
						minute = Mathf.Floor(videoSourcePlayers[0].sourceTime / 60).ToString("00");
						maximumMinute = Mathf.Floor(videoSourcePlayers[0].clipLength / 60).ToString("00");
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
							GUILayout.Box(GUIContent.none,GUILayout.Width(RangeConversion(videoSourcePlayers[0].sourceTime,0,videoSourcePlayers[0].clipLength,3,132)),GUILayout.Height(4));
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
						if(GUILayout.Button(videoSourcePlayers[0].skipBackwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							videoSourcePlayers[0].skipBackward = true;
							for(int videoSourcePlayersIndex = 1; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)
								videoSourcePlayers[videoSourcePlayersIndex].skipBackward = true;
							GUI.FocusControl(null);
						}
						GUI.enabled = false;
						bool fastBackward = videoSourcePlayers[0].fastBackward;
						videoSourcePlayers[0].fastBackward = GUILayout.RepeatButton(videoSourcePlayers[0].fastBackwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int videoSourcePlayersIndex = 1; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)
							videoSourcePlayers[videoSourcePlayersIndex].fastBackward = videoSourcePlayers[0].fastBackward;
						if(!fastBackward && videoSourcePlayers[0].fastBackward)
							GUI.FocusControl(null);
						GUI.enabled = true;
						if(GUILayout.Button(videoSourcePlayers[0].pause ? videoSourcePlayers[0].playTexture : videoSourcePlayers[0].pauseTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							videoSourcePlayers[0].pause = !videoSourcePlayers[0].pause;
							for(int videoSourcePlayersIndex = 1; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)if(videoSourcePlayers[videoSourcePlayersIndex].pause != videoSourcePlayers[0].pause)
								videoSourcePlayers[videoSourcePlayersIndex].pause = videoSourcePlayers[0].pause;
							GUI.FocusControl(null);
						}
						bool fastForward = videoSourcePlayers[0].fastForward;
						videoSourcePlayers[0].fastForward = GUILayout.RepeatButton(videoSourcePlayers[0].fastForwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int videoSourcePlayersIndex = 1; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)
							videoSourcePlayers[videoSourcePlayersIndex].fastForward = videoSourcePlayers[0].fastForward;
						if(!fastForward && videoSourcePlayers[0].fastForward)
							GUI.FocusControl(null);
						if(GUILayout.Button(videoSourcePlayers[0].skipForwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							videoSourcePlayers[0].skipForward = true;
							for(int videoSourcePlayersIndex = 1; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)
								videoSourcePlayers[videoSourcePlayersIndex].skipForward = true;
							GUI.FocusControl(null);
						}
						if(videoSourcePlayers[0].type == VideoSourcePlayer.Type.Manual)
						{
							GUI.color = (videoSourcePlayers[0].repeat ? Color.green : Color.red) * color;
							if(GUILayout.Button(videoSourcePlayers[0].repeatTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								videoSourcePlayers[0].repeat = !videoSourcePlayers[0].repeat;
								for(int videoSourcePlayersIndex = 1; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)if(videoSourcePlayers[videoSourcePlayersIndex].repeat != videoSourcePlayers[0].repeat)
									videoSourcePlayers[videoSourcePlayersIndex].repeat = videoSourcePlayers[0].repeat;
								GUI.FocusControl(null);
							}
							GUI.color = color;
						}
						if(videoSourcePlayers[0].type == VideoSourcePlayer.Type.Random)
						{
							GUI.color = (videoSourcePlayers[0].preventRepeating ? Color.green : Color.red) * color;
							if(GUILayout.Button(videoSourcePlayers[0].preventRepeatingTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								videoSourcePlayers[0].preventRepeating = !videoSourcePlayers[0].preventRepeating;
								for(int videoSourcePlayersIndex = 1; videoSourcePlayersIndex < videoSourcePlayers.Length; videoSourcePlayersIndex++)if(videoSourcePlayers[videoSourcePlayersIndex].preventRepeating != videoSourcePlayers[0].preventRepeating)
									videoSourcePlayers[videoSourcePlayersIndex].preventRepeating = videoSourcePlayers[0].preventRepeating;
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
				if(!serializedObject.isEditingMultipleObjects)SourcesSectionVideoSourcesContainer();
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Video Sources",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void SourcesSectionVideoSourcesContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Video Sources","Box",GUILayout.ExpandWidth(true)))
					{
						videoSourcePlayers[0].videoSourcesIsExpanded = !videoSourcePlayers[0].videoSourcesIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = videoSourcePlayers[0].videoSources.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						videoSourcePlayers[0].videoSources.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(videoSourcePlayers[0].videoSourcesIsExpanded)
				{
					if(videoSourcePlayers[0].videoSources.Count >= 5)videoSourcePlayers[0].videoSourcesScrollView = EditorGUILayout.BeginScrollView(videoSourcePlayers[0].videoSourcesScrollView,GUILayout.Height(101));
					else
					{
						if(videoSourcePlayers[0].videoSourcesScrollView != Vector2.zero)
							videoSourcePlayers[0].videoSourcesScrollView = Vector2.zero;
						if(videoSourcePlayers[0].videoSourcesScrollViewIndex != 0)
							videoSourcePlayers[0].videoSourcesScrollViewIndex = 0;
					}
					if(videoSourcePlayers[0].videoSourcesScrollViewIndex > 0)GUILayout.Space(videoSourcePlayers[0].videoSourcesScrollViewIndex * 26);
					for(int a = videoSourcePlayers[0].videoSourcesScrollViewIndex; a <= Mathf.Clamp(videoSourcePlayers[0].videoSourcesScrollViewIndex + 4,0,videoSourcePlayers[0].videoSources.Count - 1); a++)
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.BeginHorizontal("Box");
							GUILayout.Box(a.ToString("000"),new GUIStyle() {fontSize = 8},GUILayout.ExpandWidth(false));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("videoSources").GetArrayElementAtIndex(a),GUIContent.none,true);
							GUI.enabled = a != 0;
							if(GUILayout.Button("▲",GUILayout.Width(16),GUILayout.Height(16)))
							{
								VideoPlayer currentSource = Application.isPlaying ? videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] : null;
								VideoPlayer current = videoSourcePlayers[0].videoSources[a];
								VideoPlayer previous = videoSourcePlayers[0].videoSources[a - 1];
								Undo.RecordObject(target,"Inspector");
								videoSourcePlayers[0].videoSources[a] = previous;
								videoSourcePlayers[0].videoSources[a - 1] = current;
								if(Application.isPlaying && currentSource)
								{
									if(currentSource == videoSourcePlayers[0].videoSources[a])videoSourcePlayers[0].index = a;
									if(currentSource == videoSourcePlayers[0].videoSources[a - 1])videoSourcePlayers[0].index = a - 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = a != videoSourcePlayers[0].videoSources.Count - 1;
							if(GUILayout.Button("▼",GUILayout.Width(16),GUILayout.Height(16)))
							{
								VideoPlayer currentSource = Application.isPlaying ? videoSourcePlayers[0].videoSources[videoSourcePlayers[0].index] : null;
								VideoPlayer current = videoSourcePlayers[0].videoSources[a];
								VideoPlayer next = videoSourcePlayers[0].videoSources[a + 1];
								Undo.RecordObject(target,"Inspector");
								videoSourcePlayers[0].videoSources[a] = next;
								videoSourcePlayers[0].videoSources[a + 1] = current;
								if(Application.isPlaying && currentSource)
								{
									if(currentSource == videoSourcePlayers[0].videoSources[a])videoSourcePlayers[0].index = a;
									if(currentSource == videoSourcePlayers[0].videoSources[a + 1])videoSourcePlayers[0].index = a + 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = videoSourcePlayers[0].videoSources[a];
							if(GUILayout.Button("X",GUILayout.Width(16),GUILayout.Height(16)))
							{
								GameObject audioSource = null;
								if(videoSourcePlayers[0].videoSources[a])audioSource = videoSourcePlayers[0].videoSources[a].gameObject;
								Undo.RecordObject(target,"Inspector");
								if(Application.isPlaying && videoSourcePlayers[0].videoSources[a] && videoSourcePlayers[0].videoSources[a].isPlaying)videoSourcePlayers[0].videoSources[a].Stop();
								videoSourcePlayers[0].videoSources.RemoveAt(a);
								if(audioSource)Undo.DestroyObjectImmediate(audioSource);
								if(Application.isPlaying && a < videoSourcePlayers[0].index)videoSourcePlayers[0].index = videoSourcePlayers[0].index - 1;
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = true;
							if(GUILayout.Button("-",GUILayout.Width(16),GUILayout.Height(16)))
							{
								Undo.RecordObject(target,"Inspector");
								if(Application.isPlaying && videoSourcePlayers[0].videoSources[a] && videoSourcePlayers[0].videoSources[a].isPlaying)videoSourcePlayers[0].videoSources[a].Stop();
								videoSourcePlayers[0].videoSources.RemoveAt(a);
								if(Application.isPlaying && a < videoSourcePlayers[0].index)videoSourcePlayers[0].index = videoSourcePlayers[0].index - 1;
								GUI.FocusControl(null);
								break;
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					if(videoSourcePlayers[0].videoSourcesScrollViewIndex + 5 < videoSourcePlayers[0].videoSources.Count)
						GUILayout.Space((videoSourcePlayers[0].videoSources.Count - (videoSourcePlayers[0].videoSourcesScrollViewIndex + 5)) * 26);
					if(videoSourcePlayers[0].videoSources.Count >= 5)
					{
						if(videoSourcePlayers[0].videoSourcesScrollViewIndex != videoSourcePlayers[0].videoSourcesScrollView.y / 26 && (Event.current.type == EventType.Repaint && Event.current.type == EventType.ScrollWheel || Event.current.type != EventType.Layout && Event.current.type != EventType.ScrollWheel))
							videoSourcePlayers[0].videoSourcesScrollViewIndex = (int)videoSourcePlayers[0].videoSourcesScrollView.y / 26;
						EditorGUILayout.EndScrollView();
					}
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Add a new Video Source?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							videoSourcePlayers[0].videoSources.Add(null);
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Create a new Video Source?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							int index = videoSourcePlayers[0].videoSources.Count;
							Undo.RecordObject(target,"Inspector");
							VideoPlayer videoSource = new GameObject("Video Source " + (index + 1),typeof(VideoPlayer),typeof(AudioSource)).GetComponent<VideoPlayer>();
							AudioSource audioSource = videoSource.GetComponent<AudioSource>();
							videoSource.transform.SetParent(videoSourcePlayers[0].transform,false);
							videoSource.playOnAwake = false;
							videoSource.waitForFirstFrame = false;
							videoSource.renderMode = VideoRenderMode.CameraNearPlane;
							videoSource.targetCamera = Camera.main;
							videoSource.aspectRatio = VideoAspectRatio.FitInside;
							videoSource.SetTargetAudioSource(0,audioSource);
							audioSource.playOnAwake = false;
							audioSource.priority = 0;
							audioSource.rolloffMode = AudioRolloffMode.Linear;
							audioSource.minDistance = 0;
							audioSource.maxDistance = 10;
							videoSourcePlayers[0].videoSources.Add(videoSource);
							Undo.RegisterCreatedObjectUndo(videoSource.gameObject,"Inspector");
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
					EditorGUIUtility.labelWidth = 43;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"),true);
				}
				EditorGUILayout.EndHorizontal();
				ConfigurationSectionFadeInContainer();
				ConfigurationSectionFadeOutContainer();
				ConfigurationSectionSwitchTimeContainer();
				if(!videoSourcePlayers[0].repeat)
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
				if(videoSourcePlayers[0].useFadeIn)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeIn"),true);
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
				if(videoSourcePlayers[0].useFadeOut)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOut"),true);
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
				if(videoSourcePlayers[0].useSwitchTime)
				{
					GUI.enabled = videoSourcePlayers[0].switchSource;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchTime"),true);
					GUI.enabled = true;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchSource"),true);
				}
			}
			EditorGUILayout.EndVertical();
		}
		private float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum);}
		#else
		public override void OnInspectorGUI () {GUILayout.Box("(Requires Unity 5.6.0 or higher)",GUILayout.ExpandWidth(true));}
		#endif
	}
	#endif
}