using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE5 RID: 3045
	[ActionCategory(27)]
	[Tooltip("Tests if a GameObject has children.")]
	public class GameObjectHasChildren : FsmStateAction
	{
		// Token: 0x060064E5 RID: 25829 RVA: 0x001DFD2F File Offset: 0x001DDF2F
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060064E6 RID: 25830 RVA: 0x001DFD54 File Offset: 0x001DDF54
		public override void OnEnter()
		{
			this.DoHasChildren();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064E7 RID: 25831 RVA: 0x001DFD6D File Offset: 0x001DDF6D
		public override void OnUpdate()
		{
			this.DoHasChildren();
		}

		// Token: 0x060064E8 RID: 25832 RVA: 0x001DFD78 File Offset: 0x001DDF78
		private void DoHasChildren()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			bool flag = ownerDefaultTarget.transform.childCount > 0;
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
		}

		// Token: 0x04004C7A RID: 19578
		[Tooltip("The GameObject to test.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004C7B RID: 19579
		[Tooltip("Event to send if the GameObject has children.")]
		public FsmEvent trueEvent;

		// Token: 0x04004C7C RID: 19580
		[Tooltip("Event to send if the GameObject does not have children.")]
		public FsmEvent falseEvent;

		// Token: 0x04004C7D RID: 19581
		[Tooltip("Store the result in a bool variable.")]
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004C7E RID: 19582
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
