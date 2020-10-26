namespace Mis1eader
{
	using UnityEngine;
	using UnityEngine.Events;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Health/Health Event",1),ExecuteInEditMode]
	public class HealthEvent : MonoBehaviour
	{
		[System.Serializable] public class Event
		{
			[System.Serializable] public class Condition
			{
				public enum Statement : byte {And,Or}
				public enum Operator : byte {LessThan,LessThanOrEqualTo,NotEqualTo,EqualTo,GreaterThanOrEqualTo,GreaterThan}
				public HealthSystem source = null;
				public int index = 0;
				public Statement statement = Statement.And;
				public Operator @operator = Operator.EqualTo;
				public float health = 0f;
				public void Update ()
				{
					if(index < -1)index = -1;
					else if(source) {if(index >= source.healths.Count)index = source.healths.Count - 1;}
					else if(index > -1)index = -1;
					if(health < 0f)health = 0f;
					if(health == 0f && @operator == Operator.LessThan)
					{
						#if UNITY_EDITOR
						Debug.LogError("Health cannot be less than 0");
						#endif
						@operator = Operator.EqualTo;
					}
					if(source && source.healths.Count != 0 && index != -1)
					{
						if(health > source.healths[index].maximumHealth)health = source.healths[index].maximumHealth;
						if(health == source.healths[index].maximumHealth && @operator == Operator.GreaterThan)
						{
							#if UNITY_EDITOR
							Debug.LogError("Health cannot be greater than the source's maximum capacity " + source.healths[index].maximumHealth);
							#endif
							@operator = Operator.EqualTo;
						}
					}
				}
				public void SetSource (HealthSystem value) {source = value;}
				public void SetIndex (int value) {index = value;}
				public void SetStatement (Statement value) {statement = value;}
				public void SetStatement (int value) {statement = (Statement)value;}
				public void SetOperator (Operator value) {@operator = value;}
				public void SetOperator (int value) {@operator = (Operator)value;}
				public void SetHealth (float value) {health = value;}
				public void DecreaseHealth (float value) {health = health - (value < 0f ? -value : value);}
				public void IncreaseHealth (float value) {health = health + (value < 0f ? -value : value);}
			}
			#if UNITY_EDITOR
			public string name = string.Empty;
			#endif
			public List<Condition> conditions = new List<Condition>();
			public UnityEvent onTrue = new UnityEvent();
			public UnityEvent onFalse = new UnityEvent();
			public UnityEvent isTrue = new UnityEvent();
			public UnityEvent isFalse = new UnityEvent();
			[HideInInspector] public bool state = false;
			public void Update ()
			{
				if(conditions.Count == 0)return;
				ValidationHandler();
				#if UNITY_EDITOR
				if(!Application.isPlaying)return;
				#endif
				EventHandler();
			}
			private void ValidationHandler () {for(int a = 0,A = conditions.Count; a < A; a++)conditions[a].Update();}
			private void EventHandler ()
			{
				bool isPassed = false;
				for(int a = 0,A = conditions.Count; a < A; a++)
				{
					Condition condition = conditions[a];
					if(a != 0)
					{
						if(condition.statement == Condition.Statement.And && !isPassed)continue;
						if(condition.statement == Condition.Statement.Or && isPassed)break;
					}
					if(!condition.source || condition.index == -1)continue;
					Condition.Operator @operator = conditions[a].@operator;
					if(@operator == Condition.Operator.LessThan)
					{
						isPassed = condition.source.healths[condition.index].health < condition.health;
						continue;
					}
					if(@operator == Condition.Operator.LessThanOrEqualTo)
					{
						isPassed = condition.source.healths[condition.index].health <= condition.health;
						continue;
					}
					if(@operator == Condition.Operator.NotEqualTo)
					{
						isPassed = condition.source.healths[condition.index].health != condition.health;
						continue;
					}
					if(@operator == Condition.Operator.EqualTo)
					{
						isPassed = condition.source.healths[condition.index].health == condition.health;
						continue;
					}
					if(@operator == Condition.Operator.GreaterThanOrEqualTo)
					{
						isPassed = condition.source.healths[condition.index].health >= condition.health;
						continue;
					}
					if(@operator == Condition.Operator.GreaterThan)
					{
						isPassed = condition.source.healths[condition.index].health > condition.health;
						continue;
					}
				}
				if(isPassed)
				{
					if(!state)
					{
						onTrue.Invoke();
						state = true;
					}
					isTrue.Invoke();
				}
				else
				{
					if(state)
					{
						onFalse.Invoke();
						state = false;
					}
					isFalse.Invoke();
				}
			}
			public void SetConditions (List<Condition> value) {conditions = value;}
			public void SetConditions (Condition[] value) {conditions = new List<Condition>(value);}
			public void SetOnTrue (UnityEvent value) {onTrue = value;}
			public void SetOnFalse (UnityEvent value) {onFalse = value;}
			public void SetIsTrue (UnityEvent value) {isTrue = value;}
			public void SetIsFalse (UnityEvent value) {isFalse = value;}
		}
		public List<Event> events = new List<Event>();
		private void Update () {for(int a = 0,A = events.Count; a < A; a++)events[a].Update();}
		public void SetEvents (List<Event> value) {events = value;}
		public void SetEvents (Event[] value) {events = new List<Event>(value);}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public string eventsName = "Untitled";
		#endif
	}
}