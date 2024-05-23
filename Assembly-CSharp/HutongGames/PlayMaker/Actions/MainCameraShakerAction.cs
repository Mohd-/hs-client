using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC5 RID: 3525
	[Tooltip("[DEPRECATED - Use CameraShakerAction instead.] Shakes the Main Camera a specified amount over time.")]
	[ActionCategory("Pegasus")]
	public class MainCameraShakerAction : FsmStateAction
	{
		// Token: 0x06006D2E RID: 27950 RVA: 0x002018E0 File Offset: 0x001FFAE0
		public override void Reset()
		{
			this.m_Amount = new FsmVector3
			{
				UseVariable = false
			};
			this.m_Time = 1f;
			this.m_Delay = 0f;
			this.m_FinishedEvent = null;
		}

		// Token: 0x06006D2F RID: 27951 RVA: 0x00201928 File Offset: 0x001FFB28
		public override void OnEnter()
		{
			this.m_timerSec = 0f;
			this.m_shakeFired = false;
		}

		// Token: 0x06006D30 RID: 27952 RVA: 0x0020193C File Offset: 0x001FFB3C
		public override void OnUpdate()
		{
			this.m_timerSec += Time.deltaTime;
			if (this.m_timerSec >= this.m_Delay.Value && !this.m_shakeFired)
			{
				this.Shake();
			}
			if (this.m_timerSec >= this.m_Delay.Value + this.m_Time.Value)
			{
				base.Fsm.Event(this.m_FinishedEvent);
				base.Finish();
			}
		}

		// Token: 0x06006D31 RID: 27953 RVA: 0x002019BC File Offset: 0x001FFBBC
		private void Shake()
		{
			CameraShakeMgr.Shake(Camera.main, this.m_Amount.Value, this.m_Time.Value);
			this.m_shakeFired = true;
		}

		// Token: 0x040055D4 RID: 21972
		public FsmVector3 m_Amount;

		// Token: 0x040055D5 RID: 21973
		public FsmFloat m_Time;

		// Token: 0x040055D6 RID: 21974
		public FsmFloat m_Delay;

		// Token: 0x040055D7 RID: 21975
		public FsmEvent m_FinishedEvent;

		// Token: 0x040055D8 RID: 21976
		private float m_timerSec;

		// Token: 0x040055D9 RID: 21977
		private bool m_shakeFired;
	}
}
