using System;
using UnityEngine;

// Token: 0x0200097B RID: 2427
public class MatchingQueueTab : MonoBehaviour
{
	// Token: 0x0600580A RID: 22538 RVA: 0x001A5491 File Offset: 0x001A3691
	private void Update()
	{
		this.InitTimeStringSet();
		this.m_timeInQueue += Time.deltaTime;
		this.m_waitTime.Text = TimeUtils.GetElapsedTimeString(Mathf.RoundToInt(this.m_timeInQueue), this.m_timeStringSet);
	}

	// Token: 0x0600580B RID: 22539 RVA: 0x001A54CC File Offset: 0x001A36CC
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600580C RID: 22540 RVA: 0x001A54DA File Offset: 0x001A36DA
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600580D RID: 22541 RVA: 0x001A54E8 File Offset: 0x001A36E8
	public void ResetTimer()
	{
		this.m_timeInQueue = 0f;
	}

	// Token: 0x0600580E RID: 22542 RVA: 0x001A54F8 File Offset: 0x001A36F8
	public void UpdateDisplay(int minSeconds, int maxSeconds)
	{
		this.InitTimeStringSet();
		int num = Mathf.RoundToInt(this.m_timeInQueue);
		maxSeconds += num;
		if (maxSeconds <= 30)
		{
			this.Hide();
			return;
		}
		this.m_queueTime.Text = this.GetElapsedTimeString(minSeconds + num, maxSeconds);
		this.Show();
	}

	// Token: 0x0600580F RID: 22543 RVA: 0x001A5548 File Offset: 0x001A3748
	private void InitTimeStringSet()
	{
		if (this.m_timeStringSet == null)
		{
			this.m_timeStringSet = new TimeUtils.ElapsedStringSet
			{
				m_seconds = "GLOBAL_DATETIME_SPINNER_SECONDS",
				m_minutes = "GLOBAL_DATETIME_SPINNER_MINUTES",
				m_hours = "GLOBAL_DATETIME_SPINNER_HOURS",
				m_yesterday = "GLOBAL_DATETIME_SPINNER_DAY",
				m_days = "GLOBAL_DATETIME_SPINNER_DAYS",
				m_weeks = "GLOBAL_DATETIME_SPINNER_WEEKS",
				m_monthAgo = "GLOBAL_DATETIME_SPINNER_MONTH"
			};
		}
	}

	// Token: 0x06005810 RID: 22544 RVA: 0x001A55BC File Offset: 0x001A37BC
	private string GetElapsedTimeString(int minSeconds, int maxSeconds)
	{
		TimeUtils.ElapsedTimeType elapsedTimeType;
		int num;
		TimeUtils.GetElapsedTime(minSeconds, out elapsedTimeType, out num);
		if (minSeconds == maxSeconds)
		{
			return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME", new object[]
			{
				TimeUtils.GetElapsedTimeString(minSeconds, this.m_timeStringSet)
			});
		}
		TimeUtils.ElapsedTimeType elapsedTimeType2;
		int num2;
		TimeUtils.GetElapsedTime(maxSeconds, out elapsedTimeType2, out num2);
		if (elapsedTimeType != elapsedTimeType2)
		{
			string elapsedTimeString = TimeUtils.GetElapsedTimeString(elapsedTimeType, num, this.m_timeStringSet);
			string elapsedTimeString2 = TimeUtils.GetElapsedTimeString(elapsedTimeType2, num2, this.m_timeStringSet);
			return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", new object[]
			{
				elapsedTimeString,
				elapsedTimeString2
			});
		}
		switch (elapsedTimeType)
		{
		case TimeUtils.ElapsedTimeType.SECONDS:
			return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", new object[]
			{
				num,
				GameStrings.Format(this.m_timeStringSet.m_seconds, new object[]
				{
					num2
				})
			});
		case TimeUtils.ElapsedTimeType.MINUTES:
			return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", new object[]
			{
				num,
				GameStrings.Format(this.m_timeStringSet.m_minutes, new object[]
				{
					num2
				})
			});
		case TimeUtils.ElapsedTimeType.HOURS:
			return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", new object[]
			{
				num,
				GameStrings.Format(this.m_timeStringSet.m_hours, new object[]
				{
					num2
				})
			});
		case TimeUtils.ElapsedTimeType.YESTERDAY:
			return GameStrings.Get(this.m_timeStringSet.m_yesterday);
		case TimeUtils.ElapsedTimeType.DAYS:
			return GameStrings.Format("GLOBAL_APPROXIMATE_DATETIME_RANGE", new object[]
			{
				num,
				GameStrings.Format(this.m_timeStringSet.m_days, new object[]
				{
					num2
				})
			});
		case TimeUtils.ElapsedTimeType.WEEKS:
			return GameStrings.Format(this.m_timeStringSet.m_weeks, new object[]
			{
				num,
				num2
			});
		default:
			return GameStrings.Get(this.m_timeStringSet.m_monthAgo);
		}
	}

	// Token: 0x04003F2C RID: 16172
	private const string TIME_RANGE_STRING = "GLOBAL_APPROXIMATE_DATETIME_RANGE";

	// Token: 0x04003F2D RID: 16173
	private const string TIME_STRING = "GLOBAL_APPROXIMATE_DATETIME";

	// Token: 0x04003F2E RID: 16174
	private const int SUPPRESS_TIME = 30;

	// Token: 0x04003F2F RID: 16175
	public UberText m_waitTime;

	// Token: 0x04003F30 RID: 16176
	public UberText m_queueTime;

	// Token: 0x04003F31 RID: 16177
	private TimeUtils.ElapsedStringSet m_timeStringSet;

	// Token: 0x04003F32 RID: 16178
	private float m_timeInQueue;
}
