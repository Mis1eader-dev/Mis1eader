namespace Mis1eader.MediaPlayer
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Media/Media Player/Audio Clip Player",0),ExecuteInEditMode]
	public class AudioClipPlayer : MonoBehaviour
	{
		public enum State {SkipBackward = -3,FastBackward = -2,Play = -1,Pause = 0,Stop = 1,FastForward = 2,SkipForward = 3}
		public enum Playback {Playlist,LoopPlaylist,LoopPlayingItem,Shuffle,UniqueShuffle}
		[System.Serializable] public class Fade
		{
			public enum Type : byte {Time,Percentage}
			public Type type = Type.Time;
			public float @in = 0F;
			public float @out = 0F;
			public AudioSource crossfade = null;
			public void Update ()
			{
				if(@in < 0F)@in = 0F;
				else if(@in > 25F)@in = 25F;
				if(@out < 0F)@out = 0F;
				else if(@out > 25F)@out = 25F;
			}
			public float Volume (float time,float length,float volume)
			{
				if(type == Type.Time)
				{
					float @in = this.@in;
					float @out = length - this.@out;
					if(@out - @in < 0F)
					{
						@in = length * 0.5F;
						@out = @in;
					}
					return time < @in ? Library.RangeConversion(time,0F,@in,0F,volume) : (time <= @out ? volume : Library.RangeConversion(time,@out,length,volume,0F));
				}
				else
				{
					float @in = length * this.@in * 0.01F;
					float @out = length * (1F - this.@out * 0.01F);
					return time < @in ? Library.RangeConversion(time,0F,@in,0F,volume) : (time <= @out ? volume : Library.RangeConversion(time,@out,length,volume,0F));
				}
			}
		}
		[System.Serializable] public class Switch
		{
			public float duration = 0F;
			public AudioClip clip = null;
		}
		public AudioSource source = null;
		public State state = State.Play;
		public Playback playback = Playback.LoopPlaylist;
		public int index = -1;
		public List<AudioClip> playlist = new List<AudioClip>();
		public bool mute = false;
		public float volume = 1F;
		public float pitch = 1F;
		public Fade fade = new Fade();
		public Switch @switch = new Switch();
		[HideInInspector] private State lastState = State.Play;
		//[HideInInspector] private AudioClip next = null;
		private int lastIndex = -1;
		private void Awake ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			if((index == 0 || playback != Playback.LoopPlayingItem) && playback != Playback.Shuffle && playback != Playback.UniqueShuffle)
				index = index - 1;
			lastState = state;
		}
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			ExecutionHandler();
		}
		private void ValidationHandler ()
		{
			if(volume < 0F)volume = 0F;
			else if(volume > 1F)volume = 1F;
			if(pitch < -10F)pitch = -10F;
			else if(pitch > 10F)pitch = 10F;
			fade.Update();
			if(index < -1)index = -1;
			else if(playlist.Count != 0) {if(index >= playlist.Count)index = playlist.Count - 1;}
			else if(index > 0)index = 0;
		}
		private void ExecutionHandler ()
		{
			if(source && source.gameObject.activeInHierarchy && source.enabled)
			{
				PlayerHandler();
			}
		}
		private void PlayerHandler ()
		{
			if(state == State.SkipBackward)
			{
				if(playback != Playback.Shuffle && playback != Playback.UniqueShuffle)SkipBackward();
				else
				{
					if(lastState == State.Stop)lastIndex = index;
					Shuffle();
				}
			}
			if(state == State.SkipForward)
			{
				if(playback != Playback.Shuffle && playback != Playback.UniqueShuffle)SkipForward();
				else
				{
					if(lastState == State.Stop)lastIndex = index;
					Shuffle();
				}
			}
			if(state == State.Play)
			{
				if(lastState == State.Pause)source.UnPause();
				if(playlist.Count != 0)
				{
					//if(!source.isPlaying && lastState != State.FastBackward)
					if(!source.isPlaying)
					{
						if((playback == Playback.Playlist || playback == Playback.LoopPlaylist) && (lastState != State.SkipBackward && lastState != State.SkipForward && lastState != State.Stop || index == -1) || playback == Playback.LoopPlayingItem && index == -1)SkipForward();
						else if((playback == Playback.Shuffle || playback == Playback.UniqueShuffle) && (lastIndex != -1 || index == -1))Shuffle();
						if(index != -1)
						{
							source.clip = playlist[index];
							source.Play();
							lastIndex = index;
						}
					}
					else if(lastIndex != index)
					{
						CheckHandler();
						if(index != -1)
						{
							source.clip = playlist[index];
							source.Play();
							lastIndex = index;
						}
					}
				}
			}
			if(state == State.Pause && source.isPlaying)source.Pause();
			if(state == State.Stop && (source.isPlaying || lastState == State.SkipBackward || lastState == State.Pause || lastState == State.SkipForward))
			{
				index = -1;
				lastIndex = -1;
				source.Stop();
			}
			else if(index == -1 && lastIndex != -1)
			{
				state = State.Stop;
				lastIndex = -1;
				source.Stop();
			}
			source.mute = mute;
			source.volume = fade.@in == 0F && fade.@out == 0F ? volume : fade.Volume(source.time,source.clip ? source.clip.length : source.time,volume);
			if(state != State.FastBackward && state != State.FastForward)source.pitch = pitch;
			else
			{
				if(lastState == State.Pause)source.UnPause();
				if(!source.isPlaying)state = State.Play;
				source.pitch = pitch * (int)state;
				//state = State.Play;
			}
			lastState = state;
		}
		//A CALLBACK FROM AudioSource.PlaybackPositionChangeCallback delegate, to handle volume changes, fade ins and outs.
		private void CheckHandler (bool play = false)
		{
			if(index < 0)
			{
				if(playback == Playback.Playlist || playback == Playback.LoopPlayingItem)
				{
					if(lastState != State.Stop)
					{
						index = -1;
						state = State.Stop;
						return;
					}
					else index = playlist.Count + (index + 1) % playlist.Count;
				}
				else if(playback == Playback.LoopPlaylist)
					index = playlist.Count + (lastState != State.Stop ? index : index + 1) % playlist.Count;
			}
			else if(index >= playlist.Count)
			{
				if(playback == Playback.Playlist || playback == Playback.LoopPlayingItem)
				{
					index = -1;
					state = State.Stop;
					return;
				}
				else if(playback == Playback.LoopPlaylist)
					index = index % playlist.Count;
			}
			if(play)state = State.Play;
		}
		public void SkipBackward ()
		{
			index = index - 1;
			CheckHandler(play: true);
			lastState = State.SkipBackward;
		}
		public void FastBackward () {state = State.FastBackward;}
		public void FastBackward (bool fastBackward) {state = fastBackward ? State.FastBackward : State.Play;}
		public void ToggleFastBackward () {state = state != State.FastBackward ? State.FastBackward : State.Play;}
		public void Play () {state = State.Play;}
		public void Play (int index) {this.index = index;state = State.Play;}
		public void Pause () {state = State.Pause;}
		public void Stop () {state = State.Stop;}
		public void TogglePlayPause () {state = state == State.Play ? State.Pause : State.Play;}
		public void TogglePlayStop () {state = state != State.Play ? State.Play : State.Stop;}
		public void FastForward () {state = State.FastForward;}
		public void FastForward (bool fastForward) {state = fastForward ? State.FastForward : State.Play;}
		public void ToggleFastForward () {state = state != State.FastForward ? State.FastForward : State.Play;}
		public void SkipForward ()
		{
			index = index + 1;
			CheckHandler(play: true);
			lastState = State.SkipForward;
		}
		public void Shuffle ()
		{
			if(playback != Playback.UniqueShuffle)
			{
				if(playlist.Count > 1)
				{
					index = Random.Range(0,playlist.Count);
					if(lastIndex == index)lastIndex = -1;
				}
				else
				{
					index = 0;
					lastIndex = -1;
				}
			}
			else
			{
				if(playlist.Count > 1)while(index == lastIndex)
					index = Random.Range(0,playlist.Count);
				else
				{
					index = 0;
					lastIndex = -1;
				}
			}
			state = State.Play;
		}
		public void Shuffle (bool unique)
		{
			if(!unique)
			{
				if(playlist.Count > 1)
				{
					index = Random.Range(0,playlist.Count);
					if(lastIndex == index)lastIndex = -1;
				}
				else
				{
					index = 0;
					lastIndex = -1;
				}
			}
			else
			{
				if(playlist.Count > 1)while(index == lastIndex)
					index = Random.Range(0,playlist.Count);
				else
				{
					index = 0;
					lastIndex = -1;
				}
			}
			state = State.Play;
		}
		public void Seek (float time)
		{
			
		}
		public void Mute () {mute = true;}
		public void UnMute () {mute = false;}
		public void ToggleMute () {mute = !mute;}
		
		
		
		
		
		public void SetMute (bool value) {mute = value;}
		public void SetVolume (float value) {volume = value;}
		public void SetPitch (float value) {pitch = value;}
	}
}