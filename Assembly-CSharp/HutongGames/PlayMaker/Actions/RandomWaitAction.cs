using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD3 RID: 3539
	[ActionCategory("Pegasus")]
	[Tooltip("Delays a State from finishing by a random time. NOTE: Other actions continue, but FINISHED can't happen before the delay.")]
	public class RandomWaitAction : FsmStateAction
	{
		// Token: 0x06006D67 RID: 28007 RVA: 0x00202954 File Offset: 0x00200B54
		public override void Reset()
		{
			this.m_MinTime = 1f;
			this.m_MaxTime = 1f;
			this.m_FinishEvent = null;
			this.m_RealTime = false;
		}

		// Token: 0x06006D68 RID: 28008 RVA: 0x00202990 File Offset: 0x00200B90
		public override void OnEnter()
		{
			if (this.m_MinTime.Value <= 0f && this.m_MaxTime.Value <= 0f)
			{
				base.Finish();
				if (this.m_FinishEvent != null)
				{
					base.Fsm.Event(this.m_FinishEvent);
				}
				return;
			}
			this.m_startTime = FsmTime.RealtimeSinceStartup;
			this.m_waitTime = Random.Range(this.m_MinTime.Value, this.m_MaxTime.Value);
			this.m_updateTime = 0f;
		}

		// Token: 0x06006D69 RID: 28009 RVA: 0x00202A24 File Offset: 0x00200C24
		public override void OnUpdate()
		{
			if (this.m_RealTime)
			{
				this.m_updateTime = FsmTime.RealtimeSinceStartup - this.m_startTime;
			}
			else
			{
				this.m_updateTime += Time.deltaTime;
			}
			if (this.m_updateTime > this.m_waitTime)
			{
				base.Finish();
				if (this.m_FinishEvent != null)
				{
					base.Fsm.Event(this.m_FinishEvent);
				}
			}
		}

		// Token: 0x04005613 RID: 22035
		public FsmFloat m_MinTime;

		// Token: 0x04005614 RID: 22036
		public FsmFloat m_MaxTime;

		// Token: 0x04005615 RID: 22037
		public FsmEvent m_FinishEvent;

		// Token: 0x04005616 RID: 22038
		public bool m_RealTime;

		// Token: 0x04005617 RID: 22039
		private float m_startTime;

		// Token: 0x04005618 RID: 22040
		private float m_waitTime;

		// Token: 0x04005619 RID: 22041
		private float m_updateTime;
	}
}
