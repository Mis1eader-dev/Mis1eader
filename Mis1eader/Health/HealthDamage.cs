namespace Mis1eader
{
	using UnityEngine;
	[AddComponentMenu("Mis1eader/Health/Health Damage",2),ExecuteInEditMode]
	public class HealthDamage : MonoBehaviour
	{
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		/*public HealthSystem source = null;
		public bool useVelocity = false;
		public Rigidbody body = null;
		public bool useVelocityX = false;
		public Velocity velocityX = new Velocity();
		public bool useVelocityY = true;
		public Velocity velocityY = new Velocity();
		public bool useVelocityZ = false;
		public Velocity velocityZ = new Velocity();
		[HideInInspector] private Vector3 lastVelocity = Vector3.zero;
		[System.Serializable] public class Velocity
		{
			public float velocityRange = 1;
			public float minimumVelocity = 5;
			public float maximumVelocity = 15;
			public float minimumDamage = 1;
			public float maximumDamage = 100;
			public void Update ()
			{
				minimumVelocity = Mathf.Clamp(minimumVelocity,0,maximumVelocity);
				maximumVelocity = Mathf.Clamp(maximumVelocity,0,float.MaxValue);
				minimumDamage = Mathf.Clamp(minimumDamage,0,maximumDamage);
				maximumDamage = Mathf.Clamp(maximumDamage,0,float.MaxValue);
			}
			public void SetVelocityRange (float value) {if(velocityRange != value)velocityRange = value;}
			public void SetMinimumVelocity (float value) {if(minimumVelocity != value)minimumVelocity = value;}
			public void SetMaximumVelocity (float value) {if(maximumVelocity != value)maximumVelocity = value;}
			public void SetMinimumDamage (float value) {if(minimumDamage != value)minimumDamage = value;}
			public void SetMaximumDamage (float value) {if(maximumDamage != value)maximumDamage = value;}
		}
		private void Update ()
		{
			ValidationHandler();
			if(
			#if UNITY_EDITOR
			Application.isPlaying &&
			#endif
			source && useVelocity && body && (useVelocityX || useVelocityY || useVelocityZ))
				VelocityHandler();
		}
		private void ValidationHandler ()
		{
			if(!useVelocity)return;
			if(useVelocityX)velocityX.Update();
			if(useVelocityY)velocityY.Update();
			if(useVelocityZ)velocityZ.Update();
		}
		private void VelocityHandler ()
		{
			Vector3 velocity = new Vector3(Mathf.Abs(body.velocity.x),Mathf.Abs(body.velocity.y),Mathf.Abs(body.velocity.z));
			float damage = 0;
			int amount = 0;
			if(useVelocityX && lastVelocity.x > velocityX.minimumVelocity && lastVelocity.x - velocity.x > 0 && lastVelocity.x > velocity.x + velocityX.velocityRange)
			{
				float _velocity = Mathf.Clamp(lastVelocity.x,velocityX.minimumVelocity,velocityX.maximumVelocity);
				float _damage = RangeConversion(_velocity,velocityX.minimumVelocity,velocityX.maximumVelocity,velocityX.minimumDamage,velocityX.maximumDamage);
				damage = damage + _damage;
				amount = amount + 1;
			}
			if(useVelocityY && lastVelocity.y > velocityY.minimumVelocity && lastVelocity.y - velocity.y > 0 && lastVelocity.y > velocity.y + velocityY.velocityRange)
			{
				float _velocity = Mathf.Clamp(lastVelocity.y,velocityY.minimumVelocity,velocityY.maximumVelocity);
				float _damage = RangeConversion(_velocity,velocityY.minimumVelocity,velocityY.maximumVelocity,velocityY.minimumDamage,velocityY.maximumDamage);
				damage = damage + _damage;
				amount = amount + 1;
			}
			if(useVelocityZ && lastVelocity.z > velocityZ.minimumVelocity && lastVelocity.z - velocity.z > 0 && lastVelocity.z > velocity.z + velocityZ.velocityRange)
			{
				float _velocity = Mathf.Clamp(lastVelocity.z,velocityZ.minimumVelocity,velocityZ.maximumVelocity);
				float _damage = RangeConversion(_velocity,velocityZ.minimumVelocity,velocityZ.maximumVelocity,velocityZ.minimumDamage,velocityZ.maximumDamage);
				damage = damage + _damage;
				amount = amount + 1;
			}
			//if(damage > 0 && amount > 0)source.DecreaseHealth(damage / amount);
			lastVelocity = velocity;
		}
		private float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum);}
		public void UseVelocity (bool value) {if(useVelocity != value)useVelocity = value;}
		public void SetBody (Rigidbody value) {if(body != value)body = value;}
		public void UseVelocityX (bool value) {if(useVelocityX != value)useVelocityX = value;}
		public void SetVelocityX (Velocity value) {if(velocityX != value)velocityX = value;}
		public void UseVelocityY (bool value) {if(useVelocityY != value)useVelocityY = value;}
		public void SetVelocityY (Velocity value) {if(velocityY != value)velocityY = value;}
		public void UseVelocityZ (bool value) {if(useVelocityZ != value)useVelocityZ = value;}
		public void SetVelocityZ (Velocity value) {if(velocityZ != value)velocityZ = value;}*/
	}
}