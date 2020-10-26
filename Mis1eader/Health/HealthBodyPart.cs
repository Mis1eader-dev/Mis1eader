namespace Mis1eader
{
	using UnityEngine;
	[AddComponentMenu("Mis1eader/Health/Health Body Part",3),ExecuteInEditMode]
	public class HealthBodyPart : MonoBehaviour
	{
		public HealthSystem source = null;
		public int index = 0;
		private void Update ()
		{
			if(index < -1)index = -1;
			else if(source && source.healths.Count != 0) {if(index >= source.healths.Count)index = source.healths.Count - 1;}
			else if(index > 0)index = 0;
		}
		public void SetSource (HealthSystem value) {source = value;}
		public void SetIndex (int value) {index = value;}
		public void SetHealth (float value) {if(source && index != -1 && source.healths[index].health != value)source.healths[index].health = value;}
		public void DecreaseHealth (float value) {if(source && index != -1 && source.healths[index].health > 0)source.healths[index].health = Mathf.Clamp(source.healths[index].health - Mathf.Abs(value),0,source.healths[index].maximumHealth);}
		public void IncreaseHealth (float value) {if(source && index != -1 && source.healths[index].health < source.healths[index].maximumHealth)source.healths[index].health = Mathf.Clamp(source.healths[index].health + Mathf.Abs(value),0,source.healths[index].maximumHealth);}
		public void DecreaseHealthByDeltaTime (float value) {if(source && index != -1 && source.healths[index].health > 0)source.healths[index].health = Mathf.Clamp(source.healths[index].health - Mathf.Abs(value * UnityEngine.Time.deltaTime),0,source.healths[index].maximumHealth);}
		public void IncreaseHealthByDeltaTime (float value) {if(source && index != -1 && source.healths[index].health < source.healths[index].maximumHealth)source.healths[index].health = Mathf.Clamp(source.healths[index].health + Mathf.Abs(value * UnityEngine.Time.deltaTime),0,source.healths[index].maximumHealth);}
		public void SearchForParent ()
		{
			HealthSystem source = null;
			Transform parent = transform;
			while(parent.parent && parent.parent.GetComponent<HealthSystem>())
			{
				parent = parent.parent;
				source = parent.GetComponent<HealthSystem>();
			}
			if(!source)Debug.LogError("No parent was found with a Health System component");
			else this.source = source;
		}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
	}
}