namespace Mis1eader.Character
{
	using UnityEngine;
	[AddComponentMenu("Mis1eader/Creature/Human/Character IK",1),ExecuteInEditMode]
	public class CharacterIK : MonoBehaviour
	{
		public Animator animator = null;
		public LookIK look = new LookIK();
		public GoalIK leftHand = new GoalIK();
		public GoalIK rightHand = new GoalIK();
		public GoalIK leftFoot = new GoalIK();
		public GoalIK rightFoot = new GoalIK();
		public HintIK leftElbow = new HintIK();
		public HintIK rightElbow = new HintIK();
		public HintIK leftKnee = new HintIK();
		public HintIK rightKnee = new HintIK();
		[HideInInspector] private bool done = false;
		[System.Serializable] public class LookIK
		{
			public bool use = false;
			public Transform target = null;
			public bool useTransition = false;
			public float transition = 0.1f;
			[Range(0,1)] public float weight = 1;
			[Range(0,1)] public float bodyWeight = 1;
			[Range(0,1)] public float headWeight = 1;
			[Range(0,1)] public float eyesWeight = 1;
			[Range(0,1)] public float clampWeight = 1;
			[HideInInspector] public Vector3 position = Vector3.zero;
			[HideInInspector] public Quaternion rotation = Quaternion.identity;
		}
		[System.Serializable] public class GoalIK
		{
			public bool use = false;
			public Transform target = null;
			public bool useTransition = false;
			public float transition = 0.1f;
			[Range(0,1)] public float positionWeight = 1;
			[Range(0,1)] public float rotationWeight = 1;
			[HideInInspector] public Vector3 position = Vector3.zero;
			[HideInInspector] public Quaternion rotation = Quaternion.identity;
		}
		[System.Serializable] public class HintIK
		{
			public bool use = false;
			public Transform target = null;
			public bool useTransition = false;
			public float transition = 0.1f;
			[Range(0,1)] public float positionWeight = 1;
			[HideInInspector] public Vector3 position = Vector3.zero;
		}
		private void OnAnimatorIK ()
		{
			if(!Application.isPlaying || !animator)return;
			if(!done)
			{
				if(look.use && look.target)
				{
					look.position = look.target.position;
					look.rotation = look.target.rotation;
				}
				if(leftHand.use && leftHand.target)
				{
					leftHand.position = animator.GetIKPosition(AvatarIKGoal.LeftHand);
					leftHand.rotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);
				}
				if(rightHand.use && rightHand.target)
				{
					rightHand.position = animator.GetIKPosition(AvatarIKGoal.RightHand);
					rightHand.rotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
				}
				if(leftFoot.use && leftFoot.target)
				{
					leftFoot.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
					leftFoot.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				}
				if(rightFoot.use && rightFoot.target)
				{
					rightFoot.position = animator.GetIKPosition(AvatarIKGoal.RightFoot);
					rightFoot.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				}
				if(leftElbow.use && leftElbow.target)
					leftElbow.position = animator.GetIKHintPosition(AvatarIKHint.LeftElbow);
				if(rightElbow.use && rightElbow.target)
					rightElbow.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);
				if(leftKnee.use && leftKnee.target)
					leftKnee.position = animator.GetIKHintPosition(AvatarIKHint.LeftKnee);
				if(rightKnee.use && rightKnee.target)
					rightKnee.position = animator.GetIKHintPosition(AvatarIKHint.RightKnee);
			}
			WeightHandler();
			TransitionHandler();
			TargetingHandler();
		}
		private void WeightHandler ()
		{
			if(look.use)animator.SetLookAtWeight(look.weight,look.bodyWeight,look.headWeight,look.eyesWeight,look.clampWeight);
			if(leftHand.use)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHand.positionWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,leftHand.positionWeight);
			}
			if(rightHand.use)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHand.positionWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,rightHand.positionWeight);
			}
			if(leftFoot.use)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,leftFoot.positionWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,leftFoot.positionWeight);
			}
			if(rightFoot.use)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,rightFoot.positionWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,rightFoot.positionWeight);
			}
			if(leftElbow.use)
				animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow,leftElbow.positionWeight);
			if(rightElbow.use)
				animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow,rightElbow.positionWeight);
			if(leftKnee.use)
				animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee,leftKnee.positionWeight);
			if(rightKnee.use)
				animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee,rightKnee.positionWeight);
		}
		private void TransitionHandler ()
		{
			if(look.use && look.target)
			{
				if(look.useTransition)
				{
					look.position = Vector3.Lerp(look.position,look.target.position,look.transition);
					look.rotation = Quaternion.Lerp(look.rotation,look.target.rotation,look.transition);
				}
				else
				{
					if(look.position != look.target.position)look.position = look.target.position;
					if(look.rotation != look.target.rotation)look.rotation = look.target.rotation;
				}
			}
			if(leftHand.use && leftHand.target)
			{
				if(leftHand.useTransition)
				{
					leftHand.position = Vector3.Lerp(leftHand.position,leftHand.target.position,leftHand.transition);
					leftHand.rotation = Quaternion.Lerp(leftHand.rotation,leftHand.target.rotation,leftHand.transition);
				}
				else
				{
					if(leftHand.position != leftHand.target.position)leftHand.position = leftHand.target.position;
					if(leftHand.rotation != leftHand.target.rotation)leftHand.rotation = leftHand.target.rotation;
				}
			}
			if(rightHand.use && rightHand.target)
			{
				if(rightHand.useTransition)
				{
					rightHand.position = Vector3.Lerp(rightHand.position,rightHand.target.position,rightHand.transition);
					rightHand.rotation = Quaternion.Lerp(rightHand.rotation,rightHand.target.rotation,rightHand.transition);
				}
				else
				{
					if(rightHand.position != rightHand.target.position)rightHand.position = rightHand.target.position;
					if(rightHand.rotation != rightHand.target.rotation)rightHand.rotation = rightHand.target.rotation;
				}
			}
			if(leftFoot.use && leftFoot.target)
			{
				if(leftFoot.useTransition)
				{
					leftFoot.position = Vector3.Lerp(leftFoot.position,leftFoot.target.position,leftFoot.transition);
					leftFoot.rotation = Quaternion.Lerp(leftFoot.rotation,leftFoot.target.rotation,leftFoot.transition);
				}
				else
				{
					if(leftFoot.position != leftFoot.target.position)leftFoot.position = leftFoot.target.position;
					if(leftFoot.rotation != leftFoot.target.rotation)leftFoot.rotation = leftFoot.target.rotation;
				}
			}
			if(rightFoot.use && rightFoot.target)
			{
				if(rightFoot.useTransition)
				{
					rightFoot.position = Vector3.Lerp(rightFoot.position,rightFoot.target.position,rightFoot.transition);
					rightFoot.rotation = Quaternion.Lerp(rightFoot.rotation,rightFoot.target.rotation,rightFoot.transition);
				}
				else
				{
					if(rightFoot.position != rightFoot.target.position)rightFoot.position = rightFoot.target.position;
					if(rightFoot.rotation != rightFoot.target.rotation)rightFoot.rotation = rightFoot.target.rotation;
				}
			}
			if(leftElbow.use && leftElbow.target)
			{
				if(leftElbow.useTransition)
				{
					leftElbow.position = Vector3.Lerp(leftElbow.position,leftElbow.target.position,leftElbow.transition);
				}
				else
				{
					if(leftElbow.position != leftElbow.target.position)leftElbow.position = leftElbow.target.position;
				}
			}
			if(rightElbow.use && rightElbow.target)
			{
				if(rightElbow.useTransition)
				{
					rightElbow.position = Vector3.Lerp(rightElbow.position,rightElbow.target.position,rightElbow.transition);
				}
				else
				{
					if(rightElbow.position != rightElbow.target.position)rightElbow.position = rightElbow.target.position;
				}
			}
			if(leftKnee.use && leftKnee.target)
			{
				if(leftKnee.useTransition)
				{
					leftKnee.position = Vector3.Lerp(leftKnee.position,leftKnee.target.position,leftKnee.transition);
				}
				else
				{
					if(leftKnee.position != leftKnee.target.position)leftKnee.position = leftKnee.target.position;
				}
			}
			if(rightKnee.use && rightKnee.target)
			{
				if(rightKnee.useTransition)
				{
					rightKnee.position = Vector3.Lerp(rightKnee.position,rightKnee.target.position,rightKnee.transition);
				}
				else
				{
					if(rightKnee.position != rightKnee.target.position)rightKnee.position = rightKnee.target.position;
				}
			}
		}
		private void TargetingHandler ()
		{
			if(look.use)animator.SetLookAtPosition(look.position);
			if(leftHand.use)
			{
				animator.SetIKPosition(AvatarIKGoal.LeftHand,leftHand.position);
				animator.SetIKRotation(AvatarIKGoal.LeftHand,leftHand.rotation);
			}
			if(rightHand.use)
			{
				animator.SetIKPosition(AvatarIKGoal.RightHand,rightHand.position);
				animator.SetIKRotation(AvatarIKGoal.RightHand,rightHand.rotation);
			}
			if(leftFoot.use)
			{
				animator.SetIKPosition(AvatarIKGoal.LeftFoot,leftFoot.position);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,leftFoot.rotation);
			}
			if(rightFoot.use)
			{
				animator.SetIKPosition(AvatarIKGoal.RightFoot,rightFoot.position);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,rightFoot.rotation);
			}
			if(leftElbow.use)
				animator.SetIKHintPosition(AvatarIKHint.LeftElbow,leftElbow.position);
			if(rightElbow.use)
				animator.SetIKHintPosition(AvatarIKHint.RightElbow,rightElbow.position);
			if(leftKnee.use)
				animator.SetIKHintPosition(AvatarIKHint.LeftKnee,leftKnee.position);
			if(rightKnee.use)
				animator.SetIKHintPosition(AvatarIKHint.RightKnee,rightKnee.position);
		}
	}
}