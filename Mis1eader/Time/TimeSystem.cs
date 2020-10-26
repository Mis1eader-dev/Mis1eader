namespace AdvancedAssets
{
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	[System.Serializable] public class Time
	{
		public enum DateType {Elapsed,Calendar}
		public enum TimeType {TwelveHour,TwentyFourHour}
		public DateType dateType = DateType.Elapsed;
		public TimeType timeType = TimeType.TwentyFourHour;
		public bool isAm = true;
		public bool isPm = false;
		public bool useYear = false;
		#if UNITY_5_3_OR_NEWER
		[Delayed] public uint year = 0;
		#else
		public uint year = 0;
		#endif
		public bool useMonth = false;
		#if UNITY_5_3_OR_NEWER
		[Delayed] public int month = 0;
		#else
		public int month = 0;
		#endif
		public bool useDay = true;
		#if UNITY_5_3_OR_NEWER
		[Delayed] public int day = 0;
		#else
		public int day = 0;
		#endif
		public bool useHour = true;
		#if UNITY_5_3_OR_NEWER
		[Delayed] public int hour = 0;
		#else
		public int hour = 0;
		#endif
		public bool useMinute = true;
		#if UNITY_5_3_OR_NEWER
		[Delayed] public int minute = 0;
		#else
		public int minute = 0;
		#endif
		#if UNITY_5_3_OR_NEWER
		[Delayed] public int second = 0;
		[Delayed] public float delta = 0;
		#else
		public int second = 0;
		public float delta = 0;
		#endif
		[HideInInspector,SerializeField] internal bool wasAm = true;
		[HideInInspector,SerializeField] internal bool wasPm = false;
		[HideInInspector,SerializeField] internal bool onTwelveHourSwitch = false;
		[HideInInspector,SerializeField] internal bool onTwentyFourHourSwitch = false;
		[HideInInspector,SerializeField] internal bool isElapsedSwitch = true;
		[HideInInspector,SerializeField] internal bool isCalendarSwitch = false;
		[HideInInspector,SerializeField] internal bool isTwelveHourSwitch = false;
		[HideInInspector,SerializeField] internal bool isTwentyFourHourSwitch = false;
		[HideInInspector,SerializeField] internal bool isLeapYear = false;
		public void Update ()
		{
			SwitchHandler();
			if(delta == float.NegativeInfinity || delta == -float.MaxValue || delta == float.MaxValue || delta == float.PositiveInfinity)delta = 0;
			CalculationHandler();
		}
		private void SwitchHandler ()
		{
			bool isTwelveHour = timeType == TimeType.TwelveHour;
			if(onTwelveHourSwitch)onTwelveHourSwitch = false;
			if(onTwentyFourHourSwitch)onTwentyFourHourSwitch = false;
			if(isTwelveHourSwitch != isTwelveHour)
			{
				isTwelveHourSwitch = isTwelveHour;
				if(isTwelveHour)onTwelveHourSwitch = true;
			}
			if(isTwentyFourHourSwitch == isTwelveHour)
			{
				isTwentyFourHourSwitch = !isTwelveHour;
				if(!isTwelveHour)onTwentyFourHourSwitch = true;
			}
			if(useHour)
			{
				if(onTwelveHourSwitch)
				{
					if(hour < 12)
					{
						if(!isAm)isAm = true;
						if(isPm)isPm = false;
						if(hour == 0)
						{
							hour = hour + 12;
							wasAm = true;
							wasPm = false;
						}
						if(hour >= 1 && hour < 12)
						{
							wasAm = false;
							wasPm = true;
						}
						return;
					}
					else
					{
						if(isAm)isAm = false;
						if(!isPm)isPm = true;
						if(hour > 12 && hour < 24)
						{
							hour = hour - 12;
							wasAm = true;
							wasPm = false;
						}
						if(hour == 12)
						{
							wasAm = false;
							wasPm = true;
						}
						return;
					}
				}
				if(onTwentyFourHourSwitch)
				{
					if(isAm && hour == 12)hour = hour - 12;
					if(isPm && hour >= 1 && hour < 12)hour = hour + 12;
				}
			}
			else
			{
				if(!isAm)isAm = true;
				if(isPm)isPm = false;
				if(!wasAm)wasAm = true;
				if(wasPm)wasPm = false;
				if(isTwelveHour && hour != 12)hour = 12;
				if(!isTwelveHour && hour != 0)hour = 0;
			}
		}
		private void CalculationHandler ()
		{
			uint _year = year;
			byte[] days = new byte[] {31,28,31,30,31,30,31,31,30,31,30,31};
			if(dateType == DateType.Calendar)
			{
				day = day - 1;
				month = month - 1;
			}
			ConversionHandler(useYear,delta,second,minute,hour,day,month,ref _year,isTwelveHourSwitch,isAm,isPm);
			while(delta >= 1)
			{
				delta = delta - 1;
				second = second + 1;
			}
			while(delta < 0)
			{
				if(second > 0 || minute > 0 || hour > 0 || day > 0 || month > 0 || year > 0)
				{
					delta = delta + 1;
					second = second - 1;
					continue;
				}
				delta = 0;
			}
			if(_year % 4 == 0)days[1] = 29;
			while(second < 0)
			{
				if(useMinute && minute > 0)
				{
					second = second + 60;
					minute = minute - 1;
					continue;
				}
				if(useHour && hour > 0)
				{
					second = second + 3600;
					hour = hour - 1;
					continue;
				}
				if(useDay && day > 0)
				{
					second = second + 86400;
					day = day - 1;
					continue;
				}
				if(useMonth && month > 0)
				{
					second = second + 86400 * days[month > 0 ? month - 1 : month + 11];
					month = month - 1;
					continue;
				}
				if(useYear && year > 0)
				{
					second = second + ((_year - 1) % 4 != 0 ? 31536000 : 31622400);
					year = year - 1;
					continue;
				}
				second = 0;
			}
			while(minute < 0)
			{
				if(useHour && hour > 0)
				{
					minute = minute + 60;
					hour = hour - 1;
					continue;
				}
				if(useDay && day > 0)
				{
					minute = minute + 1440;
					day = day - 1;
					continue;
				}
				if(useMonth && month > 0)
				{
					minute = minute + 1440 * days[month > 0 ? month - 1 : month + 11];
					month = month - 1;
					continue;
				}
				if(useYear && year > 0)
				{
					minute = minute + ((_year - 1) % 4 != 0 ? 525600 : 527040);
					year = year - 1;
					continue;
				}
				minute = 0;
			}
			while(hour < 0)
			{
				if(useDay && day > 0)
				{
					hour = hour + 24;
					day = day - 1;
					continue;
				}
				if(useMonth && month > 0)
				{
					hour = hour + 24 * days[month > 0 ? month - 1 : month + 11];
					month = month - 1;
					continue;
				}
				if(useYear && year > 0)
				{
					hour = hour + ((_year - 1) % 4 != 0 ? 8760 : 8784);
					year = year - 1;
					continue;
				}
				hour = 0;
			}
			while(day < 0)
			{
				if(useMonth && month > 0)
				{
					day = day + days[month > 0 ? month - 1 : month + 11];
					month = month - 1;
					continue;
				}
				if(useYear && year > 0)
				{
					day = day + ((_year - 1) % 4 != 0 ? 365 : 366);
					year = year - 1;
					continue;
				}
				day = 0;
			}
			while(month < 0)
			{
				if(useYear && year > 0)
				{
					month = month + 12;
					year = year - 1;
					continue;
				}
				month = 0;
			}
			while(useMinute && second >= 60 || useHour && second >= 3600 || useDay && second >= 86400 || useMonth && second >= 86400 * days[month % 12] || useYear && second >= (!isLeapYear ? 31536000 : 31622400))
			{
				if(useMinute)
				{
					second = second - 60;
					minute = minute + 1;
					continue;
				}
				if(useHour)
				{
					second = second - 3600;
					hour = hour + 1;
					continue;
				}
				if(useDay)
				{
					second = second - 86400;
					day = day + 1;
					continue;
				}
				if(useMonth)
				{
					second = second - 86400 * days[month % 12];
					month = month + 1;
					continue;
				}
				if(useYear)
				{
					second = second - (!isLeapYear ? 31536000 : 31622400);
					year = year + 1;
				}
			}
			while(useHour && minute >= 60 || useDay && minute >= 1440 || useMonth && minute >= 1440 * days[month % 12] || useYear && minute >= (!isLeapYear ? 525600 : 527040))
			{
				if(useHour)
				{
					minute = minute - 60;
					hour = hour + 1;
					continue;
				}
				if(useDay)
				{
					minute = minute - 1440;
					day = day + 1;
					continue;
				}
				if(useMonth)
				{
					minute = minute - 1440 * days[month % 12];
					month = month + 1;
					continue;
				}
				if(useYear)
				{
					minute = minute - (!isLeapYear ? 525600 : 527040);
					year = year + 1;
				}
			}
			if(useHour)
			{
				if(isTwelveHourSwitch)
				{
					if(hour >= 12 && wasAm)
					{
						if(!isAm)
						{
							isAm = true;
							if(useDay)day = day + 1;
						}
						if(isPm)isPm = false;
					}
					if(hour >= 12 && wasPm)
					{
						if(isAm)isAm = false;
						if(!isPm)isPm = true;
					}
					if(hour >= 1 && hour < 12 && wasAm)
					{
						if(isAm)isAm = false;
						if(!isPm)
						{
							isPm = true;
							if(useDay)day = day - 1;
						}
					}
					if(hour >= 1 && hour < 12 && wasPm)
					{
						if(!isAm)isAm = true;
						if(isPm)isPm = false;
					}
					while(hour <= 0)
					{
						hour = hour + 12;
						if(wasAm)
						{
							if(isAm)isAm = false;
							if(!isPm)isPm = true;
							wasAm = false;
							wasPm = true;
							continue;
						}
						if(wasPm)
						{
							if(!isAm)isAm = true;
							if(isPm)isPm = false;
							wasAm = true;
							wasPm = false;
						}
					}
					while(hour > 12)
					{
						hour = hour - 12;
						if(wasAm)
						{
							if(!isAm)isAm = true;
							if(isPm)isPm = false;
							wasAm = false;
							wasPm = true;
							continue;
						}
						if(wasPm)
						{
							if(isAm)isAm = false;
							if(!isPm)isPm = true;
							wasAm = true;
							wasPm = false;
						}
					}
				}
				else
				{
					if(hour >= 0 && hour < 12)
					{
						if(!isAm)isAm = true;
						if(isPm)isPm = false;
					}
					if(hour >= 12 && hour < 24)
					{
						if(isAm)isAm = false;
						if(!isPm)isPm = true;
					}
					while(useDay && hour >= 24 || useMonth && hour >= 24 * days[month % 12] || useYear && hour >= (!isLeapYear ? 8760 : 8784))
					{
						if(useDay)
						{
							if(!isAm)isAm = true;
							if(isPm)isPm = false;
							hour = hour - 24;
							day = day + 1;
							continue;
						}
						if(useMonth)
						{
							hour = hour - 24 * days[month % 12];
							month = month + 1;
							continue;
						}
						if(useYear)
						{
							hour = hour - (!isLeapYear ? 8760 : 8784);
							year = year + 1;
						}
					}
				}
			}
			while(useMonth && day >= days[month % 12] || useYear && day >= (!isLeapYear ? 365 : 366))
			{
				if(useMonth)
				{
					day = day - days[month % 12];
					month = month + 1;
					continue;
				}
				if(useYear)
				{
					day = day - (!isLeapYear ? 365 : 366);
					year = year + 1;
				}
			}
			while(useYear && month >= 12)
			{
				month = month - 12;
				year = year + 1;
			}
			if(!useMinute && minute != 0)minute = 0;
			if(!useHour)
			{
				if(isTwelveHourSwitch && hour != 12)hour = 12;
				if(!isTwelveHourSwitch && hour != 0)hour = 0;
			}
			if(!useDay && day != 0)day = 0;
			if(!useMonth && month != 0)month = 0;
			if(!useYear && year != 0)year = 0;
			if(dateType == DateType.Calendar)
			{
				day = day + 1;
				month = month + 1;
			}
			isLeapYear = _year % 4 == 0;
		}
		protected void ConversionHandler (bool useYear,float delta,int second,int minute,int hour,int day,int month,ref uint year,bool isTwelveHour,bool isAm,bool isPm)
		{
			byte[] days = new byte[] {31,28,31,30,31,30,31,31,30,31,30,31};
			while(delta >= 1)
			{
				delta = delta - 1;
				second = second + 1;
			}
			while(second >= 60)
			{
				second = second - 60;
				minute = minute + 1;
			}
			while(minute >= 60)
			{
				minute = minute - 60;
				hour = hour + 1;
			}
			if(isTwelveHour)
			{
				if(isAm && hour == 12)hour = hour - 12;
				if(isPm && hour >= 1 && hour < 12)hour = hour + 12;
			}
			while(hour >= 24)
			{
				hour = hour - 24;
				day = day + 1;
			}
			if(useYear && year % 4 == 0)days[1] = 29;
			while(month < 0 && year > 0)month = month + 12;
			if(month < 0 && year == 0)month = 0;
			while(day >= days[month % 12])
			{
				day = day - days[month % 12];
				month = month + 1;
			}
			while(month >= 12)
			{
				month = month - 12;
				year = year + 1;
			}
		}
		protected void ConversionHandler (bool useYear,float delta,int second,int minute,int hour,ref int day,ref int month,ref uint year,bool isTwelveHour,bool isAm,bool isPm)
		{
			byte[] days = new byte[] {31,28,31,30,31,30,31,31,30,31,30,31};
			while(delta >= 1)
			{
				delta = delta - 1;
				second = second + 1;
			}
			while(second >= 60)
			{
				second = second - 60;
				minute = minute + 1;
			}
			while(minute >= 60)
			{
				minute = minute - 60;
				hour = hour + 1;
			}
			if(isTwelveHour)
			{
				if(isAm && hour == 12)hour = hour - 12;
				if(isPm && hour >= 1 && hour < 12)hour = hour + 12;
			}
			while(hour >= 24)
			{
				hour = hour - 24;
				day = day + 1;
			}
			if(useYear && year % 4 == 0)days[1] = 29;
			while(day >= days[month % 12])
			{
				day = day - days[month % 12];
				month = month + 1;
			}
			while(month >= 12)
			{
				month = month - 12;
				year = year + 1;
			}
		}
		public void SetDateType (DateType value) {if(dateType != value)dateType = value;}
		public void SetDateType (int value)
		{
			DateType convertedValue = (DateType)value;
			if(dateType != convertedValue)dateType = convertedValue;
		}
		public void SetTimeType (TimeType value) {if(timeType != value)timeType = value;}
		public void SetTimeType (int value)
		{
			TimeType convertedValue = (TimeType)value;
			if(timeType != convertedValue)timeType = convertedValue;
		}
		public void SetAm ()
		{
			if(!isAm)isAm = true;
			if(isPm)isPm = false;
			if(hour == 0)
			{
				wasAm = true;
				wasPm = false;
			}
			if(hour >= 1 && hour < 12)
			{
				wasAm = false;
				wasPm = true;
			}
		}
		public void SetPm ()
		{
			if(isAm)isAm = false;
			if(!isPm)isPm = true;
			if(hour > 12 && hour < 24)
			{
				wasAm = true;
				wasPm = false;
			}
			if(hour == 12)
			{
				wasAm = false;
				wasPm = true;
			}
		}
		public void UseYear (bool value) {if(useYear != value)useYear = value;}
		public void SetYear (uint value) {if(year != value)year = value;}
		public void DecreaseYear (uint value) {year = value;}
		public void IncreaseYear (uint value) {year = year + value;}
		public void UseMonth (bool value) {if(useMonth != value)useMonth = value;}
		public void SetMonth (int value) {if(month != value)month = value;}
		public void DecreaseMonth (int value) {month = month - Mathf.Abs(value);}
		public void IncreaseMonth (int value) {month = month + Mathf.Abs(value);}
		public void UseDay (bool value) {if(useDay != value)useDay = value;}
		public void SetDay (int value) {if(day != value)day = value;}
		public void DecreaseDay (int value) {day = day - Mathf.Abs(value);}
		public void IncreaseDay (int value) {day = day + Mathf.Abs(value);}
		public void UseHour (bool value) {if(useHour != value)useHour = value;}
		public void SetHour (int value) {if(hour != value)hour = value;}
		public void DecreaseHour (int value) {hour = hour - Mathf.Abs(value);}
		public void IncreaseHour (int value) {hour = hour + Mathf.Abs(value);}
		public void UseMinute (bool value) {if(useMinute != value)useMinute = value;}
		public void SetMinute (int value) {if(minute != value)minute = value;}
		public void DecreaseMinute (int value) {minute = minute - Mathf.Abs(value);}
		public void IncreaseMinute (int value) {minute = minute + Mathf.Abs(value);}
		public void SetSecond (int value) {if(second != value)second = value;}
		public void DecreaseSecond (int value) {second = second - Mathf.Abs(value);}
		public void IncreaseSecond (int value) {second = second + Mathf.Abs(value);}
		public void SetDelta (float value) {if(delta != value)delta = value;}
		public void DecreaseDelta (float value) {delta = delta - Mathf.Abs(value);}
		public void IncreaseDelta (float value) {delta = delta + Mathf.Abs(value);}
	}
	[AddComponentMenu("Advanced Assets/Time/Time System",20),ExecuteInEditMode]
	public class TimeSystem : MonoBehaviour
	{
		public enum CountType {CountUp,CountDown,DontCount}
		public enum SpeedType {Rotation,Time}
		public CountType countType = CountType.CountUp;
		public Time time = new Time();
		public bool useTimeScaleUsage = false;
		public SpeedType speedType = SpeedType.Time;
		public float speed = 1;
		[System.Serializable] public class Time : AdvancedAssets.Time
		{
			public bool useSeason = false;
			#if UNITY_5_3_OR_NEWER
			[Delayed] public byte season = 4;
			#else
			public byte season = 4;
			#endif
			public bool useWeek = false;
			#if UNITY_5_3_OR_NEWER
			[Delayed] public byte week = 7;
			#else
			public byte week = 7;
			#endif
			public new void Update ()
			{
				base.Update();
				if(useWeek || useSeason)
				{
					uint _year = year;
					int _month = month;
					int _day = day;
					ConversionHandler(useYear,delta,second,minute,hour,day,month,ref _year,timeType == TimeType.TwelveHour,isAm,isPm);
					ConversionHandler(useYear,delta,second,minute,hour,ref _day,ref _month,ref _year,timeType == TimeType.TwelveHour,isAm,isPm);
					if(useWeek)week = Week(_day,_month,_year);
					if(useSeason)season = Season(_month);
				}
				if(!useWeek && week != 7)week = 7;
				if(!useSeason && season != 4)season = 4;
			}
			private byte Week (int day,int month,uint year)
			{
				int result = day * 864;
				byte[] days = new byte[] {31,28,31,30,31,30,31,31,30,31,30,31};
				for(int a = 0; a < month % 12; a++)
					result = result + 864 * days[a];
				year = year % 2000;
				return (byte)(1 + (6 + (result + year * 315360 + (year / 4) * 864 + (year / 400) * 864  - (year / 100) * 864 + (year % 4 == 0 && month >= 2 ? 864 : 0) + (year % 4 > 0 ? 864 : 0)) / 864) % 7);
			}
			private byte Season (int month)
			{
				if(month == 2 || month == 3 || month == 4)return 1;
				if(month == 5 || month == 6 || month == 7)return 2;
				if(month == 8 || month == 9 || month == 10)return 3;
				if(month == 11 || month == 0 || month == 1)return 4;
				return 4;
			}
			public void UseSeason (bool value) {if(useSeason != value)useSeason = value;}
			public void UseWeek (bool value) {if(useWeek != value)useWeek = value;}
		}
		private void Update ()
		{
			speed = Mathf.Clamp(speed,0,999999);
			if(
			#if UNITY_EDITOR
			Application.isPlaying &&
			#endif
			countType != CountType.DontCount)
			{
				if(countType == CountType.CountDown && time.second == 0 && time.minute == 0 && time.hour == 0 && time.day == 0 && time.month == 0 && time.year == 0 && time.delta != 0)time.delta = 0;
				if(countType == CountType.CountUp || time.second != 0 || time.minute != 0 || time.hour != 0 || time.day != 0 || time.month != 0 || time.year != 0)
					time.delta = time.delta + (speedType == SpeedType.Time ? (useTimeScaleUsage ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime) * speed : useTimeScaleUsage ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime) * (countType == CountType.CountUp ? 1 : -1);
			}
			time.Update();
		}
		public void SetCountType (CountType value) {if(countType != value)countType = value;}
		public void SetCountType (int value)
		{
			CountType convertedValue = (CountType)value;
			if(countType != convertedValue)countType = convertedValue;
		}
		public void SetTimeType (Time.TimeType value) {if(time.timeType != value)time.timeType = value;}
		public void SetTimeType (int value)
		{
			Time.TimeType convertedValue = (Time.TimeType)value;
			if(time.timeType != convertedValue)time.timeType = convertedValue;
		}
		public void SetAm ()
		{
			if(!time.isAm)time.isAm = true;
			if(time.isPm)time.isPm = false;
			if(time.hour == 0)
			{
				time.wasAm = true;
				time.wasPm = false;
			}
			if(time.hour >= 1 && time.hour < 12)
			{
				time.wasAm = false;
				time.wasPm = true;
			}
		}
		public void SetPm ()
		{
			if(time.isAm)time.isAm = false;
			if(!time.isPm)time.isPm = true;
			if(time.hour > 12 && time.hour < 24)
			{
				time.wasAm = true;
				time.wasPm = false;
			}
			if(time.hour == 12)
			{
				time.wasAm = false;
				time.wasPm = true;
			}
		}
		public void UseYear (bool value) {if(time.useYear != value)time.useYear = value;}
		public void SetYear (uint value) {if(time.year != value)time.year = value;}
		public void DecreaseYear (uint value) {time.year = value;}
		public void IncreaseYear (uint value) {time.year = time.year + value;}
		public void UseSeason (bool value) {if(time.useSeason != value)time.useSeason = value;}
		public void UseMonth (bool value) {if(time.useMonth != value)time.useMonth = value;}
		public void SetMonth (int value) {if(time.month != value)time.month = value;}
		public void DecreaseMonth (int value) {time.month = time.month - Mathf.Abs(value);}
		public void IncreaseMonth (int value) {time.month = time.month + Mathf.Abs(value);}
		public void UseWeek (bool value) {if(time.useWeek != value)time.useWeek = value;}
		public void UseDay (bool value) {if(time.useDay != value)time.useDay = value;}
		public void SetDay (int value) {if(time.day != value)time.day = value;}
		public void DecreaseDay (int value) {time.day = time.day - Mathf.Abs(value);}
		public void IncreaseDay (int value) {time.day = time.day + Mathf.Abs(value);}
		public void UseHour (bool value) {if(time.useHour != value)time.useHour = value;}
		public void SetHour (int value) {if(time.hour != value)time.hour = value;}
		public void DecreaseHour (int value) {time.hour = time.hour - Mathf.Abs(value);}
		public void IncreaseHour (int value) {time.hour = time.hour + Mathf.Abs(value);}
		public void UseMinute (bool value) {if(time.useMinute != value)time.useMinute = value;}
		public void SetMinute (int value) {if(time.minute != value)time.minute = value;}
		public void DecreaseMinute (int value) {time.minute = time.minute - Mathf.Abs(value);}
		public void IncreaseMinute (int value) {time.minute = time.minute + Mathf.Abs(value);}
		public void SetSecond (int value) {if(time.second != value)time.second = value;}
		public void DecreaseSecond (int value) {time.second = time.second - Mathf.Abs(value);}
		public void IncreaseSecond (int value) {time.second = time.second + Mathf.Abs(value);}
		public void SetDelta (float value) {if(time.delta != value)time.delta = value;}
		public void DecreaseDelta (float value) {time.delta = time.delta - Mathf.Abs(value);}
		public void IncreaseDelta (float value) {time.delta = time.delta + Mathf.Abs(value);}
		public void UseTimeScaleUsage (bool value) {if(useTimeScaleUsage != value)useTimeScaleUsage = value;}
		public void SetSpeedType (SpeedType value) {if(speedType != value)speedType = value;}
		public void SetSpeedType (int value)
		{
			SpeedType convertedValue = (SpeedType)value;
			if(speedType != convertedValue)speedType = convertedValue;
		}
		public void SetSpeed (float value) {if(speed != value)speed = value;}
		public void DecreaseSpeed (float value) {if(speed > 0)speed = Mathf.Clamp(speed - Mathf.Abs(value),0,float.MaxValue);}
		public void IncreaseSpeed (float value) {if(speed < float.MaxValue)speed = Mathf.Clamp(speed + Mathf.Abs(value),0,float.MaxValue);}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeSystem)),CanEditMultipleObjects]
	internal class TimeSystemEditor : Editor
	{
		private TimeSystem[] timeSystems
		{
			get
			{
				TimeSystem[] timeSystems = new TimeSystem[targets.Length];
				for(int timeSystemsIndex = 0; timeSystemsIndex < targets.Length; timeSystemsIndex++)
					timeSystems[timeSystemsIndex] = (TimeSystem)targets[timeSystemsIndex];
				return timeSystems;
			}
		}
		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			TimeSystem.Time time = timeSystems[0].time;
			SerializedProperty timeProperty = serializedObject.FindProperty("time");
			StatsSection(time);
			MainSection(timeProperty);
			TimeSection(time,timeProperty);
			SpeedSection();
			if(GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				for(int timeSystemsIndex = 0; timeSystemsIndex < timeSystems.Length; timeSystemsIndex++)
					EditorUtility.SetDirty(timeSystems[timeSystemsIndex]);
			}
		}
		private void StatsSection (TimeSystem.Time time)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUIStyle style = EditorStyles.miniLabel;
				EditorGUILayout.BeginHorizontal("Box");
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("STATS",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("Time:",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useHour;
					GUILayout.Label(time.hour.ToString("00"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(":",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useMinute;
					GUILayout.Label(time.minute.ToString("00"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(":",style);
					GUILayout.FlexibleSpace();
					GUILayout.Label(time.second.ToString("00"),style);
					GUILayout.FlexibleSpace();
					GUILayout.Label(":",style);
					GUILayout.FlexibleSpace();
					GUILayout.Label(time.delta.ToString("0.00"),style);
					if(time.timeType == Time.TimeType.TwelveHour)
					{
						GUILayout.FlexibleSpace();
						GUILayout.Label("|",style);
						GUILayout.FlexibleSpace();
						GUI.enabled = time.isAm;
						GUILayout.Label("AM",style);
						GUILayout.FlexibleSpace();
						GUI.enabled = time.isPm;
						GUILayout.Label("PM",style);
						GUI.enabled = true;
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("Elapsed Date:",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useYear;
					GUILayout.Label(time.year.ToString("0000"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(":",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useMonth;
					GUILayout.Label(time.month.ToString("00"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(":",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useDay;
					GUILayout.Label(time.day.ToString("00"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("Calendar Date:",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useYear;
					GUILayout.Label(time.year.ToString("0000"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(":",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useMonth;
					GUILayout.Label((time.month + 1).ToString("00"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label(":",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useDay;
					GUILayout.Label((time.day + 1).ToString("00"),style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					string[] seasonNames = new string[] {"Spring","Summer","Autumn","Winter"};
					string[] monthNames = new string[] {"January","February","March","April","May","June","July","August","September","October","November","December"};
					string[] weekNames = new string[] {"Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"};
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useSeason;
					GUILayout.Label(seasonNames[time.season - 1],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label("|",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useMonth;
					GUILayout.Label(monthNames[time.month % 12],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label("|",style);
					GUILayout.FlexibleSpace();
					GUI.enabled = time.useWeek;
					GUILayout.Label(weekNames[time.week - 1],style);
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		private void MainSection (SerializedProperty timeProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				GUILayout.Label("MAIN",EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("countType"),true);
				EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("timeType"),true);
			}
			EditorGUILayout.EndVertical();
		}
		private void TimeSection (TimeSystem.Time time,SerializedProperty timeProperty)
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("TIME",EditorStyles.boldLabel);
					if(time.timeType == Time.TimeType.TwelveHour)
					{
						bool canChangeAm = false;
						bool canChangePm = false;
						if(serializedObject.isEditingMultipleObjects)for(int timeSystemsIndex = 0; timeSystemsIndex < timeSystems.Length; timeSystemsIndex++)
						{
							if(!timeSystems[timeSystemsIndex].time.isAm && !canChangeAm)canChangeAm = true;
							if(!timeSystems[timeSystemsIndex].time.isPm && !canChangePm)canChangePm = true;
							if(canChangeAm && canChangePm)break;
						}
						else
						{
							if(!time.isAm)canChangeAm = true;
							if(!time.isPm)canChangePm = true;
						}
						GUILayout.FlexibleSpace();
						GUI.enabled = time.useHour;
						GUI.backgroundColor = time.isAm ? Color.green : Color.red;
						if(GUILayout.Button("AM") && canChangeAm)
						{
							Undo.RecordObjects(targets,"Inspector");
							for(int timeSystemsIndex = 0; timeSystemsIndex < timeSystems.Length; timeSystemsIndex++)if(!timeSystems[timeSystemsIndex].time.isAm)
							{
								timeSystems[timeSystemsIndex].time.isAm = true;
								timeSystems[timeSystemsIndex].time.isPm = false;
								timeSystems[timeSystemsIndex].time.wasAm = true;
								timeSystems[timeSystemsIndex].time.wasPm = false;
							}
							GUI.FocusControl(null);
						}
						GUI.backgroundColor = time.isPm ? Color.green : Color.red;
						if(GUILayout.Button("PM") && canChangePm)
						{
							Undo.RecordObjects(targets,"Inspector");
							for(int timeSystemsIndex = 0; timeSystemsIndex < timeSystems.Length; timeSystemsIndex++)if(!timeSystems[timeSystemsIndex].time.isPm)
							{
								timeSystems[timeSystemsIndex].time.isAm = false;
								timeSystems[timeSystemsIndex].time.isPm = true;
								timeSystems[timeSystemsIndex].time.wasAm = false;
								timeSystems[timeSystemsIndex].time.wasPm = true;
							}
							GUI.FocusControl(null);
						}
						GUI.backgroundColor = Color.white;
						GUI.enabled = true;
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical("Box");
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUIUtility.labelWidth = 1;
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("useSeason"),GUIContent.none,true);
						EditorGUIUtility.labelWidth = 0;
						GUILayout.Label("Season");
						GUILayout.FlexibleSpace();
					}
					EditorGUILayout.EndHorizontal();
					GUI.enabled = time.useSeason;
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("season"),GUIContent.none,true);
						EditorGUILayout.BeginHorizontal(GUILayout.Width(68));
						{
							string[] seasonNames = new string[] {"Spring","Summer","Autumn","Winter"};
							GUILayout.FlexibleSpace();
							GUILayout.Label(seasonNames[time.season - 1],new GUIStyle() {fontSize = 14});
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("useWeek"),GUIContent.none,true);
							EditorGUIUtility.labelWidth = 0;
							GUILayout.Label("Week");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = time.useWeek;
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("week"),GUIContent.none,true);
							EditorGUILayout.BeginHorizontal(GUILayout.Width(82));
							{
								string[] weekNames = new string[] {"Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"};
								GUILayout.FlexibleSpace();
								GUILayout.Label(weekNames[time.week - 1],new GUIStyle() {fontSize = 14});
								GUILayout.FlexibleSpace();
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = true;
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Delta");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("delta"),GUIContent.none,true);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("useYear"),GUIContent.none,true);
							EditorGUIUtility.labelWidth = 0;
							GUILayout.Label("Year");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = time.useYear;
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("year"),GUIContent.none,true);
						GUI.enabled = true;
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("useMonth"),GUIContent.none,true);
							EditorGUIUtility.labelWidth = 0;
							GUILayout.Label("Month");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = time.useMonth;
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("month"),GUIContent.none,true);
						GUI.enabled = true;
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("useDay"),GUIContent.none,true);
							EditorGUIUtility.labelWidth = 0;
							GUILayout.Label("Day");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = time.useDay;
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("day"),GUIContent.none,true);
						GUI.enabled = true;
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("useHour"),GUIContent.none,true);
							EditorGUIUtility.labelWidth = 0;
							GUILayout.Label("Hour");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = time.useHour;
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("hour"),GUIContent.none,true);
						GUI.enabled = true;
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUIUtility.labelWidth = 1;
							EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("useMinute"),GUIContent.none,true);
							EditorGUIUtility.labelWidth = 0;
							GUILayout.Label("Minute");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = time.useMinute;
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("minute"),GUIContent.none,true);
						GUI.enabled = true;
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical("Box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							GUILayout.Label("Second");
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.PropertyField(timeProperty.FindPropertyRelative("second"),GUIContent.none,true);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		private void SpeedSection ()
		{
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("SPEED",EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
					GUI.backgroundColor = timeSystems[0].useTimeScaleUsage ? Color.green : Color.red;
					if(GUILayout.Button("Use Time Scale Usage"))
					{
						Undo.RecordObjects(targets,"Inspector");
						timeSystems[0].useTimeScaleUsage = !timeSystems[0].useTimeScaleUsage;
						for(int timeSystemsIndex = 0; timeSystemsIndex < timeSystems.Length; timeSystemsIndex++)if(timeSystems[timeSystemsIndex].useTimeScaleUsage != timeSystems[0].useTimeScaleUsage)
							timeSystems[timeSystemsIndex].useTimeScaleUsage = timeSystems[0].useTimeScaleUsage;
						GUI.FocusControl(null);
					}
					GUI.backgroundColor = Color.white;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("speedType"),true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"),true);
			}
			EditorGUILayout.EndVertical();
		}
	}
	#endif
}