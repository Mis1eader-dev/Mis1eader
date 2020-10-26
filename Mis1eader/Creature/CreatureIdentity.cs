namespace Mis1eader.Creature
{
	using UnityEngine;
	using System.Collections;
	public enum Gender : byte {Male,Female}
	[AddComponentMenu("Mis1eader/Creature/Creature Identity",0)]
	public class CreatureIdentity : MonoBehaviour
	{
		public byte Gender {get {return (byte)gender;} set {gender = (Gender)value;}}
		public Mis1eader.Creature.Gender gender = Mis1eader.Creature.Gender.Male;
		public string identity = string.Empty;
	}
}