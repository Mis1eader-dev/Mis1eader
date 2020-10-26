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
	[AddComponentMenu("Advanced Assets/Media/Media Player/Video Clip Player",18),ExecuteInEditMode]
	public class VideoClipPlayer : MonoBehaviour
	{
		#if UNITY_5_6_OR_NEWER
		public enum Type {Manual,Random}
		public VideoPlayer source = null;
		public AudioSource _source = null;
		public bool skipBackward = false;
		public bool fastBackward = false;
		public bool pause = false;
		public bool fastForward = false;
		public bool skipForward = false;
		public bool repeat = true;
		public bool preventRepeating = true;
		public Type type = Type.Manual;
		public List<VideoClip> videoClips = new List<VideoClip>();
		public float volume = 1;
		public float speed = 1;
		public bool useFadeIn = false;
		public float fadeIn = 0.01f;
		public bool useFadeOut = false;
		public float fadeOut = 0.99f;
		public bool useSwitchTime = false;
		public float switchTime = 0.4f;
		public VideoClip switchClip = null;
		public UnityEvent onFinish = new UnityEvent();
		public UnityEvent isFinished = new UnityEvent();
		#endif
		#if UNITY_EDITOR
		#if UNITY_5_6_OR_NEWER
		[HideInInspector] public bool videoClipsIsExpanded = true;
		[System.NonSerialized] public Vector2 videoClipsScrollView = Vector2.zero;
		[System.NonSerialized] public int videoClipsScrollViewIndex = 0;
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
		[HideInInspector] private bool canPlay = true;
		[HideInInspector] private bool isPaused = false;
		[HideInInspector] private bool isSwitching = false;
		[HideInInspector] private bool _isFinished = false;
		private void Update ()
		{
			ValidationHandler();
			if(
			#if UNITY_EDITOR
			Application.isPlaying &&
			#endif
			source && source.gameObject.activeInHierarchy && source.enabled)ExecutionHandler();
		}
		private void ValidationHandler ()
		{
			volume = Mathf.Clamp01(volume);
			speed = Mathf.Clamp(speed,-10,10);
			fadeIn = Mathf.Clamp(fadeIn,0,0.25f);
			fadeOut = Mathf.Clamp(fadeOut,0.75f,1);
			switchTime = Mathf.Clamp(switchTime,0.1f,float.MaxValue);
			if(pause && (skipBackward || skipForward))pause = false;
			if(source)
			{
				if(source.audioOutputMode == VideoAudioOutputMode.AudioSource && _source && _source != source.GetTargetAudioSource(0))
					source.SetTargetAudioSource(0,_source);
				if(source.audioOutputMode != VideoAudioOutputMode.AudioSource && source.GetTargetAudioSource(0))
					source.SetTargetAudioSource(0,null);
				if(videoClips.Count > 0 && index >= 0 && index < videoClips.Count && videoClips[index])
				{
					if(sourceTime != (!isSwitching ? source.time : 0))
						sourceTime = !isSwitching ? (float)source.time : 0;
					if(clipLength != videoClips[index].length)
						clipLength = (float)videoClips[index].length;
					if(
					#if UNITY_EDITOR
					Application.isPlaying &&
					#endif
					_source)FadeHandler();
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
				if((!useFadeIn && !useFadeOut || useFadeIn && useFadeOut && sourceTime > clipLength * fadeIn && sourceTime < clipLength * fadeOut || useFadeIn && !useFadeOut && sourceTime > clipLength * fadeIn || !useFadeIn && useFadeOut && sourceTime < clipLength * fadeOut) && _source.volume != volume)
					_source.volume = volume;
				if(useFadeIn && sourceTime <= clipLength * fadeIn && _source.volume != Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume))
					_source.volume = Mathf.Clamp(RangeConversion(sourceTime,0,clipLength * fadeIn,0,volume),0,volume);
				if(useFadeOut && sourceTime >= clipLength * fadeOut && _source.volume != Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume))
					_source.volume = Mathf.Clamp(RangeConversion(sourceTime,clipLength * fadeOut,clipLength,volume,0),0,volume);
			}
			else if(_source.volume != volume)
				_source.volume = volume;
		}
		private void ExecutionHandler ()
		{
			SourceHandler();
			if(isPaused || pause)return;
			SwitchHandler();
			if(videoClips.Count > 0 && (!source.clip || source.clip && !source.isPlaying || index >= 0 && index < videoClips.Count && !videoClips[index] || skipBackward || skipForward))
			{
				if(source.isPlaying && !isSwitching)source.Stop();
				ControlHandler();
				if(useSwitchTime && switchClip && isSwitching && (index < 0 || index >= videoClips.Count))
				{
					isSwitching = false;
					canPlay = true;
				}
				IndexHandler();
				if(useSwitchTime && switchClip && !isSwitching && (index >= 0 && index < videoClips.Count && videoClips[index] || index < 0 || index >= videoClips.Count))
				{
					isSwitching = true;
					canPlay = false;
				}
				if(index >= 0 && index < videoClips.Count && videoClips[index] && canPlay && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
				{
					if(videoClips.Count > 1 && preventRepeatingIndex != index)
						preventRepeatingIndex = index;
					if(useSwitchTime && isSwitching)
					{
						if(source.audioOutputMode == VideoAudioOutputMode.AudioSource && _source && _source != source.GetTargetAudioSource(0))
							source.SetTargetAudioSource(0,_source);
						isSwitching = false;
					}
					if(skipBackward)skipBackward = false;
					if(skipForward)skipForward = false;
					source.clip = videoClips[index];
					source.Play();
				}
			}
		}
		private void SourceHandler ()
		{
			if(index >= 0 && index < videoClips.Count && source.clip != videoClips[index] && !isSwitching)source.clip = videoClips[index];
			if(source.clip)
			{
				if(videoClips.Count == 0)source.clip = null;
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
					if(!fastBackward && !fastForward)
					{
						if(source.playbackSpeed != speed)
							source.playbackSpeed = speed;
						if(_source && _source.pitch != speed)
							_source.pitch = speed;
					}
					if(fastBackward)
					{
						if(source.playbackSpeed != speed * -2)
							source.playbackSpeed = speed * -2;
						if(_source && _source.pitch != speed * -2)
							_source.pitch = speed * -2;
					}
					if(fastForward)
					{
						if(source.playbackSpeed != speed * 2)
							source.playbackSpeed = speed * 2;
						if(_source && _source.pitch != speed * 2)
							_source.pitch = speed * 2;
					}
				}
			}
			if(!source.clip)
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
					if(!source.isLooping)source.isLooping = true;
					if(switchCounter < 0)switchCounter = 0;
					if(canPlay)canPlay = false;
					switchCounter = switchCounter + Time.deltaTime;
				}
				if(switchCounter >= switchTime && (index >= 0 && index < videoClips.Count && videoClips[index] || repeat && index < 0 || repeat && index >= videoClips.Count))
				{
					if(source.clip)
					{
						source.clip = null;
						source.Stop();
					}
					if(source.isLooping)source.isLooping = false;
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
				if(type == Type.Manual && (index == -1 || index >= 0 && index < videoClips.Count))index = index + 1;
				if(type == Type.Random)
				{
					if(preventRepeating)while(index == preventRepeatingIndex)
						index = Random.Range(0,videoClips.Count);
					else index = Random.Range(0,videoClips.Count);
				}
				return;
			}
			if(skipBackward)
			{
				if(index >= 0 && index < videoClips.Count)
				{
					if(type == Type.Manual)index = index - 1;
					if(type == Type.Random)index = Random.Range(0,videoClips.Count);
				}
				if(index >= 0 && index < videoClips.Count && videoClips[index] && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
					skipBackward = false;
			}
			if(skipForward)
			{
				if(index >= 0 && index < videoClips.Count)
				{
					if(type == Type.Manual)index = index + 1;
					if(type == Type.Random)index = Random.Range(0,videoClips.Count);
				}
				if(index >= 0 && index < videoClips.Count && videoClips[index] && (type == Type.Manual || type == Type.Random && !preventRepeating || type == Type.Random && preventRepeating && index != preventRepeatingIndex))
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
						index = videoClips.Count - 1;
						if(videoClips.Count == 1 && preventRepeatingIndex != -1)
							preventRepeatingIndex = -1;
						if(useSwitchTime && videoClips[index])
						{
							if(skipBackward)skipBackward = false;
							if(skipForward)skipForward = false;
						}
					}
					else
					{
						index = videoClips.Count;
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
				else index = Random.Range(0,videoClips.Count);
			}
			if(index >= videoClips.Count)
			{
				if(type == Type.Manual)
				{
					if(repeat)
					{
						index = 0;
						if(videoClips.Count == 1 && preventRepeatingIndex != -1)
							preventRepeatingIndex = -1;
						if(useSwitchTime && videoClips[index])
						{
							if(skipBackward)skipBackward = false;
							if(skipForward)skipForward = false;
						}
					}
					else
					{
						if(index != videoClips.Count)index = videoClips.Count;
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
				else index = Random.Range(0,videoClips.Count);
			}
		}
		private float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum);}
		public void SetSource (VideoPlayer value) {if(source != value)source = value;}
		public void SetSource (AudioSource value) {if(_source != value)_source = value;}
		public void Repeat (bool value) {if(repeat != value)repeat = value;}
		public void PreventRepeating (bool value) {if(preventRepeating != value)preventRepeating = value;}
		public void SetType (Type value) {if(type != value)type = value;}
		public void SetType (int value)
		{
			Type convertedValue = (Type)value;
			if(type != convertedValue)type = convertedValue;
		}
		public void SetVideoClips (List<VideoClip> value)
		{
			int A = value.Count;
			if(videoClips.Count != A)videoClips = new List<VideoClip>(new VideoClip[A]);
			for(int a = 0; a < A; a++)if(videoClips[a] != value[a])videoClips[a] = value[a];
		}
		public void SetVideoClips (VideoClip[] value)
		{
			List<VideoClip> convertedValue = new List<VideoClip>(value);
			if(videoClips != convertedValue)videoClips = convertedValue;
		}
		public void SetVolume (float value) {if(volume != value)volume = value;}
		public void SetSpeed (float value) {if(speed != value)speed = value;}
		public void UseFadeIn (bool value) {if(useFadeIn != value)useFadeIn = value;}
		public void SetFadeIn (float value) {if(fadeIn != value)fadeIn = value;}
		public void UseFadeOut (bool value) {if(useFadeOut != value)useFadeOut = value;}
		public void SetFadeOut (float value) {if(fadeOut != value)fadeOut = value;}
		public void UseSwitchTime (bool value) {if(useSwitchTime != value)useSwitchTime = value;}
		public void SetSwitchTime (float value) {if(switchTime != value)switchTime = value;}
		public void SetSwitchClip (VideoClip value) {if(switchClip != value)switchClip = value;}
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
	[CustomEditor(typeof(VideoClipPlayer)),CanEditMultipleObjects]
	internal class VideoClipPlayerInspector : Editor
	{
		#if UNITY_5_6_OR_NEWER
		private VideoClipPlayer[] videoClipPlayers
		{
			get
			{
				VideoClipPlayer[] videoClipPlayers = new VideoClipPlayer[targets.Length];
				for(int videoClipPlayersIndex = 0; videoClipPlayersIndex < targets.Length; videoClipPlayersIndex++)
					videoClipPlayers[videoClipPlayersIndex] = (VideoClipPlayer)targets[videoClipPlayersIndex];
				return videoClipPlayers;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			StatsSection();
			ConsoleSection();
			MainSection();
			ClipsSection();
			ConfigurationSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int videoClipPlayersIndex = 0; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)
					EditorUtility.SetDirty(videoClipPlayers[videoClipPlayersIndex]);
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
						if(videoClipPlayers[0].gameObject.activeInHierarchy && videoClipPlayers[0].enabled)
						{
							if(videoClipPlayers[0].source && videoClipPlayers[0].source.enabled && (videoClipPlayers[0].index >= 0 && videoClipPlayers[0].index < videoClipPlayers[0].videoClips.Count && videoClipPlayers[0].videoClips[videoClipPlayers[0].index] || videoClipPlayers[0].repeat && videoClipPlayers[0].pause && (videoClipPlayers[0].index < 0 || videoClipPlayers[0].index >= videoClipPlayers[0].videoClips.Count)))
							{
								GUI.color = videoClipPlayers[0].pause ? Color.yellow : Color.green;
								GUILayout.Box(videoClipPlayers[0].pause ? "[Paused]" : "[Playing]",GUILayout.ExpandWidth(true));
							}
							if(!videoClipPlayers[0].source || videoClipPlayers[0].videoClips.Count == 0 || videoClipPlayers[0].index >= 0 && videoClipPlayers[0].index < videoClipPlayers[0].videoClips.Count && !videoClipPlayers[0].videoClips[videoClipPlayers[0].index])
							{
								GUI.color = Color.red;
								GUILayout.Box("[Not Working]",GUILayout.ExpandWidth(true));
							}
							if(videoClipPlayers[0].source && videoClipPlayers[0].source.gameObject.activeInHierarchy && videoClipPlayers[0].source.enabled && !videoClipPlayers[0].repeat && videoClipPlayers[0].videoClips.Count > 0 && (videoClipPlayers[0].index < 0 || videoClipPlayers[0].index >= videoClipPlayers[0].videoClips.Count))
							{
								GUI.color = Color.red;
								GUILayout.Box("[Stopped]",GUILayout.ExpandWidth(true));
							}
						}
						if(!videoClipPlayers[0].gameObject.activeInHierarchy || !videoClipPlayers[0].enabled || videoClipPlayers[0].source && !videoClipPlayers[0].source.gameObject.activeInHierarchy || videoClipPlayers[0].source && !videoClipPlayers[0].source.enabled)
						{
							GUI.color = Color.red;
							GUILayout.Box("[Disabled]",GUILayout.ExpandWidth(true));
						}
					}
					if(videoClipPlayers[0].type == VideoClipPlayer.Type.Manual)
					{
						GUI.color = videoClipPlayers[0].repeat ? Color.green : Color.red;
						GUILayout.Box(videoClipPlayers[0].repeat ? "[Repeats]" : "[Does Not Repeat]",GUILayout.ExpandWidth(true));
					}
					if(videoClipPlayers[0].type == VideoClipPlayer.Type.Random)
					{
						GUI.color = videoClipPlayers[0].preventRepeating ? Color.green : Color.red;
						GUILayout.Box(videoClipPlayers[0].preventRepeating ? "[Prevents Repeating]" : "[Does Not Prevent Repeating]",GUILayout.ExpandWidth(true));
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
			if(serializedObject.isEditingMultipleObjects)for(int videoClipPlayersIndex = 0; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)
			{
				if(!videoClipPlayers[videoClipPlayersIndex].source && !videoClipPlayers[videoClipPlayersIndex].GetComponent<VideoPlayer>() || !videoClipPlayers[videoClipPlayersIndex]._source && !videoClipPlayers[videoClipPlayersIndex].GetComponent<AudioSource>())canCreateSource = true;
				if(videoClipPlayers[videoClipPlayersIndex].gameObject.activeInHierarchy && videoClipPlayers[videoClipPlayersIndex].enabled && (videoClipPlayers[videoClipPlayersIndex].source && !videoClipPlayers[videoClipPlayersIndex].source.enabled || videoClipPlayers[videoClipPlayersIndex]._source && !videoClipPlayers[videoClipPlayersIndex]._source.enabled))canEnableSource = true;
				if(canCreateSource && canEnableSource)break;
			}
			else
			{
				if(!videoClipPlayers[0].source && !videoClipPlayers[0].GetComponent<VideoPlayer>() || !videoClipPlayers[0]._source && !videoClipPlayers[0].GetComponent<AudioSource>())canCreateSource = true;
				if(videoClipPlayers[0].gameObject.activeInHierarchy && videoClipPlayers[0].enabled && (videoClipPlayers[0].source && !videoClipPlayers[0].source.enabled || videoClipPlayers[0]._source && !videoClipPlayers[0]._source.enabled))canEnableSource = true;
			}
			if(canCreateSource)
			{
				EditorGUILayout.BeginVertical("Box");
				{
					GUILayout.Label("Create a recommended source?");
					if(GUILayout.Button("Create"))
					{
						for(int videoClipPlayersIndex = 0; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)if(!videoClipPlayers[videoClipPlayersIndex].source && !videoClipPlayers[videoClipPlayersIndex].GetComponent<VideoPlayer>() || !videoClipPlayers[videoClipPlayersIndex]._source && !videoClipPlayers[videoClipPlayersIndex].GetComponent<AudioSource>())
						{
							if(!videoClipPlayers[videoClipPlayersIndex].source)
							{
								VideoPlayer videoSource = Undo.AddComponent(videoClipPlayers[videoClipPlayersIndex].gameObject,typeof(VideoPlayer)).GetComponent<VideoPlayer>();
								videoSource.playOnAwake = false;
								videoSource.waitForFirstFrame = false;
								if(videoClipPlayers[videoClipPlayersIndex].GetComponent<MeshRenderer>())
									videoSource.renderMode = VideoRenderMode.MaterialOverride;
								else
								{
									videoSource.renderMode = VideoRenderMode.CameraNearPlane;
									videoSource.targetCamera = Camera.main;
								}
								videoSource.aspectRatio = VideoAspectRatio.FitInside;
								videoClipPlayers[videoClipPlayersIndex].source = videoSource;
							}
							if(!videoClipPlayers[videoClipPlayersIndex]._source)
							{
								AudioSource audioSource = Undo.AddComponent(videoClipPlayers[videoClipPlayersIndex].gameObject,typeof(AudioSource)).GetComponent<AudioSource>();
								audioSource.playOnAwake = false;
								audioSource.priority = videoClipPlayersIndex;
								audioSource.rolloffMode = AudioRolloffMode.Linear;
								audioSource.minDistance = 0;
								audioSource.maxDistance = 10;
								videoClipPlayers[videoClipPlayersIndex]._source = audioSource;
								videoClipPlayers[videoClipPlayersIndex].source.SetTargetAudioSource(0,videoClipPlayers[videoClipPlayersIndex]._source);
							}
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
						for(int videoClipPlayersIndex = 0; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)if(videoClipPlayers[videoClipPlayersIndex].gameObject.activeInHierarchy && videoClipPlayers[videoClipPlayersIndex].enabled && (videoClipPlayers[videoClipPlayersIndex].source && !videoClipPlayers[videoClipPlayersIndex].source.enabled || videoClipPlayers[videoClipPlayersIndex]._source && !videoClipPlayers[videoClipPlayersIndex]._source.enabled))
						{
							if(videoClipPlayers[videoClipPlayersIndex].source && !videoClipPlayers[videoClipPlayersIndex].source.enabled)
							{
								Undo.RecordObject(videoClipPlayers[videoClipPlayersIndex].source,"Inspector");
								videoClipPlayers[videoClipPlayersIndex].source.enabled = true;
							}
							if(videoClipPlayers[videoClipPlayersIndex]._source && !videoClipPlayers[videoClipPlayersIndex]._source.enabled)
							{
								Undo.RecordObject(videoClipPlayers[videoClipPlayersIndex]._source,"Inspector");
								videoClipPlayers[videoClipPlayersIndex]._source.enabled = true;
							}
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
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("source"),true);
				if(videoClipPlayers[0].source && videoClipPlayers[0].source.audioOutputMode == VideoAudioOutputMode.AudioSource)
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_source"),true);
				EditorGUIUtility.labelWidth = 0;
				if(Application.isPlaying && videoClipPlayers[0].source && videoClipPlayers[0].source.gameObject.activeInHierarchy && videoClipPlayers[0].source.enabled && videoClipPlayers[0].videoClips.Count > 0 && videoClipPlayers[0].index >= 0 && videoClipPlayers[0].index < videoClipPlayers[0].videoClips.Count && videoClipPlayers[0].videoClips[videoClipPlayers[0].index])
				{
					GUIStyle style = new GUIStyle() {alignment = TextAnchor.MiddleCenter,fontStyle = FontStyle.Bold,fontSize = 10};
					EditorGUILayout.BeginVertical("Box");
					{
						GUILayout.Label("Playing",style);
						style.fontSize = 14;
						style.wordWrap = true;
						GUILayout.Label(videoClipPlayers[0].videoClips[videoClipPlayers[0].index].name,style);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.BeginVertical("Box");
				{
					if(videoClipPlayers[0].source)
					{
						string second = string.Empty;
						string maximumSecond = string.Empty;
						string minute = string.Empty;
						string maximumMinute = string.Empty;
						if(videoClipPlayers[0].videoClips.Count > 0 && videoClipPlayers[0].index >= 0 && videoClipPlayers[0].index < videoClipPlayers[0].videoClips.Count && videoClipPlayers[0].videoClips[videoClipPlayers[0].index])
						{
							second = Mathf.Floor(videoClipPlayers[0].sourceTime % 60).ToString("00");
							maximumSecond = Mathf.Floor(videoClipPlayers[0].clipLength % 60).ToString("00");
							minute = Mathf.Floor(videoClipPlayers[0].sourceTime / 60).ToString("00");
							maximumMinute = Mathf.Floor(videoClipPlayers[0].clipLength / 60).ToString("00");
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
								GUILayout.Box(GUIContent.none,GUILayout.Width(RangeConversion(videoClipPlayers[0].sourceTime,0,videoClipPlayers[0].clipLength,3,132)),GUILayout.Height(4));
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
						if(GUILayout.Button(videoClipPlayers[0].skipBackwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							videoClipPlayers[0].skipBackward = true;
							for(int videoClipPlayersIndex = 1; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)
								videoClipPlayers[videoClipPlayersIndex].skipBackward = true;
							GUI.FocusControl(null);
						}
						GUI.enabled = false;
						bool fastBackward = videoClipPlayers[0].fastBackward;
						videoClipPlayers[0].fastBackward = GUILayout.RepeatButton(videoClipPlayers[0].fastBackwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int videoClipPlayersIndex = 1; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)
							videoClipPlayers[videoClipPlayersIndex].fastBackward = videoClipPlayers[0].fastBackward;
						if(!fastBackward && videoClipPlayers[0].fastBackward)
							GUI.FocusControl(null);
						GUI.enabled = true;
						if(GUILayout.Button(videoClipPlayers[0].pause ? videoClipPlayers[0].playTexture : videoClipPlayers[0].pauseTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							videoClipPlayers[0].pause = !videoClipPlayers[0].pause;
							for(int videoClipPlayersIndex = 1; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)if(videoClipPlayers[videoClipPlayersIndex].pause != videoClipPlayers[0].pause)
								videoClipPlayers[videoClipPlayersIndex].pause = videoClipPlayers[0].pause;
							GUI.FocusControl(null);
						}
						bool fastForward = videoClipPlayers[0].fastForward;
						videoClipPlayers[0].fastForward = GUILayout.RepeatButton(videoClipPlayers[0].fastForwardTexture,GUILayout.Width(34),GUILayout.Height(34));
						for(int videoClipPlayersIndex = 1; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)
							videoClipPlayers[videoClipPlayersIndex].fastForward = videoClipPlayers[0].fastForward;
						if(!fastForward && videoClipPlayers[0].fastForward)
							GUI.FocusControl(null);
						if(GUILayout.Button(videoClipPlayers[0].skipForwardTexture,GUILayout.Width(34),GUILayout.Height(34)))
						{
							videoClipPlayers[0].skipForward = true;
							for(int videoClipPlayersIndex = 1; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)
								videoClipPlayers[videoClipPlayersIndex].skipForward = true;
							GUI.FocusControl(null);
						}
						if(videoClipPlayers[0].type == VideoClipPlayer.Type.Manual)
						{
							GUI.color = (videoClipPlayers[0].repeat ? Color.green : Color.red) * color;
							if(GUILayout.Button(videoClipPlayers[0].repeatTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								videoClipPlayers[0].repeat = !videoClipPlayers[0].repeat;
								for(int videoClipPlayersIndex = 1; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)if(videoClipPlayers[videoClipPlayersIndex].repeat != videoClipPlayers[0].repeat)
									videoClipPlayers[videoClipPlayersIndex].repeat = videoClipPlayers[0].repeat;
								GUI.FocusControl(null);
							}
							GUI.color = color;
						}
						if(videoClipPlayers[0].type == VideoClipPlayer.Type.Random)
						{
							GUI.color = (videoClipPlayers[0].preventRepeating ? Color.green : Color.red) * color;
							if(GUILayout.Button(videoClipPlayers[0].preventRepeatingTexture,GUILayout.Width(34),GUILayout.Height(34)))
							{
								Undo.RecordObject(target,"Inspector");
								videoClipPlayers[0].preventRepeating = !videoClipPlayers[0].preventRepeating;
								for(int videoClipPlayersIndex = 1; videoClipPlayersIndex < videoClipPlayers.Length; videoClipPlayersIndex++)if(videoClipPlayers[videoClipPlayersIndex].preventRepeating != videoClipPlayers[0].preventRepeating)
									videoClipPlayers[videoClipPlayersIndex].preventRepeating = videoClipPlayers[0].preventRepeating;
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
				GUILayout.Label("CLIPS",EditorStyles.boldLabel);
				EditorGUIUtility.labelWidth = 48;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("type"),true);
				EditorGUIUtility.labelWidth = 0;
				if(!serializedObject.isEditingMultipleObjects)ClipsSectionVideoClipsContainer();
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Video Clips",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void ClipsSectionVideoClipsContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Video Clips","Box",GUILayout.ExpandWidth(true)))
					{
						videoClipPlayers[0].videoClipsIsExpanded = !videoClipPlayers[0].videoClipsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = videoClipPlayers[0].videoClips.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						videoClipPlayers[0].videoClips.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(videoClipPlayers[0].videoClipsIsExpanded)
				{
					if(videoClipPlayers[0].videoClips.Count >= 5)videoClipPlayers[0].videoClipsScrollView = EditorGUILayout.BeginScrollView(videoClipPlayers[0].videoClipsScrollView,GUILayout.Height(101));
					else
					{
						if(videoClipPlayers[0].videoClipsScrollView != Vector2.zero)
							videoClipPlayers[0].videoClipsScrollView = Vector2.zero;
						if(videoClipPlayers[0].videoClipsScrollViewIndex != 0)
							videoClipPlayers[0].videoClipsScrollViewIndex = 0;
					}
					if(videoClipPlayers[0].videoClipsScrollViewIndex > 0)GUILayout.Space(videoClipPlayers[0].videoClipsScrollViewIndex * 26);
					for(int a = videoClipPlayers[0].videoClipsScrollViewIndex; a <= Mathf.Clamp(videoClipPlayers[0].videoClipsScrollViewIndex + 4,0,videoClipPlayers[0].videoClips.Count - 1); a++)
					{
						EditorGUILayout.BeginHorizontal("Box");
						{
							EditorGUILayout.BeginHorizontal("Box");
							GUILayout.Box(a.ToString("000"),new GUIStyle() {fontSize = 8},GUILayout.ExpandWidth(false));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("videoClips").GetArrayElementAtIndex(a),GUIContent.none,true);
							GUI.enabled = a != 0;
							if(GUILayout.Button("▲",GUILayout.Width(16),GUILayout.Height(16)))
							{
								VideoClip current = videoClipPlayers[0].videoClips[a];
								VideoClip previous = videoClipPlayers[0].videoClips[a - 1];
								Undo.RecordObject(target,"Inspector");
								videoClipPlayers[0].videoClips[a] = previous;
								videoClipPlayers[0].videoClips[a - 1] = current;
								if(Application.isPlaying && videoClipPlayers[0].source)
								{
									if(videoClipPlayers[0].source.clip == videoClipPlayers[0].videoClips[a])videoClipPlayers[0].index = a;
									if(videoClipPlayers[0].source.clip == videoClipPlayers[0].videoClips[a - 1])videoClipPlayers[0].index = a - 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = a != videoClipPlayers[0].videoClips.Count - 1;
							if(GUILayout.Button("▼",GUILayout.Width(16),GUILayout.Height(16)))
							{
								VideoClip current = videoClipPlayers[0].videoClips[a];
								VideoClip next = videoClipPlayers[0].videoClips[a + 1];
								Undo.RecordObject(target,"Inspector");
								videoClipPlayers[0].videoClips[a] = next;
								videoClipPlayers[0].videoClips[a + 1] = current;
								if(Application.isPlaying && videoClipPlayers[0].source)
								{
									if(videoClipPlayers[0].source.clip == videoClipPlayers[0].videoClips[a])videoClipPlayers[0].index = a;
									if(videoClipPlayers[0].source.clip == videoClipPlayers[0].videoClips[a + 1])videoClipPlayers[0].index = a + 1;
								}
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = true;
							if(GUILayout.Button("-",GUILayout.Width(16),GUILayout.Height(16)))
							{
								Undo.RecordObject(target,"Inspector");
								videoClipPlayers[0].videoClips.RemoveAt(a);
								if(Application.isPlaying && a < videoClipPlayers[0].index)videoClipPlayers[0].index = videoClipPlayers[0].index - 1;
								GUI.FocusControl(null);
								break;
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					if(videoClipPlayers[0].videoClipsScrollViewIndex + 5 < videoClipPlayers[0].videoClips.Count)
						GUILayout.Space((videoClipPlayers[0].videoClips.Count - (videoClipPlayers[0].videoClipsScrollViewIndex + 5)) * 26);
					if(videoClipPlayers[0].videoClips.Count >= 5)
					{
						if(videoClipPlayers[0].videoClipsScrollViewIndex != videoClipPlayers[0].videoClipsScrollView.y / 26 && (Event.current.type == EventType.Repaint && Event.current.type == EventType.ScrollWheel || Event.current.type != EventType.Layout && Event.current.type != EventType.ScrollWheel))
							videoClipPlayers[0].videoClipsScrollViewIndex = (int)videoClipPlayers[0].videoClipsScrollView.y / 26;
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
							videoClipPlayers[0].videoClips.Add(null);
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
				if(!videoClipPlayers[0].repeat)
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
				if(videoClipPlayers[0].useFadeIn)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeIn"),true);
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
				if(videoClipPlayers[0].useFadeOut)EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOut"),true);
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
				if(videoClipPlayers[0].useSwitchTime)
				{
					GUI.enabled = videoClipPlayers[0].switchClip;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchTime"),true);
					GUI.enabled = true;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("switchClip"),true);
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