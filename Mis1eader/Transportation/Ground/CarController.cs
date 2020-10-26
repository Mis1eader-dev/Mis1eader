#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
#define INPUT_BOTH
#endif
namespace Mis1eader.Vehicle
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Transportation/Ground/Car Controller",0),RequireComponent(typeof(Rigidbody)),ExecuteInEditMode]
	public class CarController : MonoBehaviour
	{
		public enum Gearbox : byte {Automatic,Manual}
		#if INPUT_BOTH
		public enum InputHandling : byte {InputManagerOld,InputSystemPackage}
		public InputHandling inputHandling = InputHandling.InputManagerOld;
		#endif
		public TransportationInput input = null;
		public AudioSource engine = null;
		public Library.Speed speedUnit = Library.Speed.KilometersPerHour;
		public float speed = 0F;
		public float minimumRpm = 700F;
		public float rpm = 0F;
		public float maximumRpm = 9000F;
		public float motorTorque = 1000F;
		public float brakeTorque = 1000F;
		public float handbrakeTorque = 2000F;
		public float steerAngle = 35F;
		public float downforce = 1500F;
		public Gearbox gearbox = Gearbox.Automatic;
		/*GEAR
		-2: reverse
		-1: park
		 0: neutral
		 1,2,3,..
		*/
		public sbyte gear = 1;
		//                                                          R , 1   ,  2  , 3  ,  4  , 5
		public List<float> gearRatio = new List<float>(new float[] {4F,1.2F,1.15F,1.1F,1.05F,1F});
		public float fuelPedal = 0F;
		public float brakePedal = 0F;
		public float handbrakeStick = 0F;
		public float steeringWheel = 0F;
		public Transform centerOfMass = null;
		public List<WheelController> wheels = new List<WheelController>();
		private float metersPerSecond = 0F;
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
			InputValidation();
			RigidbodyValidation();
			if(gear < -2)gear = -2;
			else if(gear >= gearRatio.Count)gear = (sbyte)(gearRatio.Count - 1);
		}
		private void InputValidation ()
		{
			if(!input)
			{
				#if INPUT_BOTH
				if(inputHandling == InputHandling.InputManagerOld)
				{
					#endif
					#if !ENABLE_INPUT_SYSTEM
					input = GetComponent<InputManagerOld>();
					if(!input)input = gameObject.AddComponent<InputManagerOld>();
					#endif
					#if INPUT_BOTH
				}
				#endif
				#if INPUT_BOTH
				else
				{
					#endif
					#if ENABLE_INPUT_SYSTEM
					input = GetComponent<InputManagerNew>();
					if(!input)input = gameObject.AddComponent<InputManagerNew>();
					#endif
					#if INPUT_BOTH
				}
				#endif
			}
			#if INPUT_BOTH
			else
			{
				if(inputHandling == InputHandling.InputManagerOld) {if(input is InputManagerNew)
				{
						input = GetComponent<InputManagerOld>();
						if(!input)input = gameObject.AddComponent<InputManagerOld>();
				}}
				else if(input is InputManagerOld)
				{
						input = GetComponent<InputManagerNew>();
						if(!input)input = gameObject.AddComponent<InputManagerNew>();
				}
			}
			#endif
		}
		private void RigidbodyValidation ()
		{
			if(!rigidbody || rigidbody.gameObject != gameObject)
			{
				rigidbody = GetComponent<Rigidbody>();
				if(!rigidbody)rigidbody = gameObject.AddComponent<Rigidbody>();
			}
		}
		private void ExecutionHandler ()
		{
			metersPerSecond = transform.InverseTransformDirection(rigidbody.velocity).z;
			InputHandler();
			GearboxHandler();
			rpm = minimumRpm;
			for(int a = 0,A = wheels.Count; a < A; a++)
			{
				WheelController wheel = wheels[a];
				if(!wheel)continue;
				if(wheel.motor)rpm = rpm + Mathf.Abs(wheel.wheelCollider.rpm);
				wheel.Handle(gear == -2 ? -motorTorque * fuelPedal * gearRatio[0] : (gear >= 1 ? motorTorque * fuelPedal * gearRatio[gear] : 0F),brakeTorque * brakePedal + handbrakeTorque * handbrakeStick,steerAngle * steeringWheel);
			}
			rpm = rpm + Random.Range(-10F,10F);
			if(rpm > maximumRpm)rpm = maximumRpm + Random.Range(-100F,100F);
			if(centerOfMass)rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
			rigidbody.AddForce(downforce * Vector3.down);
			speed = Library.Convert(metersPerSecond,speedUnit);
			if(engine)
			{
				if(!engine.isPlaying)engine.Play();
				engine.pitch = 1F + rpm / maximumRpm;
			}
			
			TEMPORARY();
		}
		private void InputHandler ()
		{
			if(input)
			{
				input.Handle();
				fuelPedal = input.movementInput.y;
				if(metersPerSecond < -0.1F || gear == -2)
				{
					if(fuelPedal < 0F)
					{
						fuelPedal = -fuelPedal;
						brakePedal = 0F;
					}
					else
					{
						fuelPedal = 0F;
						brakePedal = Mathf.Abs(input.movementInput.y);
					}
				}
				else
				{
					if(fuelPedal < 0F)
					{
						fuelPedal = 0F;
						brakePedal = Mathf.Abs(input.movementInput.y);
					}
					else brakePedal = 0F;
				}
				handbrakeStick = input.brakeInput;
				steeringWheel = input.movementInput.x;
			}
		}
		private void GearboxHandler ()
		{
			#if USE_INTERNAL_INPUT
			GearDirection(input.movementInput.y);
			if(gearbox == Gearbox.Manual && input)
			{
				if(input.gearInput == -1)GearDown();
				else if(input.gearInput == 1)GearUp();
			}
			#endif
		}
		private void TEMPORARY ()
		{
			if(Input.GetKeyDown(KeyCode.P))transform.position = Vector3.zero;
			if(Input.GetKeyDown(KeyCode.R))transform.rotation = Quaternion.identity;
		}
		public void SetGear (sbyte value) {gear = value;}
		public void GearUp () {if(gear >= 1 && gear <= gearRatio.Count)gear += 1;}
		public void GearDown () {if(gear > 1)gear -= 1;}
		//[HideInInspector,SerializeField] private sbyte lastGear = 1;
		public void GearDirection (float direction)
		{
			if((metersPerSecond < 0F ? -metersPerSecond : metersPerSecond) < 0.1F)
			{
				if(direction < 0F)gear = -2;
				else if(direction > 0F)gear = 1;
			}
		}
		public void AttachWheel (WheelController wheel)
		{
			if(!wheel)return;
			wheel.transform.parent = transform;
			wheels.Add(wheel);
		}
		public void DetachWheel (int index)
		{
			if(index < 0 || index >= wheels.Count)return;
			WheelController wheel = wheels[index];
			if(!wheel)return;
			wheel.transform.parent = null;
			wheels.RemoveAt(index);
		}
		[HideInInspector] public new Rigidbody rigidbody = null;
	}
}