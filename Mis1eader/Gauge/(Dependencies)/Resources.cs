namespace Mis1eader.Gauge
{
	using UnityEngine;
	/*#if UNITY_EDITOR
	using UnityEditor;
	internal class ResourcesAsset
	{
		[MenuItem("Assets/Create/Mis1eader/Gauge/Resources",false,0)]
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
	public class Resources : ScriptableObject {public GameObject tickSpriteRenderer,needleSpriteRenderer,tick2DImage,needle2DImage,tick2DRawImage,needle2DRawImage,tick3DMeshRenderer,needle3DMeshRenderer,text2D,text3D;}
}