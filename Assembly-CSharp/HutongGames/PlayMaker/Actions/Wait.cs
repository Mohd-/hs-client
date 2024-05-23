using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1C RID: 3356
	[ActionCategory(31)]
	[Tooltip("Delays a State from finishing by the specified time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
	public class Wait : FsmStateAction
	{
		// Token: 0x06006A27 RID: 27175 RVA: 0x001F1408 File Offset: 0x001EF608
		public override void Reset()
		{
			this.time = 1f;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006A28 RID: 27176 RVA: 0x001F1428 File Offset: 0x001EF628
		public override void OnEnter()
		{
			if (this.time.Value <= 0f)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x06006A29 RID: 27177 RVA: 0x001F1478 File Offset: 0x001EF678
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer >= this.time.Value)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}

		// Token: 0x04005205 RID: 20997
		[RequiredField]
		public FsmFloat time;

		// Token: 0x04005206 RID: 20998
		public FsmEvent finishEvent;

		// Token: 0x04005207 RID: 20999
		public bool realTime;

		// Token: 0x04005208 RID: 21000
		private float startTime;

		// Token: 0x04005209 RID: 21001
		private float timer;
	}
}
