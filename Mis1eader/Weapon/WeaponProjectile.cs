namespace Mis1eader.Weapon
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Weapon/Weapon Projectile",1),ExecuteInEditMode]
	public class WeaponProjectile : Firable
	{
		public float velocity = 340F;
		public CapsuleCollider casting = null;
		public LayerMask layerMask = -1 ^ (1 << 2);
		private void Awake ()
		{
			if(casting)
			{
				lastPosition = casting.transform.position;
				lastVelocity = transform.InverseTransformDirection(rigidbody.velocity).z;
			}
		}
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			ExecutionHandler();
		}
		private new void ValidationHandler ()
		{
			base.ValidationHandler();
		}
		private void ExecutionHandler ()
		{
			
		}
		private Vector3 lastPosition = Vector3.zero;
		private float lastVelocity = 0F;
		private void FixedUpdate ()
		{
			if(casting)
			{
				Vector3 start = lastPosition;
				Vector3 end = casting.transform.position;
				
				float radius = casting.radius;
				float height = casting.height;
				
				Vector3 direction = casting.direction == 0 ? Vector3.right : (casting.direction == 1 ? Vector3.up : Vector3.forward);
				Quaternion rotation = casting.transform.rotation;
				
				Vector3 bottom = start + rotation * (casting.center - direction * height * 0.5F);
				Vector3 top = bottom + rotation * (direction * height);
				
				float coveredDistanceInThisFrame = Vector3.Distance(start,end);
				
				while(coveredDistanceInThisFrame > 0F)
				{
					RaycastHit[] hits = Physics.CapsuleCastAll(bottom,top,radius,transform.forward,coveredDistanceInThisFrame,layerMask);
					int A = hits.Length;
					if(A != 0)
					{
						if(A != 1)for(int a = 0,b = A - 1; a < A; a++,b--)
						{
							RaycastHit hit = hits[a];
							if(hit.collider == casting)continue;
							Vector3 hitPoint = hit.point;
							float distance = hit.distance;
							
							float velocity = transform.InverseTransformDirection(rigidbody.velocity).z;
							velocity = lastVelocity + (velocity - lastVelocity) * (Vector3.Distance(start,hitPoint) / Vector3.Distance(start,end));
							
							
							coveredDistanceInThisFrame = coveredDistanceInThisFrame - distance;
							
							Vector3 reverseRelative = end - start;
							RaycastHit[] reverseHits = Physics.CapsuleCastAll(bottom + reverseRelative,top + reverseRelative,radius,-transform.forward,coveredDistanceInThisFrame,layerMask);
							
							if(b < reverseHits.Length)
							{
								RaycastHit reverseHit = reverseHits[b];
								Vector3 reverseHitPoint = reverseHit.point;
								float thickness = Vector3.Distance(hitPoint,reverseHitPoint);
								//Didn't have enough velocity to cut through.
								if(velocity < thickness)
								{
									rigidbody.velocity = Vector3.zero;
									rigidbody.position = Vector3.Lerp(hitPoint,reverseHitPoint,velocity / thickness);
								}
								//Had enough velocity to cut through.
								else
								{
									rigidbody.velocity = rigidbody.velocity - rotation * Vector3.forward * thickness;
									//Calculate position curve.
									rigidbody.position = end;
								}
								
								
								coveredDistanceInThisFrame = coveredDistanceInThisFrame - thickness;
								
								lastPosition = rigidbody.position;
								lastVelocity = transform.InverseTransformDirection(rigidbody.velocity).z;
								
								break;
							}
							else
							{
								float thickness = Vector3.Distance(hitPoint,end);
								//Didn't have enough velocity to cut through.
								if(velocity < thickness)
								{
									rigidbody.velocity = Vector3.zero;
									rigidbody.position = Vector3.Lerp(hitPoint,end,velocity / thickness);
								}
								//Had enough velocity to cut through.
								else
								{
									rigidbody.velocity = rigidbody.velocity - rotation * Vector3.forward * thickness;
									rigidbody.position = Vector3.Lerp(hitPoint,end,thickness / velocity);
								}
								
								
								coveredDistanceInThisFrame = coveredDistanceInThisFrame - thickness;
								
								lastPosition = rigidbody.position;
								lastVelocity = transform.InverseTransformDirection(rigidbody.velocity).z;
								
								
								break;
							}
						}
						else
						{
							RaycastHit hit = hits[0];
							if(hit.collider == casting)coveredDistanceInThisFrame = 0F;
							else
							{
								//CALC like the for() loop.
							}
							coveredDistanceInThisFrame = 0F;
						}
					}
					else coveredDistanceInThisFrame = 0F;
				}
			}
		}
		public override void Fire ()
		{
			rigidbody.useGravity = true;
			rigidbody.isKinematic = false;
			rigidbody.AddForce(transform.forward * velocity);
			transform.parent = null;
			lastVelocity = transform.InverseTransformDirection(rigidbody.velocity).z;
		}
	}
}