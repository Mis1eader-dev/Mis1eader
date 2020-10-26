namespace Mis1eader.Weapon
{
	using UnityEngine;
	using UnityEngine.Events;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Weapon/Weapon System",0),RequireComponent(typeof(Rigidbody)),ExecuteInEditMode]
	public class WeaponSystem : MonoBehaviour
	{
		public enum Type : byte {FullAutomatic,SemiAutomatic,SingleLikeSpringfield}
		public enum FireRate : byte {ProjectilesPerSecond,ProjectilesPerMinute,Time}
		#if INPUT_MANAGER_NEW
		public enum InputHandling : byte {InputManagerOld,InputSystemPackage}
		public InputHandling inputHandling = InputHandling.InputManagerOld;
		#endif
		public WeaponInput input = null;
		public Type type = Type.FullAutomatic;
		public FireRate fireRate = FireRate.ProjectilesPerSecond;
		public float firingRate = 10F;
		public byte shotsPerFire = 1;
		public bool chamber = true;
		public Transform chamberPoint = null;
		public Firable inChamber = null;
		public WeaponStorage storage = null;
		public UnityEvent onFire = new UnityEvent();
		public UnityEvent onEmpty = new UnityEvent();
		[HideInInspector] private bool trigger = false;
		[HideInInspector] private float fireDuration = 0F;
		[HideInInspector] private float fireCounter = float.MaxValue;
		[HideInInspector] private byte firedShots = 0;
		private void Awake ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
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
			if(!input)
			{
				#if INPUT_MANAGER_NEW
				if(inputHandling == InputHandling.InputManagerOld)
				{
					#else
					input = GetComponent<InputManagerOld>();
					if(!input)input = gameObject.AddComponent<InputManagerOld>();
					#endif
					#if INPUT_MANAGER_NEW
				}
				else
				{
					input = GetComponent<InputManagerNew>();
					if(!input)input = gameObject.AddComponent<InputManagerNew>();
				}
				#endif
			}
			if(!rigidbody || rigidbody.gameObject != gameObject)
			{
				rigidbody = GetComponent<Rigidbody>();
				if(!rigidbody)rigidbody = gameObject.AddComponent<Rigidbody>();
			}
			if(firingRate < 0F)firingRate = 0F;
			if(shotsPerFire < 1)shotsPerFire = 1;
		}
		private void ExecutionHandler ()
		{
			fireDuration = fireRate == FireRate.ProjectilesPerSecond ? 1F / firingRate : (fireRate == FireRate.ProjectilesPerMinute ? 60F / firingRate : firingRate);
			if(fireCounter < fireDuration)fireCounter = fireCounter + Time.deltaTime;
			if(chamber)Chamber();
			if(input)
			{
				input.Handle();
				Fire(input.fireInput);
			}
		}
		private void FireHandler ()
		{
			if(fireCounter >= fireDuration)
			{
				if(inChamber)
				{
					onFire.Invoke();
					//Eject the case, and fire the head (bullet) itself.
					inChamber.Fire();
					inChamber = null;
					Chamber();
					firedShots += 1;
					if(firedShots >= shotsPerFire)firedShots = 0;
					//if(!inChamber)onEmpty.Invoke();
				}
				else
				{
					onEmpty.Invoke();
					firedShots = 0;
				}
				fireCounter = 0F;
			}
		}
		//[RPC]
		public void Fire (bool trigger)
		{
			if(trigger || firedShots > 0)
			{
				if(type == Type.FullAutomatic || type == Type.SemiAutomatic && !this.trigger || firedShots > 0)
				{
					FireHandler();
					this.trigger = true;
				}
			}
			else this.trigger = false;
		}
		//[RPC]
		public void Chamber ()
		{
			if(!inChamber && storage)
			{
				inChamber = storage.PullProjectile(transform);
				if(inChamber && chamberPoint)
				{
					inChamber.transform.position = chamberPoint.position;
					inChamber.transform.rotation = chamberPoint.rotation;
				}
			}
		}
		[HideInInspector] public new Rigidbody rigidbody = null;
	}
}