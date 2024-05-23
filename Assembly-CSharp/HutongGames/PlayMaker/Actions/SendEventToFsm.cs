using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8E RID: 3214
	[Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
	[ActionCategory(12)]
	[Tooltip("Sends an Event to another Fsm after an optional delay. Specify an Fsm Name or use the first Fsm on the object.")]
	public class SendEventToFsm : FsmStateAction
	{
		// Token: 0x060067C2 RID: 26562 RVA: 0x001E91CE File Offset: 0x001E73CE
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.sendEvent = null;
			this.delay = null;
			this.requireReceiver = false;
		}

		// Token: 0x060067C3 RID: 26563 RVA: 0x001E91F4 File Offset: 0x001E73F4
		public override void OnEnter()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(this.go, this.fsmName.Value);
			if (gameObjectFsm == null)
			{
				if (this.requireReceiver)
				{
					this.LogError("GameObject doesn't have FsmComponent: " + this.go.name + " " + this.fsmName.Value);
				}
				return;
			}
			if ((double)this.delay.Value < 0.001)
			{
				gameObjectFsm.Fsm.Event(this.sendEvent.Value);
				base.Finish();
			}
			else
			{
				this.delayedEvent = gameObjectFsm.Fsm.DelayedEvent(FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
			}
		}

		// Token: 0x060067C4 RID: 26564 RVA: 0x001E92F1 File Offset: 0x001E74F1
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x04004F96 RID: 20374
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004F97 RID: 20375
		[UIHint(15)]
		[Tooltip("Optional name of Fsm on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004F98 RID: 20376
		[UIHint(16)]
		[RequiredField]
		public FsmString sendEvent;

		// Token: 0x04004F99 RID: 20377
		[HasFloatSlider(0f, 10f)]
		public FsmFloat delay;

		// Token: 0x04004F9A RID: 20378
		private bool requireReceiver;

		// Token: 0x04004F9B RID: 20379
		private GameObject go;

		// Token: 0x04004F9C RID: 20380
		private DelayedEvent delayedEvent;
	}
}
