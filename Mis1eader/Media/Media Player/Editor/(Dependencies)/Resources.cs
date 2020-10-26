/*namespace Mis1eader
{
	using UnityEngine;
	using UnityEditor;
	internal static class MediaPlayer
	{
		public static readonly GUIStyle playing = new GUIStyle(EditorContents.label1) {alignment = TextAnchor.MiddleCenter,fontSize = 12,fontStyle = FontStyle.Bold,wordWrap = true},
		title = new GUIStyle(playing) {fontSize = 10};
		private static string path = "Assets/Advanced Assets/Utility/Media Player/Editor/" + (EditorContents.isPro ? "White/" : "Black/");
		public static readonly Texture skipBackward = AssetDatabase.LoadAssetAtPath(path + "Skip Backward.png",typeof(Texture)) as Texture,
		fastBackward = AssetDatabase.LoadAssetAtPath(path + "Fast Backward.png",typeof(Texture)) as Texture,
		play = AssetDatabase.LoadAssetAtPath(path + "Play.png",typeof(Texture)) as Texture,
		pause = AssetDatabase.LoadAssetAtPath(path + "Pause.png",typeof(Texture)) as Texture,
		stop = AssetDatabase.LoadAssetAtPath(path + "Stop.png",typeof(Texture)) as Texture,
		fastForward = AssetDatabase.LoadAssetAtPath(path + "Fast Forward.png",typeof(Texture)) as Texture,
		skipForward = AssetDatabase.LoadAssetAtPath(path + "Skip Forward.png",typeof(Texture)) as Texture,
		loop = AssetDatabase.LoadAssetAtPath(path + "Loop.png",typeof(Texture)) as Texture,
		preventRepeating = AssetDatabase.LoadAssetAtPath(path + "Prevent Repeating.png",typeof(Texture)) as Texture;
	}
}*/
namespace Mis1eader.MediaPlayer
{
	using UnityEngine;
	/*#if UNITY_EDITOR
	using UnityEditor;
	internal class ResourcesAsset
	{
		[MenuItem("Assets/Create/Advanced Assets/Media Player/Resources (Editor)",false,0)]
		private static void Create ()
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Asset","Resources","asset",string.Empty);
			if(path != string.Empty)
			{
				Object asset = ScriptableObject.CreateInstance<Resources>();
				AssetDatabase.CreateAsset(asset,path);
				ProjectWindowUtil.ShowCreatedAsset(asset);
			}
		}
	}
	#endif*/
	public class Resources : ScriptableObject {public Texture skipBackwardBlack,fastBackwardBlack,playBlack,pauseBlack,stopBlack,fastForwardBlack,skipForwardBlack,loopBlack,shuffleBlack,skipBackwardWhite,fastBackwardWhite,playWhite,pauseWhite,stopWhite,fastForwardWhite,skipForwardWhite,loopWhite,shuffleWhite;}
}