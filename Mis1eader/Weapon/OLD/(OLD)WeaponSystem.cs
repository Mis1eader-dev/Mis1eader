namespace AdvancedAssets.Weapon
{
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	[ExecuteInEditMode]
	public class WeaponSystem : MonoBehaviour
	{
		public enum InputType {Tap,Hold}
		//public uint pooledObjects = 30;
		public uint shotsPerFire = 1;
		public float fireRate = 0.1f;
		
		public Transform muzzlePoint = null;
		public GameObject muzzleObject = null;
		
		public uint shots = 30;
		public uint maximumShots = 30;
		public uint storage = 60;
		public uint maximumStorage = 240;
		
		public string fireAxis = "Fire1";
		public string reloadAxis = "Reload";
		
		public InputType fireInput = InputType.Tap;
		
		[HideInInspector] private float fireCounter = 0;
		[HideInInspector] private uint firedShots = 0;
		//[HideInInspector] private uint muzzleIndex = 0;
		[HideInInspector] private bool onReloadInput = false;
		[HideInInspector] private bool isFireInput = false;
		[HideInInspector] private bool isReloading = false;
		//[HideInInspector] private GameObject[] muzzleObjects = new GameObject[0];
		private void Update ()
		{
			EditorHandler();
			if(Application.isPlaying)
			{
				InputHandler();
				ValueHandler();
				if(!isReloading && fireCounter == fireRate)FireHandler();
				if(isReloading && shots < maximumShots)ReloadHandler();
			}
		}
		private void EditorHandler ()
		{
			//pooledObjects = Clamp(pooledObjects,1,uint.MaxValue);
			maximumShots = Clamp(maximumShots,0,uint.MaxValue);
			maximumStorage = Clamp(maximumStorage,0,uint.MaxValue);
			shots = Clamp(shots,0,maximumShots);
			storage = Clamp(storage,0,maximumStorage);
			fireRate = Mathf.Clamp(fireRate,0,float.MaxValue);
			shotsPerFire = Clamp(shotsPerFire,1,maximumShots);
		}
		private void InputHandler ()
		{
			InputHandler(fireAxis,true,fireInput == InputType.Tap,ref isFireInput);
			InputHandler(reloadAxis,true,true,ref onReloadInput);
		}
		private void InputHandler (string axis,bool condition,bool once,ref bool value)
		{
			try
			{
				if(condition && axis != string.Empty)value = !once ? Input.GetButton(axis) : Input.GetButtonDown(axis);
				else value = false;
			}
			catch
			{
				value = false;
				Debug.LogError("The name '" + axis + "' is incorrect or does not exist, you can empty the field if you wish not to use it",this);
			}
		}
		private void ValueHandler ()
		{
			if(onReloadInput && storage > 0 && shots < maximumShots && !isReloading)isReloading = true;
			fireCounter = Mathf.Clamp(fireCounter + Time.deltaTime,0,fireRate);
		}
		private void FireHandler ()
		{
			if(shots > 0 && (isFireInput || firedShots > 0))
			{
				shots--;
				firedShots = (firedShots + 1) % shotsPerFire;
				if(muzzlePoint && muzzleObject)
				{
					Transform muzzle = ((GameObject)Instantiate(muzzleObject,muzzlePoint.position,muzzlePoint.rotation)).transform;
					muzzle.name = muzzleObject.name;
					muzzle.parent = muzzlePoint;
					
					
					//muzzleObjects[muzzleIndex].SetActive(true);
					//muzzleIndex = (muzzleIndex + 1) % pooledObjects;
				}
				fireCounter = 0;
			}
			if(shots == 0 && firedShots > 0)
				firedShots = 0;
		}
		private void ReloadHandler ()
		{
			if(storage > 0)
			{
				uint comparison = Clamp(maximumShots - shots,0,storage);
				shots += Clamp(storage,0,comparison);
				storage -= comparison;
				isReloading = false;
			}
		}
		private void Awake ()
		{
			if(Application.isPlaying)
			{
				fireCounter = fireRate;
				/*if(muzzlePoint && muzzleObject)
				{
					muzzleObjects = new GameObject[pooledObjects];
					for(int a = 0; a < pooledObjects; a++)
					{
						muzzleObjects[a] = (GameObject)Instantiate(muzzleObject,muzzlePoint.position,muzzlePoint.rotation);
						muzzleObjects[a].name = muzzleObject.name;
						muzzleObjects[a].transform.parent = muzzlePoint;
						muzzleObjects[a].SetActive(false);
					}
				}*/
			}
		}
		private uint Clamp (uint value,uint minimum,uint maximum)
		{
			if(value < minimum)value = minimum;
			if(value > maximum)value = maximum;
			return value;
		}
		public void SetIsFireInput (bool value) {if(isFireInput != value)isFireInput = value;}
		public void SetOnReloadInput (bool value) {if(onReloadInput != value)onReloadInput = value;}
		public void Fire ()
		{
			isFireInput = true;
		}
		public void Reload ()
		{
			if(storage > 0 && shots < maximumShots && !isReloading)isReloading = true;
		}
	}
}