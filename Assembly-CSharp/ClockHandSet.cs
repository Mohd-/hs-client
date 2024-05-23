using System;
using UnityEngine;

// Token: 0x02000EFA RID: 3834
public class ClockHandSet : MonoBehaviour
{
	// Token: 0x06007298 RID: 29336 RVA: 0x0021B0A0 File Offset: 0x002192A0
	private void Update()
	{
		DateTime now = DateTime.Now;
		int minute = now.Minute;
		if (minute != this.m_prevMinute)
		{
			float num = this.ComputeMinuteRotation(minute);
			float num2 = this.ComputeMinuteRotation(this.m_prevMinute);
			float num3 = num - num2;
			this.m_MinuteHand.transform.Rotate(Vector3.up, num3);
			this.m_prevMinute = minute;
		}
		int num4 = now.Hour % 12;
		if (num4 != this.m_prevHour)
		{
			float num5 = this.ComputeHourRotation(num4);
			float num6 = this.ComputeHourRotation(this.m_prevHour);
			float num7 = num5 - num6;
			this.m_HourHand.transform.Rotate(Vector3.up, num7);
			this.m_prevHour = num4;
		}
	}

	// Token: 0x06007299 RID: 29337 RVA: 0x0021B156 File Offset: 0x00219356
	private float ComputeMinuteRotation(int minute)
	{
		return (float)minute * 6f;
	}

	// Token: 0x0600729A RID: 29338 RVA: 0x0021B160 File Offset: 0x00219360
	private float ComputeHourRotation(int hour)
	{
		return (float)hour * 30f;
	}

	// Token: 0x04005CA5 RID: 23717
	public GameObject m_MinuteHand;

	// Token: 0x04005CA6 RID: 23718
	public GameObject m_HourHand;

	// Token: 0x04005CA7 RID: 23719
	private int m_prevMinute;

	// Token: 0x04005CA8 RID: 23720
	private int m_prevHour;
}
