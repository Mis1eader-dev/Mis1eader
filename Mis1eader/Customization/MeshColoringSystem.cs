namespace Mis1eader.Customization
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Customization/Mesh Coloring System",0),ExecuteInEditMode]
	public class MeshColoringSystem : MonoBehaviour
	{
		public enum UpdateMode : byte {OnAwake,EveryFrame,ViaScripting}
		[System.Serializable] public class MeshColoring
		{
			[System.Serializable] public class Part
			{
				public string name = string.Empty;
				public sbyte index = -1;
				public GameObject group = null;
				public List<Renderer> singles = new List<Renderer>();
				public void SetName (string value) {name = value;}
				public void SetGroup (GameObject value) {group = value;}
				public void SetGroup (Transform value) {group = value.gameObject;}
				public void SetSingles (List<Renderer> value) {singles = value;}
				public void SetSinglesUnlinked (List<Renderer> value) {int A = value.Count;if(singles.Count != A)singles = new List<Renderer>(new Renderer[A]);for(int a = 0; a < A; a++)singles[a] = value[a];}
				public void SetSingles (Renderer[] value) {singles = new List<Renderer>(value);}
				#if UNITY_EDITOR
				[HideInInspector] public Vector2 singlesScrollView = Vector2.zero;
				#endif
			}
			public string name = string.Empty;
			public List<Material> materials = new List<Material>();
			public sbyte index = -1;
			public sbyte preview = -1;
			public sbyte part = -1;
			public List<Part> parts = new List<Part>();
			[HideInInspector] public bool isUpdating = false;
			public void Update (bool run)
			{
				sbyte index = (sbyte)(materials.Count - 1);
				if(this.index < -1)this.index = -1;
				else if(this.index > index)this.index = index;
				if(preview < -1)preview = -1;
				else if(preview > index)preview = index;
				if(parts.Count == 0)part = -1;
				else
				{
					if(part < 0)part = 0;
					else if(part >= parts.Count)part = (sbyte)(parts.Count - 1);
					for(int a = 0,A = parts.Count; a < A; a++)
					{
						if(parts[a].index < -1)parts[a].index = -1;
						else if(parts[a].index > index)parts[a].index = index;
					}
				}
				if(!run || index == -1 || part == -1)return;
				index = preview != -1 ? preview : this.index;
				if(index == -1)return;
				isUpdating = false;
				for(int a = 0,A = parts[part].singles.Count; a < A; a++)
				{
					Renderer renderer = parts[part].singles[a];
					if(!renderer)continue;
					renderer.sharedMaterial = materials[index];
				}
				if(!parts[part].group)return;
				Renderer[] renderers = parts[part].group.GetComponentsInChildren<Renderer>();
				for(int a = 0,A = renderers.Length; a < A; a++)renderers[a].sharedMaterial = materials[index];
			}
			public void SetName (string value) {name = value;}
			public void SetMaterials (List<Material> value) {materials = value;}
			public void SetMaterialsUnlinked (List<Material> value) {int A = value.Count;if(materials.Count != A)materials = new List<Material>(new Material[A]);for(int a = 0; a < A; a++)materials[a] = value[a];}
			public void SetMaterials (Material[] value) {materials = new List<Material>(value);}
			public void SetIndex (sbyte value) {index = value;}
			public void SetPreview (sbyte value) {preview = value;}
			public void DisablePreview () {preview = -1;}
			public void SetPart (sbyte value) {part = value;}
			public void SetParts (List<Part> value) {parts = value;}
			public void SetPartsUnlinked (List<Part> value) {int A = value.Count;if(parts.Count != A)parts = new List<Part>(new Part[A]);for(int a = 0; a < A; a++)parts[a] = value[a];}
			public void SetParts (Part[] value) {parts = new List<Part>(value);}
			#if UNITY_EDITOR
			[HideInInspector] public string partsName = "Untitled";
			[HideInInspector] public Vector2 materialsScrollView = Vector2.zero;
			#endif
		}
		public UpdateMode updateMode = UpdateMode.EveryFrame;
		public List<MeshColoring> meshColorings = new List<MeshColoring>();
		private void Awake ()
		{
			if(
			#if UNITY_EDITOR
			!Application.isPlaying ||
			#endif
			updateMode != UpdateMode.OnAwake)return;
			for(int a = 0,A = meshColorings.Count; a < A; a++)
				meshColorings[a].Update(true);
		}
		private void Update ()
		{
			for(int a = 0,A = meshColorings.Count; a < A; a++)
				meshColorings[a].Update(
				#if UNITY_EDITOR
				(runInEditor || !runInEditor && Application.isPlaying) &&
				#endif
				(updateMode == UpdateMode.EveryFrame || (updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && meshColorings[a].isUpdating));
		}
		public void SetUpdateMode (UpdateMode value) {updateMode = value;}
		public void SetUpdateMode (int value) {updateMode = (UpdateMode)value;}
		public void SetMeshColorings (List<MeshColoring> value) {meshColorings = value;}
		public void SetMeshColoringsUnlinked (List<MeshColoring> value) {int A = value.Count;if(meshColorings.Count != A)meshColorings = new List<MeshColoring>(new MeshColoring[A]);for(int a = 0; a < A; a++)meshColorings[a] = value[a];}
		public void SetMeshColorings (MeshColoring[] value) {meshColorings = new List<MeshColoring>(value);}
		[System.NonSerialized] private int meshColoringsPointer = 0;
		public void SetMeshColoringsPointer (int value) {meshColoringsPointer = Mathf.Clamp(value,0,meshColorings.Count - 1);}
		public void SetMeshColoringsPointerIndex (sbyte value) {if(meshColoringsPointer >= 0 && meshColoringsPointer < meshColorings.Count)meshColorings[meshColoringsPointer].SetIndex(value);}
		public void SetMeshColoringsPointerPreview (sbyte value) {if(meshColoringsPointer >= 0 && meshColoringsPointer < meshColorings.Count)meshColorings[meshColoringsPointer].SetPreview(value);}
		public void SetMeshColoringsPointerPart (sbyte value) {if(meshColoringsPointer >= 0 && meshColoringsPointer < meshColorings.Count)meshColorings[meshColoringsPointer].SetPart(value);}
		public void DisablePreviewAtIndex (int index) {if(index >= 0 && index < meshColorings.Count)meshColorings[index].DisablePreview();}
		public void DisableAllPreviews () {for(int a = 0,A = meshColorings.Count; a < A; a++)meshColorings[a].DisablePreview();}
		public void UpdateMeshColoringAtIndexImmediately (int index) {if(index >= 0 && index < meshColorings.Count)meshColorings[index].Update(true);}
		public void UpdateAllMeshColoringsImmediately () {for(int a = 0,A = meshColorings.Count; a < A; a++)meshColorings[a].Update(true);}
		public void UpdateMeshColoringAtIndexPending (int index) {if((updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting) && index >= 0 && index < meshColorings.Count)meshColorings[index].isUpdating = true;}
		public void UpdateAllMeshColoringsPending () {if(updateMode == UpdateMode.OnAwake || updateMode == UpdateMode.ViaScripting)for(int a = 0,A = meshColorings.Count; a < A; a++)meshColorings[a].isUpdating = true;}
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
		[HideInInspector] public string meshColoringsName = "Untitled";
		#endif
	}
}