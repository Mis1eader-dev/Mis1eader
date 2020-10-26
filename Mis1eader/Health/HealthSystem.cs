namespace Mis1eader
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Health/Health System",0),ExecuteInEditMode]
	public class HealthSystem : MonoBehaviour
	{
		[System.Serializable] public class Health
		{
			public string name = string.Empty;
			public float health = 100f;
			public float maximumHealth = 100f;
			public int link = -1;
			public void Update ()
			{
				if(maximumHealth < 0f)maximumHealth = 0f;
				if(health < 0f)health = 0f;
				else if(health > maximumHealth)health = maximumHealth;
			}
			public void SetName (string value) {name = value;}
			public void SetHealth (float value) {health = value;}
			public void DecreaseHealth (float value) {health = health - (value < 0f ? -value : value);}
			public void DecreaseHealthByDeltaTime (float value) {health = health - (value < 0f ? -value : value) * UnityEngine.Time.deltaTime;}
			public void IncreaseHealth (float value) {health = health + (value < 0f ? -value : value);}
			public void IncreaseHealthByDeltaTime (float value) {health = health + (value < 0f ? -value : value) * UnityEngine.Time.deltaTime;}
			public void SetMaximumHealth (float value) {maximumHealth = value;}
			public void DecreaseMaximumHealth (float value) {maximumHealth = maximumHealth - (value < 0f ? -value : value);}
			public void IncreaseMaximumHealth (float value) {maximumHealth = maximumHealth + (value < 0f ? -value : value);}
			public void SetLink (int value) {link = value;}
		}
		public List<Health> healths = new List<Health>();
		private void Update () 
		{
			/*for(int a = 0,A = healths.Count,link = -1; a < A; a++)
			{
				Start:
				healths[a].link = Mathf.Clamp(healths[a].link,-1,A - 1);
				#if UNITY_EDITOR
				if(healths[a].link == a)Debug.LogError("Cannot link a health to itself");
				#endif
				if(healths[a].link != a && healths[link == -1 ? a : link].health < 0 && healths[link == -1 ? a : link].link != -1)
				{
					if(link == -1)link = a;
					healths[healths[a].link].health = healths[healths[a].link].health + healths[a].health;
					healths[a].health = 0;
					a = healths[a].link;
					goto Start;
				}
				else
				{
					healths[a].Update();
					if(link != -1)a = link;
				}
			}*/
			for(int a = 0,A = healths.Count; a < A; a++)
			{
				healths[a].link = Mathf.Clamp(healths[a].link,-1,A - 1);
				if(healths[a].link != a)
				{
					//is linked to a health
					if(healths[a].link != -1)
					{
						if(healths[a].health < 0f)
						{
							healths[healths[a].link].health = healths[healths[a].link].health + healths[a].health;
							healths[a].health = 0f;
						}
					}
					//is not linked to any health
					else healths[a].Update();
				}
				else
				{
					#if UNITY_EDITOR
					Debug.LogError("Cannot link a health to itself");
					#endif
					healths[a].Update();
				}
			}
		}
		[System.NonSerialized] private int healthsPointer = 0;
		public void SetHealthsPointer (int value) {healthsPointer = Mathf.Clamp(value,0,healths.Count - 1);}
		public void SetHealthsPointerHealth (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].SetHealth(value);}
		public void DecreaseHealthsPointerHealth (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].DecreaseHealth(value);}
		public void DecreaseHealthsPointerHealthByDeltaTime (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].DecreaseHealthByDeltaTime(value);}
		public void IncreaseHealthsPointerHealth (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].IncreaseHealth(value);}
		public void IncreaseHealthsPointerHealthByDeltaTime (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].IncreaseHealthByDeltaTime(value);}
		public void SetHealthsPointerMaximumHealth (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].SetMaximumHealth(value);}
		public void DecreaseHealthsPointerMaximumHealth (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].DecreaseMaximumHealth(value);}
		public void IncreaseHealthsPointerMaximumHealth (float value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].IncreaseMaximumHealth(value);}
		public void SetHealthsPointerLink (int value) {if(healthsPointer >= 0 && healthsPointer < healths.Count)healths[healthsPointer].SetLink(value);}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public string healthsName = "Untitled";
		#endif
	}
}