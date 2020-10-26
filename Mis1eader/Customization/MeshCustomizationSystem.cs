namespace Mis1eader.Customization
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Customization/Mesh Customization System",1),ExecuteInEditMode]
	public class MeshCustomizationSystem : MonoBehaviour
	{
		public enum UpdateMode : byte {OnAwake,EveryFrame,ViaScripting}
		[System.Serializable] public class MeshCustomization
		{
			[System.Serializable] public class Type
			{
				public string name = string.Empty;
				public GameObject group = null;
				public List<GameObject> singles = new List<GameObject>();
				public void SetName (string value) {name = value;}
				public void SetGroup (GameObject value) {group = value;}
				public void SetGroup (Transform value) {group = value.gameObject;}
				public void SetSingles (List<GameObject> value) {singles = value;}
				public void SetSinglesUnlinked (List<GameObject> value) {int A = value.Count;if(singles.Count != A)singles = new List<GameObject>(new GameObject[A]);for(int a = 0; a < A; a++)singles[a] = value[a];}
				public void SetSingles (GameObject[] value) {singles = new List<GameObject>(value);}
				#if UNITY_EDITOR
				[HideInInspector] public Vector2 singlesScrollView = Vector2.zero;
				#endif
			}
			public string name = string.Empty;
			public sbyte index = -1;
			public sbyte preview = -1;
			public List<Type> types = new List<Type>();
			[HideInInspector] public bool isUpdating = false;
			public void Update (bool run)
			{
				sbyte index = (sbyte)(types.Count - 1);
				if(this.index < -1)this.index = -1;
				else if(this.index > index)this.index = index;
				if(preview < -1)preview = -1;
				else if(preview > index)preview = index;
				if(!run || index == -1)return;
				index = preview != -1 ? preview : this.index;
				if(index == -1)return;
				isUpdating = false;
				for(int a = 0,A = types.Count; a < A; a++)
				{
					for(int b = 0,B = types[a].singles.Count; b < B; b++)
					{
						GameObject gameObject = types[a].singles[b];
						if(!gameObject)continue;
						gameObject.SetActive(index == a);
					}
					if(!types[a].group)continue;
					types[a].group.SetActive(index == a);
				}
			}
			public void SetName (string value) {name = value;}
			public void SetIndex (sbyte value) {index = value;}
			public void SetPreview (sbyte value) {preview = value;}
			public void DisablePreview () {preview = -1;}
			public void SetTypes (List<Type> value) {types = value;}
			public void SetTypesUnlinked (List<Type> value) {int A = value.Count;if(types.Count != A)types = new List<Type>(new Type[A]);for(int a = 0; a < A; a++)types[a] = value[a];}
			public void SetTypes (Type[] value) {types = new List<Type>(value);}
			#if UNITY_EDITOR
			[HideInInspector] public string typesName = "Untitled";
			#endif
		}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		public List<MeshCustomization> meshCustomizations = new List<MeshCustomization>();
		private void Awake ()
		{
			if(
			#if UNITY_EDITOR
			!Application.isPlaying ||
			#endif
			updateMode != UpdateMode.OnAwake)return;
			for(int a = 0,A = meshCustomizations.Count; a < A; a++)
				meshCustomizations[a].Update(true);
		}
		private void Update ()
		{
			for(int a = 0,A = meshCustomizations.Count; a < A; a++)
				meshCustomizations[a].Update(
				#if UNITY_EDITOR
				(runInEditor || !runInEditor && Application.isPlaying) &&
				#endif
				(updateMode == UpdateMode.EveryFrame || (updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && meshCustomizations[a].isUpdating));
		}
		public void SetUpdateMode (UpdateMode value) {updateMode = value;}
		public void SetUpdateMode (int value) {updateMode = (UpdateMode)value;}
		public void SetMeshCustomizations (List<MeshCustomization> value) {meshCustomizations = value;}
		public void SetMeshCustomizationsUnlinked (List<MeshCustomization> value) {int A = value.Count;if(meshCustomizations.Count != A)meshCustomizations = new List<MeshCustomization>(new MeshCustomization[A]);for(int a = 0; a < A; a++)meshCustomizations[a] = value[a];}
		public void SetMeshCustomizations (MeshCustomization[] value) {meshCustomizations = new List<MeshCustomization>(value);}
		[System.NonSerialized] private int meshCustomizationsPointer = 0;
		public void SetMeshCustomizationsPointer (int value) {meshCustomizationsPointer = Mathf.Clamp(value,0,meshCustomizations.Count - 1);}
		public void SetMeshCustomizationsPointerIndex (sbyte value) {if(meshCustomizationsPointer >= 0 && meshCustomizationsPointer < meshCustomizations.Count)meshCustomizations[meshCustomizationsPointer].SetIndex(value);}
		[System.NonSerialized] private int meshCustomizationsPreviewPointer = 0;
		public void SetMeshCustomizationsPreviewPointer (int value) {meshCustomizationsPreviewPointer = Mathf.Clamp(value,0,meshCustomizations.Count - 1);}
		public void SetMeshCustomizationsPreviewPointerIndex (sbyte value) {if(meshCustomizationsPreviewPointer >= 0 && meshCustomizationsPreviewPointer < meshCustomizations.Count)meshCustomizations[meshCustomizationsPreviewPointer].SetPreview(value);}
		public void DisablePreviewAtIndex (int index) {if(index >= 0 && index < meshCustomizations.Count)meshCustomizations[index].DisablePreview();}
		public void DisableAllPreviews () {for(int a = 0,A = meshCustomizations.Count; a < A; a++)meshCustomizations[a].DisablePreview();}
		public void UpdateMeshCustomizationAtIndexImmediately (int index) {if(index >= 0 && index < meshCustomizations.Count)meshCustomizations[index].Update(true);}
		public void UpdateAllMeshCustomizationsImmediately () {for(int a = 0,A = meshCustomizations.Count; a < A; a++)meshCustomizations[a].Update(true);}
		public void UpdateMeshCustomizationAtIndexPending (int index) {if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && index >= 0 && index < meshCustomizations.Count)meshCustomizations[index].isUpdating = true;}
		public void UpdateAllMeshCustomizationsPending () {if(updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting)for(int a = 0,A = meshCustomizations.Count; a < A; a++)meshCustomizations[a].isUpdating = true;}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public bool runInEditor = false;
		[HideInInspector] public string meshCustomizationsName = "Untitled";
		#endif
	}
}