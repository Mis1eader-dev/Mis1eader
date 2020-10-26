namespace Mis1eader.Character
{
	using UnityEngine;
	using UnityEngine.Events;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Creature/Human/Character Controller",0),RequireComponent(typeof(CapsuleCollider),typeof(Rigidbody)),ExecuteInEditMode]
	public class CharacterController : MonoBehaviour
	{
		public enum PerspectiveType : byte {FirstPerson,ThirdPerson}
		public enum ControlType : byte {PlayerControlled,AIControlled}
		public enum RotationType : byte {MouseXAndY,MouseX,MouseY}
		public enum ThirdPersonDirection : byte {CameraDirection,InputDirection}
		public bool isAlive = true;
		public bool switchablePerspectives = false;
		public PerspectiveType perspectiveType = PerspectiveType.FirstPerson;
		public ControlType controlType = ControlType.PlayerControlled;
		public bool airControl = false;
		public float walkSpeed = 2.5f;
		public float runSpeed = 6.5f;
		public float jumpForce = 7.5f;
		public float crouchSpeed = 1.25f;
		public float proneSpeed = 0.8f;
		#if UNITY_EDITOR
		public string statesName = "Untitled";
		#endif
		public List<State> states = new List<State>();
		#if UNITY_EDITOR
		public string eventsName = "Untitled";
		#endif
		public List<Event> events = new List<Event>();
		#if UNITY_EDITOR
		public bool automaticallyAddParameters = true;
		#endif
		public bool useSmoothMovements = false;
		public float idleDamping = 0.3f;
		public float walkDamping = 0.5f;
		public float runDamping = 0.8f;
		public float crouchDamping = 1.2f;
		public float proneDamping = 0.9f;
		public bool useAnimator = false;
		public Animator animator = null;
		#if UNITY_5_3_OR_NEWER
		[Delayed] public string horizontalParameter = "Horizontal";
		[Delayed] public string verticalParameter = "Vertical";
		[Delayed] public string runParameter = "Is Running";
		[Delayed] public string jumpParameter = "Is Jumping";
		[Delayed] public string crouchParameter = "Is Crouching";
		[Delayed] public string proneParameter = "Is Proning";
		#else
		public string horizontalParameter = "Horizontal";
		public string verticalParameter = "Vertical";
		public string runParameter = "Is Running";
		public string jumpParameter = "Is Jumping";
		public string crouchParameter = "Is Crouching";
		public string proneParameter = "Is Proning";
		#endif
		#if UNITY_EDITOR
		public string parametersName = "Untitled";
		#endif
		public List<Parameter> parameters = new List<Parameter>();
		public bool useRagdoll = false;
		public bool ragdollIsActive = false;
		public List<Rigidbody> ragdollBodies = new List<Rigidbody>();
		public bool useEnergy = false;
		public float energy = 100;
		public float capacity = 100;
		public bool useEnergyUsages = false;
		public float idleUsage = 0.075f;
		public float walkUsage = 0.2f;
		public float runUsage = 0.25f;
		public float jumpUsage = 3.75f;
		public float crouchUsage = 0.15f;
		public float proneUsage = 0.1f;
		public new Transform camera = null;
		public RotationType rotationType = RotationType.MouseXAndY;
		public bool lockCursor = false;
		public bool hideCursor = false;
		public float distance = 5;
		public bool swapAxis = false;
		public bool invertAxisX = false;
		public bool invertAxisY = false;
		public bool clampX = false;
		public float minimumX = -180;
		public float maximumX = 180;
		public bool clampY = true;
		public float minimumY = -80;
		public float maximumY = 80;
		public bool useDeltaTimeX = false;
		public bool useDeltaTimeY = false;
		public bool useTimeScaleX = true;
		public bool useTimeScaleY = true;
		public float rotationDamping = 0.4f;
		public float turnDamping = 135;
		public Vector2 sensitivity = Vector2.one * 10;
		public Vector2 sensitivityRange = Vector2.one * 5;
		public bool useSmoothPerspectives = true;
		public float firstPersonSwitchTime = 0.1f;
		public float thirdPersonSwitchTime = 0.3f;
		public bool useParentalRotationX = false;
		public bool useParentalRotationY = false;
		public bool useParentalOffset = false;
		public Vector3 firstPersonWorldOffset = Vector3.zero;
		public Vector3 firstPersonSelfOffset = Vector3.zero;
		public Vector3 firstPersonLocalOffset = new Vector3(0,0.8f,0.2f);
		public Vector3 thirdPersonWorldOffset = new Vector3(0,1,0);
		public Vector3 thirdPersonSelfOffset = Vector3.zero;
		public Vector3 thirdPersonLocalOffset = Vector3.zero;
		public ThirdPersonDirection thirdPersonDirection = ThirdPersonDirection.InputDirection;
		public LayerMask dontClip = 1 << 0 | 1 << 1;
		public float clipRadius = 0.5f;
		#if UNITY_5_3_OR_NEWER
		[Delayed] public string horizontalAxis = "Horizontal";
		[Delayed] public string verticalAxis = "Vertical";
		[Delayed] public string runAxis = "Run";
		[Delayed] public string jumpAxis = "Jump";
		[Delayed] public string crouchAxis = string.Empty;
		[Delayed] public string proneAxis = string.Empty;
		[Delayed] public string mouseXAxis = "Mouse X";
		[Delayed] public string mouseYAxis = "Mouse Y";
		[Delayed] public string perspectiveAxis = "Perspective";
		#else
		public string horizontalAxis = "Horizontal";
		public string verticalAxis = "Vertical";
		public string runAxis = "Run";
		public string jumpAxis = "Jump";
		public string crouchAxis = string.Empty;
		public string proneAxis = string.Empty;
		public string mouseXAxis = "Mouse X";
		public string mouseYAxis = "Mouse Y";
		public string perspectiveAxis = "Perspective";
		#endif
		public bool toggleRun = true;
		public bool toggleCrouch = true;
		public bool toggleProne = true;
		public bool canRun = true;
		public bool canJump = true;
		public bool canCrouch = true;
		public bool canProne = false;
		public float horizontalSensitivity = 3;
		public float verticalSensitivity = 3;
		public float mouseXSensitivity = 3;
		public float mouseYSensitivity = 3;
		#if UNITY_EDITOR
		[HideInInspector] public int tabIndex = 0;
		[HideInInspector] public int ragdollBodiesScrollViewIndex = 0;
		[HideInInspector] public bool statesIsExpanded = false;
		[HideInInspector] public bool eventsIsExpanded = false;
		[HideInInspector] public bool smoothMovementsIsExpanded = false;
		[HideInInspector] public bool animatorIsExpanded = false;
		[HideInInspector] public bool parametersIsExpanded = true;
		[HideInInspector] public bool ragdollIsExpanded = false;
		[HideInInspector] public bool energyIsExpanded = false;
		[HideInInspector] public bool smoothPerspectivesIsExpanded = false;
		[HideInInspector] public Vector2 ragdollBodiesScrollView = Vector2.zero;
		#endif
		[HideInInspector,SerializeField] private CapsuleCollider capsuleCollider = null;
		[HideInInspector,SerializeField] private new Rigidbody rigidbody = null;
		[HideInInspector] private Vector2 movementInput = Vector2.zero;
		[HideInInspector] private Vector2 mouseInput = Vector2.zero;
		[HideInInspector] private bool onJumpInput = false;
		[HideInInspector] private bool onPerspectiveInput = false;
		[HideInInspector] private bool isHorizontalInput = false;
		[HideInInspector] private bool isVerticalInput = false;
		[HideInInspector] private bool isRunInput = false;
		[HideInInspector] private bool isCrouchInput = false;
		[HideInInspector] private bool isProneInput = false;
		[HideInInspector] private bool onJump = false;
		[HideInInspector] private bool isRunning = false;
		[HideInInspector] private bool isJumping = false;
		[HideInInspector] private bool isCrouching = false;
		[HideInInspector] private bool isProning = false;
		[HideInInspector] private bool isControlled = true;
		[HideInInspector] private bool isGrounded = false;
		[HideInInspector] private Vector2 movement = Vector2.zero;
		[System.Serializable] public class State
		{
			public string name = "Untitled";
			public string axis = string.Empty;
			public float range = 1;
			public UnityEvent onPositive = new UnityEvent();
			public UnityEvent onNegative = new UnityEvent();
			public UnityEvent onKeyDown = new UnityEvent();
			public UnityEvent onKeyUp = new UnityEvent();
			public UnityEvent onTrue = new UnityEvent();
			public UnityEvent onFalse = new UnityEvent();
			public UnityEvent isPositive = new UnityEvent();
			public UnityEvent isNegative = new UnityEvent();
			public UnityEvent isKeyDown = new UnityEvent();
			public UnityEvent isKeyUp = new UnityEvent();
			public UnityEvent isTrue = new UnityEvent();
			public UnityEvent isFalse = new UnityEvent();
			[HideInInspector] public float floatInput = 0;
			[HideInInspector] public int intInput = 0;
			[HideInInspector] public bool _onPositive = false;
			[HideInInspector] public bool _onNegative = false;
			[HideInInspector] public bool onInputDown = false;
			[HideInInspector] public bool onInputUp = false;
			[HideInInspector] public bool _onTrue = false;
			[HideInInspector] public bool _onFalse = false;
			[HideInInspector] public bool _isPositive = false;
			[HideInInspector] public bool _isNegative = false;
			[HideInInspector] public bool isInputDown = false;
			[HideInInspector] public bool isInputUp = false;
			[HideInInspector] public bool _isTrue = false;
			[HideInInspector] public bool _isFalse = true;
			#if UNITY_EDITOR
			[HideInInspector] public bool isExpanded = false;
			#endif
			public void Update ()
			{
				if(_onPositive)_onPositive = false;
				if(_onNegative)_onNegative = false;
				if(_onTrue)_onTrue = false;
				if(_onFalse)_onFalse = false;
				if(axis != string.Empty)
				{
					floatInput = Mathf.Clamp(Input.GetAxis(axis),-Mathf.Abs(range),Mathf.Abs(range));
					intInput = (int)Mathf.Clamp(Input.GetAxisRaw(axis),-Mathf.Abs(range),Mathf.Abs(range));
					onInputDown = Input.GetButtonDown(axis);
					onInputUp = Input.GetButtonUp(axis);
					isInputDown = Input.GetButton(axis);
					isInputUp = !Input.GetButton(axis);
				}
				else
				{
					floatInput = 0;
					intInput = 0;
					onInputDown = false;
					onInputUp = false;
					isInputDown = false;
					isInputUp = false;
				}
				if(!_isPositive && floatInput > 0)
				{
					onPositive.Invoke();
					_onPositive = true;
					_isPositive = true;
					_isNegative = false;
				}
				if(!_isNegative && floatInput < 0)
				{
					onNegative.Invoke();
					_onNegative = true;
					_isPositive = false;
					_isNegative = true;
				}
				if(onInputDown)
				{
					onKeyDown.Invoke();
					if(_isTrue)
					{
						onFalse.Invoke();
						_onFalse = true;
						_isTrue = false;
						_isFalse = true;
					}
					else if(_isFalse)
					{
						onTrue.Invoke();
						_onTrue = true;
						_isTrue = true;
						_isFalse = false;
					}
				}
				if(onInputUp)onKeyUp.Invoke();
				if(_isPositive)isPositive.Invoke();
				if(_isNegative)isNegative.Invoke();
				if(isInputDown)isKeyDown.Invoke();
				if(isInputUp)isKeyUp.Invoke();
				if(_isTrue)isTrue.Invoke();
				if(_isFalse)isFalse.Invoke();
			}
		}
		[System.Serializable] public class Event
		{
			public string name = string.Empty;
			public List<Condition> conditions = new List<Condition>();
			public UnityEvent onTrue = new UnityEvent();
			public UnityEvent onFalse = new UnityEvent();
			public UnityEvent isTrue = new UnityEvent();
			public UnityEvent isFalse = new UnityEvent();
			[HideInInspector] public bool _isTrue = false;
			[HideInInspector] public bool _isFalse = false;
			#if UNITY_EDITOR
			[HideInInspector] public bool isExpanded = false;
			[HideInInspector] public bool conditionsIsExpanded = true;
			#endif
			[System.Serializable] public class Condition
			{
				public enum Usage : byte {States,Character}
				public enum Statement : byte {And,Or}
				public enum Type : byte {Float,Int,Bool}
				public enum Target : byte {Value,OnPositive,OnNegative,OnKeyDown,OnKeyUp,OnTrue,OnFalse,IsPositive,IsNegative,IsKeyDown,IsKeyUp,IsTrue,IsFalse,IsAlive,SwitchablePerspectives,FirstPerson,ThirdPerson,PlayerControlled,AIControlled,WalkSpeed,RunSpeed,JumpForce,CrouchSpeed,ProneSpeed,Energy,Capacity,MouseXAndY,MouseX,MouseY,LockCursor,HideCursor,InvertAxis,CameraDirection,InputDirection,ToggleRun,ToggleCrouch,ToggleProne,CanRun,CanJump,CanCrouch,CanProne,IsRunning,IsJumping,IsCrouching,IsProning,IsGrounded}
				public enum Operator : byte {EqualTo,NotEqualTo,GreaterThan,LessThan,GreaterThanOrEqualTo,LessThanOrEqualTo}
				public Usage usage = Usage.States;
				public int index = 0;
				public Statement statement = Statement.And;
				public Type type = Type.Float;
				public Target target = Target.Value;
				public Operator _operator = Operator.EqualTo;
				public float floatValue = 0;
				public int intValue = 0;
				public bool boolValue = false;
				[HideInInspector] public bool isPassed = false;
				#if UNITY_EDITOR
				[HideInInspector] public bool isExpanded = false;
				#endif
			}
			public void Update (List<State> states,bool isAlive,bool switchablePerspectives,bool isFirstPerson,bool isThirdPerson,bool isPlayerControlled,bool isAIControlled,float walkSpeed,float runSpeed,float jumpForce,float crouchSpeed,float proneSpeed,float energy,float capacity,bool isMouseXAndY,bool isMouseX,bool isMouseY,bool lockCursor,bool hideCursor,bool swapAxis,bool isCameraDirection,bool isInputDirection,bool toggleRun,bool toggleCrouch,bool toggleProne,bool canRun,bool canJump,bool canCrouch,bool canProne,bool isRunning,bool isJumping,bool isCrouching,bool isProning,bool isGrounded)
			{
				EditorHandler(states);
				if(Application.isPlaying && states.Count > 0)EventHandler(states,isAlive,switchablePerspectives,isFirstPerson,isThirdPerson,isPlayerControlled,isAIControlled,walkSpeed,runSpeed,jumpForce,crouchSpeed,proneSpeed,energy,capacity,isMouseXAndY,isMouseX,isMouseY,lockCursor,hideCursor,swapAxis,isCameraDirection,isInputDirection,toggleRun,toggleCrouch,toggleProne,canRun,canJump,canCrouch,canProne,isRunning,isJumping,isCrouching,isProning,isGrounded);
			}
			private void EditorHandler (List<State> states)
			{
				for(int a = 0,countA = conditions.Count; a < countA; a++)
				{
					Condition condition = conditions[a];
					Condition.Usage usage = condition.usage;
					if(usage == Condition.Usage.States)
					{
						condition.index = Mathf.Clamp(condition.index,0,states.Count > 0 ? states.Count - 1 : 0);
						if((condition.type == Condition.Type.Float || condition.type == Condition.Type.Int) && condition.target != Condition.Target.Value)
							condition.target = Condition.Target.Value;
						if(condition.type == Condition.Type.Bool && condition.target == Condition.Target.Value)
							condition.target = Condition.Target.OnPositive;
					}
					else
					{
						if(condition.index != 0)condition.index = 0;
						if((condition.type == Condition.Type.Float || condition.type == Condition.Type.Int) && condition.target != Condition.Target.WalkSpeed && condition.target != Condition.Target.RunSpeed && condition.target != Condition.Target.JumpForce && condition.target != Condition.Target.CrouchSpeed && condition.target != Condition.Target.ProneSpeed && condition.target != Condition.Target.Energy && condition.target != Condition.Target.Capacity)
							condition.target = Condition.Target.WalkSpeed;
						if(condition.type == Condition.Type.Bool && condition.target != Condition.Target.IsAlive && condition.target != Condition.Target.SwitchablePerspectives && condition.target != Condition.Target.FirstPerson && condition.target != Condition.Target.ThirdPerson && condition.target != Condition.Target.PlayerControlled && condition.target != Condition.Target.AIControlled && condition.target != Condition.Target.MouseXAndY && condition.target != Condition.Target.MouseX && condition.target != Condition.Target.MouseY && condition.target != Condition.Target.LockCursor && condition.target != Condition.Target.HideCursor && condition.target != Condition.Target.InvertAxis && condition.target != Condition.Target.CameraDirection && condition.target != Condition.Target.InputDirection && condition.target != Condition.Target.ToggleRun && condition.target != Condition.Target.ToggleCrouch && condition.target != Condition.Target.ToggleProne && condition.target != Condition.Target.CanRun && condition.target != Condition.Target.CanJump && condition.target != Condition.Target.CanCrouch && condition.target != Condition.Target.CanProne && condition.target != Condition.Target.IsRunning && condition.target != Condition.Target.IsJumping && condition.target != Condition.Target.IsCrouching && condition.target != Condition.Target.IsProning && condition.target != Condition.Target.IsGrounded)
							condition.target = Condition.Target.IsAlive;
					}
				}
			}
			private void EventHandler (List<State> states,bool isAlive,bool switchablePerspectives,bool isFirstPerson,bool isThirdPerson,bool isPlayerControlled,bool isAIControlled,float walkSpeed,float runSpeed,float jumpForce,float crouchSpeed,float proneSpeed,float energy,float capacity,bool isMouseXAndY,bool isMouseX,bool isMouseY,bool lockCursor,bool hideCursor,bool swapAxis,bool isCameraDirection,bool isInputDirection,bool toggleRun,bool toggleCrouch,bool toggleProne,bool canRun,bool canJump,bool canCrouch,bool canProne,bool isRunning,bool isJumping,bool isCrouching,bool isProning,bool isGrounded)
			{
				if(conditions.Count != 0)
				{
					bool passed = false;
					List<bool> passes = new List<bool>();
					passes.Add(true);
					for(int a = 0,countA = conditions.Count,index = 0; a < countA; a++)
					{
						Condition condition = conditions[a];
						Condition.Usage usage = condition.usage;
						Condition.Type type = condition.type;
						Condition.Target target = condition.target;
						if(usage == Condition.Usage.States)
						{
							if(target == Condition.Target.Value)
							{
								Condition.Operator _operator = condition._operator;
								if(type == Condition.Type.Float)
								{
									if(_operator == Condition.Operator.EqualTo)
										condition.isPassed = states[condition.index].floatInput == condition.floatValue;
									else if(_operator == Condition.Operator.NotEqualTo)
										condition.isPassed = states[condition.index].floatInput != condition.floatValue;
									else if(_operator == Condition.Operator.GreaterThan)
										condition.isPassed = states[condition.index].floatInput > condition.floatValue;
									else if(_operator == Condition.Operator.LessThan)
										condition.isPassed = states[condition.index].floatInput < condition.floatValue;
									else if(_operator == Condition.Operator.GreaterThanOrEqualTo)
										condition.isPassed = states[condition.index].floatInput >= condition.floatValue;
									else condition.isPassed = states[condition.index].floatInput <= condition.floatValue;
								}
								else if(type == Condition.Type.Int)
								{
									if(_operator == Condition.Operator.EqualTo)
										condition.isPassed = states[condition.index].intInput == condition.intValue;
									else if(_operator == Condition.Operator.NotEqualTo)
										condition.isPassed = states[condition.index].intInput != condition.intValue;
									else if(_operator == Condition.Operator.GreaterThan)
										condition.isPassed = states[condition.index].intInput > condition.intValue;
									else if(_operator == Condition.Operator.LessThan)
										condition.isPassed = states[condition.index].intInput < condition.intValue;
									else if(_operator == Condition.Operator.GreaterThanOrEqualTo)
										condition.isPassed = states[condition.index].intInput >= condition.intValue;
									else condition.isPassed = states[condition.index].intInput <= condition.intValue;
								}
							}
							else if(type == Condition.Type.Bool)
							{
								if(target == Condition.Target.OnPositive)
									condition.isPassed = states[condition.index]._onPositive == condition.boolValue;
								else if(target == Condition.Target.OnNegative)
									condition.isPassed = states[condition.index]._onNegative == condition.boolValue;
								else if(target == Condition.Target.OnKeyDown)
									condition.isPassed = states[condition.index].onInputDown == condition.boolValue;
								else if(target == Condition.Target.OnKeyUp)
									condition.isPassed = states[condition.index].onInputUp == condition.boolValue;
								else if(target == Condition.Target.OnTrue)
									condition.isPassed = states[condition.index]._onTrue == condition.boolValue;
								else if(target == Condition.Target.OnFalse)
									condition.isPassed = states[condition.index]._onFalse == condition.boolValue;
								else if(target == Condition.Target.IsPositive)
									condition.isPassed = states[condition.index]._isPositive == condition.boolValue;
								else if(target == Condition.Target.IsNegative)
									condition.isPassed = states[condition.index]._isNegative == condition.boolValue;
								else if(target == Condition.Target.IsKeyDown)
									condition.isPassed = states[condition.index].isInputDown == condition.boolValue;
								else if(target == Condition.Target.IsKeyUp)
									condition.isPassed = states[condition.index].isInputUp == condition.boolValue;
								else if(target == Condition.Target.IsTrue)
									condition.isPassed = states[condition.index]._isTrue == condition.boolValue;
								else condition.isPassed = states[condition.index]._isFalse == condition.boolValue;
							}
						}
						else
						{
							if(type == Condition.Type.Float)
							{
								Condition.Operator _operator = condition._operator;
								float floatValue = 0;
								if(target == Condition.Target.WalkSpeed)floatValue = walkSpeed;
								else if(target == Condition.Target.RunSpeed)floatValue = runSpeed;
								else if(target == Condition.Target.JumpForce)floatValue = jumpForce;
								else if(target == Condition.Target.CrouchSpeed)floatValue = crouchSpeed;
								else if(target == Condition.Target.ProneSpeed)floatValue = proneSpeed;
								else if(target == Condition.Target.Energy)floatValue = energy;
								else floatValue = capacity;
								if(_operator == Condition.Operator.EqualTo)
									condition.isPassed = floatValue == condition.floatValue;
								else if(_operator == Condition.Operator.NotEqualTo)
									condition.isPassed = floatValue != condition.floatValue;
								else if(_operator == Condition.Operator.GreaterThan)
									condition.isPassed = floatValue > condition.floatValue;
								else if(_operator == Condition.Operator.LessThan)
									condition.isPassed = floatValue < condition.floatValue;
								else if(_operator == Condition.Operator.GreaterThanOrEqualTo)
									condition.isPassed = floatValue >= condition.floatValue;
								else condition.isPassed = floatValue <= condition.floatValue;
							}
							else if(type == Condition.Type.Int)
							{
								Condition.Operator _operator = condition._operator;
								int intValue = 0;
								if(target == Condition.Target.WalkSpeed)intValue = (int)walkSpeed;
								else if(target == Condition.Target.RunSpeed)intValue = (int)runSpeed;
								else if(target == Condition.Target.JumpForce)intValue = (int)jumpForce;
								else if(target == Condition.Target.CrouchSpeed)intValue = (int)crouchSpeed;
								else if(target == Condition.Target.ProneSpeed)intValue = (int)proneSpeed;
								else if(target == Condition.Target.Energy)intValue = (int)energy;
								else intValue = (int)capacity;
								if(_operator == Condition.Operator.EqualTo)
									condition.isPassed = intValue == condition.intValue;
								else if(_operator == Condition.Operator.NotEqualTo)
									condition.isPassed = intValue != condition.intValue;
								else if(_operator == Condition.Operator.GreaterThan)
									condition.isPassed = intValue > condition.intValue;
								else if(_operator == Condition.Operator.LessThan)
									condition.isPassed = intValue < condition.intValue;
								else if(_operator == Condition.Operator.GreaterThanOrEqualTo)
									condition.isPassed = intValue >= condition.intValue;
								else condition.isPassed = intValue <= condition.intValue;
							}
							else if(type == Condition.Type.Bool)
							{
								if(target == Condition.Target.IsAlive)
									condition.isPassed = isAlive == condition.boolValue;
								else if(target == Condition.Target.SwitchablePerspectives)
									condition.isPassed = switchablePerspectives == condition.boolValue;
								else if(target == Condition.Target.FirstPerson)
									condition.isPassed = isFirstPerson == condition.boolValue;
								else if(target == Condition.Target.ThirdPerson)
									condition.isPassed = isThirdPerson == condition.boolValue;
								else if(target == Condition.Target.PlayerControlled)
									condition.isPassed = isPlayerControlled == condition.boolValue;
								else if(target == Condition.Target.AIControlled)
									condition.isPassed = isAIControlled == condition.boolValue;
								else if(target == Condition.Target.MouseXAndY)
									condition.isPassed = isMouseXAndY == condition.boolValue;
								else if(target == Condition.Target.MouseX)
									condition.isPassed = isMouseX == condition.boolValue;
								else if(target == Condition.Target.MouseY)
									condition.isPassed = isMouseY == condition.boolValue;
								else if(target == Condition.Target.LockCursor)
									condition.isPassed = lockCursor == condition.boolValue;
								else if(target == Condition.Target.HideCursor)
									condition.isPassed = hideCursor == condition.boolValue;
								else if(target == Condition.Target.InvertAxis)
									condition.isPassed = swapAxis == condition.boolValue;
								else if(target == Condition.Target.CameraDirection)
									condition.isPassed = isCameraDirection == condition.boolValue;
								else if(target == Condition.Target.InputDirection)
									condition.isPassed = isInputDirection == condition.boolValue;
								else if(target == Condition.Target.ToggleRun)
									condition.isPassed = toggleRun == condition.boolValue;
								else if(target == Condition.Target.ToggleCrouch)
									condition.isPassed = toggleCrouch == condition.boolValue;
								else if(target == Condition.Target.ToggleProne)
									condition.isPassed = toggleProne == condition.boolValue;
								else if(target == Condition.Target.CanRun)
									condition.isPassed = canRun == condition.boolValue;
								else if(target == Condition.Target.CanJump)
									condition.isPassed = canJump == condition.boolValue;
								else if(target == Condition.Target.CanCrouch)
									condition.isPassed = canCrouch == condition.boolValue;
								else if(target == Condition.Target.CanProne)
									condition.isPassed = canProne == condition.boolValue;
								else if(target == Condition.Target.IsRunning)
									condition.isPassed = isRunning == condition.boolValue;
								else if(target == Condition.Target.IsJumping)
									condition.isPassed = isJumping == condition.boolValue;
								else if(target == Condition.Target.IsCrouching)
									condition.isPassed = isCrouching == condition.boolValue;
								else if(target == Condition.Target.IsProning)
									condition.isPassed = isProning == condition.boolValue;
								else condition.isPassed = isGrounded == condition.boolValue;
							}
						}
						if(a > 0 && condition.statement == Condition.Statement.Or)
						{
							passes.Add(true);
							index++;
						}
						if(!condition.isPassed)passes[index] = false;
					}
					for(int a = 0,countA = passes.Count; a < countA; a++)
					{
						if(passes[a])
						{
							passed = true;
							break;
						}
					}
					if(passed)
					{
						if(!_isTrue)
						{
							onTrue.Invoke();
							_isFalse = false;
							_isTrue = true;
						}
						isTrue.Invoke();
					}
					else
					{
						if(!_isFalse)
						{
							onFalse.Invoke();
							_isFalse = true;
							_isTrue = false;
						}
						isFalse.Invoke();
					}
				}
			}
		}
		[System.Serializable] public class Parameter
		{
			public enum Usage : byte {States,Character}
			public enum Type : byte {Float,Int,Bool}
			public enum Target : byte {Value,OnPositive,OnNegative,OnKeyDown,OnKeyUp,OnTrue,OnFalse,IsPositive,IsNegative,IsKeyDown,IsKeyUp,IsTrue,IsFalse,IsAlive,SwitchablePerspectives,FirstPerson,ThirdPerson,PlayerControlled,AIControlled,WalkSpeed,RunSpeed,JumpForce,CrouchSpeed,ProneSpeed,Energy,Capacity,MouseXAndY,MouseX,MouseY,LockCursor,HideCursor,InvertAxis,CameraDirection,InputDirection,ToggleRun,ToggleCrouch,ToggleProne,CanRun,CanJump,CanCrouch,CanProne,IsGrounded}
			public string name = "Untitled";
			public Usage usage = Usage.States;
			public int index = 0;
			public Type type = Type.Float;
			public Target target = Target.Value;
			[HideInInspector] public float floatValue = 0;
			[HideInInspector] public int intValue = 0;
			[HideInInspector] public bool boolValue = false;
			#if UNITY_EDITOR
			[HideInInspector] public bool isExpanded = false;
			#endif
			public void Update (List<State> states,bool isAlive,bool switchablePerspectives,bool isFirstPerson,bool isThirdPerson,bool isPlayerControlled,bool isAIControlled,float walkSpeed,float runSpeed,float jumpForce,float crouchSpeed,float proneSpeed,float energy,float capacity,bool isMouseXAndY,bool isMouseX,bool isMouseY,bool lockCursor,bool hideCursor,bool swapAxis,bool isCameraDirection,bool isInputDirection,bool toggleRun,bool toggleCrouch,bool toggleProne,bool canRun,bool canJump,bool canCrouch,bool canProne,bool isGrounded)
			{
				if(usage == Usage.States)
				{
					index = Mathf.Clamp(index,0,states.Count > 0 ? states.Count - 1 : 0);
					if((type == Type.Float || type == Type.Int) && target != Target.Value)
						target = Target.Value;
					if(type == Type.Bool && target == Target.Value)
						target = Target.OnPositive;
				}
				else
				{
					if(index != 0)index = 0;
					if((type == Type.Float || type == Type.Int) && target != Target.WalkSpeed && target != Target.RunSpeed && target != Target.JumpForce && target != Target.CrouchSpeed && target != Target.ProneSpeed && target != Target.Energy && target != Target.Capacity)
						target = Target.WalkSpeed;
					if(type == Type.Bool && target != Target.IsAlive && target != Target.SwitchablePerspectives && target != Target.FirstPerson && target != Target.ThirdPerson && target != Target.PlayerControlled && target != Target.AIControlled && target != Target.MouseXAndY && target != Target.MouseX && target != Target.MouseY && target != Target.LockCursor && target != Target.HideCursor && target != Target.InvertAxis && target != Target.CameraDirection && target != Target.InputDirection && target != Target.ToggleRun && target != Target.ToggleCrouch && target != Target.ToggleProne && target != Target.CanRun && target != Target.CanJump && target != Target.CanCrouch && target != Target.CanProne && target != Target.IsGrounded)
						target = Target.IsAlive;
				}
				if(Application.isPlaying)
				{
					if(usage == Usage.States)
					{
						if(states.Count > 0)
						{
							if(type == Type.Float)
								floatValue = states[index].floatInput;
							else if(type == Type.Int)
								intValue = states[index].intInput;
							else if(type == Type.Bool)
							{
								if(target == Target.OnPositive)
									boolValue = states[index]._onPositive;
								else if(target == Target.OnNegative)
									boolValue = states[index]._onNegative;
								else if(target == Target.OnKeyDown)
									boolValue = states[index].onInputDown;
								else if(target == Target.OnKeyUp)
									boolValue = states[index].onInputUp;
								else if(target == Target.OnTrue)
									boolValue = states[index]._onTrue;
								else if(target == Target.OnFalse)
									boolValue = states[index]._onFalse;
								else if(target == Target.IsPositive)
									boolValue = states[index]._isPositive;
								else if(target == Target.IsNegative)
									boolValue = states[index]._isNegative;
								else if(target == Target.IsKeyDown)
									boolValue = states[index].isInputDown;
								else if(target == Target.IsKeyUp)
									boolValue = states[index].isInputUp;
								else if(target == Target.IsTrue)
									boolValue = states[index]._isTrue;
								else boolValue = states[index]._isFalse;
							}
						}
						else
						{
							floatValue = 0;
							intValue = 0;
							boolValue = false;
						}
					}
					else
					{
						if(type == Type.Float)
						{
							if(target == Target.WalkSpeed)floatValue = walkSpeed;
							else if(target == Target.RunSpeed)floatValue = runSpeed;
							else if(target == Target.JumpForce)floatValue = jumpForce;
							else if(target == Target.CrouchSpeed)floatValue = crouchSpeed;
							else if(target == Target.ProneSpeed)floatValue = proneSpeed;
							else if(target == Target.Energy)floatValue = energy;
							else floatValue = capacity;
						}
						else if(type == Type.Int)
						{
							if(target == Target.WalkSpeed)floatValue = (int)walkSpeed;
							else if(target == Target.RunSpeed)floatValue = (int)runSpeed;
							else if(target == Target.JumpForce)floatValue = (int)jumpForce;
							else if(target == Target.CrouchSpeed)floatValue = (int)crouchSpeed;
							else if(target == Target.ProneSpeed)floatValue = (int)proneSpeed;
							else if(target == Target.Energy)floatValue = (int)energy;
							else floatValue = (int)capacity;
						}
						else
						{
							if(target == Target.IsAlive)boolValue = isAlive;
							else if(target == Target.SwitchablePerspectives)boolValue = switchablePerspectives;
							else if(target == Target.FirstPerson)boolValue = isFirstPerson;
							else if(target == Target.ThirdPerson)boolValue = isThirdPerson;
							else if(target == Target.PlayerControlled)boolValue = isPlayerControlled;
							else if(target == Target.AIControlled)boolValue = isAIControlled;
							else if(target == Target.MouseXAndY)boolValue = isMouseXAndY;
							else if(target == Target.MouseX)boolValue = isMouseX;
							else if(target == Target.MouseY)boolValue = isMouseY;
							else if(target == Target.LockCursor)boolValue = lockCursor;
							else if(target == Target.HideCursor)boolValue = hideCursor;
							else if(target == Target.InvertAxis)boolValue = swapAxis;
							else if(target == Target.CameraDirection)boolValue = isCameraDirection;
							else if(target == Target.InputDirection)boolValue = isInputDirection;
							else if(target == Target.ToggleRun)boolValue = toggleRun;
							else if(target == Target.ToggleCrouch)boolValue = toggleCrouch;
							else if(target == Target.ToggleProne)boolValue = toggleProne;
							else if(target == Target.CanRun)boolValue = canRun;
							else if(target == Target.CanJump)boolValue = canJump;
							else if(target == Target.CanCrouch)boolValue = canCrouch;
							else if(target == Target.CanProne)boolValue = canProne;
							else boolValue = isGrounded;
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
				Cursor.lockState = !lockCursor ? CursorLockMode.None : CursorLockMode.Locked;
				Cursor.visible = !hideCursor;
				InputHandler();
				GroundHandler();
				for(int a = 0,countA = states.Count; a < countA; a++)
					states[a].Update();
			}
			for(int a = 0,countA = events.Count; a < countA; a++)
				events[a].Update(states,isAlive,switchablePerspectives,perspectiveType == PerspectiveType.FirstPerson,perspectiveType == PerspectiveType.ThirdPerson,controlType == ControlType.PlayerControlled,controlType == ControlType.AIControlled,walkSpeed,runSpeed,jumpForce,crouchSpeed,proneSpeed,energy,capacity,rotationType == RotationType.MouseXAndY,rotationType == RotationType.MouseX,rotationType == RotationType.MouseY,lockCursor,hideCursor,swapAxis,thirdPersonDirection == ThirdPersonDirection.CameraDirection,thirdPersonDirection == ThirdPersonDirection.InputDirection,toggleRun,toggleCrouch,toggleProne,canRun,canJump,canCrouch,canProne,isRunning,isJumping,isCrouching,isProning,isGrounded);
			if(Application.isPlaying)
			{
				if(isAlive)
				{
					ValueHandler();
					MovementInputHandler();
				}
				if(isControlled)
				{
					if(switchablePerspectives && onPerspectiveInput)SwitchPerspective();
					if(useEnergy && useEnergyUsages)EnergyHandler();
				}
				if(useRagdoll)RagdollHandler();
			}
			for(int a = 0,countA = parameters.Count; a < countA; a++)
				parameters[a].Update(states,isAlive,switchablePerspectives,perspectiveType == PerspectiveType.FirstPerson,perspectiveType == PerspectiveType.ThirdPerson,controlType == ControlType.PlayerControlled,controlType == ControlType.AIControlled,walkSpeed,runSpeed,jumpForce,crouchSpeed,proneSpeed,energy,capacity,rotationType == RotationType.MouseXAndY,rotationType == RotationType.MouseX,rotationType == RotationType.MouseY,lockCursor,hideCursor,swapAxis,thirdPersonDirection == ThirdPersonDirection.CameraDirection,thirdPersonDirection == ThirdPersonDirection.InputDirection,toggleRun,toggleCrouch,toggleProne,canRun,canJump,canCrouch,canProne,isGrounded);
			if(Application.isPlaying && useAnimator)AnimatorHandler();
		}
		private void EditorHandler ()
		{
			if(!capsuleCollider || capsuleCollider.gameObject != gameObject)
			{
				capsuleCollider = GetComponent<CapsuleCollider>();
				if(!capsuleCollider)capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
			}
			if(!rigidbody || rigidbody.gameObject != gameObject)
			{
				rigidbody = GetComponent<Rigidbody>();
				if(!rigidbody)rigidbody = gameObject.AddComponent<Rigidbody>();
			}
			if(!rigidbody.freezeRotation)
				rigidbody.freezeRotation = true;
			#if UNITY_EDITOR
			if(!useAnimator && automaticallyAddParameters)
				automaticallyAddParameters = false;
			#endif
			walkSpeed = Mathf.Clamp(walkSpeed,0,float.MaxValue);
			runSpeed = Mathf.Clamp(runSpeed,walkSpeed,float.MaxValue);
			jumpForce = Mathf.Clamp(jumpForce,0,float.MaxValue);
			crouchSpeed = Mathf.Clamp(crouchSpeed,0,walkSpeed);
			proneSpeed = Mathf.Clamp(proneSpeed,0,crouchSpeed);
			energy = Mathf.Clamp(energy,0,capacity);
			idleUsage = Mathf.Clamp(idleUsage,0.0001f,capacity);
			walkUsage = Mathf.Clamp(walkUsage,idleUsage,capacity);
			runUsage = Mathf.Clamp(runUsage,walkUsage,capacity);
			jumpUsage = Mathf.Clamp(jumpUsage,0.0001f,capacity);
			crouchUsage = Mathf.Clamp(crouchUsage,0.0001f,walkUsage);
			proneUsage = Mathf.Clamp(proneUsage,0.0001f,crouchUsage);
			turnDamping = Mathf.Clamp(turnDamping,0,720);
			minimumX = Mathf.Clamp(minimumX,-180,0);
			maximumX = Mathf.Clamp(maximumX,0,180);
			minimumY = Mathf.Clamp(minimumY,-80,0);
			maximumY = Mathf.Clamp(maximumY,0,80);
			rotationDamping = Mathf.Clamp01(rotationDamping);
			firstPersonSwitchTime = Mathf.Clamp(firstPersonSwitchTime,0.0001f,float.MaxValue);
			thirdPersonSwitchTime = Mathf.Clamp(thirdPersonSwitchTime,0.0001f,float.MaxValue);
		}
		private void InputHandler ()
		{
			if(isAlive && controlType == ControlType.PlayerControlled)
			{
				InputHandler(horizontalAxis,-1,1,ref movementInput.x);
				InputHandler(verticalAxis,-1,1,ref movementInput.y);
				InputHandler(mouseXAxis,-Mathf.Abs(sensitivityRange.x),Mathf.Abs(sensitivityRange.x),ref mouseInput.x);
				InputHandler(mouseYAxis,-Mathf.Abs(sensitivityRange.y),Mathf.Abs(sensitivityRange.y),ref mouseInput.y);
				InputHandler(horizontalAxis,true,false,ref isHorizontalInput);
				InputHandler(verticalAxis,true,false,ref isVerticalInput);
				InputHandler(runAxis,true,toggleRun,ref isRunInput);
				InputHandler(jumpAxis,true,true,ref onJumpInput);
				InputHandler(crouchAxis,true,toggleCrouch,ref isCrouchInput);
				InputHandler(proneAxis,true,toggleProne,ref isProneInput);
				InputHandler(perspectiveAxis,switchablePerspectives,true,ref onPerspectiveInput);
			}
			if(!isControlled)
			{
				movementInput = Vector2.zero;
				if(perspectiveType == PerspectiveType.FirstPerson)
					mouseInput = Vector2.zero;
				isHorizontalInput = false;
				isVerticalInput = false;
				isRunInput = false;
				onJumpInput = false;
				isCrouchInput = false;
				isProneInput = false;
				onPerspectiveInput = false;
			}
		}
		private void InputHandler (string axis,float minimum,float maximum,ref float value)
		{
			try
			{
				if(axis != string.Empty)value = Mathf.Clamp(Input.GetAxis(axis),minimum,maximum);
				else value = 0;
			}
			catch
			{
				value = 0;
				Debug.LogError("The name '" + axis + "' is incorrect or does not exist, you can empty the field if you wish not to use it",this);
			}
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
		private void GroundHandler ()
		{
			RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position + capsuleCollider.center + new Vector3(0,(-capsuleCollider.height * transform.localScale.y) * 0.5f + (capsuleCollider.radius * 0.9f) + 0.01f,0),capsuleCollider.radius * 0.9f,-transform.up,0.02f);
			isGrounded = false;
			if(raycastHits.Length > 0)for(int a = raycastHits.Length - 2; a >= 0 ; a--)if(!raycastHits[a].collider.isTrigger)
			{
				isGrounded = true;
				break;
			}
			if(isGrounded)
			{
				rigidbody.drag = rigidbody.mass * 0.25f;
				if(canJump && onJumpInput)rigidbody.drag = 0;
				if(!isJumping && Mathf.Abs(movementInput.x) < float.Epsilon && Mathf.Abs(movementInput.y) < float.Epsilon && rigidbody.velocity.magnitude < 1)
					rigidbody.Sleep();
			}
			else rigidbody.drag = 0;
		}
		private void ValueHandler ()
		{
			if(canRun)
			{
				if(!toggleRun)isRunning = isRunInput;
				else if(isRunInput && !isRunning)isRunning = true;
			}
			else if(isRunning)isRunning = false;
			if(canJump && isGrounded && onJumpInput)
			{
				onJump = true;
				isJumping = true;
				isGrounded = false;
			}
			if(isGrounded && !onJump && isJumping)isJumping = false;
			if(!canJump && onJump)onJump = false;
			if(canCrouch)
			{
				if(!toggleCrouch)
				{
					isCrouching = isCrouchInput;
					if(isCrouching && isProning)isProning = false;
				}
				else if(isCrouchInput)
				{
					isCrouching = !isCrouching;
					if(isCrouching && isProning)isProning = false;
				}
			}
			else if(isCrouching)isCrouching = false;
			if(canProne)
			{
				if(!toggleProne)
				{
					isProning = isProneInput;
					if(isProning && isCrouching)isCrouching = false;
				}
				else if(isProneInput)
				{
					isProning = !isProning;
					if(isProning && isCrouching)isCrouching = false;
				}
			}
			else if(isProning)isProning = false;
			if(isRunning)
			{
				bool enabled = true;
				if(!isHorizontalInput && !isVerticalInput)
				{
					isRunning = false;
					enabled = false;
				}
				if(isCrouching && enabled)isCrouching = false;
				if(isProning && enabled)isProning = false;
			}
		}
		[HideInInspector] private Vector2 lastMovementInput = Vector2.zero;
		private void MovementInputHandler ()
		{
			Vector2 movement = new Vector2(useSmoothMovements && !isHorizontalInput ? 0 : (!isProning ? (!isCrouching ? (!isRunning ? walkSpeed : runSpeed) : crouchSpeed) : proneSpeed),useSmoothMovements && !isVerticalInput ? 0 : (!isProning ? (!isCrouching ? (!isRunning ? walkSpeed : runSpeed) : crouchSpeed) : proneSpeed));
			if(useSmoothMovements)
			{
				lastMovementInput.x = (int)Input.GetAxisRaw(horizontalAxis);
				lastMovementInput.y = (int)Input.GetAxisRaw(verticalAxis);
				Vector2 movementInput = Vector2.ClampMagnitude(this.lastMovementInput,1);
				this.movement.x = Mathf.MoveTowards(this.movement.x,movement.x * movementInput.x,!isHorizontalInput && !isVerticalInput ? idleDamping : (!isProning ? (!isCrouching ? (!isRunning ? walkDamping : runDamping) : crouchDamping) : proneSpeed));
				this.movement.y = Mathf.MoveTowards(this.movement.y,movement.y * movementInput.y,!isHorizontalInput && !isVerticalInput ? idleDamping : (!isProning ? (!isCrouching ? (!isRunning ? walkDamping : runDamping) : crouchDamping) : proneSpeed));
			}
			else this.movement = new Vector2(movement.x,movement.y);
		}
		private void EnergyHandler ()
		{
			if(!isHorizontalInput && !isVerticalInput)energy -= idleUsage * Time.deltaTime;
			else
			{
				if(!isRunning)
				{
					if(!isCrouching && !isProning)energy -= walkUsage * Time.deltaTime;
					else
					{
						if(isCrouching)energy -= crouchUsage * Time.deltaTime;
						if(isProning)energy -= proneUsage * Time.deltaTime;
					}
				}
				else energy -= runUsage * Time.deltaTime;
			}
			if(isJumping)energy -= jumpUsage;
		}
		private void RagdollHandler ()
		{
			if(animator && animator.enabled == ragdollIsActive)animator.enabled = !ragdollIsActive;
			for(int a = 0,countA = ragdollBodies.Count; a < countA; a++)if(ragdollBodies[a])
			{
				Rigidbody body = ragdollBodies[a];
				Collider collider = body.GetComponent<Collider>();
				if(body.useGravity != ragdollIsActive)
					body.useGravity = ragdollIsActive;
				if(body.isKinematic == ragdollIsActive)
					body.isKinematic = !ragdollIsActive;
				if(collider && collider.isTrigger == ragdollIsActive)
					collider.isTrigger = !ragdollIsActive;
			}
		}
		private void AnimatorHandler ()
		{
			if(animator)
			{
				if(animator.parameters.Length > 0)for(int a = 0,countA = animator.parameters.Length; a < countA; a++)
				{
					AnimatorControllerParameter animatorParameter = animator.parameters[a];
					if(animatorParameter.name == verticalParameter)animator.SetFloat(verticalParameter,movementInput.y);
					if(animatorParameter.name == horizontalParameter)animator.SetFloat(horizontalParameter,movementInput.x);
					if(animatorParameter.name == runParameter)animator.SetBool(runParameter,isRunning);
					if(animatorParameter.name == jumpParameter)animator.SetBool(jumpParameter,isJumping);
					if(animatorParameter.name == crouchParameter)animator.SetBool(crouchParameter,isCrouching);
					if(animatorParameter.name == proneParameter)animator.SetBool(proneParameter,isProning);
					if(states.Count > 0)for(int b = 0,countB = parameters.Count; b < countB; b++)
					{
						Parameter parameter = parameters[b];
						if(animatorParameter.name == parameter.name)
						{
							if(parameter.type == Parameter.Type.Float)animator.SetFloat(parameter.name,parameter.floatValue);
							if(parameter.type == Parameter.Type.Int)animator.SetInteger(parameter.name,parameter.intValue);
							if(parameter.type == Parameter.Type.Bool)animator.SetBool(parameter.name,parameter.boolValue);
						}
					}
				}
			}
		}
		private void FixedUpdate ()
		{
			if(Application.isPlaying)MovementHandler();
		}
		private void MovementHandler ()
		{
			if(isControlled && (isGrounded || airControl))
			{
				Vector2 movementInput = Vector2.ClampMagnitude(this.movementInput,1);
				Vector3 movement = new Vector3(!useSmoothMovements ? this.movement.x * movementInput.x : this.movement.x,0,!useSmoothMovements ? this.movement.y * movementInput.y : this.movement.y);
				if(airControl && !isGrounded)
				{
					movement.x *= 0.9f;
					movement.z *= 0.9f;
				}
				if(perspectiveType == PerspectiveType.ThirdPerson && thirdPersonDirection == ThirdPersonDirection.InputDirection)if(this.movement.x != 0 || this.movement.y != 0)
				{
					Vector3 sidewaysDirection = Vector3.Scale(camera ? camera.right : Vector3.right,new Vector3(1,0,1)).normalized;
					Vector3 forwardDirection = Vector3.Scale(camera ? camera.forward : Vector3.forward,new Vector3(1,0,1)).normalized;
					movement = sidewaysDirection * movement.x + Vector3.up * rigidbody.velocity.y + forwardDirection * movement.z;
				}
				if(perspectiveType == PerspectiveType.FirstPerson || perspectiveType == PerspectiveType.ThirdPerson && thirdPersonDirection == ThirdPersonDirection.CameraDirection)
					movement = transform.right * movement.x + Vector3.up * rigidbody.velocity.y + transform.forward * movement.z;
				rigidbody.velocity = movement;
				if(onJump)
				{
					rigidbody.velocity += transform.up * jumpForce;
					onJump = false;
					isGrounded = false;
				}
			}
			rigidbody.velocity -= Vector3.up * (rigidbody.mass * 0.004f) * (Time.fixedDeltaTime * 50);
		}
		private void LateUpdate ()
		{
			if(Application.isPlaying)
			{
				if(isControlled && perspectiveType == PerspectiveType.FirstPerson)Look();
				if(perspectiveType == PerspectiveType.ThirdPerson)Orbit();
			}
		}
		[HideInInspector] private float cameraDistance = 0;
		[HideInInspector] private float firstPersonCounter = 0;
		[HideInInspector] private float thirdPersonCounter = 0;
		[HideInInspector] private bool isFirstPerson = false;
		[HideInInspector] private bool isThirdPerson = false;
		[HideInInspector] private Vector2 rotation = Vector2.zero;
		[HideInInspector] private Vector2 smoothRotation = Vector2.zero;
		[HideInInspector] private Vector3 switchOffset = Vector3.zero;
		private void Look ()
		{
			CalculationHandler();
			ClampHandler();
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x,smoothRotation.x,transform.eulerAngles.z);
			if(camera)
			{
				Vector3 position = Vector3.zero;
				Quaternion rotation = Quaternion.Euler((!useParentalRotationX || !camera.parent ? transform.eulerAngles.x : camera.parent.eulerAngles.x) + smoothRotation.y,(!useParentalRotationY || !camera.parent ? transform.eulerAngles.y : camera.parent.eulerAngles.y),0);
				Vector3 offset = (!useParentalOffset || !camera.parent ? transform.position : camera.parent.position) + firstPersonWorldOffset + (rotation * firstPersonSelfOffset) + (transform.rotation * firstPersonLocalOffset);
				if(!isFirstPerson)isFirstPerson = true;
				if(isThirdPerson)
				{
					firstPersonCounter = (thirdPersonSwitchTime - thirdPersonCounter) / thirdPersonSwitchTime * firstPersonSwitchTime;
					thirdPersonCounter = 0;
					isThirdPerson = false;
				}
				switchOffset = Vector3.Lerp(switchOffset,offset,firstPersonCounter / firstPersonSwitchTime);
				position = rotation * new Vector3(0,0,-cameraDistance * (1 - (firstPersonCounter / firstPersonSwitchTime))) + switchOffset;
				if(useSmoothPerspectives && firstPersonCounter <= firstPersonSwitchTime)
				{
					camera.position = position;
					firstPersonCounter += Time.deltaTime;
					if(firstPersonCounter > firstPersonSwitchTime)
						firstPersonCounter = firstPersonSwitchTime + 0.0001f;
				}
				else camera.position = position;
				camera.rotation = rotation;
			}
		}
		private void Orbit ()
		{
			if(isAlive && isControlled)
			{
				if(thirdPersonDirection == ThirdPersonDirection.CameraDirection)
				{
					if(transform.rotation != Quaternion.Euler(transform.eulerAngles.x,Mathf.MoveTowardsAngle(transform.eulerAngles.y,camera ? camera.eulerAngles.y : 0,turnDamping * Time.deltaTime),transform.eulerAngles.z))
						transform.rotation = Quaternion.Euler(transform.eulerAngles.x,Mathf.MoveTowardsAngle(transform.eulerAngles.y,camera ? camera.eulerAngles.y : 0,turnDamping * Time.deltaTime),transform.eulerAngles.z);
				}
				if(thirdPersonDirection == ThirdPersonDirection.InputDirection && (movementInput.x != 0 || movementInput.y != 0))
				{
					if(transform.rotation != Quaternion.Euler(transform.eulerAngles.x,Mathf.MoveTowardsAngle(transform.eulerAngles.y,Quaternion.LookRotation(new Vector3(movementInput.x,0,movementInput.y)).eulerAngles.y + camera.eulerAngles.y,turnDamping * Time.deltaTime),transform.eulerAngles.z))
						transform.rotation = Quaternion.Euler(transform.eulerAngles.x,Mathf.MoveTowardsAngle(transform.eulerAngles.y,Quaternion.LookRotation(new Vector3(movementInput.x,0,movementInput.y)).eulerAngles.y + camera.eulerAngles.y,turnDamping * Time.deltaTime),transform.eulerAngles.z);
				}
			}
			if(camera)
			{
				CalculationHandler();
				ClampHandler();
				CollisionHandler();
				camera.rotation = Quaternion.Euler(smoothRotation.y,smoothRotation.x,0);
			}
		}
		private void CalculationHandler ()
		{
			if(rotationType == RotationType.MouseX || rotationType == RotationType.MouseXAndY)
			{
				if(!swapAxis && mouseInput.x != 0 || swapAxis && mouseInput.y != 0)
					rotation.x += (!swapAxis ? mouseInput.x : mouseInput.y) * sensitivity.x * (!useTimeScaleX ? 1 : Time.timeScale) * (!invertAxisX ? 1 : -1);
				smoothRotation.x = Mathf.Lerp(smoothRotation.x,rotation.x,rotationDamping * (!useDeltaTimeX ? 1 : Time.unscaledDeltaTime * 30));
			}
			if(rotationType == RotationType.MouseY || rotationType == RotationType.MouseXAndY)
			{
				if(!swapAxis && mouseInput.y != 0 || swapAxis && mouseInput.x != 0)
					rotation.y -= (!swapAxis ? mouseInput.y : mouseInput.x) * sensitivity.y * (!useTimeScaleY ? 1 : Time.timeScale) * (!invertAxisY ? 1 : -1);
				smoothRotation.y = Mathf.Lerp(smoothRotation.y,rotation.y,rotationDamping * (!useDeltaTimeY ? 1 : Time.unscaledDeltaTime * 30));
			}
		}
		private void ClampHandler ()
		{
			if(clampX)
			{
				rotation.x = Mathf.Clamp(rotation.x,minimumX,maximumX);
				smoothRotation.x = Mathf.Clamp(smoothRotation.x,minimumX,maximumX);
			}
			else
			{
				if(smoothRotation.x < -360)
				{
					rotation.x += 360;
					smoothRotation.x += 360;
				}
				if(smoothRotation.x > 360)
				{
					rotation.x -= 360;
					smoothRotation.x -= 360;
				}
			}
			if(clampY)
			{
				rotation.y = Mathf.Clamp(rotation.y,minimumY,maximumY);
				smoothRotation.y = Mathf.Clamp(smoothRotation.y,minimumY,maximumY);
			}
			else
			{
				if(smoothRotation.y < -360)
				{
					rotation.y += 360;
					smoothRotation.y += 360;
				}
				if(smoothRotation.y > 360)
				{
					rotation.y -= 360;
					smoothRotation.y -= 360;
				}
			}
		}
		private void CollisionHandler ()
		{
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.Euler(smoothRotation.y,smoothRotation.x,0);
			Vector3 offset = (!useParentalOffset || !camera.parent ? transform.position : camera.parent.position) + thirdPersonWorldOffset + (rotation * thirdPersonSelfOffset) + (transform.rotation * thirdPersonLocalOffset);
			if(!isThirdPerson)isThirdPerson = true;
			if(isFirstPerson)
			{
				thirdPersonCounter = (firstPersonSwitchTime - firstPersonCounter) / firstPersonSwitchTime * thirdPersonSwitchTime;
				firstPersonCounter = 0;
				isFirstPerson = false;
			}
			RaycastHit[] raycastHits = Physics.SphereCastAll(offset,clipRadius,rotation * Vector3.back,distance * (thirdPersonCounter / thirdPersonSwitchTime),dontClip);
			switchOffset = Vector3.Lerp(switchOffset,offset,thirdPersonCounter / thirdPersonSwitchTime);
			position = rotation * new Vector3(0,0,-distance * (thirdPersonCounter / thirdPersonSwitchTime)) + switchOffset;
			cameraDistance = distance;
			if(raycastHits.Length > 0)
			{
				for(int a = raycastHits.Length - 2; a >= 0 ; a--)if(!raycastHits[a].collider.isTrigger && raycastHits[a].distance < cameraDistance)
					cameraDistance = raycastHits[a].distance;
				position = rotation * new Vector3(0,0,-cameraDistance) + offset;
			}
			if(useSmoothPerspectives && thirdPersonCounter <= thirdPersonSwitchTime)
			{
				camera.position = position;
				thirdPersonCounter += Time.deltaTime;
				if(thirdPersonCounter > thirdPersonSwitchTime)
					thirdPersonCounter = thirdPersonSwitchTime + 0.0001f;
			}
			else camera.position = position;
		}
		private void Awake ()
		{
			if(!capsuleCollider || capsuleCollider.gameObject != gameObject)
			{
				capsuleCollider = GetComponent<CapsuleCollider>();
				if(!capsuleCollider)capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
			}
			if(!rigidbody || rigidbody.gameObject != gameObject)
			{
				rigidbody = GetComponent<Rigidbody>();
				if(!rigidbody)rigidbody = gameObject.AddComponent<Rigidbody>();
			}
			if(Application.isPlaying)
			{
				Collider[] colliders = GetComponentsInChildren<Collider>();
				if(useSmoothPerspectives && enabled)
				{
					if(perspectiveType == PerspectiveType.FirstPerson)
					{
						firstPersonCounter = firstPersonSwitchTime + 0.0001f;
						switchOffset = firstPersonWorldOffset + (transform.rotation * firstPersonLocalOffset);
						if(camera)camera.position = transform.position + firstPersonWorldOffset + (transform.rotation * firstPersonLocalOffset);
					}
					if(perspectiveType == PerspectiveType.ThirdPerson)
					{
						thirdPersonCounter = thirdPersonSwitchTime + 0.0001f;
						switchOffset = thirdPersonWorldOffset + (transform.rotation * thirdPersonLocalOffset);
						if(camera)camera.position = new Vector3(0,0,-distance) + transform.position + thirdPersonWorldOffset + (transform.rotation * thirdPersonLocalOffset);
					}
				}
				for(int a = 0,countA = colliders.Length; a < countA; a++)if(colliders[a].enabled)
					Physics.IgnoreCollision(capsuleCollider,colliders[a],true);
				if(useRagdoll)for(int a = 0,countA = ragdollBodies.Count; a < countA; a++)if(ragdollBodies[a])
				{
					Rigidbody body = ragdollBodies[a];
					Collider collider = body.GetComponent<Collider>();
					body.useGravity = ragdollIsActive;
					body.isKinematic = !ragdollIsActive;
					if(collider)
					{
						Physics.IgnoreCollision(capsuleCollider,collider,true);
						collider.isTrigger = !ragdollIsActive;
					}
				}
			}
		}
		public void SetHorizontalInput (float value) {if(movementInput.x != value)movementInput.x = value;}
		public void SetVerticalInput (float value) {if(movementInput.y != value)movementInput.y = value;}
		public void SetMouseXInput (float value) {if(mouseInput.x != value)mouseInput.x = value;}
		public void SetMouseYInput (float value) {if(mouseInput.y != value)mouseInput.y = value;}
		public void SetOnJumpInput (bool value) {if(onJumpInput != value)onJumpInput = value;}
		public void SetOnPerspectiveInput (bool value) {if(onPerspectiveInput != value)onPerspectiveInput = value;}
		public void SetIsHorizontalInput (bool value) {if(isHorizontalInput != value)isHorizontalInput = value;}
		public void SetIsVerticalInput (bool value) {if(isVerticalInput != value)isVerticalInput = value;}
		public void SetIsRunInput (bool value) {if(isRunInput != value)isRunInput = value;}
		public void SetIsCrouchInput (bool value) {if(isCrouchInput != value)isCrouchInput = value;}
		public void SetIsProneInput (bool value) {if(isProneInput != value)isProneInput = value;}
		public void Run () {if(canRun && !isRunning)isRunning = true;}
		public void Jump ()
		{
			if(canJump && isGrounded && !onJump && !isJumping)
			{
				onJump = true;
				isJumping = true;
				isGrounded = false;
			}
		}
		public void Crouch () {if(canCrouch && !isCrouching)isCrouching = true;}
		public void Prone () {if(canProne && !isProning)isProning = true;}
		public void SwitchPerspective ()
		{
			if(isControlled && perspectiveType == PerspectiveType.ThirdPerson)
			{
				perspectiveType = PerspectiveType.FirstPerson;
				return;
			}
			if(isControlled && perspectiveType == PerspectiveType.FirstPerson)
			{
				perspectiveType = PerspectiveType.ThirdPerson;
				return;
			}
		}
		public void ActivateRagdoll () {if(!ragdollIsActive)ragdollIsActive = true;}
		public void DeactivateRagdoll () {if(ragdollIsActive)ragdollIsActive = false;}
		public void IsRunning (bool value) {if(isRunning != value)isRunning = value;}
		public void IsCrouching (bool value) {if(isCrouching != value)isCrouching = value;}
		public void IsProning (bool value) {if(isProning != value)isProning = value;}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(CharacterController)),CanEditMultipleObjects]
	public class CharacterControllerEditor : Editor
	{
		private CharacterController[] characterControllers
		{
			get
			{
				CharacterController[] characterControllers = new CharacterController[targets.Length];
				for(int characterControllersIndex = 0; characterControllersIndex < targets.Length; characterControllersIndex++)
					characterControllers[characterControllersIndex] = (CharacterController)targets[characterControllersIndex];
				return characterControllers;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			StatsSection();
			ConsoleSection();
			TabsSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)
					EditorUtility.SetDirty(characterControllers[characterControllersIndex]);
			}
		}
		private void StatsSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("Stats",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					GUI.backgroundColor = characterControllers[0].isAlive ? Color.green : Color.red;
					if(GUILayout.Button("Alive"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].isAlive = !characterControllers[0].isAlive;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].isAlive != characterControllers[0].isAlive)
							characterControllers[characterControllersIndex].isAlive = characterControllers[0].isAlive;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = characterControllers[0].switchablePerspectives ? Color.green : Color.red;
					if(GUILayout.Button("Switchable Perspectives"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].switchablePerspectives = !characterControllers[0].switchablePerspectives;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].switchablePerspectives != characterControllers[0].switchablePerspectives)
							characterControllers[characterControllersIndex].switchablePerspectives = characterControllers[0].switchablePerspectives;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		private void ConsoleSection ()
		{
			bool canChangeAnimator = false;
			if(serializedObject.isEditingMultipleObjects)for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)
			{
				if((characterControllers[characterControllersIndex].useParentalRotationX || characterControllers[characterControllersIndex].useParentalRotationY || characterControllers[characterControllersIndex].useParentalOffset) && characterControllers[characterControllersIndex].useAnimator && characterControllers[characterControllersIndex].animator && characterControllers[characterControllersIndex].animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)canChangeAnimator = true;
				if(canChangeAnimator)break;
			}
			else if((characterControllers[0].useParentalRotationX || characterControllers[0].useParentalRotationY || characterControllers[0].useParentalOffset) && characterControllers[0].useAnimator && characterControllers[0].animator && characterControllers[0].animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)canChangeAnimator = true;
			if(canChangeAnimator)
			{
				EditorGUILayout.BeginVertical("Box");
				{
					GUILayout.Label("Change animator Culling Mode?");
					if(GUILayout.Button("Change"))
					{
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if((characterControllers[characterControllersIndex].useParentalRotationX || characterControllers[characterControllersIndex].useParentalRotationY || characterControllers[characterControllersIndex].useParentalOffset) && characterControllers[characterControllersIndex].useAnimator && characterControllers[characterControllersIndex].animator && characterControllers[characterControllersIndex].animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
						{
							Undo.RecordObject(characterControllers[characterControllersIndex].animator,"Inspector");
							characterControllers[characterControllersIndex].animator.enabled = false;
							characterControllers[characterControllersIndex].animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
							characterControllers[characterControllersIndex].animator.enabled = true;
						}
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndVertical();
			}
		}
		private void TabsSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				characterControllers[0].tabIndex = GUILayout.Toolbar(characterControllers[0].tabIndex,new string[] {"Main","Camera","Input"});
				for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].tabIndex != characterControllers[0].tabIndex)
					characterControllers[characterControllersIndex].tabIndex = characterControllers[0].tabIndex;
				if(characterControllers[0].tabIndex == 0)MainTab();
				if(characterControllers[0].tabIndex == 1)CameraTab();
				if(characterControllers[0].tabIndex == 2)InputTab();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTab ()
		{
			MainTabMainSection();
			MainTabExtraSection();
		}
		private void CameraTab ()
		{
			CameraTabMainSection();
			CameraTabAxisSection();
			CameraTabSensitivityAndDampingSection();
			CameraTabConfigureSection();
		}
		private void InputTab ()
		{
			InputTabMainSection();
			if(characterControllers[0].controlType == CharacterController.ControlType.PlayerControlled)InputTabConfigureSection();
		}
		private void MainTabMainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Main","BoldLabel");
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("perspectiveType"),GUIContent.none,true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("controlType"),GUIContent.none,true);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("airControl"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("walkSpeed"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("runSpeed"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpForce"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchSpeed"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("proneSpeed"),true);
				if(!serializedObject.isEditingMultipleObjects)
				{
					MainTabMainSectionStatesContainer();
					MainTabMainSectionEventsContainer();
				}
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("States",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Events",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
				MainTabMainSectionSmoothMovementsContainer();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabMainSectionStatesContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("States","Box",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].statesIsExpanded = !characterControllers[0].statesIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = characterControllers[0].states.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						characterControllers[0].states.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].statesIsExpanded)
				{
					for(int a = 0; a < characterControllers[0].states.Count; a++)
					{
						CharacterController.State currentState = characterControllers[0].states[a];
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button(currentState.name,"Box",GUILayout.ExpandWidth(true)))
								{
									currentState.isExpanded = !currentState.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.State previous = characterControllers[0].states[a - 1];
									for(int b = 0; b < characterControllers[0].parameters.Count; b++)
									{
										bool changed = false;
										if(characterControllers[0].parameters[b].index == a && !changed)
										{
											characterControllers[0].parameters[b].index = a - 1;
											changed = true;
										}
										if(characterControllers[0].parameters[b].index == a - 1 && !changed)
										{
											characterControllers[0].parameters[b].index = a;
											changed = true;
										}
									}
									characterControllers[0].states[a - 1] = currentState;
									characterControllers[0].states[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != characterControllers[0].states.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.State next = characterControllers[0].states[a + 1];
									for(int b = 0; b < characterControllers[0].parameters.Count; b++)
									{
										bool changed = false;
										if(characterControllers[0].parameters[b].index == a && !changed)
										{
											characterControllers[0].parameters[b].index = a + 1;
											changed = true;
										}
										if(characterControllers[0].parameters[b].index == a + 1 && !changed)
										{
											characterControllers[0].parameters[b].index = a;
											changed = true;
										}
									}
									characterControllers[0].states[a + 1] = currentState;
									characterControllers[0].states[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									characterControllers[0].states.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentState.isExpanded)
							{
								SerializedProperty currentStateProperty = serializedObject.FindProperty("states").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 40;
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("name"),true);
										EditorGUIUtility.labelWidth = 48;
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("axis"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("range"),true);
										EditorGUIUtility.labelWidth = 0;
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("onPositive"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("onNegative"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("onKeyDown"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("onKeyUp"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("onTrue"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("onFalse"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("isPositive"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("isNegative"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("isKeyDown"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("isKeyUp"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("isTrue"),true);
										EditorGUILayout.PropertyField(currentStateProperty.FindPropertyRelative("isFalse"),true);
									}
									EditorGUILayout.EndVertical();
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 40;
							EditorGUILayout.PropertyField(serializedObject.FindProperty("statesName"),new GUIContent("Name"),true);
							if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
							{
								Undo.RecordObject(target,"Inspector");
								characterControllers[0].states.Add(new CharacterController.State());
								characterControllers[0].states[characterControllers[0].states.Count - 1].name = characterControllers[0].statesName;
								if(characterControllers[0].automaticallyAddParameters)
								{
									characterControllers[0].parameters.Add(new CharacterController.Parameter());
									characterControllers[0].parameters[characterControllers[0].parameters.Count - 1].name = characterControllers[0].statesName;
									characterControllers[0].parameters[characterControllers[0].parameters.Count - 1].index = characterControllers[0].states.Count - 1;
								}
								GUI.FocusControl(null);
							}
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = characterControllers[0].useAnimator;
						GUI.backgroundColor = characterControllers[0].automaticallyAddParameters ? Color.green : Color.red;
						if(GUILayout.Button("Automatically Add Parameters"))
						{
							Undo.RecordObject(target,"Inspector");
							characterControllers[0].automaticallyAddParameters = !characterControllers[0].automaticallyAddParameters;
							GUI.FocusControl(null);
						}
						GUI.enabled = true;
						GUI.backgroundColor = Color.white;
					}
					EditorGUILayout.EndVertical();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabMainSectionEventsContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Events","Box",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].eventsIsExpanded = !characterControllers[0].eventsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = characterControllers[0].events.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						characterControllers[0].events.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].eventsIsExpanded)
				{
					for(int a = 0; a < characterControllers[0].events.Count; a++)
					{
						CharacterController.Event currentEvent = characterControllers[0].events[a];
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								Color color = GUI.color;
								GUI.color = Application.isPlaying ? (currentEvent._isTrue ? Color.green : Color.red) : Color.yellow;
								GUILayout.Box(a.ToString("000"));
								GUI.color = color;
								if(GUILayout.Button(currentEvent.name,"Box",GUILayout.ExpandWidth(true)))
								{
									currentEvent.isExpanded = !currentEvent.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.Event previous = characterControllers[0].events[a - 1];
									characterControllers[0].events[a - 1] = currentEvent;
									characterControllers[0].events[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != characterControllers[0].events.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.Event next = characterControllers[0].events[a + 1];
									characterControllers[0].events[a + 1] = currentEvent;
									characterControllers[0].events[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									characterControllers[0].events.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentEvent.isExpanded)
							{
								SerializedProperty currentEventProperty = serializedObject.FindProperty("events").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 40;
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("name"),true);
										EditorGUIUtility.labelWidth = 0;
										MainTabMainSectionEventsContainerConditionsContainer(currentEvent,currentEventProperty);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("onTrue"),true);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("onFalse"),true);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("isTrue"),true);
										EditorGUILayout.PropertyField(currentEventProperty.FindPropertyRelative("isFalse"),true);
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
						EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsName"),new GUIContent("Name"),true);
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							characterControllers[0].events.Add(new CharacterController.Event());
							characterControllers[0].events[characterControllers[0].events.Count - 1].name = characterControllers[0].eventsName;
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabMainSectionEventsContainerConditionsContainer (CharacterController.Event currentEvent,SerializedProperty currentEventProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Conditions","Box",GUILayout.ExpandWidth(true)))
					{
						currentEvent.conditionsIsExpanded = !currentEvent.conditionsIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = currentEvent.conditions.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						currentEvent.conditions.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(currentEvent.conditionsIsExpanded)
				{
					for(int a = 0; a < currentEvent.conditions.Count; a++)
					{
						CharacterController.Event.Condition currentCondition = currentEvent.conditions[a];
						if(a > 0)
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.FlexibleSpace();
								GUI.backgroundColor = currentCondition.statement == CharacterController.Event.Condition.Statement.And ? Color.green : Color.red;
								if(GUILayout.Button("&& - And"))
								{
									if(currentCondition.statement != CharacterController.Event.Condition.Statement.And)
									{
										Undo.RecordObject(target,"Inspector");
										currentCondition.statement = CharacterController.Event.Condition.Statement.And;
									}
									GUI.FocusControl(null);
								}
								GUI.backgroundColor = currentCondition.statement == CharacterController.Event.Condition.Statement.Or ? Color.green : Color.red;
								if(GUILayout.Button("|| - Or"))
								{
									if(currentCondition.statement != CharacterController.Event.Condition.Statement.Or)
									{
										Undo.RecordObject(target,"Inspector");
										currentCondition.statement = CharacterController.Event.Condition.Statement.Or;
									}
									GUI.FocusControl(null);
								}
								GUI.backgroundColor = Color.white;
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button("Condition " + (a + 1),"Box",GUILayout.ExpandWidth(true)))
								{
									currentCondition.isExpanded = !currentCondition.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.Event.Condition previous = currentEvent.conditions[a - 1];
									currentEvent.conditions[a - 1] = currentCondition;
									currentEvent.conditions[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != currentEvent.conditions.Count - 1;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.Event.Condition next = currentEvent.conditions[a + 1];
									currentEvent.conditions[a + 1] = currentCondition;
									currentEvent.conditions[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = true;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									currentEvent.conditions.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentCondition.isExpanded)
							{
								SerializedProperty currentConditionProperty = currentEventProperty.FindPropertyRelative("conditions").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(24);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 60;
										EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("usage"),true);
										MainTabMainSectionEventsContainerConditionsContainerIndexContainer(currentCondition,currentConditionProperty);
										EditorGUILayout.BeginHorizontal();
										{
											EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("type"),GUIContent.none,true);
											EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("target"),GUIContent.none,true);
										}
										EditorGUILayout.EndHorizontal();
										EditorGUIUtility.labelWidth = 60;
										if(currentCondition.type == CharacterController.Event.Condition.Type.Float || currentCondition.type == CharacterController.Event.Condition.Type.Int)
											EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("_operator"),true);
										if(currentCondition.type == CharacterController.Event.Condition.Type.Float)
											EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("floatValue"),new GUIContent("Value"),true);
										if(currentCondition.type == CharacterController.Event.Condition.Type.Int)
											EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("intValue"),new GUIContent("Value"),true);
										if(currentCondition.type == CharacterController.Event.Condition.Type.Bool)
											EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("boolValue"),new GUIContent("Value"),true);
										EditorGUIUtility.labelWidth = 0;
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
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Add a new Condition?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							currentEvent.conditions.Add(new CharacterController.Event.Condition());
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabMainSectionEventsContainerConditionsContainerIndexContainer (CharacterController.Event.Condition currentCondition,SerializedProperty currentConditionProperty)
		{
			if(characterControllers[0].states.Count > 0 && currentCondition.usage == CharacterController.Event.Condition.Usage.States)
			{
				EditorGUILayout.BeginHorizontal();
				{
					string[] stateNames = new string[characterControllers[0].states.Count];
					for(int a = 0; a < stateNames.Length; a++)
						stateNames[a] = "[" + a.ToString() + "] " + characterControllers[0].states[a].name;
					EditorGUIUtility.labelWidth = 39;
					EditorGUIUtility.fieldWidth = 14;
					EditorGUILayout.PropertyField(currentConditionProperty.FindPropertyRelative("index"),true);
					EditorGUIUtility.labelWidth = 35;
					EditorGUIUtility.fieldWidth = 0;
					EditorGUI.BeginChangeCheck();
					int index = EditorGUILayout.Popup("State",currentCondition.index,stateNames);
					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target,"Inspector");
						currentCondition.index = index;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			if(characterControllers[0].states.Count == 0 && currentCondition.usage == CharacterController.Event.Condition.Usage.States)
				EditorGUILayout.HelpBox("Conditions cannot be used without any States",MessageType.Error);
		}
		private void MainTabMainSectionSmoothMovementsContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useSmoothMovements"),true,GUILayout.Width(15));
					if(GUILayout.Button("Smooth Movements","Label",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].smoothMovementsIsExpanded = !characterControllers[0].smoothMovementsIsExpanded;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].smoothMovementsIsExpanded != characterControllers[0].smoothMovementsIsExpanded)
							characterControllers[characterControllersIndex].smoothMovementsIsExpanded = characterControllers[0].smoothMovementsIsExpanded;
						GUI.FocusControl(null);
					}
					EditorGUIUtility.labelWidth = 0;
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].smoothMovementsIsExpanded)
				{
					GUI.enabled = characterControllers[0].useSmoothMovements;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("idleDamping"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("walkDamping"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("runDamping"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchDamping"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("proneDamping"),true);
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabExtraSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Extra","BoldLabel");
				MainTabExtraSectionAnimatorContainer();
				if(!serializedObject.isEditingMultipleObjects)MainTabExtraSectionRagdollContainer();
				else
				{
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Box("Ragdoll",GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
				MainTabExtraSectionEnergyContainer();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabExtraSectionAnimatorContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useAnimator"),true,GUILayout.Width(15));
					if(GUILayout.Button("Animator","Label",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].animatorIsExpanded = !characterControllers[0].animatorIsExpanded;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].animatorIsExpanded != characterControllers[0].animatorIsExpanded)
							characterControllers[characterControllersIndex].animatorIsExpanded = characterControllers[0].animatorIsExpanded;
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].animatorIsExpanded)
				{
					GUI.enabled = characterControllers[0].useAnimator;
					EditorGUIUtility.labelWidth = 60;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"),true);
					EditorGUIUtility.labelWidth = 135;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalParameter"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalParameter"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("runParameter"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpParameter"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchParameter"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("proneParameter"),true);
					EditorGUIUtility.labelWidth = 0;
					if(!serializedObject.isEditingMultipleObjects)MainTabExtraSectionAnimatorContainerParametersContainer();
					else
					{
						GUI.enabled = false;
						EditorGUILayout.BeginHorizontal("Box");
						GUILayout.Box("Parameters",GUILayout.ExpandWidth(true));
						EditorGUILayout.EndHorizontal();
					}
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabExtraSectionAnimatorContainerParametersContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if(GUILayout.Button("Parameters","Box",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].parametersIsExpanded = !characterControllers[0].parametersIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.enabled = characterControllers[0].parameters.Count != 0 && characterControllers[0].useAnimator;
					if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.Height(20)))
					{
						Undo.RecordObject(target,"Inspector");
						characterControllers[0].parameters.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = characterControllers[0].useAnimator;
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].parametersIsExpanded)
				{
					for(int a = 0; a < characterControllers[0].parameters.Count; a++)
					{
						CharacterController.Parameter currentParameter = characterControllers[0].parameters[a];
						EditorGUILayout.BeginVertical("Box");
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Box(a.ToString("000"));
								if(GUILayout.Button(currentParameter.name,"Box",GUILayout.ExpandWidth(true)))
								{
									currentParameter.isExpanded = !currentParameter.isExpanded;
									GUI.FocusControl(null);
								}
								GUI.enabled = a != 0 && characterControllers[0].useAnimator;
								if(GUILayout.Button("▲",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.Parameter previous = characterControllers[0].parameters[a - 1];
									characterControllers[0].parameters[a - 1] = currentParameter;
									characterControllers[0].parameters[a] = previous;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = a != characterControllers[0].parameters.Count - 1 && characterControllers[0].useAnimator;
								if(GUILayout.Button("▼",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									CharacterController.Parameter next = characterControllers[0].parameters[a + 1];
									characterControllers[0].parameters[a + 1] = currentParameter;
									characterControllers[0].parameters[a] = next;
									GUI.FocusControl(null);
									break;
								}
								GUI.enabled = characterControllers[0].useAnimator;
								if(GUILayout.Button("-",GUILayout.Width(20),GUILayout.Height(20)))
								{
									Undo.RecordObject(target,"Inspector");
									characterControllers[0].parameters.RemoveAt(a);
									GUI.FocusControl(null);
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
							if(currentParameter.isExpanded)
							{
								SerializedProperty currentParameterProperty = serializedObject.FindProperty("parameters").GetArrayElementAtIndex(a);
								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(20);
									EditorGUILayout.BeginVertical();
									{
										EditorGUIUtility.labelWidth = 40;
										EditorGUILayout.PropertyField(currentParameterProperty.FindPropertyRelative("name"),true);
										EditorGUIUtility.labelWidth = 45;
										EditorGUILayout.PropertyField(currentParameterProperty.FindPropertyRelative("usage"),true);
										EditorGUIUtility.labelWidth = 0;
										if(characterControllers[0].states.Count > 0 && currentParameter.usage == CharacterController.Parameter.Usage.States)
										{
											EditorGUILayout.BeginHorizontal();
											{
												string[] stateNames = new string[characterControllers[0].states.Count];
												for(int b = 0; b < stateNames.Length; b++)
													stateNames[b] = "[" + b.ToString() + "] " + characterControllers[0].states[b].name;
												EditorGUIUtility.labelWidth = 40;
												EditorGUIUtility.fieldWidth = 45;
												EditorGUILayout.PropertyField(currentParameterProperty.FindPropertyRelative("index"),true);
												EditorGUIUtility.labelWidth = 35;
												EditorGUIUtility.fieldWidth = 0;
												EditorGUI.BeginChangeCheck();
												int index = EditorGUILayout.Popup("State",currentParameter.index,stateNames);
												if(EditorGUI.EndChangeCheck())
												{
													Undo.RecordObject(target,"Inspector");
													currentParameter.index = index;
												}
												EditorGUIUtility.labelWidth = 0;
											}
											EditorGUILayout.EndHorizontal();
										}
										EditorGUILayout.BeginHorizontal();
										{
											EditorGUILayout.PropertyField(currentParameterProperty.FindPropertyRelative("type"),GUIContent.none,true);
											EditorGUILayout.PropertyField(currentParameterProperty.FindPropertyRelative("target"),GUIContent.none,true);
										}
										EditorGUILayout.EndHorizontal();
										if(characterControllers[0].states.Count == 0 && currentParameter.usage == CharacterController.Parameter.Usage.States)EditorGUILayout.HelpBox("Parameters cannot be used without any States",MessageType.Error);
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
						EditorGUILayout.PropertyField(serializedObject.FindProperty("parametersName"),new GUIContent("Name"),true);
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							characterControllers[0].parameters.Add(new CharacterController.Parameter());
							characterControllers[0].parameters[characterControllers[0].parameters.Count - 1].name = characterControllers[0].parametersName;
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabExtraSectionRagdollContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useRagdoll"),true,GUILayout.Width(15));
					EditorGUIUtility.labelWidth = 0;
					if(GUILayout.Button("Ragdoll","Label",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].ragdollIsExpanded = !characterControllers[0].ragdollIsExpanded;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = characterControllers[0].ragdollIsActive ? Color.green : Color.red;
					if(GUILayout.Button(characterControllers[0].ragdollIsActive ? "Active" : "Inactive",GUILayout.Width(58),GUILayout.Height(16)))
					{
						Undo.RecordObject(target,"Inspector");
						characterControllers[0].ragdollIsActive = !characterControllers[0].ragdollIsActive;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
					GUI.enabled = characterControllers[0].ragdollBodies.Count != 0;
					if(GUILayout.Button("X",GUILayout.Width(16),GUILayout.Height(16)))
					{
						Undo.RecordObject(target,"Inspector");
						characterControllers[0].ragdollBodies.Clear();
						GUI.FocusControl(null);
					}
					GUI.enabled = true;
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].ragdollIsExpanded)
				{
					if(characterControllers[0].ragdollBodies.Count >= 5)characterControllers[0].ragdollBodiesScrollView = EditorGUILayout.BeginScrollView(characterControllers[0].ragdollBodiesScrollView,GUILayout.Height(100));
					else
					{
						if(characterControllers[0].ragdollBodiesScrollView != Vector2.zero)
							characterControllers[0].ragdollBodiesScrollView = Vector2.zero;
						if(characterControllers[0].ragdollBodiesScrollViewIndex != 0)
							characterControllers[0].ragdollBodiesScrollViewIndex = 0;
					}
					if(characterControllers[0].ragdollBodiesScrollViewIndex > 0)GUILayout.Space(characterControllers[0].ragdollBodiesScrollViewIndex * 26);
					GUI.enabled = characterControllers[0].useRagdoll;
					for(int a = characterControllers[0].ragdollBodiesScrollViewIndex; a <= Mathf.Clamp(characterControllers[0].ragdollBodiesScrollViewIndex + 4,0,characterControllers[0].ragdollBodies.Count - 1); a++)
					{
						EditorGUILayout.BeginHorizontal("Box");
						{
							EditorGUILayout.BeginHorizontal("Box");
							GUILayout.Box(a.ToString("000"),new GUIStyle() {fontSize = 8},GUILayout.ExpandWidth(false));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("ragdollBodies").GetArrayElementAtIndex(a),GUIContent.none,true);
							GUI.enabled = a != 0 && characterControllers[0].useRagdoll;
							if(GUILayout.Button("▲",GUILayout.Width(16),GUILayout.Height(16)))
							{
								Undo.RecordObject(target,"Inspector");
								Rigidbody current = characterControllers[0].ragdollBodies[a];
								Rigidbody previous = characterControllers[0].ragdollBodies[a - 1];
								characterControllers[0].ragdollBodies[a - 1] = current;
								characterControllers[0].ragdollBodies[a] = previous;
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = a != characterControllers[0].ragdollBodies.Count - 1 && characterControllers[0].useRagdoll;
							if(GUILayout.Button("▼",GUILayout.Width(16),GUILayout.Height(16)))
							{
								Undo.RecordObject(target,"Inspector");
								Rigidbody current = characterControllers[0].ragdollBodies[a];
								Rigidbody next = characterControllers[0].ragdollBodies[a + 1];
								characterControllers[0].ragdollBodies[a + 1] = current;
								characterControllers[0].ragdollBodies[a] = next;
								GUI.FocusControl(null);
								break;
							}
							GUI.enabled = characterControllers[0].useRagdoll;;
							if(GUILayout.Button("-",GUILayout.Width(16),GUILayout.Height(16)))
							{
								Undo.RecordObject(target,"Inspector");
								characterControllers[0].ragdollBodies.RemoveAt(a);
								GUI.FocusControl(null);
								break;
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					GUI.enabled = true;
					if(characterControllers[0].ragdollBodiesScrollViewIndex + 5 < characterControllers[0].ragdollBodies.Count)
						GUILayout.Space((characterControllers[0].ragdollBodies.Count - (characterControllers[0].ragdollBodiesScrollViewIndex + 5)) * 26);
					if(characterControllers[0].ragdollBodies.Count >= 5)
					{
						if(characterControllers[0].ragdollBodiesScrollViewIndex != characterControllers[0].ragdollBodiesScrollView.y / 26 && (Event.current.type == EventType.Repaint && Event.current.type == EventType.ScrollWheel || Event.current.type != EventType.Layout && Event.current.type != EventType.ScrollWheel))
							characterControllers[0].ragdollBodiesScrollViewIndex = (int)characterControllers[0].ragdollBodiesScrollView.y / 26;
						EditorGUILayout.EndScrollView();
					}
					GUI.enabled = characterControllers[0].useRagdoll;
					EditorGUILayout.BeginHorizontal("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Add a new Ragdoll Body?");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						if(GUILayout.Button("+",GUILayout.Width(20),GUILayout.Height(20)))
						{
							Undo.RecordObject(target,"Inspector");
							characterControllers[0].ragdollBodies.Add(null);
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void MainTabExtraSectionEnergyContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useEnergy"),true,GUILayout.Width(15));
					if(GUILayout.Button("Energy","Label",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].energyIsExpanded = !characterControllers[0].energyIsExpanded;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].energyIsExpanded != characterControllers[0].energyIsExpanded)
							characterControllers[characterControllersIndex].energyIsExpanded = characterControllers[0].energyIsExpanded;
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].energyIsExpanded)
				{
					GUI.enabled = characterControllers[0].useEnergy;
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUIUtility.labelWidth = 48;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("energy"),true);
						EditorGUIUtility.labelWidth = 57;
						EditorGUILayout.PropertyField(serializedObject.FindProperty("capacity"),true);
					}
					EditorGUILayout.EndHorizontal();
					GUI.backgroundColor = characterControllers[0].useEnergyUsages ? Color.green : Color.red;
					if(GUILayout.Button("Use Energy Usages"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].useEnergyUsages = !characterControllers[0].useEnergyUsages;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useEnergyUsages != characterControllers[0].useEnergyUsages)
							characterControllers[characterControllersIndex].useEnergyUsages = characterControllers[0].useEnergyUsages;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
					EditorGUIUtility.labelWidth = 0;
					GUI.enabled = characterControllers[0].useEnergy && characterControllers[0].useEnergyUsages;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("idleUsage"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("walkUsage"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("runUsage"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpUsage"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchUsage"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("proneUsage"),true);
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void CameraTabMainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Main","BoldLabel");
				EditorGUIUtility.labelWidth = 55;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("camera"),true);
				EditorGUIUtility.labelWidth = 0;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationType"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("lockCursor"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("hideCursor"),true);
				if(characterControllers[0].perspectiveType == CharacterController.PerspectiveType.ThirdPerson || characterControllers[0].switchablePerspectives)
				{
					GUI.enabled = characterControllers[0].camera;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"),true);
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void CameraTabAxisSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("Axis","BoldLabel");
					GUILayout.FlexibleSpace();
					GUI.backgroundColor = characterControllers[0].swapAxis ? Color.green : Color.red;
					if(GUILayout.Button("Swap Axis"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].swapAxis = !characterControllers[0].swapAxis;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].swapAxis != characterControllers[0].swapAxis)
							characterControllers[characterControllersIndex].swapAxis = characterControllers[0].swapAxis;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.Label("Invert Axis","BoldLabel");
					GUILayout.FlexibleSpace();
					GUI.backgroundColor = characterControllers[0].invertAxisX ? Color.green : Color.red;
					if(GUILayout.Button("X"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].invertAxisX = !characterControllers[0].invertAxisX;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].invertAxisX != characterControllers[0].invertAxisX)
							characterControllers[characterControllersIndex].invertAxisX = characterControllers[0].invertAxisX;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = characterControllers[0].invertAxisY ? Color.green : Color.red;
					if(GUILayout.Button("Y"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].invertAxisY = !characterControllers[0].invertAxisY;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].invertAxisY != characterControllers[0].invertAxisY)
							characterControllers[characterControllersIndex].invertAxisY = characterControllers[0].invertAxisY;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
				}
				EditorGUILayout.EndHorizontal();
				CameraTabAxisSectionClampXContainer();
				CameraTabAxisSectionClampYContainer();
			}
			EditorGUILayout.EndVertical();
		}
		private void CameraTabAxisSectionClampXContainer ()
		{
			GUI.enabled = characterControllers[0].camera;
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("clampX"),true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Clamp X",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].clampX)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumX"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumX"),true);
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void CameraTabAxisSectionClampYContainer ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("clampY"),true);
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.LabelField("Clamp Y",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].clampY)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumY"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumY"),true);
				}
			}
			EditorGUILayout.EndVertical();
			GUI.enabled = true;
		}
		private void CameraTabSensitivityAndDampingSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Sensitivity And Damping","BoldLabel");
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.Label("Delta Time","BoldLabel");
					GUILayout.FlexibleSpace();
					GUI.backgroundColor = characterControllers[0].useDeltaTimeX ? Color.green : Color.red;
					if(GUILayout.Button("X"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].useDeltaTimeX = !characterControllers[0].useDeltaTimeX;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useDeltaTimeX != characterControllers[0].useDeltaTimeX)
							characterControllers[characterControllersIndex].useDeltaTimeX = characterControllers[0].useDeltaTimeX;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = characterControllers[0].useDeltaTimeY ? Color.green : Color.red;
					if(GUILayout.Button("Y"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].useDeltaTimeY = !characterControllers[0].useDeltaTimeY;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useDeltaTimeY != characterControllers[0].useDeltaTimeY)
							characterControllers[characterControllersIndex].useDeltaTimeY = characterControllers[0].useDeltaTimeY;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.Label("Time Scale","BoldLabel");
					GUILayout.FlexibleSpace();
					GUI.backgroundColor = characterControllers[0].useTimeScaleX ? Color.green : Color.red;
					if(GUILayout.Button("X"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].useTimeScaleX = !characterControllers[0].useTimeScaleX;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useTimeScaleX != characterControllers[0].useTimeScaleX)
							characterControllers[characterControllersIndex].useTimeScaleX = characterControllers[0].useTimeScaleX;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = characterControllers[0].useTimeScaleY ? Color.green : Color.red;
					if(GUILayout.Button("Y"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].useTimeScaleY = !characterControllers[0].useTimeScaleY;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useTimeScaleY != characterControllers[0].useTimeScaleY)
							characterControllers[characterControllersIndex].useTimeScaleY = characterControllers[0].useTimeScaleY;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = 140;
				EditorGUIUtility.labelWidth = 140;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationDamping"),true);
				if(characterControllers[0].perspectiveType == CharacterController.PerspectiveType.ThirdPerson || characterControllers[0].switchablePerspectives)
					EditorGUILayout.PropertyField(serializedObject.FindProperty("turnDamping"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("sensitivity"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("sensitivityRange"),true);
				EditorGUIUtility.labelWidth = 0;
				CameraTabSensitivityAndDampingSectionSmoothPerspectivesContainer();
			}
			EditorGUILayout.EndVertical();
		}
		private void CameraTabSensitivityAndDampingSectionSmoothPerspectivesContainer ()
		{
			GUI.enabled = characterControllers[0].camera;
			EditorGUILayout.BeginVertical("Box");
			{
				GUI.enabled = characterControllers[0].camera && characterControllers[0].switchablePerspectives;
				EditorGUILayout.BeginHorizontal("Box");
				{
					EditorGUIUtility.labelWidth = 1;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("useSmoothPerspectives"),true,GUILayout.Width(15));
					if(GUILayout.Button("Smooth Perspectives","Label",GUILayout.ExpandWidth(true)))
					{
						characterControllers[0].smoothPerspectivesIsExpanded = !characterControllers[0].smoothPerspectivesIsExpanded;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].smoothPerspectivesIsExpanded != characterControllers[0].smoothPerspectivesIsExpanded)
							characterControllers[characterControllersIndex].smoothPerspectivesIsExpanded = characterControllers[0].smoothPerspectivesIsExpanded;
						GUI.FocusControl(null);
					}
					EditorGUIUtility.labelWidth = 0;
				}
				EditorGUILayout.EndHorizontal();
				if(characterControllers[0].smoothPerspectivesIsExpanded)
				{
					GUI.enabled = characterControllers[0].camera && characterControllers[0].switchablePerspectives && characterControllers[0].useSmoothPerspectives;
					EditorGUIUtility.labelWidth = 163;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("firstPersonSwitchTime"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("thirdPersonSwitchTime"),true);
					EditorGUIUtility.labelWidth = 0;
					GUI.enabled = true;
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndVertical();
			GUI.enabled = true;
		}
		private void CameraTabConfigureSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Configure","BoldLabel");
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.Label("Parental Rotation","BoldLabel");
					GUILayout.FlexibleSpace();
					GUI.backgroundColor = characterControllers[0].useParentalRotationX ? Color.green : Color.red;
					if(GUILayout.Button("X"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].useParentalRotationX = !characterControllers[0].useParentalRotationX;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useParentalRotationX != characterControllers[0].useParentalRotationX)
							characterControllers[characterControllersIndex].useParentalRotationX = characterControllers[0].useParentalRotationX;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = characterControllers[0].useParentalRotationY ? Color.green : Color.red;
					if(GUILayout.Button("Y"))
					{
						Undo.RecordObjects(targets,"Inspector");
						characterControllers[0].useParentalRotationY = !characterControllers[0].useParentalRotationY;
						for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useParentalRotationY != characterControllers[0].useParentalRotationY)
							characterControllers[characterControllersIndex].useParentalRotationY = characterControllers[0].useParentalRotationY;
						GUI.FocusControl(null);
					}
				}
				EditorGUILayout.EndHorizontal();
				GUI.backgroundColor = characterControllers[0].useParentalOffset ? Color.green : Color.red;
				if(GUILayout.Button("Parental Offset"))
				{
					Undo.RecordObjects(targets,"Inspector");
					characterControllers[0].useParentalOffset = !characterControllers[0].useParentalOffset;
					for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].useParentalOffset != characterControllers[0].useParentalOffset)
						characterControllers[characterControllersIndex].useParentalOffset = characterControllers[0].useParentalOffset;
					GUI.FocusControl(null);
				}
				GUI.backgroundColor = Color.white;
				GUI.enabled = characterControllers[0].camera;
				if(characterControllers[0].perspectiveType == CharacterController.PerspectiveType.FirstPerson || characterControllers[0].switchablePerspectives)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("firstPersonWorldOffset"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("firstPersonSelfOffset"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("firstPersonLocalOffset"),true);
				}
				if(characterControllers[0].perspectiveType == CharacterController.PerspectiveType.ThirdPerson || characterControllers[0].switchablePerspectives)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("thirdPersonWorldOffset"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("thirdPersonSelfOffset"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("thirdPersonLocalOffset"),true);
					GUI.enabled = true;
					EditorGUIUtility.labelWidth = 145;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("thirdPersonDirection"),true);
					EditorGUIUtility.labelWidth = 0;
					GUI.enabled = characterControllers[0].camera;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("dontClip"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("clipRadius"),true);
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndVertical();
		}
		private void InputTabMainSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Main","BoldLabel");
				if(characterControllers[0].controlType == CharacterController.ControlType.PlayerControlled)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalAxis"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalAxis"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("runAxis"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpAxis"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("crouchAxis"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("proneAxis"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseXAxis"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseYAxis"),true);
					GUI.enabled = characterControllers[0].switchablePerspectives;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("perspectiveAxis"),true);
					GUI.enabled = true;
				}
				if(characterControllers[0].controlType == CharacterController.ControlType.AIControlled)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalSensitivity"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalSensitivity"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseXSensitivity"),true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseYSensitivity"),true);
				}
			}
			EditorGUILayout.EndVertical();
		}
		private void InputTabConfigureSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("Configure","BoldLabel");
				GUI.backgroundColor = characterControllers[0].toggleRun ? Color.green : Color.red;
				if(GUILayout.Button("Toggle Run"))
				{
					Undo.RecordObjects(targets,"Inspector");
					characterControllers[0].toggleRun = !characterControllers[0].toggleRun;
					for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].toggleRun != characterControllers[0].toggleRun)
						characterControllers[characterControllersIndex].toggleRun = characterControllers[0].toggleRun;
					GUI.FocusControl(null);
				}
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical();
					{
						GUI.backgroundColor = characterControllers[0].toggleCrouch ? Color.green : Color.red;
						if(GUILayout.Button("Toggle Crouch"))
						{
							Undo.RecordObjects(targets,"Inspector");
							characterControllers[0].toggleCrouch = !characterControllers[0].toggleCrouch;
							for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].toggleCrouch != characterControllers[0].toggleCrouch)
								characterControllers[characterControllersIndex].toggleCrouch = characterControllers[0].toggleCrouch;
							GUI.FocusControl(null);
						}
						GUI.backgroundColor = characterControllers[0].canRun ? Color.green : Color.red;
						if(GUILayout.Button("Can Run"))
						{
							Undo.RecordObjects(targets,"Inspector");
							characterControllers[0].canRun = !characterControllers[0].canRun;
							for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].canRun != characterControllers[0].canRun)
								characterControllers[characterControllersIndex].canRun = characterControllers[0].canRun;
							GUI.FocusControl(null);
						}
						GUI.backgroundColor = characterControllers[0].canCrouch ? Color.green : Color.red;
						if(GUILayout.Button("Can Crouch"))
						{
							Undo.RecordObjects(targets,"Inspector");
							characterControllers[0].canCrouch = !characterControllers[0].canCrouch;
							for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].canCrouch != characterControllers[0].canCrouch)
								characterControllers[characterControllersIndex].canCrouch = characterControllers[0].canCrouch;
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					{
						GUI.backgroundColor = characterControllers[0].toggleProne ? Color.green : Color.red;
						if(GUILayout.Button("Toggle Prone"))
						{
							Undo.RecordObjects(targets,"Inspector");
							characterControllers[0].toggleProne = !characterControllers[0].toggleProne;
							for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].toggleProne != characterControllers[0].toggleProne)
								characterControllers[characterControllersIndex].toggleProne = characterControllers[0].toggleProne;
							GUI.FocusControl(null);
						}
						GUI.backgroundColor = characterControllers[0].canJump ? Color.green : Color.red;
						if(GUILayout.Button("Can Jump"))
						{
							Undo.RecordObjects(targets,"Inspector");
							characterControllers[0].canJump = !characterControllers[0].canJump;
							for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].canJump != characterControllers[0].canJump)
								characterControllers[characterControllersIndex].canJump = characterControllers[0].canJump;
							GUI.FocusControl(null);
						}
						GUI.backgroundColor = characterControllers[0].canProne ? Color.green : Color.red;
						if(GUILayout.Button("Can Prone"))
						{
							Undo.RecordObjects(targets,"Inspector");
							characterControllers[0].canProne = !characterControllers[0].canProne;
							for(int characterControllersIndex = 0; characterControllersIndex < characterControllers.Length; characterControllersIndex++)if(characterControllers[characterControllersIndex].canProne != characterControllers[0].canProne)
								characterControllers[characterControllersIndex].canProne = characterControllers[0].canProne;
							GUI.FocusControl(null);
						}
						GUI.backgroundColor = Color.white;
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
	}
	#endif
}