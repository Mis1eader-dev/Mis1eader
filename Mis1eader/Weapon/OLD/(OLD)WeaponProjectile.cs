namespace AdvancedAssets.Weapon
{
	using UnityEngine;
	using UnityEngine.Events;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using System.Collections.Generic;
	[RequireComponent(typeof(Rigidbody)),ExecuteInEditMode]
	public class WeaponProjectile : MonoBehaviour
	{
		// SUB-SONIC and SUPER-SONIC features, if the velocity of the projectile is more than speed of sound 343 m/s, it makes a great crack sound
		// 61 m/s is required to penetrate human skin
		public enum MassUnit {Gram,Kilogram}
		public enum VelocityUnit {MeterPerSecond,FootPerSecond}
		//public float joules = 0;
		public MassUnit massUnit = MassUnit.Kilogram;
		public VelocityUnit velocityUnit = VelocityUnit.MeterPerSecond;
		public float mass = 25;
		public float velocity = 100;
		public float diameter = 25;
		public float grains = 50;
		public float lifetime = 60;
		public SphereCollider cast = null;
		public LayerMask layer = 1 << 0 | 1 << 1 | 1 << 4;
		public UnityEvent onContact = new UnityEvent();
		public UnityEvent onDeath = new UnityEvent();
		#if UNITY_EDITOR
		public string invokesName = "Untitled";
		#endif
		public List<Invoke> invokes = new List<Invoke>();
		#if UNITY_EDITOR
		[HideInInspector] public bool invokesIsExpanded = true;
		#endif
		[HideInInspector,SerializeField] private new Rigidbody rigidbody = null;
		[HideInInspector] private float lifetimeCounter = 0;
		[HideInInspector] private Vector3 lastPos = Vector3.zero;
		[System.Serializable] public new class Invoke
		{
			public string name = "Untitled";
			#if UNITY_EDITOR
			public MonoScript source = null;
			#endif
			public string sourceName = string.Empty;
			public string methodName = string.Empty;
			public float floatValue = 0;
			public int intValue = 0;
			public bool boolValue = false;
			[HideInInspector] public Vector3 vector3Value = Vector3.zero;
			[HideInInspector] public Quaternion quaternionValue = Quaternion.identity;
			#if UNITY_EDITOR
			[HideInInspector] public int methodIndex = 0;
			[HideInInspector] public bool isExpanded = false;
			public void Update ()
			{
				if(source && sourceName != source.name)
					sourceName = source.name;
			}
			#endif
			public void FixedUpdate (Transform target)
			{
				if(target)
				{
					Component component = target.GetComponent(sourceName);
					if(component && methodName != string.Empty)
					{
						System.Reflection.MethodInfo methodInfo = component.GetType().GetMethod(methodName);
						if(methodInfo != null)
						{
							System.Reflection.ParameterInfo[] parameterInfo = methodInfo.GetParameters();
							if(parameterInfo.Length == 1)
							{
								if(parameterInfo[0].ParameterType == typeof(float))
								{
									methodInfo.Invoke(component,new object[] {floatValue});
									return;
								}
								if(parameterInfo[0].ParameterType == typeof(int))
								{
									methodInfo.Invoke(component,new object[] {intValue});
									return;
								}
								if(parameterInfo[0].ParameterType == typeof(bool))
								{
									methodInfo.Invoke(component,new object[] {boolValue});
									return;
								}
								if(parameterInfo[0].ParameterType == typeof(Vector3))
								{
									methodInfo.Invoke(component,new object[] {vector3Value});
									return;
								}
							}
							if(parameterInfo.Length == 2 && parameterInfo[0].ParameterType == typeof(Vector3) && parameterInfo[1].ParameterType == typeof(Quaternion))
							{
								methodInfo.Invoke(component,new object[] {vector3Value,quaternionValue});
								return;
							}
						}
					}
				}
			}
		}
		private void Update ()
		{
			EditorHandler();
			if(Application.isPlaying)
			{
				if(cast)ProjectileHandler();
				if(transform.parent)transform.parent = null;
				if(lifetimeCounter < lifetime)lifetimeCounter = lifetimeCounter + Time.deltaTime;
				if(lifetimeCounter >= lifetime)onDeath.Invoke();
			}
		}
		private void EditorHandler ()
		{
			float mass = (massUnit == MassUnit.Gram ? this.mass * 0.001f : this.mass);
			//float drag = (massUnit == MassUnit.Gram ? mass : mass * 1000) * 1.422f / Mathf.Pow(diameter,2);
			#if UNITY_EDITOR
			for(int a = 0,A = invokes.Count; a < A; a++)
				invokes[a].Update();
			#endif
			if(rigidbody != GetComponent<Rigidbody>())
				rigidbody = GetComponent<Rigidbody>();
			if(rigidbody.mass != mass)
				rigidbody.mass = mass;
			/*if(rigidbody.drag != drag)
				rigidbody.drag = drag;*/
		}
		private void ExecutionHandler ()
		{
			//if(rigidbody.velocity != Vector3.zero)rigidbody.rotation = Quaternion.LookRotation(rigidbody.velocity);
			//joules = 0.5f * rigidbody.mass * Mathf.Pow(transform.InverseTransformDirection(rigidbody.velocity).z,2);
		}
		private void ProjectileHandler ()
		{
			Vector3 pos = transform.position;
			float generalDist = Vector3.Distance(pos,lastPos);
			float thickness = 0;
			
			RaycastHit[] raycastHits = Physics.SphereCastAll(new Ray(lastPos,pos - lastPos),cast.radius,generalDist,layer);
			RaycastHit[] reverseRaycastHits = Physics.SphereCastAll(new Ray(pos,lastPos - pos),cast.radius,generalDist,layer);
			for(int a = 0,A = raycastHits.Length,aa = A - 1; a < A; a++)
			{
				if(reverseRaycastHits.Length > 0)
				{
					thickness = Vector3.Distance(raycastHits[a].point,reverseRaycastHits[aa].point);
					
					{
						//ContactMaterial cm = raycastHits[a].transform.GetComponent<ContactMaterial>();
						float resistance = thickness;
						Vector3 vel = transform.InverseTransformDirection(rigidbody.velocity);
						rigidbody.velocity = transform.TransformDirection(new Vector3(vel.x,vel.y,Mathf.Clamp(vel.z - resistance,0,vel.z)));
					}
					if(a + 1 < A)
					{
						//float space = Vector3.Distance(reverseRaycastHits[aa].point,raycastHits[a + 1].point);
						transform.position = raycastHits[a + 1].point;
						continue;
					}
					else
					{
						//CALCULATE CURVED POSITION
					}
				}
				else
				{
					
				}
				aa = aa - 1;
			}
			
			
			
			
			
			Debug.Log("Velocity: " + transform.InverseTransformDirection(rigidbody.velocity) + "\nGeneral Distance: " + generalDist + "\nThickness: " + thickness);
			lastPos = pos;
		}
		/*private void ProjectileHandler ()
		{
			Vector3 position = cast.transform.position + cast.transform.rotation * cast.center;
			RaycastHit[] raycastHits = Physics.SphereCastAll(lastPos,cast.radius,position - lastPos,Vector3.Distance(lastPos,position),layer);
			RaycastHit[] reverseRaycastHits = Physics.SphereCastAll(position,cast.radius,lastPos - position,Vector3.Distance(position,lastPos),layer);
			if(raycastHits.Length > 0)
			{
				bool wasInside = false;
				Collider[] colliders = Physics.OverlapSphere(lastPos,cast.radius,layer);
				for(int a = 0,A = colliders.Length; a < A; a++)if(colliders[a] != cast)
				{
					wasInside = true;
					break;
				}
				
				
				
				
				
				for(int a = raycastHits.Length - 1; a >= 0 ; a--)if(raycastHits[a].collider != cast)
				{
					ContactMaterial cm = raycastHits[a].transform.GetComponent<ContactMaterial>();
					if(reverseRaycastHits.Length > 0 && cm && reverseRaycastHits[a].collider != cast)
					{
						float overallDistancePerSecond = Vector3.Distance(lastPos,position);
						float initialToEnter = Vector3.Distance(lastPos - cast.transform.rotation * cast.center,raycastHits[a].point);
						float enterToExit = Vector3.Distance(!wasInside ? raycastHits[a].point : lastPos,reverseRaycastHits[a].point);
						float exitToFinal = Vector3.Distance(reverseRaycastHits[a].point,position - cast.transform.rotation * cast.center);
						
						float resistance = enterToExit * cm.toughness;
						
						Vector3 vel = transform.InverseTransformDirection(rigidbody.velocity);
						
						print(enterToExit);
						
						if(resistance > 0)
						{
							Vector3 enter = wasInside ? lastPos - cast.transform.rotation * cast.center : raycastHits[a].point;
							Vector3 exit = reverseRaycastHits[a].point;
							if(resistance < vel.z)
							{
								transform.position = exit;
								rigidbody.velocity = transform.TransformDirection(vel - new Vector3(0,0,resistance));
								
								//transform.position = Vector3.Lerp(exit,position - cast.transform.rotation * cast.center,);
							}
							if(resistance == vel.z)
							{
								transform.position = exit;
								rigidbody.velocity = Vector3.zero;
							}
							if(resistance > vel.z)
							{
								transform.position = Vector3.Lerp(enter,exit,1f / (resistance / vel.z));
								rigidbody.velocity = Vector3.zero;
								
								
							}
							
							//transform.position = Vector3.Lerp((!wasInside ? raycastHits[a].point : lastPos),position - cast.transform.rotation * cast.center,(enterToExit + exitToFinal) / (enterToExit + exitToFinal + (resistance / vel.z)));
							//rigidbody.velocity = transform.TransformDirection(vel - new Vector3(0,0,Mathf.Clamp(resistance,0,vel.z)));
							
							
						}
					}
					if(!wasInside)
					{
						for(int b = 0,B = invokes.Count; b < B; b++)
						{
							invokes[b].vector3Value = raycastHits[a].point;
							invokes[b].quaternionValue = Quaternion.LookRotation(raycastHits[a].normal);
							invokes[b].FixedUpdate(raycastHits[a].transform);
						}
						onContact.Invoke();
					}
					else
					{
						
					}
					if(LOST PENETRATION)
					{
						onDeath.Invoke();
						break;
					}
					
				}
				//DO ANOTHER BACKWARDS SPHERECAST (reverseRaycastHits) and find a way to know if the raycast has penetrated, or just got out of the object, so the system won't spawn 2 hit objects when the projectile takes 2 frames to finish the hit of 1 object.
				//You can check if the last SphereCastAll is the same as the new SphereCastAll, if true, then don't invoke "On Contact".
			}
		}*/
		private void Awake ()
		{
			if(Application.isPlaying)
			{
				if(cast)lastPos = transform.position;
				//if(cast)lastPos = cast.transform.position + cast.transform.rotation * cast.center;
				rigidbody.velocity = transform.forward * (velocityUnit == VelocityUnit.MeterPerSecond ? velocity : velocity / 3.281f);
				//rigidbody.velocity = (velocityUnit == VelocityUnit.MeterPerSecond ? worldVelocity : worldVelocity / 3.281f) + (transform.rotation * (velocityUnit == VelocityUnit.MeterPerSecond ? selfVelocity : selfVelocity / 3.281f));
			}
		}
		public void Destroy () {GameObject.Destroy(gameObject);}
		public void Destroy (GameObject value) {if(value)GameObject.Destroy(value);}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(WeaponProjectile)),CanEditMultipleObjects]
	internal class WeaponProjectileEditor : Editor
	{
		private WeaponProjectile[] weaponProjectiles
		{
			get
			{
				WeaponProjectile[] weaponProjectiles = new WeaponProjectile[targets.Length];
				for(int weaponProjectilesIndex = 0; weaponProjectilesIndex < targets.Length; weaponProjectilesIndex++)
					weaponProjectiles[weaponProjectilesIndex] = (WeaponProjectile)targets[weaponProjectilesIndex];
				return weaponProjectiles;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			MainSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int weaponProjectilesIndex = 0; weaponProjectilesIndex < weaponProjectiles.Length; weaponProjectilesIndex++)
					EditorUtility.SetDirty(weaponProjectiles[weaponProjectilesIndex]);
			}
		}
		private void MainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Main",EditorStyles.boldLabel);
				EditorGUIUtility.labelWidth = 40;
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("mass"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("massUnit"),GUIContent.none,true);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = 60;
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("velocity"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("velocityUnit"),GUIContent.none,true);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = 0;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("cast"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("layer"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("diameter"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("grains"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("lifetime"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("onContact"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("onDeath"),true);
				MainSectionInvokesContainer();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionInvokesContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Invokes","Box",GUILayout.ExpandWidth(true)))
					{
						weaponProjectiles[0].invokesIsExpanded = !weaponProjectiles[0].invokesIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = weaponProjectiles[0].invokes.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						weaponProjectiles[0].invokes.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(weaponProjectiles[0].invokesIsExpanded)
				{
					for(int a = 0; a < weaponProjectiles[0].invokes.Count; a++)
					{
						WeaponProjectile.Invoke currentInvoke = weaponProjectiles[0].invokes[a];
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button(currentInvoke.name,"Box",GUILayout.ExpandWidth(true)))
								{
									currentInvoke.isExpanded = !currentInvoke.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									WeaponProjectile.Invoke previous = weaponProjectiles[0].invokes[a - 1];
									weaponProjectiles[0].invokes[a - 1] = currentInvoke;
									weaponProjectiles[0].invokes[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != weaponProjectiles[0].invokes.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									WeaponProjectile.Invoke next = weaponProjectiles[0].invokes[a + 1];
									weaponProjectiles[0].invokes[a + 1] = currentInvoke;
									weaponProjectiles[0].invokes[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									weaponProjectiles[0].invokes.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(weaponProjectiles[0].invokes[a].isExpanded)
							{
								SerializedProperty currentInvokeProperty = serializedObject.FindProperty("invokes").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical("Box");
									{
										EditorGUIUtility.labelWidth = 100;
										EditorGUILayout.PropertyField(currentInvokeProperty.FindPropertyRelative("name"),true);
										EditorGUILayout.PropertyField(currentInvokeProperty.FindPropertyRelative("source"),true);
										MainSectionInvokesContainerMethodContainer(currentInvoke,currentInvokeProperty);
									}
									EditorGUILayout.EndVertical();
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUIUtility.labelWidth = 40;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("invokesName"),new GUIContent("Name"),true);
						EditorGUIUtility.labelWidth = 0;
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							weaponProjectiles[0].invokes.Add(new WeaponProjectile.Invoke());
							weaponProjectiles[0].invokes[weaponProjectiles[0].invokes.Count - 1].name = weaponProjectiles[0].invokesName;
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSectionInvokesContainerMethodContainer (WeaponProjectile.Invoke currentInvoke,SerializedProperty currentInvokeProperty)
		{
			List<string> names = new List<string>();
			string methodName = string.Empty;
			bool methodExists = false;
			Color color = GUI.color;
			names.Add("Not Specified");
			if(currentInvoke.source)
			{
				System.Reflection.MethodInfo[] methodInfos = currentInvoke.source.GetClass().GetMethods();
				for(int a = 0; a < methodInfos.Length; a++)if(methodInfos[a].Name != "CancelInvoke" && methodInfos[a].Name != "IsInvoking" && methodInfos[a].Name != "GetComponent" && methodInfos[a].Name != "GetComponentInChildren" && methodInfos[a].Name != "GetComponentsInChildren" && methodInfos[a].Name != "GetComponentInParent" && methodInfos[a].Name != "GetComponentsInParent" && methodInfos[a].Name != "GetComponents")
				{
					System.Reflection.ParameterInfo[] parameterInfos = methodInfos[a].GetParameters();
					if(currentInvoke.methodName == methodInfos[a].Name)
					{
						if(currentInvoke.methodIndex == 0)
							methodName = methodInfos[a].Name;
						methodExists = true;
					}
					if(parameterInfos.Length == 0)names.Add(methodInfos[a].Name + " ()");
					if(parameterInfos.Length == 1)
					{
						if(parameterInfos[0].ParameterType == typeof(float))
						{
							names.Add(methodInfos[a].Name + " (float)");
							continue;
						}
						if(parameterInfos[0].ParameterType == typeof(int))
						{
							names.Add(methodInfos[a].Name + " (int)");
							continue;
						}
						if(parameterInfos[0].ParameterType == typeof(bool))
						{
							names.Add(methodInfos[a].Name + " (bool)");
							continue;
						}
						if(parameterInfos[0].ParameterType == typeof(Vector3))
						{
							names.Add(methodInfos[a].Name + " (vector3)");
							continue;
						}
					}
					if(parameterInfos.Length == 2 && parameterInfos[0].ParameterType == typeof(Vector3) && parameterInfos[1].ParameterType == typeof(Quaternion))
					{
						names.Add(methodInfos[a].Name + " (vector3,quaternion)");
						continue;
					}
				}
			}
			GUI.enabled = currentInvoke.source;
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(currentInvoke.methodIndex > 0 && !methodExists)
						currentInvoke.methodIndex = 0;
					GUI.color = currentInvoke.source ? (currentInvoke.methodIndex > 0 && methodExists ? Color.green : Color.red) : Color.yellow;
					GUILayout.Box(GUIContent.none,GUILayout.Width(12),GUILayout.Height(12));
					GUI.color = color;
					GUILayout.Label("Method Name");
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				if(currentInvoke.source && methodName != string.Empty)for(int a = 1; a < names.Count; a++)if(methodName == names[a].Replace(" ()",string.Empty).Replace(" (float)",string.Empty).Replace(" (int)",string.Empty).Replace(" (bool)",string.Empty).Replace(" (vector3)",string.Empty).Replace(" (vector3,quaternion)",string.Empty))
					currentInvoke.methodIndex = a;
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(currentInvokeProperty.FindPropertyRelative("methodName"),GUIContent.none,true);
					EditorGUI.BeginChangeCheck();
					int index = EditorGUILayout.Popup(currentInvoke.methodIndex,names.ToArray());
					if(index >= 0 && EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						currentInvoke.methodName = index < names.Count ? names[index].Replace("Not Specified",string.Empty).Replace(" ()",string.Empty).Replace(" (float)",string.Empty).Replace(" (int)",string.Empty).Replace(" (bool)",string.Empty).Replace(" (vector3)",string.Empty).Replace(" (vector3,quaternion)",string.Empty) : string.Empty;
						currentInvoke.methodIndex = index;
					}
				}
				EditorGUILayout.EndHorizontal();
				if(currentInvoke.source)
				{
					System.Reflection.MethodInfo methodInfo = currentInvoke.source.GetClass().GetMethod(currentInvoke.methodName);
					if(methodInfo != null)
					{
						System.Reflection.ParameterInfo[] parameterInfos = methodInfo.GetParameters();
						if(parameterInfos.Length == 1)
						{
							if(parameterInfos[0].ParameterType == typeof(float))
								EditorGUILayout.PropertyField(currentInvokeProperty.FindPropertyRelative("floatValue"),true);
							if(parameterInfos[0].ParameterType == typeof(int))
								EditorGUILayout.PropertyField(currentInvokeProperty.FindPropertyRelative("intValue"),true);
							if(parameterInfos[0].ParameterType == typeof(bool))
								EditorGUILayout.PropertyField(currentInvokeProperty.FindPropertyRelative("boolValue"),true);
						}
					}
				}
			}
			EditorGUILayout.EndVertical();
			GUI.enabled = true;
		}
	}
	#endif
}