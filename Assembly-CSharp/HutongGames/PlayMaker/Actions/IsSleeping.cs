using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4F RID: 3151
	[Tooltip("Tests if a Game Object's Rigid Body is sleeping.")]
	[ActionCategory(9)]
	public class IsSleeping : ComponentAction<Rigidbody>
	{
		// Token: 0x060066AB RID: 26283 RVA: 0x001E5035 File Offset: 0x001E3235
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x060066AC RID: 26284 RVA: 0x001E505A File Offset: 0x001E325A
		public override void OnEnter()
		{
			this.DoIsSleeping();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066AD RID: 26285 RVA: 0x001E5073 File Offset: 0x001E3273
		public override void OnUpdate()
		{
			this.DoIsSleeping();
		}

		// Token: 0x060066AE RID: 26286 RVA: 0x001E507C File Offset: 0x001E327C
		private void DoIsSleeping()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool flag = base.rigidbody.IsSleeping();
				this.store.Value = flag;
				base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
			}
		}

		// Token: 0x04004E64 RID: 20068
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E65 RID: 20069
		public FsmEvent trueEvent;

		// Token: 0x04004E66 RID: 20070
		public FsmEvent falseEvent;

		// Token: 0x04004E67 RID: 20071
		[UIHint(10)]
		public FsmBool store;

		// Token: 0x04004E68 RID: 20072
		public bool everyFrame;
	}
}
