using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4E RID: 3150
	[ActionCategory(9)]
	[Tooltip("Tests if a Game Object's Rigid Body is Kinematic.")]
	public class IsKinematic : ComponentAction<Rigidbody>
	{
		// Token: 0x060066A6 RID: 26278 RVA: 0x001E4F82 File Offset: 0x001E3182
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x060066A7 RID: 26279 RVA: 0x001E4FA7 File Offset: 0x001E31A7
		public override void OnEnter()
		{
			this.DoIsKinematic();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066A8 RID: 26280 RVA: 0x001E4FC0 File Offset: 0x001E31C0
		public override void OnUpdate()
		{
			this.DoIsKinematic();
		}

		// Token: 0x060066A9 RID: 26281 RVA: 0x001E4FC8 File Offset: 0x001E31C8
		private void DoIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool isKinematic = base.rigidbody.isKinematic;
				this.store.Value = isKinematic;
				base.Fsm.Event((!isKinematic) ? this.falseEvent : this.trueEvent);
			}
		}

		// Token: 0x04004E5F RID: 20063
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E60 RID: 20064
		public FsmEvent trueEvent;

		// Token: 0x04004E61 RID: 20065
		public FsmEvent falseEvent;

		// Token: 0x04004E62 RID: 20066
		[UIHint(10)]
		public FsmBool store;

		// Token: 0x04004E63 RID: 20067
		public bool everyFrame;
	}
}
