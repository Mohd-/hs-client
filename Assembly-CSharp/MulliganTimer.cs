using System;
using UnityEngine;

// Token: 0x020008FE RID: 2302
public class MulliganTimer : MonoBehaviour
{
	// Token: 0x06005615 RID: 22037 RVA: 0x0019DC7C File Offset: 0x0019BE7C
	private void Start()
	{
		if (MulliganManager.Get() == null)
		{
			return;
		}
		Vector3 position = MulliganManager.Get().GetMulliganButton().transform.position;
		if (UniversalInputManager.UsePhoneUI)
		{
			base.transform.position = new Vector3(position.x + 1.8f, position.y, position.z);
		}
		else
		{
			base.transform.position = new Vector3(position.x, position.y, position.z - 1f);
		}
	}

	// Token: 0x06005616 RID: 22038 RVA: 0x0019DD1C File Offset: 0x0019BF1C
	private void Update()
	{
		if (!this.m_remainingTimeSet)
		{
			return;
		}
		float num = this.ComputeCountdownRemainingSec();
		int num2 = Mathf.RoundToInt(num);
		if (num2 < 0)
		{
			num2 = 0;
		}
		this.m_timeText.Text = string.Format(":{0:D2}", num2);
		if (num > 0f)
		{
			return;
		}
		if (MulliganManager.Get())
		{
			MulliganManager.Get().AutomaticContinueMulligan();
		}
		else
		{
			this.SelfDestruct();
		}
	}

	// Token: 0x06005617 RID: 22039 RVA: 0x0019DD98 File Offset: 0x0019BF98
	private float ComputeCountdownRemainingSec()
	{
		float num = this.m_endTimeStamp - Time.realtimeSinceStartup;
		if (num < 0f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x06005618 RID: 22040 RVA: 0x0019DDC4 File Offset: 0x0019BFC4
	public void SetEndTime(float endTimeStamp)
	{
		this.m_endTimeStamp = endTimeStamp;
		this.m_remainingTimeSet = true;
	}

	// Token: 0x06005619 RID: 22041 RVA: 0x0019DDD4 File Offset: 0x0019BFD4
	public void SelfDestruct()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04003C75 RID: 15477
	public UberText m_timeText;

	// Token: 0x04003C76 RID: 15478
	private bool m_remainingTimeSet;

	// Token: 0x04003C77 RID: 15479
	private float m_endTimeStamp;
}
