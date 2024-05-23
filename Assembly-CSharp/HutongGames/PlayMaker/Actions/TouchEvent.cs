using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFF RID: 3327
	[ActionCategory(33)]
	[Tooltip("Sends events based on Touch Phases. Optionally filter by a fingerID.")]
	public class TouchEvent : FsmStateAction
	{
		// Token: 0x060069B3 RID: 27059 RVA: 0x001EF610 File Offset: 0x001ED810
		public override void Reset()
		{
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.storeFingerId = null;
		}

		// Token: 0x060069B4 RID: 27060 RVA: 0x001EF638 File Offset: 0x001ED838
		public override void OnUpdate()
		{
			if (Input.touchCount > 0)
			{
				foreach (Touch touch in Input.touches)
				{
					if ((this.fingerId.IsNone || touch.fingerId == this.fingerId.Value) && touch.phase == this.touchPhase)
					{
						this.storeFingerId.Value = touch.fingerId;
						base.Fsm.Event(this.sendEvent);
					}
				}
			}
		}

		// Token: 0x04005170 RID: 20848
		public FsmInt fingerId;

		// Token: 0x04005171 RID: 20849
		public TouchPhase touchPhase;

		// Token: 0x04005172 RID: 20850
		public FsmEvent sendEvent;

		// Token: 0x04005173 RID: 20851
		[UIHint(10)]
		public FsmInt storeFingerId;
	}
}
