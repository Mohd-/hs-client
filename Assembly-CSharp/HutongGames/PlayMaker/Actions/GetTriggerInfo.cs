using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3B RID: 3131
	[ActionCategory(9)]
	[Tooltip("Gets info on the last Trigger event and store in variables.")]
	public class GetTriggerInfo : FsmStateAction
	{
		// Token: 0x06006652 RID: 26194 RVA: 0x001E41D4 File Offset: 0x001E23D4
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.physicsMaterialName = null;
		}

		// Token: 0x06006653 RID: 26195 RVA: 0x001E41E4 File Offset: 0x001E23E4
		private void StoreTriggerInfo()
		{
			if (base.Fsm.TriggerCollider == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.TriggerCollider.gameObject;
			this.physicsMaterialName.Value = base.Fsm.TriggerCollider.material.name;
		}

		// Token: 0x06006654 RID: 26196 RVA: 0x001E4243 File Offset: 0x001E2443
		public override void OnEnter()
		{
			this.StoreTriggerInfo();
			base.Finish();
		}

		// Token: 0x04004E0E RID: 19982
		[UIHint(10)]
		public FsmGameObject gameObjectHit;

		// Token: 0x04004E0F RID: 19983
		[UIHint(10)]
		[Tooltip("Useful for triggering different effects. Audio, particles...")]
		public FsmString physicsMaterialName;
	}
}
