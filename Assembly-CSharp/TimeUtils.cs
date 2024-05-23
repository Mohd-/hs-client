using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x020000BA RID: 186
public class TimeUtils
{
	// Token: 0x060009E5 RID: 2533 RVA: 0x0002AA80 File Offset: 0x00028C80
	public static long BinaryStamp()
	{
		return DateTime.UtcNow.ToBinary();
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x0002AA9C File Offset: 0x00028C9C
	public static DateTime ConvertEpochMicrosecToDateTime(ulong microsec)
	{
		return TimeUtils.EPOCH_TIME.AddMilliseconds(microsec / 1000.0);
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x0002AAC8 File Offset: 0x00028CC8
	public static TimeSpan GetElapsedTimeSinceEpoch(DateTime? endDateTime = null)
	{
		DateTime dateTime = (endDateTime == null) ? DateTime.UtcNow : endDateTime.Value;
		return dateTime - TimeUtils.EPOCH_TIME;
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0002AB00 File Offset: 0x00028D00
	public static string GetElapsedTimeStringFromEpochMicrosec(ulong microsec, TimeUtils.ElapsedStringSet stringSet)
	{
		DateTime dateTime = TimeUtils.ConvertEpochMicrosecToDateTime(microsec);
		DateTime utcNow = DateTime.UtcNow;
		int seconds = (int)(utcNow - dateTime).TotalSeconds;
		return TimeUtils.GetElapsedTimeString(seconds, stringSet);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0002AB34 File Offset: 0x00028D34
	public static string GetElapsedTimeString(int seconds, TimeUtils.ElapsedStringSet stringSet)
	{
		TimeUtils.ElapsedTimeType timeType;
		int time;
		TimeUtils.GetElapsedTime(seconds, out timeType, out time);
		return TimeUtils.GetElapsedTimeString(timeType, time, stringSet);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0002AB54 File Offset: 0x00028D54
	public static string GetElapsedTimeString(TimeUtils.ElapsedTimeType timeType, int time, TimeUtils.ElapsedStringSet stringSet)
	{
		switch (timeType)
		{
		case TimeUtils.ElapsedTimeType.SECONDS:
			return GameStrings.Format(stringSet.m_seconds, new object[]
			{
				time
			});
		case TimeUtils.ElapsedTimeType.MINUTES:
			return GameStrings.Format(stringSet.m_minutes, new object[]
			{
				time
			});
		case TimeUtils.ElapsedTimeType.HOURS:
			return GameStrings.Format(stringSet.m_hours, new object[]
			{
				time
			});
		case TimeUtils.ElapsedTimeType.YESTERDAY:
			if (stringSet.m_yesterday == null)
			{
				return GameStrings.Format(stringSet.m_days, new object[]
				{
					1
				});
			}
			return GameStrings.Get(stringSet.m_yesterday);
		case TimeUtils.ElapsedTimeType.DAYS:
			return GameStrings.Format(stringSet.m_days, new object[]
			{
				time
			});
		case TimeUtils.ElapsedTimeType.WEEKS:
			return GameStrings.Format(stringSet.m_weeks, new object[]
			{
				time
			});
		default:
			return GameStrings.Get(stringSet.m_monthAgo);
		}
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0002AC4C File Offset: 0x00028E4C
	public static void GetElapsedTime(int seconds, out TimeUtils.ElapsedTimeType timeType, out int time)
	{
		time = 0;
		if (seconds < 60)
		{
			timeType = TimeUtils.ElapsedTimeType.SECONDS;
			time = seconds;
			return;
		}
		if (seconds < 3600)
		{
			timeType = TimeUtils.ElapsedTimeType.MINUTES;
			time = seconds / 60;
			return;
		}
		int num = seconds / 86400;
		if (num == 0)
		{
			timeType = TimeUtils.ElapsedTimeType.HOURS;
			time = seconds / 3600;
			return;
		}
		if (num == 1)
		{
			timeType = TimeUtils.ElapsedTimeType.YESTERDAY;
			return;
		}
		int num2 = seconds / 604800;
		if (num2 == 0)
		{
			timeType = TimeUtils.ElapsedTimeType.DAYS;
			time = num;
			return;
		}
		if (num2 < 4)
		{
			timeType = TimeUtils.ElapsedTimeType.WEEKS;
			time = num2;
			return;
		}
		timeType = TimeUtils.ElapsedTimeType.MONTH_AGO;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0002ACCC File Offset: 0x00028ECC
	public static string GetDevElapsedTimeString(TimeSpan span)
	{
		return TimeUtils.GetDevElapsedTimeString((long)span.TotalMilliseconds);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0002ACDC File Offset: 0x00028EDC
	public static string GetDevElapsedTimeString(long ms)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		if (ms >= 3600000L)
		{
			TimeUtils.AppendDevTimeUnitsString("{0}h", 3600000, stringBuilder, ref ms, ref num);
		}
		if (ms >= 60000L)
		{
			TimeUtils.AppendDevTimeUnitsString("{0}m", 60000, stringBuilder, ref ms, ref num);
		}
		if (ms >= 1000L)
		{
			TimeUtils.AppendDevTimeUnitsString("{0}s", 1000, stringBuilder, ref ms, ref num);
		}
		if (num <= 1)
		{
			if (num > 0)
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.AppendFormat("{0}ms", ms);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0002AD80 File Offset: 0x00028F80
	public static string GetDevElapsedTimeString(float sec)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		if (sec >= 3600f)
		{
			TimeUtils.AppendDevTimeUnitsString("{0}h", 3600f, stringBuilder, ref sec, ref num);
		}
		if (sec >= 60f)
		{
			TimeUtils.AppendDevTimeUnitsString("{0}m", 60f, stringBuilder, ref sec, ref num);
		}
		if (sec >= 1f)
		{
			TimeUtils.AppendDevTimeUnitsString("{0}s", 1f, stringBuilder, ref sec, ref num);
		}
		if (num <= 1)
		{
			if (num > 0)
			{
				stringBuilder.Append(' ');
			}
			float num2 = sec * 1000f;
			if (num2 > 0f)
			{
				stringBuilder.AppendFormat("{0:f0}ms", num2);
			}
			else
			{
				stringBuilder.AppendFormat("{0}ms", num2);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0002AE4C File Offset: 0x0002904C
	public static bool TryParseDevSecFromElapsedTimeString(string timeStr, out float sec)
	{
		sec = 0f;
		MatchCollection matchCollection = Regex.Matches(timeStr, "(?<number>(?:[0-9]+,)*[0-9]+)\\s*(?<units>[a-zA-Z]+)");
		if (matchCollection.Count == 0)
		{
			return false;
		}
		Match match = matchCollection[0];
		if (!match.Groups[0].Success)
		{
			return false;
		}
		Group group = match.Groups["number"];
		Group group2 = match.Groups["units"];
		if (!group.Success || !group2.Success)
		{
			return false;
		}
		string value = group.Value;
		string text = group2.Value;
		if (!float.TryParse(value, ref sec))
		{
			return false;
		}
		text = TimeUtils.ParseTimeUnitsStr(text);
		if (text == "min")
		{
			sec *= 60f;
		}
		else if (text == "hour")
		{
			sec *= 3600f;
		}
		return true;
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0002AF38 File Offset: 0x00029138
	public static float ForceDevSecFromElapsedTimeString(string timeStr)
	{
		float result;
		TimeUtils.TryParseDevSecFromElapsedTimeString(timeStr, out result);
		return result;
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0002AF50 File Offset: 0x00029150
	private static void AppendDevTimeUnitsString(string formatString, int msPerUnit, StringBuilder builder, ref long ms, ref int unitCount)
	{
		long num = ms / (long)msPerUnit;
		if (num > 0L)
		{
			if (unitCount > 0)
			{
				builder.Append(' ');
			}
			builder.AppendFormat(formatString, num);
			unitCount++;
		}
		ms -= num * (long)msPerUnit;
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0002AF9C File Offset: 0x0002919C
	private static void AppendDevTimeUnitsString(string formatString, float secPerUnit, StringBuilder builder, ref float sec, ref int unitCount)
	{
		float num = Mathf.Floor(sec / secPerUnit);
		if (num > 0f)
		{
			if (unitCount > 0)
			{
				builder.Append(' ');
			}
			builder.AppendFormat(formatString, num);
			unitCount++;
		}
		sec -= num * secPerUnit;
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x0002AFF0 File Offset: 0x000291F0
	private static string ParseTimeUnitsStr(string unitsStr)
	{
		if (unitsStr == null)
		{
			return "sec";
		}
		unitsStr = unitsStr.ToLowerInvariant();
		string text = unitsStr;
		if (text != null)
		{
			if (TimeUtils.<>f__switch$map90 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("s", 0);
				dictionary.Add("sec", 0);
				dictionary.Add("secs", 0);
				dictionary.Add("second", 0);
				dictionary.Add("seconds", 0);
				dictionary.Add("m", 1);
				dictionary.Add("min", 1);
				dictionary.Add("mins", 1);
				dictionary.Add("minute", 1);
				dictionary.Add("minutes", 1);
				dictionary.Add("h", 2);
				dictionary.Add("hour", 2);
				dictionary.Add("hours", 2);
				TimeUtils.<>f__switch$map90 = dictionary;
			}
			int num;
			if (TimeUtils.<>f__switch$map90.TryGetValue(text, ref num))
			{
				switch (num)
				{
				case 0:
					return "sec";
				case 1:
					return "min";
				case 2:
					return "hour";
				}
			}
		}
		return "sec";
	}

	// Token: 0x040004C2 RID: 1218
	public const int SEC_PER_MINUTE = 60;

	// Token: 0x040004C3 RID: 1219
	public const int SEC_PER_HOUR = 3600;

	// Token: 0x040004C4 RID: 1220
	public const int SEC_PER_DAY = 86400;

	// Token: 0x040004C5 RID: 1221
	public const int SEC_PER_WEEK = 604800;

	// Token: 0x040004C6 RID: 1222
	public const int MS_PER_SEC = 1000;

	// Token: 0x040004C7 RID: 1223
	public const int MS_PER_MINUTE = 60000;

	// Token: 0x040004C8 RID: 1224
	public const int MS_PER_HOUR = 3600000;

	// Token: 0x040004C9 RID: 1225
	public const string DEFAULT_TIME_UNITS_STR = "sec";

	// Token: 0x040004CA RID: 1226
	public static readonly DateTime EPOCH_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 1);

	// Token: 0x040004CB RID: 1227
	public static readonly TimeUtils.ElapsedStringSet SPLASHSCREEN_DATETIME_STRINGSET = new TimeUtils.ElapsedStringSet
	{
		m_seconds = "GLOBAL_DATETIME_SPLASHSCREEN_SECONDS",
		m_minutes = "GLOBAL_DATETIME_SPLASHSCREEN_MINUTES",
		m_hours = "GLOBAL_DATETIME_SPLASHSCREEN_HOURS",
		m_yesterday = "GLOBAL_DATETIME_SPLASHSCREEN_DAY",
		m_days = "GLOBAL_DATETIME_SPLASHSCREEN_DAYS",
		m_weeks = "GLOBAL_DATETIME_SPLASHSCREEN_WEEKS",
		m_monthAgo = "GLOBAL_DATETIME_SPLASHSCREEN_MONTH"
	};

	// Token: 0x020004B3 RID: 1203
	public class ElapsedStringSet
	{
		// Token: 0x040024BF RID: 9407
		public string m_seconds;

		// Token: 0x040024C0 RID: 9408
		public string m_minutes;

		// Token: 0x040024C1 RID: 9409
		public string m_hours;

		// Token: 0x040024C2 RID: 9410
		public string m_yesterday;

		// Token: 0x040024C3 RID: 9411
		public string m_days;

		// Token: 0x040024C4 RID: 9412
		public string m_weeks;

		// Token: 0x040024C5 RID: 9413
		public string m_monthAgo;
	}

	// Token: 0x0200064A RID: 1610
	public enum ElapsedTimeType
	{
		// Token: 0x04002C44 RID: 11332
		SECONDS,
		// Token: 0x04002C45 RID: 11333
		MINUTES,
		// Token: 0x04002C46 RID: 11334
		HOURS,
		// Token: 0x04002C47 RID: 11335
		YESTERDAY,
		// Token: 0x04002C48 RID: 11336
		DAYS,
		// Token: 0x04002C49 RID: 11337
		WEEKS,
		// Token: 0x04002C4A RID: 11338
		MONTH_AGO
	}
}
