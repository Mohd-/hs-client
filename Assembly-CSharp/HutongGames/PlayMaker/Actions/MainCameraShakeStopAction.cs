using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC4 RID: 3524
	[ActionCategory("Pegasus")]
	[Tooltip("[DEPRECATED - Use CameraShakeStopAction instead.] Stops shaking the Main Camera over time.")]
	public class MainCameraShakeStopAction : FsmStateAction
	{
		// Token: 0x06006D29 RID: 27945 RVA: 0x002017F9 File Offset: 0x001FF9F9
		public override void Reset()
		{
			this.m_Time = 0.5f;
			this.m_Delay = 0f;
			this.m_FinishedEvent = null;
		}

		// Token: 0x06006D2A RID: 27946 RVA: 0x00201822 File Offset: 0x001FFA22
		public override void OnEnter()
		{
			this.m_timerSec = 0f;
			this.m_shakeStopped = false;
		}

		// Token: 0x06006D2B RID: 27947 RVA: 0x00201838 File Offset: 0x001FFA38
		public override void OnUpdate()
		{
			this.m_timerSec += Time.deltaTime;
			if (this.m_timerSec >= this.m_Delay.Value && !this.m_shakeStopped)
			{
				this.StopShake();
			}
			if (this.m_timerSec >= this.m_Delay.Value + this.m_Time.Value)
			{
				base.Fsm.Event(this.m_FinishedEvent);
				base.Finish();
			}
		}

		// Token: 0x06006D2C RID: 27948 RVA: 0x002018B7 File Offset: 0x001FFAB7
		private void StopShake()
		{
			CameraShakeMgr.Stop(Camera.main, this.m_Time.Value);
			this.m_shakeStopped = true;
		}

		// Token: 0x040055CF RID: 21967
		public FsmFloat m_Time;

		// Token: 0x040055D0 RID: 21968
		public FsmFloat m_Delay;

		// Token: 0x040055D1 RID: 21969
		public FsmEvent m_FinishedEvent;

		// Token: 0x040055D2 RID: 21970
		private float m_timerSec;

		// Token: 0x040055D3 RID: 21971
		private bool m_shakeStopped;
	}
}
