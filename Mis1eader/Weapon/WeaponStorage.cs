namespace Mis1eader.Weapon
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Weapon/Weapon Storage",2),RequireComponent(typeof(Rigidbody)),ExecuteInEditMode]
	public class WeaponStorage : MonoBehaviour
	{
		public int Capacity
		{
			get {return projectiles.Length;}
			set
			{
				int length = projectiles.Length;
				if(length != value)
				{
					Firable[] previous = projectiles;
					projectiles = new Firable[value];
					if(length < value)for(int a = 0; a < length; a++)
						projectiles[a] = previous[a];
					else for(int a = 0; a < value; a++)
						projectiles[a] = previous[a];
				}
			}
		}
		public int inStorage = -1;
		[Tooltip("-2: Undetermined.\n-1: All are visible.\n 0: All are invisible.\n N: N are visible.")]
		public int visibleProjectiles = 3;
		public bool applyPathRotations = false;
		public List<Transform> path = new List<Transform>();
		public Firable insert = null;
		#if UNITY_EDITOR
		public bool insertFromChildren1 = false;
		public bool insertFromChildren2 = false;
		#endif
		[SerializeField] private Firable[] projectiles = new Firable[0];
		[HideInInspector,SerializeField] private int index = 0;
		private void Awake ()
		{
			if(inStorage < 0)
			{
				inStorage = 0;
				for(int a = 0,A = projectiles.Length; a < A; a++)
				{
					Firable projectile = projectiles[a];
					if(!projectile)continue;
					inStorage = inStorage + 1;
				}
			}
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			if(lastVisibleProjectiles != visibleProjectiles)
			{
				if(visibleProjectiles == 0)
				{
					for(int a = 0,A = projectiles.Length; a < A; a++)
					{
						Firable projectile = projectiles[a];
						if(!projectile)continue;
						projectile.gameObject.SetActive(false);
						
					}
				}
				else if(visibleProjectiles > 0)
				{
					for(int a = 0,A = projectiles.Length; a < A; a++)
					{
						Firable projectile = projectiles[a];
						if(!projectile)continue;
						projectile.gameObject.SetActive(a < index + visibleProjectiles);
						
					}
				}
				else for(int a = 0,A = projectiles.Length; a < A; a++)
				{
					Firable projectile = projectiles[a];
					if(!projectile)continue;
					projectile.gameObject.SetActive(true);
					
				}
				lastVisibleProjectiles = visibleProjectiles;
			}
		}
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			if(insert)
			{
				insert.rigidbody.isKinematic = true;
				InsertProjectile(insert);
				insert = null;
			}
			ExecutionHandler();
		}
		private void ValidationHandler ()
		{
			if(!rigidbody || rigidbody.gameObject != gameObject)
			{
				rigidbody = GetComponent<Rigidbody>();
				if(!rigidbody)rigidbody = gameObject.AddComponent<Rigidbody>();
			}
			#if UNITY_EDITOR
			if(insertFromChildren1)
			{
				projectiles = GetComponentsInChildren<WeaponProjectile>();
				index = 0;
				insertFromChildren1 = false;
			}
			if(insertFromChildren2)
			{
				projectiles = GetComponentsInChildren<WeaponProjectileCasing>();
				index = 0;
				insertFromChildren2 = false;
			}
			#endif
		}
		[HideInInspector,SerializeField] private int? lastVisibleProjectiles = null;
		private void ExecutionHandler ()
		{
			if(lastVisibleProjectiles != visibleProjectiles)
			{
				if(visibleProjectiles == 0)
				{
					for(int a = 0,A = projectiles.Length; a < A; a++)
					{
						Firable projectile = projectiles[a];
						if(!projectile)continue;
						projectile.gameObject.SetActive(false);
						
					}
				}
				else if(visibleProjectiles > 0)
				{
					for(int a = 0,A = projectiles.Length; a < A; a++)
					{
						Firable projectile = projectiles[a];
						if(!projectile)continue;
						projectile.gameObject.SetActive(a < index + visibleProjectiles);
						
					}
				}
				else for(int a = 0,A = projectiles.Length; a < A; a++)
				{
					Firable projectile = projectiles[a];
					if(!projectile)continue;
					projectile.gameObject.SetActive(true);
					
				}
				lastVisibleProjectiles = visibleProjectiles;
			}
			else for(int a = 0,A = projectiles.Length; a < A; a++)
			{
				Firable projectile = projectiles[a];
				if(!projectile)continue;
				
			}
		}
		public Firable PullProjectile ()
		{
			if(index < projectiles.Length)
			{
				Firable projectile = projectiles[index];
				projectiles[index] = null;
				if(visibleProjectiles > 0)
				{
					int nextVisible = index + visibleProjectiles;
					if(nextVisible < projectiles.Length)
					{
						Firable nextVisibleProjectile = projectiles[nextVisible];
						if(nextVisibleProjectile)nextVisibleProjectile.gameObject.SetActive(true);
					}
				}
				index = index + 1;
				inStorage = inStorage - 1;
				return projectile;
			}
			return null;
		}
		public Firable PullProjectile (Transform parent)
		{
			if(index < projectiles.Length)
			{
				Firable projectile = projectiles[index];
				projectile.transform.parent = parent;
				projectiles[index] = null;
				if(visibleProjectiles > 0)
				{
					int nextVisible = index + visibleProjectiles;
					if(nextVisible < projectiles.Length)
					{
						Firable nextVisibleProjectile = projectiles[nextVisible];
						if(nextVisibleProjectile)nextVisibleProjectile.gameObject.SetActive(true);
					}
				}
				index = index + 1;
				inStorage = inStorage - 1;
				return projectile;
			}
			return null;
		}
		public bool InsertProjectile (Firable projectile)
		{
			if(projectile && index > 0)
			{
				projectile.transform.parent = transform;
				projectile.transform.SetAsFirstSibling();
				index = index - 1;
				inStorage = inStorage + 1;
				projectiles[index] = projectile;
				if(visibleProjectiles > 0)
				{
					int nextVisible = index + visibleProjectiles;
					if(nextVisible < projectiles.Length)
					{
						Firable nextVisibleProjectile = projectiles[nextVisible];
						if(nextVisibleProjectile)nextVisibleProjectile.gameObject.SetActive(false);
					}
				}
				return true;
			}
			return false;
		}
		public int InsertProjectile (ref Firable[] projectiles)
		{
			if(index > 0)
			{
				int A = projectiles.Length;
				if(visibleProjectiles > 0)for(int a = 0; a < A; a++)
				{
					Firable projectile = projectiles[a];
					if(projectile)
					{
						projectile.gameObject.SetActive(true);
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles[a] = null;
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					int nextVisible = index + visibleProjectiles;
					if(nextVisible < this.projectiles.Length)
					{
						Firable nextVisibleProjectile = this.projectiles[nextVisible];
						if(nextVisibleProjectile)nextVisibleProjectile.gameObject.SetActive(false);
					}
					if(index == 0)return a + 1;
				}
				else if(visibleProjectiles == 0)for(int a = 0; a < A; a++)
				{
					Firable projectile = projectiles[a];
					if(projectile)
					{
						projectile.gameObject.SetActive(false);
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles[a] = null;
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					if(index == 0)return a + 1;
				}
				else if(visibleProjectiles == -1)for(int a = 0; a < A; a++)
				{
					Firable projectile = projectiles[a];
					if(projectile)
					{
						projectile.gameObject.SetActive(true);
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles[a] = null;
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					if(index == 0)return a + 1;
				}
				else for(int a = 0; a < A; a++)
				{
					Firable projectile = projectiles[a];
					if(projectile)
					{
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles[a] = null;
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					if(index == 0)return a + 1;
				}
				return A;
			}
			return 0;
		}
		public void InsertProjectile (ref List<Firable> projectiles)
		{
			if(index > 0)
			{
				if(visibleProjectiles > 0)for(int count = projectiles.Count; count > 0; count--)
				{
					Firable projectile = projectiles[0];
					if(projectile)
					{
						projectile.gameObject.SetActive(true);
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles.RemoveAt(0);
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					int nextVisible = index + visibleProjectiles;
					if(nextVisible < this.projectiles.Length)
					{
						Firable nextVisibleProjectile = this.projectiles[nextVisible];
						if(nextVisibleProjectile)nextVisibleProjectile.gameObject.SetActive(false);
					}
					if(index == 0)return;
				}
				else if(visibleProjectiles == 0)for(int count = projectiles.Count; count > 0; count--)
				{
					Firable projectile = projectiles[0];
					if(projectile)
					{
						projectile.gameObject.SetActive(false);
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles.RemoveAt(0);
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					if(index == 0)return;
				}
				else if(visibleProjectiles == -1)for(int count = projectiles.Count; count > 0; count--)
				{
					Firable projectile = projectiles[0];
					if(projectile)
					{
						projectile.gameObject.SetActive(true);
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles.RemoveAt(0);
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					if(index == 0)return;
				}
				else for(int count = projectiles.Count; count > 0; count--)
				{
					Firable projectile = projectiles[0];
					if(projectile)
					{
						projectile.transform.parent = transform;
						projectile.transform.SetAsFirstSibling();
					}
					projectiles.RemoveAt(0);
					index = index - 1;
					inStorage = inStorage + 1;
					this.projectiles[index] = projectile;
					if(index == 0)return;
				}
			}
		}
		[HideInInspector] public new Rigidbody rigidbody = null;
	}
}